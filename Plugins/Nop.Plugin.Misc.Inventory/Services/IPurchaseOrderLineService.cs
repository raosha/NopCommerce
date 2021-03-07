using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Plugin.Misc.Inventory.Domains;

namespace Nop.Plugin.Misc.Inventory.Services
{
    public interface IPurchaseOrderLineService
    {
        List<PurchaseOrderLine> GetOrderLines(int orderId);
        void AddOrderLines(List<PurchaseOrderLine> orderLines);
        /// <summary>
        /// Updates the Purchase Order Line and returns current PO Line status (It could be either PartiallyReceived or Received)
        /// This method Should only be called on when Order Line Quantity Received/Delivered value is changed from UI.
        /// </summary>
        /// <param name="orderLine"></param>
        /// <returns>PurchaseOrderLineStatus</returns>
        PurchaseOrderLineStatus UpdateOrderLine(PurchaseOrderLine orderLine);
        public void AddOrderLine(PurchaseOrderLine orderLine);


    }
}