using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Ebay.Models.CoreExtension
{
    public class EbayOrdersSearchModel : OrderSearchModel
    {
        public IList<SelectListItem> AvailableClients { get; set; }
        public IList<SelectListItem> AvailablePrintOptions { get; set; }
        public string ClientId { get; set; }
    }
}
