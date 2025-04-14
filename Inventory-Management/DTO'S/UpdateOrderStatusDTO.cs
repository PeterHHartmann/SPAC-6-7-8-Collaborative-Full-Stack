using Inventory_Management.Model;

namespace Inventory_Management.DTO_S
{
    public class UpdateOrderStatusDTO
    {
        public int NewStatus { get; set; }  // Changed to int to match the enum values directly
    }
} 