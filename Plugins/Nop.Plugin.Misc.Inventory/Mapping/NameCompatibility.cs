using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Plugin.Misc.Inventory.Domains;

namespace Nop.Plugin.Misc.Inventory.Mapping
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames
        {
            get
            {
                var dictionary = new Dictionary<Type, string>();
                dictionary.Add(typeof(PurchaseOrder), "PurchaseOrder");
                dictionary.Add(typeof(PurchaseOrderLine), "PurchaseOrderLine");
                dictionary.Add(typeof(PurchaseOrderNote), "PurchaseOrderNote");
                return dictionary;
            }
        }

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
    }
}