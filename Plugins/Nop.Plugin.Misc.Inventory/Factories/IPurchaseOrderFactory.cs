using System.Threading.Tasks;
using Nop.Plugin.Misc.Inventory.Models;

namespace Nop.Plugin.Misc.Inventory.Factories
{
    public interface IPurchaseOrderFactory
    {
        bool AddPurchaseOrder(PurchaseOrderViewModel model);
        /// <summary>
        /// Important !!! When you Call this method make sure your PurchaseOrderViewModel all the Purchase Order Lines
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdatePurchaseOrder(PurchaseOrderViewModel model);
        bool MarkOrderAsFulfilled(PurchaseOrderViewModel model);
        bool MarkOrderAsPaid(PurchaseOrderViewModel model);
        bool AddPurchaseOrderNotes(PurchaseOrderNoteViewModel orderNotes);
        PurchaseOrderViewModel PrepareViewModel(int id = 0, bool creatingNewPurchaseOrder = true);
        PurchaseOrderSearchModel PrepareSearchModel();

        PurchaseOrderLineViewModel PrepareProductsForVendors(int vendorId);
    }
}