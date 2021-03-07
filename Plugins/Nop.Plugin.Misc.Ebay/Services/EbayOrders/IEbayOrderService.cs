using System.Threading.Tasks;
using EbaySoapServiceClient;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;

namespace Nop.Plugin.Misc.Ebay.Services.EbayOrders
{
    public interface IEbayOrderService
    {
        Task<int> CreateOrder(OrderType eBayOrder, Customer cr, EbayClientViewModel client, EbayOrderRequestModel requestModel);
        Order GetOrderByCustomOrderNumber(string customOrderNumber);
    }
}