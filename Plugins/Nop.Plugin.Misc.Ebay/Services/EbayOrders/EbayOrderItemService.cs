using System;
using System.Threading.Tasks;
using EbaySoapServiceClient;
using Newtonsoft.Json;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.Ebay.Domains;
using Nop.Plugin.Misc.Ebay.Models.EbayRequests;
using Nop.Plugin.Misc.Ebay.Services.EbayExternal;
using Nop.Services.Catalog;
using Nop.Services.Logging;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.Ebay.Services.EbayOrders
{
    public class EbayOrderItemService : IEbayOrderItemService
    {

        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        private readonly IExternalEbayOrderService _ebayWebService;
        private readonly IEbayCustomItemService _ebayCustomItemService;

        public EbayOrderItemService(IOrderService orderService, 
            IProductService productService, ILogger logger,
            IExternalEbayOrderService ebayWebService, IEbayCustomItemService ebayCustomItemService)
        {
            _orderService = orderService;
            _productService = productService;
            _logger = logger;
            _ebayWebService = ebayWebService;
            _ebayCustomItemService = ebayCustomItemService;
        }
        
        public  async Task<Product> CreateOrderLine(TransactionType trans, Order order, EbayOrderRequestModel requestModel)
        {
            try
            {
                var product =  await GetProductByItemSku(trans, requestModel);
                if (product != null)
                {
                    var price = Convert.ToDecimal(trans.TransactionPrice.Value);
                    var val = new OrderItem
                    {
                        OrderItemGuid = Guid.NewGuid(),
                        UnitPriceInclTax = Convert.ToDecimal(trans.TransactionPrice.Value),
                        UnitPriceExclTax = price,
                        PriceInclTax = Convert.ToDecimal(trans.TransactionPrice.Value) *
                                       trans.QuantityPurchased,
                        PriceExclTax = price * trans.QuantityPurchased,
                        Quantity = trans.QuantityPurchased,
                        OrderId = order.Id,
                        ProductId = product.Id,
                        LicenseDownloadId = 0,
                        ItemWeight = product.Weight * trans.QuantityPurchased,
                        OriginalProductCost = product.ProductCost
                    };
                    var orderItem = val;
                    _orderService.InsertOrderItem(orderItem);
                    _productService.AdjustInventory(product, -trans.QuantityPurchased); 
                }
                return product;
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, $"Import order (CreateOrderLine): {ex.Message}", $"{JsonConvert.SerializeObject(ex)}");
                return null;
            }
        }

        public async Task<Product> GetProductByItemSku(TransactionType trans, EbayOrderRequestModel requestModel)
        {
            string sku;
            if (string.IsNullOrEmpty(trans.Item.SKU))
            {
                var itemProduct = await _ebayWebService.GetItem(requestModel,trans.Item.ItemID).ConfigureAwait(false);
                sku = itemProduct.Item.SKU;
            }
            else
                sku = trans.Item.SKU;

            if (!string.IsNullOrEmpty(sku))
            {
                var product = _productService.GetProductBySku(sku);
                if (product != null)
                    return product;
                else
                    return null;
            }
            else
                return await InsertProduct(trans, requestModel);
        }

        public async Task<Product> InsertProduct(TransactionType trans, EbayOrderRequestModel requestModel)
        {
            var itemProduct = (await _ebayWebService.GetItem(requestModel, trans.Item.ItemID).ConfigureAwait(false)).Item;
            var product = new Product
            {
                AdminComment = "Import from eBay",
                Sku = itemProduct.SKU,
                Name = itemProduct.Title,
                Price = itemProduct.BuyItNowPrice != null ? Convert.ToDecimal(itemProduct.BuyItNowPrice.Value) : 0m,
                FullDescription = itemProduct.Description
            };
            if (product.Price == 0m)
            {
                product.Price=itemProduct.StartPrice != null ? Convert.ToDecimal(itemProduct.StartPrice.Value) : 0m;
            }
            product.CreatedOnUtc=DateTime.UtcNow;
            product.UpdatedOnUtc=DateTime.UtcNow;
            product.Published=false;
            product.StockQuantity=itemProduct.Quantity;
            product.ProductType=(ProductType)5;
            product.ProductTemplateId=1;
            product.VisibleIndividually=true;
            //  product.StockQuantity=(itemProduct.Quantity);
            _productService.InsertProduct(product);

            return product;
        }



    }
}
