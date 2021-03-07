using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Inventory.Models
{
    public partial class PurchaseOrderSearchModel : BaseSearchModel
    {

        
        [NopResourceDisplayName("Admin.Inventory.List.SearchByVendor")]
        public List<SelectListItem> AvailableVendors { get; set; }
        public int AvailableVendorId { get; set; }
        [NopResourceDisplayName("Admin.Inventory.List.AvailablePurchaseOrderStatuses")]
        public List<SelectListItem> AvailablePurchaseOrderStatuses { get; set; }
        public int AvailablePurchaseOrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.Inventory.List.AvailablePurchaseOrderPaymentStatuses")]
        public List<SelectListItem> AvailablePurchaseOrderPaymentStatuses { get; set; }
        public int AvailablePurchaseOrderPaymentStatusId { get; set; }
        public List<SelectListItem> AvailableCurrencies { get; set; }
        [NopResourceDisplayName("Admin.Inventory.List.WarehouseSearch")]
        public List<SelectListItem> AvailableWarehouses { get; set; }
        public int AvailableWarehouseId { get; set; }

        public PurchaseOrderSearchModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailablePurchaseOrderStatuses = new List<SelectListItem>();
            AvailablePurchaseOrderPaymentStatuses = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
        }

    }
}
