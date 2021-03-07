namespace Nop.Plugin.Misc.Ebay.Models.EbayRequests
{
    public class EbayOrderRequestComplete : EbayOrderRequestModel
    {

        public string TransactionId { get; set; }
        public string ItemId { get; set; }
    }
}