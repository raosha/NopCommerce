using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Misc.Ebay.Domains
{
    public class EbayDispatchableOrders : BaseEntity
    {
        public int ItemId { get; set; }
        public int TransactionId { get; set; }
        public bool IsDispatched { get; set; }
        public DateTime DispatchDateTime { get; set; }
    }
}
