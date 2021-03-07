using System;
using System.Linq;
using System.Threading.Tasks;
using EbaySoapServiceClient;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.Ebay.Services.EbayOrders
{
    public class EbayOrderService : IEbayOrderService
    {
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ILanguageService _languageService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICustomNumberFormatter _customNumberFormatter;
        private readonly IEbayOrderItemService _ebayOrderItemService;
        private readonly IAddressService _addressService;
        private readonly IRepository<Order> _orderRepository;
        public EbayOrderService(IOrderService orderService, IOrderProcessingService orderProcessingService, ILanguageService languageService, IEventPublisher eventPublisher, ICustomNumberFormatter customNumberFormatter, 
            IEbayOrderItemService ebayOrderItemService, IClientService clientService, IAddressService addressService, IRepository<Order> orderRepository)
        {
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _languageService = languageService;
            _eventPublisher = eventPublisher;
            _customNumberFormatter = customNumberFormatter;
            _ebayOrderItemService = ebayOrderItemService;
            _addressService = addressService;
            _orderRepository = orderRepository;
        }

        public async Task<int> CreateOrder(OrderType eBayorder, Customer cr, EbayClientViewModel client, EbayOrderRequestModel requestModel)
        {
            
            var order = await AddNewOrder(eBayorder, cr, eBayorder.TransactionArray, requestModel, client.Id, client.UserName);
            var orderItems = _orderService.GetOrderItems(order.Id, null, null, 0);
            if (orderItems.Count > 0)
            {
                var excludeTax = orderItems.Sum((OrderItem x) => x.PriceExclTax);
                
                order.OrderSubtotalExclTax = excludeTax;
                order.OrderTax=order.OrderSubtotalInclTax - excludeTax + (order.OrderShippingInclTax - order.OrderShippingExclTax);
              
                if (order.OrderTax == order.OrderTotal)
                {
                    order.OrderTax=0m;
                }
            }

            if (eBayorder.CheckoutStatus.eBayPaymentStatus == PaymentStatusCodeType.NoPaymentFailure && order.OrderTotal != 0m)
                _orderProcessingService.MarkOrderAsPaid(order);

            // Once Inserted Upate the status to be Pennding because this is New Order in Ebay World for Us.
            SetOrderStatusToPending(order);

            return order.Id;
        }

        private void SetOrderStatusToPending(Order order)
        {
            
            order.OrderStatus = OrderStatus.Pending;
            _orderService.UpdateOrder(order);
        }

        private async Task<Order> AddNewOrder(OrderType inputOrder, Customer cr,
            TransactionType[] transactionArray, EbayOrderRequestModel requestModel, int clientId, string clientName)
        {
            // Pick the first Transaction
            var trans = transactionArray?.FirstOrDefault();
            var ebayOrder = new Order();
            var paymentStatus = (PaymentStatus) 10;
            ebayOrder.StoreId = 1;  // Default it to 1 as we just got One store at the minute
            ebayOrder.CustomOrderNumber = string.Empty;
            ebayOrder.OrderStatus = OrderStatus.Pending;
            ebayOrder.OrderGuid = Guid.NewGuid();
            ebayOrder.OrderTotal = Convert.ToDecimal(inputOrder.Total.Value);
            ebayOrder.OrderSubtotalInclTax = Convert.ToDecimal(inputOrder.Subtotal.Value);
            ebayOrder.OrderSubtotalExclTax = Convert.ToDecimal(inputOrder.Subtotal.Value);
            ebayOrder.OrderShippingInclTax = Convert.ToDecimal(inputOrder.ShippingServiceSelected?.ShippingServiceCost?.Value);
            ebayOrder.OrderShippingExclTax = Convert.ToDecimal(inputOrder.ShippingServiceSelected?.ShippingServiceCost?.Value);
            ebayOrder.ShippingStatus = (ShippingStatus) 20;
            ebayOrder.PaymentStatus = paymentStatus;
            ebayOrder.CustomerId = cr.Id;
            ebayOrder.CustomerLanguageId = _languageService.GetAllLanguages(false,0).FirstOrDefault().Id;
            ebayOrder.ShippingAddressId = cr.ShippingAddressId;
            ebayOrder.BillingAddressId = cr.BillingAddressId.HasValue ? cr.BillingAddressId.Value : 0;
            ebayOrder.CreatedOnUtc = inputOrder.CreatedTime;
            ebayOrder.ShippingMethod = inputOrder.ShippingServiceSelected?.ShippingService;
            ebayOrder.PaymentMethodSystemName = inputOrder.CheckoutStatus.PaymentMethod.ToString();
            ebayOrder.CurrencyRate = 1m;
            ebayOrder.CustomerCurrencyCode = inputOrder.Total.currencyID.ToString();
            ebayOrder.CustomerTaxDisplayType = 0;
            ebayOrder.CustomOrderId = inputOrder.OrderID;
            ebayOrder.Title = trans?.Item.Title;
            ebayOrder.CustomOrderItemId = trans?.Item.ItemID;
            ebayOrder.TransactionId = trans?.TransactionID;
            ebayOrder.CustomOrderLineItemId = trans?.OrderLineItemID;
            ebayOrder.ClientId = clientId;
            ebayOrder.ClientName = clientName;
            ebayOrder.ExtendedOrderId = trans?.ExtendedOrderID;
            ebayOrder.Sku = trans?.Item?.SKU;
            ebayOrder.PostCode = GetPostCodeFromCustomerShippingAddress(cr);
            _orderService.InsertOrder(ebayOrder);
            
            ebayOrder.CustomOrderNumber = _customNumberFormatter.GenerateOrderCustomNumber(ebayOrder);
            _orderService.UpdateOrder(ebayOrder);
            _eventPublisher.Publish<OrderPlacedEvent>(new OrderPlacedEvent(ebayOrder));
            
            AddOrderNotes(inputOrder, ebayOrder);

            var product = await _ebayOrderItemService.CreateOrderLine(trans, ebayOrder, requestModel);
            
            if (string.IsNullOrEmpty(ebayOrder.Sku))
                ebayOrder.Sku = product?.Sku ?? "Unknown";

            return ebayOrder;
        }

        private void AddOrderNotes(OrderType inputOrder, Order ebayOrder)
        {
            var val2 = new OrderNote
            {
                DisplayToCustomer = true,
                Note = "Import order from ebay, OrderId = " + inputOrder.OrderID + ", user eBay " +
                       inputOrder.BuyerUserID + ", payment date " + inputOrder.PaidTime,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = ebayOrder.Id
            };
            var orderNote = val2;
            _orderService.InsertOrderNote(orderNote);
        }

        private string GetPostCodeFromCustomerShippingAddress(Customer cr)
        {
            var postCode = "Unknown";
            if (cr.ShippingAddressId.HasValue)
            {
                var shippingAddress = _addressService.GetAddressById(cr.ShippingAddressId.Value);

                if (shippingAddress != null && !string.IsNullOrEmpty(shippingAddress.ZipPostalCode))
                    postCode = shippingAddress.ZipPostalCode;
            }

            return postCode;
        }

        /// <summary>
        /// Gets an order
        /// </summary>
        /// <param name="customOrderNumber">The custom order number</param>
        /// <returns>Order</returns>
        public virtual Order GetOrderByCustomOrderNumber(string customOrderNumber)
        {
            if (string.IsNullOrEmpty(customOrderNumber))
                return null;

            return _orderRepository.Table.FirstOrDefault(o => o.ExtendedOrderId == customOrderNumber);
        }

    }
}
