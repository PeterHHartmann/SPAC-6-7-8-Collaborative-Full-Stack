using Inventory_Management.DTO_S;
using Inventory_Management.Models;

namespace Inventory_Management.Interfaces
{
    public interface IProductFactory
    {
        IProduct CreateProductItem(int productId, int categoryId,
                                        string productName, decimal price, int quanitity);
    }
}
