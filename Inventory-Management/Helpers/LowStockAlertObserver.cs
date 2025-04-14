using Inventory_Management.Interfaces;
using Inventory_Management.Models;

namespace Inventory_Management.Helpers
{
    public class LowStockAlertObserver : IProductStockObserver
    {
        private readonly ILogger<LowStockAlertObserver> _logger;

        public LowStockAlertObserver(ILogger<LowStockAlertObserver> logger)
        {
            _logger = logger;
        }

        public void OnLowQuantity(Product product)
        {
            _logger.LogWarning($"Low inventory alert: Product {product.ProductName} (ID: {product.ProductId}) has quantity {product.Quantity}");
        }
    }
}
