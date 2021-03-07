using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using EbaySoapServiceClient;
using iTextSharp.text;
using iTextSharp.text.pdf;
using LinqToDB.Common;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Html;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using OfficeOpenXml.Style;

namespace Nop.Plugin.Misc.Ebay.Services.Print
{
    /// </summary>
    public partial class CustomerPrintService : ICustomerPrintService
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGiftCardService _giftCardService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly INopFileProvider _fileProvider;
        private readonly IOrderService _orderService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly ISettingService _settingService;
        private readonly IShipmentService _shipmentService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly MeasureSettings _measureSettings;
        private readonly PdfSettings _pdfSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IDeliveryLabelService _deliveryLabelService;
        private readonly ILogger _logger;
        #endregion

        #region Ctor

        public CustomerPrintService(AddressSettings addressSettings,
            CatalogSettings catalogSettings,
            IDeliveryLabelService deliveryLabelService,
            CurrencySettings currencySettings,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressService addressService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IDateTimeHelper dateTimeHelper,
            IGiftCardService giftCardService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            INopFileProvider fileProvider,
            IOrderService orderService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            ISettingService settingService,
            IShipmentService shipmentService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            IVendorService vendorService,
            IWorkContext workContext,
            MeasureSettings measureSettings,
            PdfSettings pdfSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings, ILogger logger)
        {
            _addressSettings = addressSettings;
            _addressService = addressService;
            _catalogSettings = catalogSettings;
            _countryService = countryService;
            _currencySettings = currencySettings;
            _addressAttributeFormatter = addressAttributeFormatter;
            _currencyService = currencyService;
            _dateTimeHelper = dateTimeHelper;
            _giftCardService = giftCardService;
            _languageService = languageService;
            _localizationService = localizationService;
            _measureService = measureService;
            _fileProvider = fileProvider;
            _orderService = orderService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _settingService = settingService;
            _shipmentService = shipmentService;
            _storeContext = storeContext;
            _stateProvinceService = stateProvinceService;
            _storeService = storeService;
            _vendorService = vendorService;
            _workContext = workContext;
            _measureSettings = measureSettings;
            _pdfSettings = pdfSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
            _logger = logger;
            _deliveryLabelService = deliveryLabelService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get font
        /// </summary>
        /// <returns>Font</returns>
        protected virtual Font GetFont()
        {
            //nopCommerce supports Unicode characters
            //nopCommerce uses Free Serif font by default (~/App_Data/Pdf/FreeSerif.ttf file)
            //It was downloaded from http://savannah.gnu.org/projects/freefont
            return GetFont(_pdfSettings.FontFileName);
        }

        /// <summary>
        /// Get font
        /// </summary>
        /// <param name="fontFileName">Font file name</param>
        /// <returns>Font</returns>
        protected virtual Font GetFont(string fontFileName)
        {
            if (fontFileName == null)
                throw new ArgumentNullException(nameof(fontFileName));

            var fontPath = _fileProvider.Combine(_fileProvider.MapPath("~/App_Data/Pdf/"), fontFileName);
            var baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            var font = new Font(baseFont, 10, Font.NORMAL);
            return font;
        }


        protected virtual int GetDirection(Language lang)
        {
            return lang.Rtl ? PdfWriter.RUN_DIRECTION_RTL : PdfWriter.RUN_DIRECTION_LTR;
        }


        protected virtual int GetAlignment(Language lang, bool isOpposite = false)
        {
            //if we need the element to be opposite, like logo etc`.
            if (!isOpposite)
                return lang.Rtl ? Element.ALIGN_RIGHT : Element.ALIGN_LEFT;

            return lang.Rtl ? Element.ALIGN_LEFT : Element.ALIGN_RIGHT;
        }


        protected virtual PdfPCell GetPdfCell(string resourceKey, Language lang, Font font)
        {
            return new PdfPCell(new Phrase(_localizationService.GetResource(resourceKey, lang.Id), font));
        }


        protected virtual PdfPCell GetPdfCell(object text, Font font)
        {
            return new PdfPCell(new Phrase(text.ToString(), font));
        }

        protected virtual Paragraph GetParagraph(string resourceKey, Language lang, Font font, params object[] args)
        {
            return GetParagraph(resourceKey, string.Empty, lang, font, args);
        }

        protected virtual Paragraph GetParagraph(string resourceKey, string indent, Language lang, Font font, params object[] args)
        {
            var formatText = _localizationService.GetResource(resourceKey, lang.Id);
            return new Paragraph(indent + (args.Any() ? string.Format(formatText, args) : formatText), font);
        }


        protected virtual void PrintProducts(int vendorId, Language lang, Font titleFont, Document doc, Order order, Font font, Font attributesFont)
        {
            var productsHeader = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };
            var cellProducts = GetPdfCell("PDFInvoice.Product(s)", lang, titleFont);
            cellProducts.Border = Rectangle.NO_BORDER;
            productsHeader.AddCell(cellProducts);
            doc.Add(productsHeader);
            doc.Add(new Paragraph(" "));

            //a vendor should have access only to products
            var orderItems = _orderService.GetOrderItems(order.Id, vendorId: vendorId);

            var count = 4 + (_catalogSettings.ShowSkuOnProductDetailsPage ? 1 : 0)
                        + (_vendorSettings.ShowVendorOnOrderDetailsPage ? 1 : 0);

            var productsTable = new PdfPTable(count)
            {
                RunDirection = GetDirection(lang),
                WidthPercentage = 100f
            };

            var widths = new Dictionary<int, int[]>
            {
                { 4, new[] { 50, 20, 10, 20 } },
                { 5, new[] { 45, 15, 15, 10, 15 } },
                { 6, new[] { 40, 13, 13, 12, 10, 12 } }
            };

            productsTable.SetWidths(lang.Rtl ? widths[count].Reverse().ToArray() : widths[count]);

            //product name
            var cellProductItem = GetPdfCell("PDFInvoice.ProductName", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //SKU
            if (_catalogSettings.ShowSkuOnProductDetailsPage)
            {
                cellProductItem = GetPdfCell("PDFInvoice.SKU", lang, font);
                cellProductItem.BackgroundColor = BaseColor.LightGray;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);
            }

            //Vendor name
            if (_vendorSettings.ShowVendorOnOrderDetailsPage)
            {
                cellProductItem = GetPdfCell("PDFInvoice.VendorName", lang, font);
                cellProductItem.BackgroundColor = BaseColor.LightGray;
                cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                productsTable.AddCell(cellProductItem);
            }

            //price
            cellProductItem = GetPdfCell("PDFInvoice.ProductPrice", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //qty
            cellProductItem = GetPdfCell("PDFInvoice.ProductQuantity", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            //total
            cellProductItem = GetPdfCell("PDFInvoice.ProductTotal", lang, font);
            cellProductItem.BackgroundColor = BaseColor.LightGray;
            cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
            productsTable.AddCell(cellProductItem);

            var vendors = _vendorSettings.ShowVendorOnOrderDetailsPage ? _vendorService.GetVendorsByProductIds(orderItems.Select(item => item.ProductId).ToArray()) : new List<Vendor>();

            foreach (var orderItem in orderItems)
            {
                var product = _productService.GetProductById(orderItem.ProductId);

                var pAttribTable = new PdfPTable(1) { RunDirection = GetDirection(lang) };
                pAttribTable.DefaultCell.Border = Rectangle.NO_BORDER;

                //product name
                var name = _localizationService.GetLocalized(product, x => x.Name, lang.Id);
                pAttribTable.AddCell(new Paragraph(name, font));
                cellProductItem.AddElement(new Paragraph(name, font));
                //attributes
                if (!string.IsNullOrEmpty(orderItem.AttributeDescription))
                {
                    var attributesParagraph =
                        new Paragraph(HtmlHelper.ConvertHtmlToPlainText(orderItem.AttributeDescription, true, true),
                            attributesFont);
                    pAttribTable.AddCell(attributesParagraph);
                }

                //rental info
                if (product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalStartDateUtc.Value)
                        : string.Empty;
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue
                        ? _productService.FormatRentalDate(product, orderItem.RentalEndDateUtc.Value)
                        : string.Empty;
                    var rentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);

                    var rentalInfoParagraph = new Paragraph(rentalInfo, attributesFont);
                    pAttribTable.AddCell(rentalInfoParagraph);
                }

                productsTable.AddCell(pAttribTable);

                //SKU
                if (_catalogSettings.ShowSkuOnProductDetailsPage)
                {
                    var sku = _productService.FormatSku(product, orderItem.AttributesXml);
                    cellProductItem = GetPdfCell(sku ?? string.Empty, font);
                    cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cellProductItem);
                }

                //Vendor name
                if (_vendorSettings.ShowVendorOnOrderDetailsPage)
                {
                    var vendorName = vendors.FirstOrDefault(v => v.Id == product.VendorId)?.Name ?? string.Empty;
                    cellProductItem = GetPdfCell(vendorName, font);
                    cellProductItem.HorizontalAlignment = Element.ALIGN_CENTER;
                    productsTable.AddCell(cellProductItem);
                }

                //price
                string unitPrice;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    unitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true,
                        order.CustomerCurrencyCode, lang.Id, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    unitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true,
                        order.CustomerCurrencyCode, lang.Id, false);
                }

                cellProductItem = GetPdfCell(unitPrice, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //qty
                cellProductItem = GetPdfCell(orderItem.Quantity, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);

                //total
                string subTotal;
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var priceInclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    subTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        lang.Id, true);
                }
                else
                {
                    //excluding tax
                    var priceExclTaxInCustomerCurrency =
                        _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    subTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                        lang.Id, false);
                }

                cellProductItem = GetPdfCell(subTotal, font);
                cellProductItem.HorizontalAlignment = Element.ALIGN_LEFT;
                productsTable.AddCell(cellProductItem);
            }

            doc.Add(productsTable);
        }
        protected virtual PdfPCell GetShippingInfoCell(Language lang, Order order, Font titleFont, Font font)
        {
            var shippingAddressPdf = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang)
            };

            shippingAddressPdf.WidthPercentage = 100f;
            shippingAddressPdf.DefaultCell.Border = Rectangle.NO_BORDER;

                const string indent = "   ";
                var cellHeader = GetPdfCell(string.Format(_localizationService.GetResource("PDFInvoice.Order#", lang.Id), order.ExtendedOrderId), titleFont);
                cellHeader.Phrase.Add(new Phrase(Environment.NewLine));
                cellHeader.Border = Rectangle.NO_BORDER;
                shippingAddressPdf.AddCell(cellHeader);

                if (!order.PickupInStore)
                {
                    if (order.ShippingAddressId == null || !(_addressService.GetAddressById(order.ShippingAddressId.Value) is Address shippingAddress))
                        throw new NopException($"Shipping is required, but address is not available. Order ID = {order.ExtendedOrderId}");
                    var cell = new PdfPCell(GetParagraph("PDFInvoice.Name", indent, lang, font,
                        shippingAddress.FirstName + " " + shippingAddress.LastName)) {Border = Rectangle.NO_BORDER};
                    shippingAddressPdf.AddCell(cell);
                    if (_addressSettings.PhoneEnabled)
                        shippingAddressPdf.AddCell(new PdfPCell(GetParagraph("PDFInvoice.Phone", indent, lang, font, shippingAddress.PhoneNumber)) { Border = Rectangle.NO_BORDER });
                    if (_addressSettings.StreetAddressEnabled)
                        shippingAddressPdf.AddCell(new PdfPCell(GetParagraph("PDFInvoice.Address", indent, lang, font, shippingAddress.Address1)) { Border = Rectangle.NO_BORDER });
                    if (_addressSettings.StreetAddress2Enabled && !string.IsNullOrEmpty(shippingAddress.Address2))
                        shippingAddressPdf.AddCell(new PdfPCell(GetParagraph("PDFInvoice.Address2", indent, lang, font, shippingAddress.Address2)){ Border = Rectangle.NO_BORDER });
                    if (_addressSettings.CityEnabled || _addressSettings.StateProvinceEnabled ||
                        _addressSettings.CountyEnabled || _addressSettings.ZipPostalCodeEnabled)
                    {
                        var addressLine = $"{indent}{shippingAddress.City}, " +
                            $"{(!string.IsNullOrEmpty(shippingAddress.County) ? $"{shippingAddress.County}, " : string.Empty)}" +
                            $"{(_stateProvinceService.GetStateProvinceByAddress(shippingAddress) is StateProvince stateProvince ? _localizationService.GetLocalized(stateProvince, x => x.Name, lang.Id) : string.Empty)} " +
                            $"{shippingAddress.ZipPostalCode}";
                        shippingAddressPdf.AddCell(new PdfPCell(new Paragraph(addressLine, font)){ Border = Rectangle.NO_BORDER });
                    }

                    if (_addressSettings.CountryEnabled && _countryService.GetCountryByAddress(shippingAddress) is Country country)
                    {
                        shippingAddressPdf.AddCell(new PdfPCell(
                            new Paragraph(indent + _localizationService.GetLocalized(country, x => x.Name, lang.Id), font)){ Border = Rectangle.NO_BORDER });
                    }
                    //custom attributes
                    var customShippingAddressAttributes = _addressAttributeFormatter
                        .FormatAttributes(shippingAddress.CustomAttributes, $"<br />{indent}");
                    if (!string.IsNullOrEmpty(customShippingAddressAttributes))
                    {
                        var text = HtmlHelper.ConvertHtmlToPlainText(customShippingAddressAttributes, true, true);
                        shippingAddressPdf.AddCell(new Paragraph(indent + text, font));
                    }

                    //shippingAddressPdf.AddCell(new Paragraph(" "));
                }

                var addressCell = new PdfPCell(shippingAddressPdf) {Border = Rectangle.NO_BORDER};

                return addressCell;
        }

        protected virtual PdfPCell GetLabelCell(string filePath, Language lang, Order order, Font font, Font titleFont)
        {
            //logo

            //header
            var headerTable = new PdfPTable(1)
            {
                RunDirection = GetDirection(lang)
            };

            //var logoFilePath = _pictureService.GetThumbLocalPath(logoPicture, 0, false);
            Uri imageUri = new Uri(filePath);
            var logo = Image.GetInstance(imageUri);
            logo.Alignment = GetAlignment(lang, true);
            logo.Border = Rectangle.NO_BORDER;
            logo.ScaleToFit(100, 100);
            var pdfCelllogo = new PdfPCell
            {
                Border = Rectangle.NO_BORDER,
                VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP,
                HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT
            };
            pdfCelllogo.AddElement(logo);
            return pdfCelllogo;
        }

        private PdfPCell GetProductInformation(Order order, Font font)
        {
            var orderItems = _orderService.GetOrderItems(order.Id);
            font.Size = 10f;
            var allItemText = "Products: ";
            var productSkus = new List<string>();

            foreach (var item in orderItems)
            {
                var product = _productService.GetProductById(item.ProductId);
                if (product != null && !string.IsNullOrEmpty(product.Sku))
                {
                    var skuWithQty = $"{product.Sku} | Qty:{item.Quantity}";
                    productSkus.Add(skuWithQty);
                }
            }

            if (!productSkus.Any())
            {
                var sku = string.IsNullOrEmpty(order.Sku) ? "Unknown" : order.Sku;
                var skuWithQty = $"{sku} | Qty:{1}"; // No Order Item found default quantity to 1
                productSkus.Add(skuWithQty);
            }

            allItemText = allItemText + string.Join(", ", productSkus);

            var itemCell = new PdfPCell { Border = Rectangle.NO_BORDER };
            itemCell.AddElement(new Paragraph(allItemText,font));
            itemCell.VerticalAlignment = iTextSharp.text.Element.ALIGN_TOP;
            itemCell.HorizontalAlignment = iTextSharp.text.Element.ALIGN_RIGHT;
            return itemCell;
        }

        #endregion


        public virtual void PrintDeliveryLabelsToPdf(Stream stream, IList<Order> orders, int deliveryLabelId, string printRequestedBy, int languageId = 0)
        {
            try
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));

                if (orders == null)
                    throw new ArgumentNullException(nameof(orders));

                var pageSize = PageSize.A4;

                if (_pdfSettings.LetterPageSizeEnabled)
                {
                    pageSize = PageSize.Letter;
                }
                var doc = new Document(pageSize, 0, 0, 0, 0);
                var pdfWriter = PdfWriter.GetInstance(doc, stream);
                doc.Open();
                doc.Add(new Chunk(""));            //fonts
                var titleFont = GetFont();
                titleFont.Size = 10f;

                titleFont.SetStyle(Font.BOLD);
                titleFont.Color = BaseColor.Black;
                var font = GetFont();
                var attributesFont = GetFont();
                attributesFont.SetStyle(Font.ITALIC);

                var label = _deliveryLabelService.GetDeliveryLabelById(deliveryLabelId);

                PdfPCell blankCell = new PdfPCell(new Phrase(Chunk.Newline));
                blankCell.Border = PdfPCell.NO_BORDER;

                var mainPageTable = new PdfPTable(2);
                mainPageTable.SetWidths(new float[] { 2f, 2f });
                mainPageTable.WidthPercentage = 100;

                var orderCounter = 0;
                var printedOrders = new List<Order>();

                foreach (var order in orders)
                {
                    if (!order.ShippingAddressId.HasValue)
                        continue;
                    
                    var fullOrderTable = new PdfPTable(1);
                    fullOrderTable.AddCell(blankCell); // empty cell

                    var orderTable = new PdfPTable(2);

                    orderTable.SetWidths(new float[] { 400f, 200f });
                    var pdfSettingsByStore = _settingService.LoadSetting<PdfSettings>(order.StoreId);
                    var lang = _languageService.GetLanguageById(languageId == 0 ? order.CustomerLanguageId : languageId);
                    if (lang == null || !lang.Published)
                        lang = _workContext.WorkingLanguage;

                    var shippingCell = GetShippingInfoCell(lang, order, titleFont, font);
                    var stampCell = GetLabelCell(label.Url, lang, order, font, titleFont);
                    var productSKuCell = GetProductInformation(order, titleFont);
                    shippingCell.Border = Rectangle.NO_BORDER;
                    stampCell.Border = Rectangle.NO_BORDER;
                    productSKuCell.Border = Rectangle.NO_BORDER;
                    productSKuCell.Colspan = 2;

                    orderTable.AddCell(shippingCell);
                    orderTable.AddCell(stampCell);
                    orderTable.AddCell(productSKuCell);


                    Phrase phrase = new Phrase(@"Unit 11, Sovereign Enterprise Centre, Wigan WN13AB", font);
                    var paragraph = new Paragraph(phrase);
                    var returnAddress = new PdfPCell(paragraph);
                    returnAddress.Border = Rectangle.NO_BORDER;
                    returnAddress.VerticalAlignment = Element.ALIGN_MIDDLE;

                    var orderTableInCell = new PdfPCell(orderTable);
                    orderTableInCell.Border = Rectangle.NO_BORDER;
                    
                    fullOrderTable.AddCell(orderTableInCell);
                    fullOrderTable.AddCell(returnAddress);
                    
                    fullOrderTable.AddCell(blankCell); // empty cell

                    var fullOrderCell = new PdfPCell(fullOrderTable) {Border = Rectangle.NO_BORDER};

                    mainPageTable.AddCell(fullOrderCell);
                    

                    orderCounter++;
                    printedOrders.Add(order);
                }
                if (orderCounter % 2 != 0)
                    mainPageTable.AddCell(blankCell);

                doc.Add(mainPageTable);
                doc.Close();
                pdfWriter.Close();

                // Now Set the status of printed Orders
                SetOrderStatusToPrinted(printedOrders, printRequestedBy);
            }
            catch (Exception e)
            {
                var orderIds = string.Join(",",orders?.Select(x => x.Id.ToString()));
                var logMessage = $"Failed to Print Label(s) for Order Id(s) >> {orderIds}";
                _logger.Error(logMessage,e);
            }
        }

        private void SetOrderStatusToPrinted(List<Order> printedOrders, string printRequestedBy)
        {
            foreach (var printedOrder in printedOrders)
            {
                if (printedOrder.OrderStatus == OrderStatus.Complete) // if its already dispatched, dont bother setting its status to printed.
                    continue;
                
                printedOrder.OrderStatus = OrderStatus.Processing;  // Nop Processing is Our Printed.
                _orderService.UpdateOrder(printedOrder);

                _orderService.InsertOrderNote(new OrderNote
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    DisplayToCustomer = true,
                    Note = $"Order is Printed and its status is updated to Printed. Print requested By {printRequestedBy} on {DateTime.UtcNow}.",
                    OrderId = printedOrder.Id
                });
            }
        }
    }
}

