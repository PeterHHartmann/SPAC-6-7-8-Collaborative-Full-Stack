using Inventory_Management.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Inventory_Management.Interfaces;

namespace Inventory_Management.Models
{
    public class Product
    {
        private const int LOW_QUANTITY_THRESHOLD = 10;
        private readonly List<IProductStockObserver> _observers = new();
        private int quantity;

        [Key]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public int Quantity
        {
            get => quantity;
            set
            {
                quantity = value;
                if (quantity <= LOW_QUANTITY_THRESHOLD)
                {
                    NotifyObservers();
                }
            }
        }

        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

        public void AttachObserver(IProductStockObserver observer)
        {
            _observers.Add(observer);
        }

        public void DetachObserver(IProductStockObserver observer)
        {
            _observers.Remove(observer);
        }

        private void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.OnLowQuantity(this);
            }
        }
    }
}
