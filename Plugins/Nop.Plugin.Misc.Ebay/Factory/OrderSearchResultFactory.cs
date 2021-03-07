using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.CoreExtension;
using Nop.Plugin.Misc.Ebay.Services;
using Nop.Plugin.Misc.Ebay.Services.Clients;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Ebay.Factory
{
    public class OrderSearchResultFactory : IOrderSearchResultFactory
    {
        private readonly IAddressService _addressService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly AddressSettings _addressSettings;
        private readonly IClientService _ebayClients;
        private readonly IRepository<EbayClient> _cleintRepository;
        private readonly IRepository<Order> _ebayOrderRepos;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IDeliveryLabelService _deliveryLabelService;

        public OrderSearchResultFactory(IAddressService addressService, IDateTimeHelper dateTimeHelper, ILocalizationService localizationService, IOrderService orderService, IPriceFormatter priceFormatter, IProductService productService, IStoreService storeService, IWorkContext workContext, IBaseAdminModelFactory baseAdminModelFactory, AddressSettings addressSettings, IClientService ebayClients, IRepository<Order> ebayOrderRepos, IRepository<OrderItem> orderItemRepository, IRepository<Order> orderRepository, IRepository<OrderNote> orderNoteRepository, IRepository<Product> productRepository, IRepository<EbayClient> cleintRepository, IDeliveryLabelService deliveryLabelService)
        {
            _addressService = addressService;
            _dateTimeHelper = dateTimeHelper;
            _localizationService = localizationService;
            _orderService = orderService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _storeService = storeService;
            _workContext = workContext;
            _baseAdminModelFactory = baseAdminModelFactory;
            _addressSettings = addressSettings;
            _ebayClients = ebayClients;
            _ebayOrderRepos = ebayOrderRepos;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _orderNoteRepository = orderNoteRepository;
            _productRepository = productRepository;
            _cleintRepository = cleintRepository;
            _deliveryLabelService = deliveryLabelService;
        }
        public CustomOrderListModel PrepareOrderListModel(EbayOrdersSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            int clientId = 0;
            if (!string.IsNullOrEmpty(searchModel.ClientId) && !searchModel.ClientId.Equals("0"))
                clientId = int.Parse(searchModel.ClientId);

            //get parameters to filter orders
            var orderStatusIds = (searchModel.OrderStatusIds?.Contains(0) ?? true) ? null : searchModel.OrderStatusIds.ToList();
            var startDateValue = !searchModel.StartDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.StartDate.Value, _dateTimeHelper.CurrentTimeZone);
            var endDateValue = !searchModel.EndDate.HasValue ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);
            var product = _productService.GetProductById(searchModel.ProductId);
            var filterByProductId = product != null && (_workContext.CurrentVendor == null || product.VendorId == _workContext.CurrentVendor.Id)
                ? searchModel.ProductId : 0;

            //get orders
            var orders = SearchOrders(clientId,
                productId: filterByProductId,
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,false, searchModel.CustomOrderIds.ToList());

            var nopOrderId = orders.Select(x => x.Id);

           // var noOrderWithClient = _ebayOrderRepos.Table.Where(x => nopOrderId.Contains(x.NopOrderId));
            
            //prepare list model
            var model = new CustomOrderListModel().PrepareToGrid(searchModel, orders, () =>
            {
                //fill in model values from the entity
                return orders.Select(order =>
                {
                    var billingAddress = _addressService.GetAddressById(order.BillingAddressId);

                    //fill in model values from the entity
                    var orderModel = new CustomOrderModel
                    {
                        Id = order.Id,
                        OrderStatusId = order.OrderStatusId,
                        PaymentStatusId = order.PaymentStatusId,
                        ShippingStatusId = order.ShippingStatusId,
                        CustomerEmail = billingAddress.Email,
                        CustomerFullName = $"{billingAddress.FirstName} {billingAddress.LastName}",
                        CustomerId = order.CustomerId,
                        CustomOrderNumber = order.CustomOrderNumber,
                       
                    };

                    orderModel.SellerName = order.ClientName;
                    orderModel.Sku = order.Sku;
                    orderModel.Title = order.Title;
                    orderModel.PostCode = order.PostCode;
                    orderModel.ExtendedOrderId = order.ExtendedOrderId;

                    //convert dates to the user time
                    orderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    orderModel.StoreName = _storeService.GetStoreById(order.StoreId)?.Name ?? "Deleted";
                    orderModel.OrderStatus = _localizationService.GetLocalizedEnum(order.OrderStatus);
                    orderModel.PaymentStatus = _localizationService.GetLocalizedEnum(order.PaymentStatus);
                    orderModel.ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus);
                    orderModel.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);

                    return orderModel;
                });
            });
            
            return model;
        }


        public  OrderSearchModel PrepareOrderSearchModel(EbayOrdersSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.IsLoggedInAsVendor = _workContext.CurrentVendor != null;
            searchModel.BillingPhoneEnabled = _addressSettings.PhoneEnabled;

            //prepare available order, payment and shipping statuses
            _baseAdminModelFactory.PrepareOrderStatuses(searchModel.AvailableOrderStatuses);
            if (searchModel.AvailableOrderStatuses.Any())
            {
                if (searchModel.OrderStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.OrderStatusIds.Select(id => id.ToString());
                    searchModel.AvailableOrderStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailableOrderStatuses.FirstOrDefault().Selected = true;
            }

            _baseAdminModelFactory.PreparePaymentStatuses(searchModel.AvailablePaymentStatuses);
            if (searchModel.AvailablePaymentStatuses.Any())
            {
                if (searchModel.PaymentStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.PaymentStatusIds.Select(id => id.ToString());
                    searchModel.AvailablePaymentStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailablePaymentStatuses.FirstOrDefault().Selected = true;
            }

            _baseAdminModelFactory.PrepareShippingStatuses(searchModel.AvailableShippingStatuses);
            if (searchModel.AvailableShippingStatuses.Any())
            {
                if (searchModel.ShippingStatusIds?.Any() ?? false)
                {
                    var ids = searchModel.ShippingStatusIds.Select(id => id.ToString());
                    searchModel.AvailableShippingStatuses.Where(statusItem => ids.Contains(statusItem.Value)).ToList()
                        .ForEach(statusItem => statusItem.Selected = true);
                }
                else
                    searchModel.AvailableShippingStatuses.FirstOrDefault().Selected = true;
            }

            var allClients = _ebayClients.GetAllActiveClients();
             
            var items = allClients.Select(x=>    new SelectListItem
             {
                 Text = x.UserName.ToUpper(),
                 Value = x.Id.ToString() 
             }).ToList();

             items.Add(new SelectListItem("-- Please Select Store --","0",true));
             searchModel.AvailableClients = new List<SelectListItem>(items);

             var allOrders = _orderRepository.Table.Where(o => !o.Deleted);

             if (allOrders != null && allOrders.Any())
             {
                var allOrderIds = allOrders.ToDictionary(c => c.Id, c => c.ExtendedOrderId);
                if (allOrderIds.Any())
                    searchModel.AvailableCustomOrderIds = allOrderIds
                        .Select(x => new SelectListItem { Text = x.Value.ToString(), Value = x.Key.ToString() }).ToList();
             }

            var availablePrintOprionts = _deliveryLabelService.GetAllDeliveryLables();
            var printOptions = availablePrintOprionts.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Id.ToString()
            }).ToList();

            searchModel.AvailablePrintOptions = new List<SelectListItem>(printOptions);
            searchModel.SetGridPageSize();
            return searchModel;
        }

        public virtual IPagedList<Order> SearchOrders(int clientId,
            int productId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            List<int> osIds = null,
             int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, List<int> customOrderIds = null)
        {
            var query = _orderRepository.Table;
            
            if (productId > 0)
                query = from o in query
                    join oi in _orderItemRepository.Table on o.Id equals oi.OrderId
                    where oi.ProductId == productId
                    select o;

            if (clientId > 0)
                query = query.Where(o => o.ClientId == clientId);


            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);

            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);

            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));

            query = query.Where(o => !o.Deleted);

            if (customOrderIds != null && customOrderIds.Any())
            {
                var queryable = query.Where(o => customOrderIds.Contains(o.Id));
                query = queryable;
            }

            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            return new PagedList<Order>(query, pageIndex, pageSize, getOnlyTotalCount);
        }

    }
}
