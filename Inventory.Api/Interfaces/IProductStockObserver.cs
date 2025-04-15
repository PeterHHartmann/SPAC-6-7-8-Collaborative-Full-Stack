using Inventory.Api.Models;

namespace Inventory.Api.Interfaces
{
	public interface IProductStockObserver
	{
		void OnLowQuantity(Product product);
	}
}
