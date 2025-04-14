using Inventory_Management.Context;
using Inventory_Management.DTO;
using Inventory_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.Managers
{
    public class InventoryTransactionManager
    {
        private readonly InventoryDbContext _context;

        public InventoryTransactionManager(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryTransaction> CreateTransactionAsync(CreateInventoryTransactionDTO dto, int customerId)
        {
            try
            {
                if (dto == null)
                {
                    throw new ArgumentNullException(nameof(dto), "Transaction data cannot be null");
                }

                if (dto.ProductId <= 0)
                {
                    throw new ArgumentException("Product ID must be greater than zero", nameof(dto.ProductId));
                }

                if (customerId <= 0)
                {
                    throw new ArgumentException("Customer ID must be greater than zero", nameof(customerId));
                }

                if (string.IsNullOrWhiteSpace(dto.TransactionType))
                {
                    throw new ArgumentException("Transaction type cannot be empty", nameof(dto.TransactionType));
                }

                if (dto.Quantity <= 0)
                {
                    throw new ArgumentException("Quantity must be greater than zero", nameof(dto.Quantity));
                }

                // Find product with tracking enabled
                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {dto.ProductId} not found");
                }

                // Find customer with tracking enabled
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    throw new InvalidOperationException($"Customer with ID {customerId} not found");
                }

                // For sales, check if we have enough stock
                var normalizedType = dto.TransactionType.ToLower();
                if (normalizedType == "sale" && product.Quantity < dto.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product '{product.ProductName}'. Available: {product.Quantity}, Requested: {dto.Quantity}");
                }

                var transaction = new InventoryTransaction
                {
                    ProductId = dto.ProductId,
                    Product = product,
                    CustomerId = customerId,
                    Customer = customer,
                    TransactionType = dto.TransactionType.ToUpperInvariant(), // Standardize to uppercase
                    Quantity = dto.Quantity,
                    TransactionDate = DateTime.UtcNow
                };

                // Update product quantity based on transaction type
                switch (normalizedType)
                {
                    case "sale":
                        product.Quantity -= dto.Quantity;
                        break;
                    case "return":
                    case "transfer":
                        product.Quantity += dto.Quantity;
                        break;
                }

                _context.InventoryTransactions.Add(transaction);
                await _context.SaveChangesAsync();

                return transaction;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while creating transaction: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentNullException &&
                                      ex is not ArgumentException &&
                                      ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error creating transaction: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsAsync(
            string? productName = null,
            string? transactionType = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? customerId = null)
        {
            try
            {
                IQueryable<InventoryTransaction> query = _context.InventoryTransactions
                    .Include(t => t.Product)
                    .Include(t => t.Customer);

                // Apply filters
                if (!string.IsNullOrWhiteSpace(productName))
                {
                    query = query.Where(t => t.Product.ProductName.Contains(productName));
                }

                if (!string.IsNullOrWhiteSpace(transactionType))
                {
                    query = query.Where(t => t.TransactionType.ToLower() == transactionType.ToLower());
                }

                if (startDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate <= endDate.Value.AddDays(1).AddSeconds(-1));
                }

                if (customerId.HasValue && customerId.Value > 0)
                {
                    query = query.Where(t => t.CustomerId == customerId.Value);
                }

                return await query.OrderByDescending(t => t.TransactionDate).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving transactions: {ex.Message}", ex);
            }
        }

        public async Task<InventoryTransaction> GetTransactionByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Transaction ID must be greater than zero", nameof(id));
                }

                var transaction = await _context.InventoryTransactions
                    .Include(t => t.Product)
                    .Include(t => t.Customer)
                    .FirstOrDefaultAsync(t => t.TransactionId == id);

                if (transaction == null)
                {
                    throw new InvalidOperationException($"Transaction with ID {id} not found");
                }

                return transaction;
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving transaction with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByProductIdAsync(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID must be greater than zero", nameof(productId));
                }

                // Check if product exists
                bool productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
                if (!productExists)
                {
                    throw new InvalidOperationException($"Product with ID {productId} not found");
                }

                return await _context.InventoryTransactions
                    .Include(t => t.Customer)
                    .Where(t => t.ProductId == productId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving transactions for product ID {productId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByCustomerIdAsync(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    throw new ArgumentException("Customer ID must be greater than zero", nameof(customerId));
                }

                // Check if customer exists
                bool customerExists = await _context.Customers.AnyAsync(u => u.CustomerId == customerId);
                if (!customerExists)
                {
                    throw new InvalidOperationException($"Customer with ID {customerId} not found");
                }

                return await _context.InventoryTransactions
                    .Include(t => t.Product)
                    .Where(t => t.CustomerId == customerId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error retrieving transactions for customer ID {customerId}: {ex.Message}", ex);
            }
        }
    }
}