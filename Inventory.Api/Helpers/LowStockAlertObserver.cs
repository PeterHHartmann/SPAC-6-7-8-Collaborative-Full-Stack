using Inventory.Api.Interfaces;
using Inventory.Api.Models;

namespace Inventory.Api.Helpers
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
			var sanitizedProductName = product.ProductName.Replace(Environment.NewLine, "").Replace("\n", "").Replace("\r", "");
			_logger.LogWarning($"Low inventory alert: Product {sanitizedProductName} (ID: {product.ProductId}) has quantity {product.Quantity}");
		}
	}
}
