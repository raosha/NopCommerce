using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Http;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Ebay.Models
{
    public class DeliveryLabelViewModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Admin.Ebay.DeliveryLabel.Title")]
        public string Title { get; set; }
        public string Base64Contents { get; set; }
        public byte[] Content { get; set; }
        [NopResourceDisplayName("Admin.Ebay.DeliveryLabel.Name")]
        public string Name { get; set; }
        public string MimeType { get; set; }
        public string Url { get; set; }
        [NopResourceDisplayName("Admin.Ebay.DeliveryLabel.Size")]
        public long Size { get; set; }
        [NopResourceDisplayName("Admin.Ebay.DeliveryLabel.UploadOn")]
        public DateTime UploadOn { get; set; }
    }
}
