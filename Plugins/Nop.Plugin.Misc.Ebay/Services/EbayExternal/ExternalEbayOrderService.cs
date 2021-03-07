using System.IO;
using System.Threading.Tasks;
using EbaySoapServiceClient;
using Newtonsoft.Json;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;

namespace Nop.Plugin.Misc.Ebay.Services.EbayExternal
{
    public class ExternalEbayOrderService : ExternalEbayBaseRequest, IExternalEbayOrderService
    {
        public async Task<CompleteSaleResponseType> CompleteSaleRequest(EbayOrderRequestComplete model)
        {
            // For CompleteSale version is 1083
            model.Version = "1083";
            Init(model, CompleteSaleMethodName);
            var completeSaleRequestType = new CompleteSaleRequestType
            {
                ItemID = model.ItemId, 
                TransactionID = model.TransactionId,
                Shipped = true,
                ShippedSpecified = true,
                Version = "1083"
            };

            var securityHeaderType = new CustomSecurityHeaderType
            {
                eBayAuthToken = model.AuthToken,
                Credentials = new UserIdPasswordType
                {
                    AppId = model.AppId, 
                    DevId = model.DevId, 
                    AuthCert = model.AuthCert
                }
            };

            var response = await _eBayClient.CompleteSaleAsync(securityHeaderType, completeSaleRequestType);
            return response.CompleteSaleResponse1;
        }


       

        public async Task<GetOrdersResponseType> GetOrdersRequest(EbayOrderRequestModel model)
        {
            Init(model, GetOrdersMethodName);
            //TODO If more than 200 orders then we need to do pagging 
            var orderRequest = new GetOrdersRequestType
            {
                OrderStatus = OrderStatusCodeType.Completed,
                OrderStatusSpecified = true,
                CreateTimeFrom = model.DateFrom,
                CreateTimeTo = model.DateTo,
                CreateTimeFromSpecified = true,
                CreateTimeToSpecified = true,
                OrderRole = TradingRoleCodeType.Seller,
                OrderRoleSpecified = true,
                SortingOrder = SortOrderCodeType.Ascending,
                SortingOrderSpecified = true,
                Pagination = new PaginationType
                {
                    EntriesPerPage = 100, // We set it to max orders per page.
                    EntriesPerPageSpecified = true,
                    PageNumber = 1,
                    PageNumberSpecified = true
                },
                ErrorLanguage = "11",
                Version = model.Version,
                DetailLevel = new[] { DetailLevelCodeType.ReturnAll }
            };
            
            var response = await _eBayClient.GetOrdersAsync(_customSecurityHeader, orderRequest);
            return response.GetOrdersResponse1;
        }
        public async Task <GetItemResponseType> GetItem(EbayOrderRequestModel model , string item)
        {
            Init(model, GetItemMethodName);
            
            var itemRequest = new GetItemRequestType
            {
                ItemID = item, IncludeItemSpecifics = true, IncludeItemSpecificsSpecified = true,
                ErrorLanguage = "11",
                Version = model.Version,
                DetailLevel = new DetailLevelCodeType[1]
            };

            var response = await _eBayClient.GetItemAsync(_customSecurityHeader, itemRequest);
            return response.GetItemResponse1;
        }
       



       
    }
}
