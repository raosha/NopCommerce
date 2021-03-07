using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Plugin.Misc.Inventory.Domains;
using Nop.Services.Catalog;

namespace Nop.Plugin.Misc.Inventory.Services
{
    public class PurchaseOrderLineService : IPurchaseOrderLineService
    {
        private readonly IRepository<PurchaseOrderLine> _orderLineRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IProductService _productService;
        public PurchaseOrderLineService(IRepository<PurchaseOrderLine> orderLineRepository, IProductService productService, IRepository<Product> productRepository)
        {
            _orderLineRepository = orderLineRepository;
            _productService = productService;
            _productRepository = productRepository;
        }
        public List<PurchaseOrderLine> GetOrderLines(int orderId)
        {
            var result = _orderLineRepository.Table.Where(x => x.PurchaseOrderId == orderId).ToList();
            return result;
        }
        public void AddOrderLine(PurchaseOrderLine orderLine)
        {
            _orderLineRepository.Insert(orderLine);
        }
        public void AddOrderLines(List<PurchaseOrderLine> orderLines)
        {
            foreach (var purchaseOrderLine in orderLines)
            {
                purchaseOrderLine.RemainingQuantity = purchaseOrderLine.OrderedQuantity;
                UpdateOrderLine(purchaseOrderLine);
            }

            _orderLineRepository.Insert(orderLines);
        }
        /// <summary>
        /// Updates the Purchase Order Line and returns current PO Line status (It could be either PartiallyReceived or Received)
        /// This method Should only be called on when Order Line Quantity Received/Delivered value is changed from UI.
        /// </summary>
        /// <param name="orderLine"></param>
        /// <returns>PurchaseOrderLineStatus</returns>
        public PurchaseOrderLineStatus UpdateOrderLine(PurchaseOrderLine orderLine)
        {
            if (orderLine.ReceivedQuantity > 0)
            {
                if (orderLine.ReceivedQuantity == orderLine.OrderedQuantity) // Order Line Products are fully received/delivered
                    orderLine.Status = PurchaseOrderLineStatus.Received;
                else if (orderLine.OrderedQuantity > orderLine.ReceivedQuantity) // Order Line Products are partially received/delivered
                    orderLine.Status = PurchaseOrderLineStatus.PartiallyReceived;

                var product = _productService.GetProductById(orderLine.ProductId);

                if (product != null)
                {
                    product.ManageInventoryMethod = ManageInventoryMethod.ManageStock;

                    if (orderLine.OrderedQuantity == orderLine.RemainingQuantity) // Means Very First lot received.
                    {
                        product.StockQuantity += orderLine.ReceivedQuantity;
                        orderLine.RemainingQuantity -= orderLine.ReceivedQuantity;
                    }
                    else
                    {
                        product.StockQuantity += ((orderLine.ReceivedQuantity + orderLine.RemainingQuantity) - orderLine.OrderedQuantity);
                        orderLine.RemainingQuantity = orderLine.OrderedQuantity - orderLine.ReceivedQuantity;
                    }

                    _productRepository.Update(product);
                }
            }
            
            orderLine.LastUpdateDate = DateTime.UtcNow;
            
            _orderLineRepository.Update(orderLine);

            return orderLine.Status;
        }
    }
}
