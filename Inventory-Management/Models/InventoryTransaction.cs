using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory_Management.Model;
using Inventory_Management.Models;

public class InventoryTransaction
{
    [Key]
    public int TransactionId { get; set; }
    
    public int ProductId { get; set; }
    public required string TransactionType { get; set; } // 'SALE', 'RETURN', 'TRANSFER'
    public int Quantity { get; set; }
    public DateTime TransactionDate { get; set; }
    public int CustomerId { get; set; }

    // Navigation properties
    [ForeignKey("ProductId")]
    public required virtual Product Product { get; set; }
    
    [ForeignKey("CustomerId")]
    public required virtual Customer Customer { get; set; }
}