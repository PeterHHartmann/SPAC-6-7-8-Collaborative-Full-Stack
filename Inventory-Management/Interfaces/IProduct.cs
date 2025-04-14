using Inventory_Management.Models;

namespace Inventory_Management.Interfaces
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
