using Inventory.Api.Context;
using Inventory.Api.DataTransferObjects;
using Inventory.Api.Interfaces;
using Inventory.Api.Models;
using Inventory.Api.Models.Products;

namespace Inventory.Api.Factories
{
	public class ProductFactory : IProductFactory
	{

		// This method creates a product item based on the category ID
		public IProduct CreateProductItem(int productId, int categoryId,
												 string productName, decimal price, int quantity)
		{
			switch (categoryId) // Category ID is used to determine the type of product
			{
				case 10:
					return new ElectronicsProduct
					{
						ProductId = productId,
						CategoryId = categoryId,
						ProductName = productName,
						Price = price,
						Quantity = quantity,
					};

				case 20:
					return new FashionItem
					{
						ProductId = productId,
						CategoryId = categoryId,
						ProductName = productName,
						Price = price,
						Quantity = quantity,
					};

				case 30:
					return new HomeAndLivingItem
					{
						ProductId = productId,
						CategoryId = categoryId,
						ProductName = productName,
						Price = price,
						Quantity = quantity,
					};

				case 40:
					return new BooksAndStationeryItem
					{
						ProductId = productId,
						CategoryId = categoryId,
						ProductName = productName,
						Price = price,
						Quantity = quantity,

					};

				case 50:
					return new SportsAndOutdoorsItem
					{
						ProductId = productId,
						CategoryId = categoryId,
						ProductName = productName,
						Price = price,
						Quantity = quantity,
					};

				default:
					throw new ArgumentException($"Unsupported category: {categoryId}");
			}
		}
	}
}
