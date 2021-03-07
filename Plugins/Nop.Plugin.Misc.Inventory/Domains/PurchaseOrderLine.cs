using System;
using Nop.Core;

namespace Nop.Plugin.Misc.Inventory.Domains
{
    public class PurchaseOrderLine : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        public PurchaseOrderLineStatus Status { get; set; }
        public int ProductId { get; set; }
        public string Description { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public decimal Freight { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
