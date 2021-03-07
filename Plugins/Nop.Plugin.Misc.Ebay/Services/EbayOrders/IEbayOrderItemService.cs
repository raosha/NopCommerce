using System.Threading.Tasks;
using EbaySoapServiceClient;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;

namespace Nop.Plugin.Misc.Ebay.Services.EbayOrders
{
    public interface IEbayOrderItemService
    {
        Task<Product> CreateOrderLine(TransactionType trans, Order order, EbayOrderRequestModel requestModel);
        Task<Product> GetProductByItemSku(TransactionType trans, EbayOrderRequestModel requestModel);
        Task<Product> InsertProduct(TransactionType trans, EbayOrderRequestModel requestModel);
    }
}