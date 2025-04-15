namespace Inventory.Api.DataTransferObjects
{
	public class OrderCreateDTO
	{
		public int CustomerId { get; set; }
		public string PaymentMethod { get; set; }
		public List<OrderItemDTO> OrderItems { get; set; }
	}

	public class OrderItemDTO
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
	}
}