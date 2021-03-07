using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nop.Core.Domain.Tax;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.Ebay.Models.CoreExtension
{
    public class CustomOrderModel : OrderModel
    {
        [NopResourceDisplayName("Admin.Orders.Fields.SellerName")]
        public string SellerName { get; set; }
        //order Sku
        [NopResourceDisplayName("Admin.Orders.Fields.Sku")]
        public string Sku { get; set; }
        //order Title
        [NopResourceDisplayName("Admin.Orders.Fields.Title")]
        public string Title { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.PostCode")]
        public string PostCode { get; set; }

        [NopResourceDisplayName("Admin.Orders.Fields.ExtendedOrderId")]
        public string ExtendedOrderId { get; set; }
    }
}