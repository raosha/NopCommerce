using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Misc.Ebay.Domains
{
    public class DispatchedOrder : BaseEntity
    {
        /// <summary>
        /// A Comma Seperated List of Order Ids which will be dispatched
        /// </summary>
        public string OrdersToDispatch { get; set; }
        /// <summary>
        /// A Comma Seperated List of Order Ids which are failed to be dispatched
        /// </summary>
        public string FailedToDispatch { get; set; }
        public DateTime DispatchDateTime { get; set; }
        public bool Processed { get; set; }
        public string DispatchRequestedBy { get; set; }
        public DateTime DispatchRequestedOn { get; set; }
    }
}
