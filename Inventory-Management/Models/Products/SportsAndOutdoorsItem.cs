using Inventory_Management.Interfaces;

namespace Inventory_Management.Models.Products
{
    public class SportsAndOutdoorsItem : IProduct
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
