using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.Ebay.Models
{
    public class EbayDispatchableOrdersViewModel : BaseNopEntityModel
    {
        public int ItemId { get; set; }
        public int TransactionId { get; set; }
        public bool IsDispatched { get; set; }
        public DateTime DispatchDateTime { get; set; }
    }
}
