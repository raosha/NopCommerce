using System;
using Nop.Core;

namespace Nop.Plugin.Misc.Inventory.Domains
{
    public partial class PurchaseOrderNote : BaseEntity
    {
        public int PurchaseOrderId { get; set; }
        /// <summary>
        /// User Id of entity who added the note.
        /// </summary>
        public int AddedById { get; set; }
        public string Note { get; set; }
        public bool HasAttachment { get; set; } // Not for D-Link but This will be very handy in future.
        public int AttachmentId { get; set; }   // Not for D-Link but This will be very handy in future.
        public DateTime CreatedOnUtc { get; set; }
    }
}
