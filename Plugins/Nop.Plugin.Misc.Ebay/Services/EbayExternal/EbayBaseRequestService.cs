using System.ServiceModel;
using EbaySoapServiceClient;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;

namespace Nop.Plugin.Misc.Ebay.Services.EbayExternal
{
    public class ExternalEbayBaseRequest
    {
        public eBayAPIInterfaceClient _eBayClient;
        public  CustomSecurityHeaderType _customSecurityHeader;
        public readonly string _urlDetails = "{0}?callname={1}&siteid={2}&client={3}";
        public readonly string _client = "netsoap";
        public readonly string _apiEndpointUrl = "https://api.ebay.com/wsapi";
        public readonly string _apiSandboxEndpointUrl = "https://api.sandbox.ebay.com/wsapi";
        public string GetOrdersMethodName => "GetOrders";
        public string CompleteSaleMethodName => "CompleteSale";
        public  string GetItemMethodName => "GetItem";
        public  string GetSessionMethodName => "GetSessionID";
        public string FetchTokenMethodName => "FetchToken";

        public virtual void Init(EbayOrderRequestModel requestModel , string methodName)
        {
            InitEndpoints(requestModel, methodName);
            _customSecurityHeader = new CustomSecurityHeaderType
            {
                eBayAuthToken = requestModel.AuthToken,
                Credentials = new UserIdPasswordType
                {
                    AppId = requestModel.AppId, DevId = requestModel.DevId, AuthCert = requestModel.AuthCert
                }
            };
        }
        public virtual void Init(EbaySignInRequestModel requestModel , string methodName)
        {
             InitEndpoints(requestModel, methodName);
            _customSecurityHeader = new CustomSecurityHeaderType
            {
                Credentials = new UserIdPasswordType
                {
                    AppId = requestModel.AppId, DevId = requestModel.DevId, AuthCert = requestModel.AuthCert
                }
            };
        }

        private void InitEndpoints(EbayRequestModel requestModel, string methodName)
        {
            _eBayClient = new eBayAPIInterfaceClient();
            _eBayClient.Endpoint.Address = new EndpointAddress(BuildEndPointUrl(requestModel, methodName));
        }

        private  string BuildEndPointUrl(EbayRequestModel model, string methodName)
        {
            if (model.IsSandBox)
            {
                return string.Format(_urlDetails, _apiSandboxEndpointUrl, methodName, model.SiteId, _client);
            }
            return string.Format(_urlDetails, _apiEndpointUrl, methodName, model.SiteId, _client);
            
        }
        public  EbaySignInRequestModel BuildSignInRequestModel(EbayConfigurationViewModel model)
        {
            var requestModel =
                new EbaySignInRequestModel
                {
                    AppId = model.AppId,
                    AuthCert = model.CertId,
                    DevId = model.DevId,
                    SiteId = model.SiteCode,
                    Version = model.Version,
                    SiteCode = model.SiteCode,
                    SignInUrl = model.SignInUrl,
                    RuName = model.RuName,
                    IsSandBox = model.IsSandBox
                    
                };
            return requestModel;
        }
    }
}