using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Misc.Ebay.Domains
{
    public class DeliveryLabel : BaseEntity
    {
        public string Title { get; set; }
        public byte[] Content { get; set; }
        public string Base64Contents { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public DateTime UploadOn { get; set; }
    }
}
