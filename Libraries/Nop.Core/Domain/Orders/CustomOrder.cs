using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Orders
{
    public partial class Order : BaseEntity
    {
        public int ClientId { get; set; }
        public virtual string ClientName { get; set; }
        public virtual string CustomOrderId { get; set; }
        public virtual string CustomOrderItemId { get; set; }
        public virtual string ExtendedOrderId { get; set; }
        public virtual string CustomOrderLineItemId { get; set; }
        public virtual string TransactionId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Sku { get; set; }
        public virtual string PostCode { get; set; }
    }
}
