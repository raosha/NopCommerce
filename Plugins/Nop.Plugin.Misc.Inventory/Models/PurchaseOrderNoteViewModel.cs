using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Inventory.Models
{
    public class PurchaseOrderNoteViewModel : BaseNopEntityModel
    {
        public int PurchaseOrderId { get; set; }
        public int AddedById { get; set; }
        public string Note { get; set; }
        public bool HasAttachment { get; set; }
        public int AttachmentId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
