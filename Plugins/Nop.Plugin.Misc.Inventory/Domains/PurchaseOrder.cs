using System;
using Nop.Core;

namespace Nop.Plugin.Misc.Inventory.Domains
{
    public class PurchaseOrder : BaseEntity
    {
        public string PurchaseOrderName { get; set; }
        public string OrderReference { get; set; }
        public int StoreId { get; set; }
        public int VendorId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public PurchaseOrderPaymentStatus PaymentStatus { get; set; }
        public PurchaseOrderStatus OrderStatus { get; set; }
        public PurchaseOrderStatus OrderPaymentStatus { get; set; }  // Leave it for the time being. Just added for future use.
        public int CurrencyId { get; set; }
        public int CreatedByUserId { get; set; }
        public string Remarks { get; set; }
        public decimal Amount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Freight { get; set; }
        public decimal Tax { get; set; }
        public DateTime LastUpdateDate { get; set; }

    }
}
