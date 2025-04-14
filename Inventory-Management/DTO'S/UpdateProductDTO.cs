namespace Inventory_Management.DTO_S
{
    public class UpdateProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public int Quantity { get; set; }

    }
}
