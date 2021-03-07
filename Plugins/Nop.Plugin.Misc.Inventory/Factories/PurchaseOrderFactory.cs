using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Plugin.Misc.Inventory.Domains;
using Nop.Plugin.Misc.Inventory.Models;
using Nop.Plugin.Misc.Inventory.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace Nop.Plugin.Misc.Inventory.Factories
{
    public class PurchaseOrderFactory : IPurchaseOrderFactory
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IPurchaseOrderLineService _purchaseOrderLineService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        private static string PO_REF_PREFIX = "PO000";

        public PurchaseOrderFactory(IPurchaseOrderService purchaseOrderService, IPurchaseOrderLineService purchaseOrderLineService, IBaseAdminModelFactory baseAdminModelFactory, ILogger logger, IProductService productService)
        {
            _purchaseOrderService = purchaseOrderService;
            _purchaseOrderLineService = purchaseOrderLineService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _logger = logger;
            _productService = productService;
        }

        public bool AddPurchaseOrder(PurchaseOrderViewModel model)
        {
            try
            {
                var logMessage = "Starting process of Adding new Purchase Order.";
                _logger.InsertLog(logLevel: Core.Domain.Logging.LogLevel.Information, logMessage);

                UpdateTotals(model);

                var modelToInsert = model.ToEntity<PurchaseOrder>();
                modelToInsert.LastUpdateDate = DateTime.UtcNow;
                modelToInsert.OrderDate = DateTime.UtcNow;
                modelToInsert.PaymentStatus = (PurchaseOrderPaymentStatus)model.AvailablePurchaseOrderPaymentStatusId;

                var purchaseOrderId = _purchaseOrderService.AddPurchaseOrder(modelToInsert);

                if (purchaseOrderId <= 0)
                {
                    _logger.Error($"Add Purchase Order: Failed to Add Purchase Order >>> {JsonConvert.SerializeObject(model)}. Skipping without bothering about PO Lines. ", null, null);
                    return false;
                }

                logMessage = $"Purchase Order with Id {purchaseOrderId} is added. Now going to add PO Lines for it.";
                _logger.InsertLog(logLevel: Core.Domain.Logging.LogLevel.Information, logMessage);

                foreach (var line in model.PurchaseOrderLines)
                {
                    line.PurchaseOrderId = purchaseOrderId;
                    line.LastUpdateDate = DateTime.UtcNow;
                    if (line.ProductId > 0)
                    {
                        var product = _productService.GetProductById(line.ProductId);
                        if (product != null)
                            line.Description = product.Name;
                    }
                }
                var orderLines = model.PurchaseOrderLines.Select(x => x.ToEntity<PurchaseOrderLine>());
                
                // NOTE: Bulk Insert is transaction scoped. So its safe to call below.
                _purchaseOrderLineService.AddOrderLines(orderLines.ToList());

                // Assign PO reference:
                AssignPOReference(purchaseOrderId);

                logMessage = $"Successfully Added Purchase Order with Id {purchaseOrderId} & PO Line Ids {string.Join("",orderLines.Select(x => x.Id))}";
                _logger.InsertLog(logLevel: Core.Domain.Logging.LogLevel.Information, logMessage);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to Add Purchase Order: " + ex.Message, null, null);
                return false;
            }
        }

        private void AssignPOReference(int purchaseOrderId)
        {
            var orderTopUpdate = _purchaseOrderService.GetById(purchaseOrderId);
            orderTopUpdate.OrderReference = $"{PO_REF_PREFIX}{purchaseOrderId}";
            _purchaseOrderService.UpdatePurchaseOrder(orderTopUpdate);
        }

        /// <summary>
        /// Important !!! When you Call this method make sure your PurchaseOrderViewModel have all the Purchase Order Lines
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdatePurchaseOrder(PurchaseOrderViewModel model)
        {
            var logMessage = "Starting process of Updating Purchase Order.";
            _logger.InsertLog(logLevel: Core.Domain.Logging.LogLevel.Information, logMessage);

            try
            {
                var orderLineStatuses = new List<PurchaseOrderLineStatus>();
                // First update the purchase order Lines to determine Purchase Order status
                var orderLines = model.PurchaseOrderLines.Select(x => x.ToEntity<PurchaseOrderLine>());
                foreach (var purchaseOrderLine in orderLines)
                    orderLineStatuses.Add(_purchaseOrderLineService.UpdateOrderLine(purchaseOrderLine));

                var modelToUpdate = model.ToEntity<PurchaseOrder>();
                
                if (orderLineStatuses.All(x => x == PurchaseOrderLineStatus.Received))
                    modelToUpdate.OrderStatus = PurchaseOrderStatus.Received;
                else 
                    modelToUpdate.OrderStatus = PurchaseOrderStatus.PartiallyReceived;

                modelToUpdate.LastUpdateDate = DateTime.UtcNow;

                _purchaseOrderService.UpdatePurchaseOrder(modelToUpdate);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to Update Purchase Order: " + ex.Message, null, null);
                return false;
            }
        }
        /// <summary>
        /// This method can be used when user have received all the products and currently order is set to Received and wants to mark is as fulfilled.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool MarkOrderAsFulfilled(PurchaseOrderViewModel model)
        {

            var logMessage = "Starting process of Updating Purchase Order as Fulfilled.";
            _logger.InsertLog(logLevel: Core.Domain.Logging.LogLevel.Information, logMessage);

            try
            {
                var modeToUpdate = model.ToEntity<PurchaseOrder>();
                modeToUpdate.OrderStatus = PurchaseOrderStatus.Fulfilled;
                modeToUpdate.PaymentStatus = PurchaseOrderPaymentStatus.PendingPayment;
                _purchaseOrderService.UpdatePurchaseOrder(modeToUpdate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to Update Purchase Order as Fulfilled: " + ex.Message, null, null);
                return false;
            }

           
        }
        public bool MarkOrderAsPaid(PurchaseOrderViewModel model)
        {
            var logMessage = "Starting process of Updating Purchase Order as Paid.";
            _logger.InsertLog(logLevel: Core.Domain.Logging.LogLevel.Information, logMessage);

            try
            {
                var modeToUpdate = model.ToEntity<PurchaseOrder>();
                modeToUpdate.PaymentStatus = PurchaseOrderPaymentStatus.FullyPaid;
                _purchaseOrderService.UpdatePurchaseOrder(modeToUpdate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to Update Purchase Order as Fulfilled: " + ex.Message, null, null);
                return false;
            }
        }
        public bool AddPurchaseOrderNotes(PurchaseOrderNoteViewModel orderNotes)
        {
            try
            {
                var logMessage = "Starting process of Adding new Purchase Order Note.";
                _logger.InsertLog(logLevel: Core.Domain.Logging.LogLevel.Information, logMessage);

                var modeToInsert = orderNotes.ToEntity<PurchaseOrderNote>();
                _purchaseOrderService.AddPurchaseNote(modeToInsert);

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to Add Purchase Order Note: " + ex.Message, null, null);
                return false;
            }
        }
        public PurchaseOrderViewModel PrepareViewModel(int id = 0, bool creatingNewPurchaseOrder = true)
        {
            var viewModel = new PurchaseOrderViewModel();
            if (id != 0)
            {
                viewModel = _purchaseOrderService.GetById(id).ToModel<PurchaseOrderViewModel>();

                var poLines = _purchaseOrderLineService.GetOrderLines(id);
                var products = _productService.SearchProducts(vendorId: viewModel.VendorId);
                if (poLines?.Count > 0)
                {
                    viewModel.PurchaseOrderLines = poLines.Select(x => x.ToModel<PurchaseOrderLineViewModel>()).ToList();
                    foreach (var line in viewModel.PurchaseOrderLines)
                    {
                        var prod = products.First(x => x.Id == line.ProductId);
                        line.AvailableProducts.Add(new SelectListItem { Value = prod.Id.ToString(), Text = prod.Sku});
                    }
                }
            }

            viewModel.CreatingNewPurchaseOrder = creatingNewPurchaseOrder;
            foreach (var viewModelPurchaseOrderLine in viewModel.PurchaseOrderLines)
                viewModelPurchaseOrderLine.CreatingNewPurchaseOrderLine = creatingNewPurchaseOrder;

            PrepareOrderStatuses(viewModel.AvailablePurchaseOrderStatuses, false, PurchaseOrderStatus.NewOrder.ToString());
            PreparePaymentStatuses(viewModel.AvailablePurchaseOrderPaymentStatuses, false, PurchaseOrderPaymentStatus.PendingPayment.ToString());
            _baseAdminModelFactory.PrepareVendors(viewModel.AvailableVendors);
            _baseAdminModelFactory.PrepareWarehouses(viewModel.AvailableWarehouses);
            

            return viewModel;
        }
        public PurchaseOrderSearchModel PrepareSearchModel()
        {

            var model = new PurchaseOrderSearchModel();

            PrepareOrderStatuses(model.AvailablePurchaseOrderStatuses, false, PurchaseOrderStatus.NewOrder.ToString());
            PreparePaymentStatuses(model.AvailablePurchaseOrderPaymentStatuses, false, PurchaseOrderPaymentStatus.PendingPayment.ToString());
            _baseAdminModelFactory.PrepareVendors(model.AvailableVendors);
            _baseAdminModelFactory.PrepareWarehouses(model.AvailableWarehouses);

            return model;
        }

        private void UpdateTotals(PurchaseOrderViewModel model)
        {
            var orderTotal = 0;

            foreach (var purchaseOrderLineViewModel in model.PurchaseOrderLines)
            {
                purchaseOrderLineViewModel.SubTotal =
                    purchaseOrderLineViewModel.OrderedQuantity * purchaseOrderLineViewModel.UnitPrice;
                purchaseOrderLineViewModel.Total =
                    purchaseOrderLineViewModel.OrderedQuantity * purchaseOrderLineViewModel.UnitPrice;
                purchaseOrderLineViewModel.Total += purchaseOrderLineViewModel.Freight;
            }

            var linesTotal = model.PurchaseOrderLines.Sum(x => x.Total);
            model.Amount = linesTotal;
            model.SubTotal = linesTotal; // Total Excluding Tax but (Its Lines Total including Freight)
            model.Total = model.SubTotal + model.Tax; // Total Including Tax
        }
        private void PrepareOrderStatuses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            var availableStatusItems = PurchaseOrderStatus.NewOrder.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }
        private void PreparePaymentStatuses(IList<SelectListItem> items, bool withSpecialDefaultItem = true, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            var availableStatusItems = PurchaseOrderPaymentStatus.PendingPayment.ToSelectList(false);
            foreach (var statusItem in availableStatusItems)
            {
                items.Add(statusItem);
            }
            PrepareDefaultItem(items, withSpecialDefaultItem, defaultItemText);
        }
        private void PrepareDefaultItem(IList<SelectListItem> items, bool withSpecialDefaultItem, string defaultItemText = null)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            //whether to insert the first special item for the default value
            if (!withSpecialDefaultItem)
                return;

            //at now we use "0" as the default value
            const string value = "0";

            //prepare item text
            //defaultItemText ??= _localizationService.GetResource("Admin.Common.All");

            //insert this default item at first
            items.Insert(0, new SelectListItem { Text = defaultItemText, Value = value });
        }
        public PurchaseOrderLineViewModel PrepareProductsForVendors(int vendorId)
        {
            var product = new PurchaseOrderLineViewModel {AvailableProducts = new List<SelectListItem>()};
            var products = _productService.SearchProducts(vendorId: vendorId);
           PrepareOrderStatuses(product.AvailablePurchaseOrderLineStatuses, false, PurchaseOrderLineStatus.Ordered.ToString());

            foreach (var range in products)
            {
                product.AvailableProducts.Add(new SelectListItem { Value = range.Id.ToString(), Text = range.Sku });
            }

            return product;
        }

    }
}
