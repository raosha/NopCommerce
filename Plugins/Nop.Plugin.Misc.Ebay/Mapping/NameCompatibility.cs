using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.Ebay.Domains;

namespace Nop.Plugin.Misc.Ebay.Mapping
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames
        {
            get
            {
                var dictionary = new Dictionary<Type, string>();
                dictionary.Add(typeof(EbayClient), "EbayClients");
                dictionary.Add(typeof(EbayConfiguration), "EbayConfiguration");
                dictionary.Add(typeof(DeliveryLabel), "DeliveryLabel");
                dictionary.Add(typeof(EbayDispatchableOrders), "EbayDispatchableOrders");
                dictionary.Add(typeof(DispatchedOrder), "DispatchedOrder");
                return dictionary;
            }
        }

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
    }
}