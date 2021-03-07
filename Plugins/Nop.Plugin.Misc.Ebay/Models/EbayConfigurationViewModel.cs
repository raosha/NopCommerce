using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Ebay.Models
{
    public class EbayConfigurationViewModel : BaseNopEntityModel
    {

        [NopResourceDisplayName("Admin.Ebay.IsSandbox")]
        [Required]
        public bool IsSandBox { get; set; }
        [NopResourceDisplayName("Admin.Ebay.IsActive")]
        [Required]
        public bool IsActive { get; set; }
        [NopResourceDisplayName("Admin.Ebay.DevId")]
        [Required]
        public string DevId { get; set; }
        [NopResourceDisplayName("Admin.Ebay.AppId")]
        [Required]
        public string AppId { get; set; }
        [NopResourceDisplayName("Admin.Ebay.CertId")]
        [Required]
        public string CertId { get; set; }
        [NopResourceDisplayName("Admin.Ebay.Version")]
        [Required]
        public string Version { get; set; }
        [NopResourceDisplayName("Admin.Ebay.SiteCode")]
        [Required]
        public string SiteCode { get; set; }
        [NopResourceDisplayName("Admin.Ebay.EndPoint")]
        [Required]
        public string EndPoint { get; set; }
        [Required]
        [NopResourceDisplayName("Admin.Ebay.SignInUrl")]
        public string SignInUrl { get; set; }
        [Required]
        [NopResourceDisplayName("Admin.Ebay.RuName")]
        public string RuName { get; set; }

    }
}
