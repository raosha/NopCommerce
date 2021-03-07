using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Inventory.Models
{
    public class PurchaseOrderViewModel : BaseNopEntityModel
    {
        [Required]
        [NopResourceDisplayName("Admin.Inventory.PurchaseOrder.Name")]
        public string PurchaseOrderName { get; set; }
        public string OrderReference { get; set; }
        public string WarehouseName { get; set; }
        public int WarehouseId { get; set; }
        public int CurrencyId { get; set; }
        [NopResourceDisplayName("Admin.Inventory.List.SearchByVendor")]
        public List<SelectListItem> AvailableVendors { get; set; }
        [Required]
        public int VendorId { get; set; }
        [NopResourceDisplayName("Admin.Inventory.List.WarehouseSearch")]
        public List<SelectListItem> AvailableWarehouses { get; set; }
        public int AvailableWarehouseId { get; set; }
        [NopResourceDisplayName("Admin.Inventory.List.AvailablePurchaseOrderStatuses")]
        public List<SelectListItem> AvailablePurchaseOrderStatuses { get; set; }
        public int AvailablePurchaseOrderStatusId { get; set; }
        [NopResourceDisplayName("Admin.Inventory.List.AvailablePurchaseOrderPaymentStatuses")]
        public List<SelectListItem> AvailablePurchaseOrderPaymentStatuses { get; set; }
        public int AvailablePurchaseOrderPaymentStatusId { get; set; }
        public List<SelectListItem> AvailableCurrencies { get; set; }
        [NopResourceDisplayName("Admin.Inventory.PurchaseOrder.OrderDate")]
        public DateTime OrderDate { get; set; }
        [NopResourceDisplayName("Admin.Inventory.PurchaseOrder.DeliveryDate")]
        public DateTime DeliveryDate { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal Freight { get; set; }
        [NopResourceDisplayName("Admin.Inventory.PurchaseOrder.Total")]
        public decimal Total { get; set; }
        public bool CreatingNewPurchaseOrder { get; set; }
        public List<PurchaseOrderLineViewModel> PurchaseOrderLines { get; set; }
        public PurchaseOrderViewModel()
        {
            AvailableCurrencies = new List<SelectListItem>();
            AvailableVendors = new List<SelectListItem>();
            AvailablePurchaseOrderStatuses = new List<SelectListItem>();
            AvailablePurchaseOrderPaymentStatuses = new List<SelectListItem>();
            AvailableWarehouses = new List<SelectListItem>();
            PurchaseOrderLines = new List<PurchaseOrderLineViewModel>();
            CreatingNewPurchaseOrder = true; // default
        }

    }
}
