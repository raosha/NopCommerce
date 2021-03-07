using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Ebay.Models
{
    public class FileUploadViewModel : BaseSearchModel
    {
        [Required]
        [NopResourceDisplayName("Admin.Ebay.DeliveryLabel.Title")]
        public string Title { get; set; }
        [Required]
        [NopResourceDisplayName("Admin.Ebay.DeliveryLabel.File")]
        public IFormFile FormFile { get; set; }

    }
}
