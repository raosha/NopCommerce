using System;
using Nop.Core;

namespace Nop.Plugin.Misc.Ebay.Domains
{
    public class EbayClient : BaseEntity
    {
        public int ConfigurationId { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public DateTime TokenExpiresOn { get; set; }
        public DateTime LastImportTime { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }


    }
}
