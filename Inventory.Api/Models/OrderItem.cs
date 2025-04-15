using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Inventory.Api.Models;
using System.Text.Json.Serialization;

namespace Inventory.Api.Models
{
	public class OrderItem
	{
		[Key]
		public int OrderItemId { get; set; }

		public int OrderId { get; set; }

		public int ProductId { get; set; }

		public int Quantity { get; set; }

		// Navigation properties
		[JsonIgnore]
		[ForeignKey("OrderId")]
		public virtual Order Order { get; set; }

		[JsonIgnore]
		[ForeignKey("ProductId")]
		public virtual Product Product { get; set; }
	}
}
