using Nop.Core;

namespace Nop.Plugin.Misc.Ebay.Domains
{
    public class EbayConfiguration : BaseEntity
    {
        public int StoreId { get; set; }
        public bool IsSandBox { get; set; }
        public bool IsActive { get; set; }
        public string DevId { get; set; }
        public string AppId { get; set; }
        public string CertId { get; set; }
        public string Version { get; set; }
        // <summary>
        /// This is Auth n Auth SignIn url for getting authNauth token.
        /// </summary>
        public string SignInUrl { get; set; }
        public string Endpoint { get; set; }
        public string SiteCode { get; set; }
        public string RuName { get; set; }
    }
}