using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Misc.Inventory.Domains;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Inventory.Models
{
    public class PurchaseOrderLineViewModel : BaseNopEntityModel
    {
        public int LineNumber { get; set; }
        public int PurchaseOrderId { get; set; }
        public PurchaseOrderLineStatus Status { get; set; }
        public List<SelectListItem> AvailablePurchaseOrderLineStatuses { get; set; }
        public List<SelectListItem> AvailableProducts { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Freight { get; set; }
        public decimal Total { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool IsVisible { get; set; }
        public bool CreatingNewPurchaseOrderLine { get; set; }
        public string CreatingNewPurchaseOrderLineFlag
        {
            get
            {
                if (CreatingNewPurchaseOrderLine)
                    return "disabled";

                return string.Empty;
            }
        }

        public PurchaseOrderLineViewModel()
        {
            AvailablePurchaseOrderLineStatuses = new List<SelectListItem>();
            AvailableProducts = new List<SelectListItem>();
            UnitPrice = 0.1M;
            OrderedQuantity = 1;
            Total = UnitPrice * OrderedQuantity;
            ReceivedQuantity = 0;
            IsVisible = false;
            CreatingNewPurchaseOrderLine = true;
        }
    }
}
