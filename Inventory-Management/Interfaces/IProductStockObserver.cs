using Inventory_Management.Models;

namespace Inventory_Management.Interfaces
{
    public interface IProductStockObserver
    {
        void OnLowQuantity(Product product);
    }
}
