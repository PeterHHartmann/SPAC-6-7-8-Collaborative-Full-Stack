namespace Inventory_Management.DTOs
{
    public class OrderCreateDto
    {
        public int CustomerId { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
} 