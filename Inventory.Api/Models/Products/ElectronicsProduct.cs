using Inventory.Api.DataTransferObjects;
using Inventory.Api.Interfaces;

namespace Inventory.Api.Models.Products
{
	public class ElectronicsProduct : IProduct
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public decimal Price { get; set; }
		public int CategoryId { get; set; }
		public int Quantity { get; set; }

	}
}
