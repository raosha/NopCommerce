using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.Inventory.Domains;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Plugin.Misc.Inventory.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
        private readonly IRepository<PurchaseOrderNote> _purchaseOrderNoteRepository;
        public PurchaseOrderService(IRepository<PurchaseOrder> purchaseOrderRepository, IRepository<PurchaseOrderNote> purchaseOrderNoteRepository)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseOrderNoteRepository = purchaseOrderNoteRepository;
        }

        public int AddPurchaseOrder(PurchaseOrder model)
        {
            _purchaseOrderRepository.Insert(model);
            return model.Id;
        }

        public int UpdatePurchaseOrder(PurchaseOrder model)
        {
            _purchaseOrderRepository.Update(model);
            return model.Id;
        }

        public int AddPurchaseNote(PurchaseOrderNote model)
        {
            _purchaseOrderNoteRepository.Insert(model);
            return model.Id;
        }

        public PurchaseOrder GetById(int id)
        {
            var purchaseOrder =_purchaseOrderRepository.Table.Where(x=>x.Id == id);
            return purchaseOrder.Any() ? purchaseOrder.First() : new PurchaseOrder();
        }

         public  PagedList<PurchaseOrder> GetPurchasedOrders(int vendorId = 0,
            int paymentStatusId = 0,
            int orderStatusId = 0,
            int warehouseId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue
            )
        {
            var query = _purchaseOrderRepository.Table;
            if (vendorId != 0 )
                query = query.Where(m => m.VendorId == vendorId);

            if (paymentStatusId != 0 )
                query = query.Where(m =>  (int)m.PaymentStatus == paymentStatusId );

            if (orderStatusId != 0 )
                query = query.Where(m =>  (int)m.OrderStatus == orderStatusId );

            if (warehouseId != 0 )
                query = query.Where(m => m.WarehouseId == warehouseId);
        
            var pagedResult = new PagedList<PurchaseOrder>(query, pageIndex, pageSize);

            return pagedResult;
            
        }






     












    }
}
