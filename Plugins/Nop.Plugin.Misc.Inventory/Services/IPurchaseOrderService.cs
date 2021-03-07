using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Misc.Inventory.Domains;

namespace Nop.Plugin.Misc.Inventory.Services
{
    public interface IPurchaseOrderService
    {
        int AddPurchaseOrder(PurchaseOrder model);
        int UpdatePurchaseOrder(PurchaseOrder model);

        int AddPurchaseNote(PurchaseOrderNote model);
        PurchaseOrder GetById(int id);
        PagedList<PurchaseOrder> GetPurchasedOrders(int vendorId = 0,
            int paymentStatusId = 0,
            int orderStatusId = 0,
            int warehouseId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue
        );
    }
}