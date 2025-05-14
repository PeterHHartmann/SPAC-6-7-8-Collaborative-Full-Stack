using Inventory.Api.DataTransferObjects;
using Inventory.Api.Models;

namespace Inventory.Api.Interfaces
{
	public interface IProductFactory
	{
		IProduct CreateProductItem(int productId, int categoryId,
										string productName, decimal price, int quanitity);
	}
}
