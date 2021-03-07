using System;
using System.Threading.Tasks;
using EbaySoapServiceClient;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;
using Nop.Plugin.Misc.Ebay.Services.Configurations;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.Ebay.Services.EbayExternal
{
    public class ExternalEbayAuthService : ExternalEbayBaseRequest, IExternalEbayAuthService
    {
        private readonly IConfigurationService _configurationService;
        private readonly ILogger _logger;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IWorkContext _currentContext;
        private string TokenCacheString =>  "TokenCache" +  "_" + _currentContext.CurrentCustomer.Email;


        public ExternalEbayAuthService(IConfigurationService configurationService, 
            ILogger logger, IStaticCacheManager cacheManager, IWorkContext currentContext)
        {
            _configurationService = configurationService;
            _logger = logger;
            _cacheManager = cacheManager;
            _currentContext = currentContext;
        }

        public async Task<string> GetAuthSignInUrl()
        {
            try
            {
                var requestModel = GetSignInRequestModel();
                if (requestModel == null) return null;
                var response = await GetSessionId(requestModel);

                if (response.Ack != AckCodeType.Failure && !string.IsNullOrEmpty(response.SessionID))
                {
                    var cacheKey = new CacheKey(TokenCacheString);
                    _cacheManager.Set(cacheKey, response);
                    return string.Format(requestModel.SignInUrl,requestModel.RuName, response.SessionID);
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.Error($"AtGetAuthSignInUrl : Failed to get session Id. Error: {e.Message}", null, null);
            }

            return null;
        }

        public async Task<string> FetchToken(string sessionId ="")
        {
            var requestModel = GetSignInRequestModel();
            if (requestModel == null) return null;
            Init(requestModel, FetchTokenMethodName);
            if (string.IsNullOrEmpty(sessionId))
            {
                var cacheKey = new CacheKey(TokenCacheString);
                var session = _cacheManager.Get<GetSessionIDResponseType>(cacheKey,  null);
                if (session != null && !string.IsNullOrEmpty(session.SessionID))
                {
                    sessionId = session.SessionID;
                    //Removing As this is Utilized now 
                    _cacheManager.Remove(cacheKey);
                }
            }
            if (string.IsNullOrEmpty(sessionId))
                return null;

            var fetchRequestToken = new FetchTokenRequestType
            {
                SessionID = sessionId,
                Version = "1083"
            };
            var response =  await _eBayClient.FetchTokenAsync(_customSecurityHeader, fetchRequestToken);
            return response.FetchTokenResponse1.eBayAuthToken;
        }

        private EbaySignInRequestModel GetSignInRequestModel()
        {
            var activeConfiguration = _configurationService.GetActiveConfiguration();
            if (activeConfiguration == null)
            {
                _logger.Error($"At GetSignInRequestModel : No Active ebay configuration Found in the system",
                    null, null);
                return null;
            }

            var requestModel = BuildSignInRequestModel(activeConfiguration);
            return requestModel;
        }
        private async Task<GetSessionIDResponseType> GetSessionId(EbaySignInRequestModel model)
        {
            Init(model , GetSessionMethodName);
            var reqType = new GetSessionIDRequestType 
            {
                RuName = model.RuName,
                Version = "1083"
            };
            var response = await _eBayClient.GetSessionIDAsync(_customSecurityHeader, reqType);
         
            return response.GetSessionIDResponse1;
        }
    }
}
