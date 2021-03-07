using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Orders;
using Order = StackExchange.Redis.Order;

namespace Nop.Plugin.Misc.Ebay.Domains
{
    public enum EbayOrderStatus
    {
        NewOrder = OrderStatus.Pending,
        Printed =  OrderStatus.Processing,
        Dispatched = OrderStatus.Complete,
        Cancelled = OrderStatus.Cancelled
    }
}
