using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Plugin.Misc.Ebay.Services.Configurations;
using Nop.Plugin.Misc.Ebay.Services.EbayExternal;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public class OrderDispatchService: IOrderDispatchService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IClientService _ebayClientService;
        private readonly IExternalEbayOrderService _ebayWebService;
        private readonly IOrderService _orderService;
        private readonly IEbayCustomItemService _ebayCustomItemService;
        private readonly IRepository<DispatchedOrder> _dispatchedOrderRepository;
        private readonly ILogger _logger;

        public OrderDispatchService(IConfigurationService configurationService, IClientService ebayClientService, IExternalEbayOrderService ebayWebService, IOrderService orderService, IEbayCustomItemService ebayCustomItemService, ILogger logger, IRepository<DispatchedOrder> dispatchedOrderRepository)
        {
            _configurationService = configurationService;
            _ebayClientService = ebayClientService;
            _ebayWebService = ebayWebService;
            _orderService = orderService;
            _ebayCustomItemService = ebayCustomItemService;
            _logger = logger;
            _dispatchedOrderRepository = dispatchedOrderRepository;
        }

        
        public async Task DispatchOrders()
        {
            var dispatchableOrders = _dispatchedOrderRepository.Table.Where(x => x.Processed == false);
            
            if (dispatchableOrders == null || !dispatchableOrders.Any())
            {
                _logger.InsertLog((LogLevel)20, $"At OrderDispatchService.DispatchOrders : No Dispatch-able orders found to process. Skipping!!!", null);
                return;
            }

            foreach (var dispatchableOrder in dispatchableOrders)
            {
                var orderIdsFailedToDispatch = new List<string>();
                var orderIds = dispatchableOrder.OrdersToDispatch.Split(',').ToList();
                
                foreach (var orderId in orderIds)
                    try
                    {
                        var ebayOrder = _ebayCustomItemService.GetOrderRecordByNopOrderId(int.Parse(orderId));
                        if (ebayOrder == null)
                        {
                            _logger.InsertLog((LogLevel)20,
                                $"At OrderDispatchService.DispatchOrders : Could Not find Order Record for Id {orderId}. Skipping and Moving to next one.",
                                null);

                            orderIdsFailedToDispatch.Add(orderId);
                        }
                        else
                        {
                            if (ebayOrder.OrderStatus == OrderStatus.Complete && ebayOrder.ShippingStatus == ShippingStatus.Shipped) // Only Dispatch if its not already
                            {
                                _logger.InsertLog((LogLevel)20, $"At OrderDispatchService.DispatchOrders : Order With Id {ebayOrder.Id}, Transaction Id {ebayOrder.TransactionId} and Ebay Item Id {ebayOrder.CustomOrderItemId} is already set to status Dispatched. Skipping this Order!!!", null);
                                orderIdsFailedToDispatch.Add(orderId);
                                continue;
                            }

                            await MarkOrderShipped(ebayOrder.CustomOrderItemId, ebayOrder.TransactionId, ebayOrder.ClientId);
                            UpdateOrderStatusToDispatched(int.Parse(orderId));
                            AddOrderNote(ebayOrder,dispatchableOrder);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.InsertLog(LogLevel.Error, $"At OrderDispatchService.DispatchOrders: Failed to Dispatch Order with Id {orderId}. Skipping and Moving to next one: {ex.Message}", $"{JsonConvert.SerializeObject(ex)}");

                        orderIdsFailedToDispatch.Add(orderId);
                    }

                UpdateDispatchableOrderRecordStatus(dispatchableOrder, orderIdsFailedToDispatch);
            }
        }

        private void AddOrderNote(Order ebayOrder, DispatchedOrder dispatchedOrder)
        {
            _orderService.InsertOrderNote(new OrderNote
            {
                CreatedOnUtc = DateTime.UtcNow,
                DisplayToCustomer = true,
                Note = $"Order with Id {ebayOrder.ExtendedOrderId} updated and set to Dispatched. Dispatch was requested by {dispatchedOrder.DispatchRequestedBy} on {dispatchedOrder.DispatchRequestedOn}",
                OrderId = ebayOrder.Id
            });
        }

        public bool AddDispatchableOrdersRecord(IList<string> orderIds, string dispatchRequestedBy)
        {
            try
            {
                if (orderIds == null || !orderIds.Any())
                    return false;

                var dispatchableOrder = new DispatchedOrder
                {
                    OrdersToDispatch = string.Join(",",orderIds),
                    Processed = false,
                    DispatchRequestedBy = dispatchRequestedBy,
                    DispatchRequestedOn = DateTime.UtcNow
                };
                
                _dispatchedOrderRepository.Insert(dispatchableOrder);

                //DispatchOrders();

                return true;
            }
            catch (Exception e)
            {
                var msg =
                    $"At OrderDispatchService.AddOrderDispatchRecord : Failed to add DispatchedOrder record for {string.Join(",", orderIds)}.";
                _logger.Error(msg, e);
            }

            return false;
        }
        private async Task<bool> MarkOrderShipped(string itemId, string transactionId, int clientId)
        {
            try
            {
                var activeConfiguration = _configurationService.GetActiveConfiguration();
                if (activeConfiguration == null)
                {
                    _logger.InsertLog((LogLevel)40, $"At OrderDispatchService.MarkOrderShipped : No Active ebay configuration Found in the system", null);
                    return false;
                }

                var client = _ebayClientService.GetById(clientId);
                if (client == null)
                {
                    _logger.Error($"At OrderDispatchService.MarkOrderShipped : No Client found with Id {clientId}", null, null);
                    return false;
                }

                var requestModel = BuildRequestModel(activeConfiguration, client);
                requestModel.ItemId = itemId;
                requestModel.TransactionId = transactionId;
                var completeStatusResponse = await _ebayWebService.CompleteSaleRequest(requestModel);

                if (completeStatusResponse.Ack != 0)
                    if (completeStatusResponse.Errors != null)
                    {
                        var errors = completeStatusResponse.Errors;
                        foreach (var error in errors)
                            _logger.InsertLog((LogLevel)40,
                                "eBay - Complete sale error " + error.ErrorCode + " - " + error.ShortMessage,
                                error.LongMessage ?? "", null);
                    }

                return true;
            }
            catch (Exception e)
            {
                var msg =
                    $"At OrderDispatchService.MarkOrderShipped : Failed to Complete Order Status for Item with Id {itemId} and transaction id {transactionId}.";
                _logger.Error(msg, e);
            }

            return false;
        }
        private void UpdateDispatchableOrderRecordStatus(DispatchedOrder dispatchedOrder, List<string> orderIdsFailedToDispatch)
        {
            try
            {
                dispatchedOrder.Processed = true;
                dispatchedOrder.DispatchDateTime = DateTime.UtcNow;
                dispatchedOrder.FailedToDispatch = (orderIdsFailedToDispatch != null && orderIdsFailedToDispatch.Any()) ? string.Join(",", orderIdsFailedToDispatch) : string.Empty;

                _dispatchedOrderRepository.Update(dispatchedOrder);
            }
            catch (Exception e)
            {
                var msg =
                    $"At OrderDispatchService.UpdateDispatchableOrderRecordStatus : After processing dispatch-able order request, Failed to update DispatchedOrder record for record Id {dispatchedOrder.Id}.";
                _logger.Error(msg, e);
            }
           
        }
        private void UpdateOrderStatusToDispatched(int nopOrderId)
        {
            try
            {
                var nopOrder = _orderService.GetOrderById(nopOrderId);
                if (nopOrder != null)
                {
                    nopOrder.OrderStatus = OrderStatus.Complete;
                    nopOrder.ShippingStatus = ShippingStatus.Shipped;
                    _orderService.UpdateOrder(nopOrder);
                }
            }
            catch (Exception e)
            {
                var msg =
                    $"At OrderDispatchService.UpdateOrderStatus : Failed to Update Nop Order status for order Id {nopOrderId}.";
                _logger.Error(msg, e);
            }
        }
        private EbayOrderRequestComplete BuildRequestModel(EbayConfigurationViewModel model, EbayClientViewModel client)
        {
            var requestModel =
                new EbayOrderRequestComplete
                {
                    AppId = model.AppId,
                    AuthCert = model.CertId,
                    DevId = model.DevId,
                    SiteId = model.SiteCode,
                    DateFrom = client.LastImportTime,
                    AuthToken = client.Token,
                    Version = model.Version,
                    SiteCode = model.SiteCode,
                    DateTo = DateTime.Now
                };
            return requestModel;
        }
    }
}
