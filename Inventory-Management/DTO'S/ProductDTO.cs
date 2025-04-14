namespace Inventory_Management.DTO_S
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public CategoryDTO Category { get; set; }

        public int Quantity { get; set; }
    }
}
