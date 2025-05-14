using Inventory.Api.Models;

namespace Inventory.Api.Interfaces
{
	public interface IProduct
	{
		int ProductId { get; set; }
		int CategoryId { get; set; }
		string ProductName { get; set; }
		decimal Price { get; set; }
		int Quantity { get; set; }
	}
}
