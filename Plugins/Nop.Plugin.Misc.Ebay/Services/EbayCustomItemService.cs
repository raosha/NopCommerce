using System;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Data;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public class EbayCustomItemService : IEbayCustomItemService
    {
        private readonly IRepository<Order> _orderRecord;

        public EbayCustomItemService(IRepository<Order> orderRecord)
        {
            _orderRecord = orderRecord;
        }
        public  void InsertOrderRecord(Order item)
        {
            _orderRecord.Insert(item);
        }

        public  void DeleteOrderRecord(Order item)
        {
            _orderRecord.Delete(item);
        }

        public  Order GetOrderRecordByEbay(string eBayOrderId)
        {
            var query = _orderRecord?.Table.Where(x => x.CustomOrderId == eBayOrderId);
            return query.Any() ? query.First() : null;
        }

        public Order GetOrderRecordItemAndTransactionId(string itemId, string transactionId)
        {
            var query = _orderRecord.Table.Where(x => x.CustomOrderItemId == itemId && x.TransactionId == transactionId);
            return query.Any() ? query.First() : null;
        }

        public  Order GetOrderRecordById(int id)
        {
            var query = _orderRecord.Table.Where(x => x.Id == id);
            return query.Any() ? query.First() : null;
        }
        public Order GetOrderRecordByNopOrderId(int nopOrderId)
        {
            var query = _orderRecord.Table.Where(x => x.Id == nopOrderId);
            return query.Any() ? query.First() : null;
        }
        public IPagedList<Order> GetAllOrderRecord(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from gp in _orderRecord.Table
                orderby gp.CreatedOnUtc descending
                select gp;
            return (IPagedList<Order>)(object)new PagedList<Order>((IQueryable<Order>)query, pageIndex, pageSize, false);
        }
    }
}
