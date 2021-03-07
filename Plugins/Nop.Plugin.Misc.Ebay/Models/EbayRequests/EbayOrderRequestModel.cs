using System;

namespace Nop.Plugin.Misc.Ebay.Models.EbayRequests
{
    public class EbayOrderRequestModel : EbayRequestModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string AuthToken { get; set; }
    
    }
}