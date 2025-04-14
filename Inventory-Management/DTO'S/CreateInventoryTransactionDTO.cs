using System.ComponentModel.DataAnnotations;

namespace Inventory_Management.DTO
{
    public class CreateInventoryTransactionDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [RegularExpression("^(Sale|Return|Transfer)$", ErrorMessage = "TransactionType must be either 'Sale', 'Return', or 'Transfer'")]
        public string TransactionType { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        // UserId will be set from the authenticated user
    }
} 