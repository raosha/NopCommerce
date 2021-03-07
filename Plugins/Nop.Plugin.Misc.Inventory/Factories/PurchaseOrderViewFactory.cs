using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Nop.Data;
using Nop.Plugin.Misc.Inventory.Domains;
using Nop.Plugin.Misc.Inventory.Models;
using Nop.Plugin.Misc.Inventory.Services;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Plugin.Misc.Inventory.Factories
{
    public class PurchaseOrderViewFactory : IPurchaseOrderViewFactory
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrderViewFactory(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        public PurchaseOrderListModel PrepareList(PurchaseOrderSearchModel searchModel)
        {
           var model = _purchaseOrderService.GetPurchasedOrders(
                vendorId: searchModel.AvailableVendorId,
                paymentStatusId: searchModel.AvailablePurchaseOrderStatusId,
                orderStatusId: searchModel.AvailablePurchaseOrderStatusId,
                warehouseId: searchModel.AvailableWarehouseId,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize
            );
            var model1 = new PurchaseOrderListModel().PrepareToGrid(searchModel, model, () =>
            {
                //fill in model values from the entity
                return model.Select(config =>
                {
                    var dlModel = config.ToModel<PurchaseOrderViewModel>();
                    return dlModel;
                });
            });

            return model1;
        }
    }
}
