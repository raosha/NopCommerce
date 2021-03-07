using Nop.Core;
using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Misc.Ebay.Services
{
    public interface IEbayCustomItemService
    {
        void InsertOrderRecord(Order item);
        void DeleteOrderRecord(Order item);
        Order GetOrderRecordByEbay(string eBayOrderId);
        Order GetOrderRecordById(int id);
        Order GetOrderRecordByNopOrderId(int nopOrderId);
        IPagedList<Order> GetAllOrderRecord(int pageIndex = 0, int pageSize = int.MaxValue);
        Order GetOrderRecordItemAndTransactionId(string itemId, string transactionId);
    }
}
