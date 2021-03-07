using System.Threading.Tasks;
using EbaySoapServiceClient;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;

namespace Nop.Plugin.Misc.Ebay.Services.EbayExternal
{
    public interface IExternalEbayOrderService
    {
        Task<CompleteSaleResponseType> CompleteSaleRequest(EbayOrderRequestComplete model);
        Task<GetOrdersResponseType> GetOrdersRequest(EbayOrderRequestModel model);
        Task <GetItemResponseType> GetItem(EbayOrderRequestModel model , string item);
    }
}