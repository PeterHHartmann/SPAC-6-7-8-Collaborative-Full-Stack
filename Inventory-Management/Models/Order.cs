using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Inventory_Management.Model
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        [Column(TypeName = "date")]
        public DateTime OrderDate { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; }

        public enum Status
        {
            Pending,
            Completed,
            Cancelled
        }

        public Status OrderStatus { get; set; }

        // Navigation properties
        [JsonIgnore]
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
