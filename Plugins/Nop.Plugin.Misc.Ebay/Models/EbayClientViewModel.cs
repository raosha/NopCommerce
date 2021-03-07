using System;
using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Ebay.Models
{
    public class EbayClientViewModel : BaseNopEntityModel
    {
        [Required]
        [NopResourceDisplayName("Admin.Ebay.Token")]
        public string Token { get; set; }
        [NopResourceDisplayName("Admin.Ebay.TokenExpiresOn")]
        public DateTime TokenExpiresOn { get; set; }
        [Required]
        [NopResourceDisplayName("Admin.Ebay.LastImportTime")]
        public DateTime LastImportTime { get; set; }
        [NopResourceDisplayName("Admin.Ebay.Comments")]
        public string Comments { get; set; }
        [NopResourceDisplayName("Admin.Ebay.IsActive")]
        public bool IsActive { get; set; }
        [NopResourceDisplayName("Admin.Ebay.UserName")]
        public string UserName { get; set; }
    }
}
