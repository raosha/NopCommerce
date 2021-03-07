using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ebay.Services.EbayExternal
{
    public interface IExternalEbayAuthService
    {
        Task<string> GetAuthSignInUrl();
        Task<string> FetchToken(string sessionId ="");
    }
}