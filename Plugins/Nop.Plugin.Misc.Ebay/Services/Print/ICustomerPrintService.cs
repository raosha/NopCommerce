using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.Ebay.Models;

namespace Nop.Plugin.Misc.Ebay.Services.Print
{
    public interface ICustomerPrintService
    {
        void PrintDeliveryLabelsToPdf(Stream stream, IList<Order> orders, int deliveryLabelId, string printRequestedBy, int languageId = 0);
    }

}