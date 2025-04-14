using Inventory_Management.Context;
using Inventory_Management.Interfaces;
using Inventory_Management.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory_Management.Managers
{
    public class OrderManager
    {
        private readonly InventoryDbContext _context;
        private readonly ProductManager _productManager;

        public OrderManager(InventoryDbContext context, ProductManager productManager)
        {
            _context = context;
            _productManager = productManager;
        }
      
        public async Task<(List<Order> Orders, int TotalCount)> GetAllOrdersAsync(int pageNumber = 1, int pageSize = 40)
        {
            try
            {
                if (pageNumber <= 0)
                {
                    throw new ArgumentException("Page number must be greater than zero", nameof(pageNumber));
                }
                if (pageSize <= 0)
                {
                    throw new ArgumentException("Page size must be greater than zero", nameof(pageSize));
                }
                // Get total count
                int totalCount = await _context.Orders.CountAsync();

                // Fetch paginated orders from the database
                var orders = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (orders, totalCount);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Invalid pagination parameters: {ex.Message}", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while retrieving orders: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving orders: {ex.Message}", ex);
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than zero", nameof(id));
                }

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {id} not found");
                }

                return order;
            }
            catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Error retrieving order with ID {id}: {ex.Message}", ex);
            }
        }

        // Get orders by customer ID, for only the logged-in customer and admin
        public async Task<List<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    throw new ArgumentException("Customer ID must be greater than zero", nameof(customerId));
                }

                // Verify that customer exists
                var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
                if (!customerExists)
                {
                    throw new InvalidOperationException($"Customer with ID {customerId} not found");
                }

                return await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .Where(o => o.CustomerId == customerId)
                    .ToListAsync();
            }
            catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Error retrieving orders for customer ID {customerId}: {ex.Message}", ex);
            }
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            try
            {
                if (order == null)
                {
                    throw new ArgumentNullException(nameof(order), "Order cannot be null");
                }

                if (order.CustomerId <= 0)
                {
                    throw new ArgumentException("Customer ID must be greater than zero", nameof(order.CustomerId));
                }

                if (order.OrderItems == null || !order.OrderItems.Any())
                {
                    throw new ArgumentException("Order must contain at least one item", nameof(order.OrderItems));
                }

                // Verify that customer exists
                var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == order.CustomerId);
                if (!customerExists)
                {
                    throw new InvalidOperationException($"Customer with ID {order.CustomerId} not found");
                }

                // Verify product existence and check stock availability
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(orderItem.ProductId);
                    if (product == null)
                    {
                        throw new InvalidOperationException($"Product with ID {orderItem.ProductId} not found");
                    }

                    if (product.Quantity < orderItem.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product '{product.ProductName}'. Available: {product.Quantity}, Requested: {orderItem.Quantity}");
                    }
                }

                // Add the order
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Update product quantities
                foreach (var orderItem in order.OrderItems)
                {
                    var product = await _context.Products.FindAsync(orderItem.ProductId);
                    await _productManager.UpdateProductQuantity(
                        orderItem.ProductId,
                        product.Quantity - orderItem.Quantity
                    );
                }

                return order;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while creating order: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException && ex is not ArgumentNullException)
            {
                throw new InvalidOperationException($"Error creating order: {ex.Message}", ex);
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than zero", nameof(id));
                }

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {id} not found");
                }

                // If order is completed, we might need to restore product quantities
                if (order.OrderStatus == Order.Status.Completed)
                {
                    // Optional: Restore product quantities
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = await _context.Products.FindAsync(orderItem.ProductId);
                        if (product != null)
                        {
                            await _productManager.UpdateProductQuantity(
                                orderItem.ProductId,
                                product.Quantity + orderItem.Quantity
                            );
                        }
                    }
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while deleting order: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Error deleting order with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<Order> UpdateOrderStatusAsync(int id, int newStatus)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Order ID must be greater than zero", nameof(id));
                }

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == id);

                if (order == null)
                {
                    throw new InvalidOperationException($"Order with ID {id} not found");
                }

                if (!Enum.IsDefined(typeof(Order.Status), newStatus))
                {
                    throw new ArgumentException(
                        $"Invalid status value {newStatus}. Must be {(int)Order.Status.Pending} (Pending), " +
                        $"{(int)Order.Status.Completed} (Completed), or {(int)Order.Status.Cancelled} (Cancelled).",
                        nameof(newStatus)
                    );
                }

                // If changing from pending to cancelled, restore product quantities
                if (order.OrderStatus == Order.Status.Pending && (Order.Status)newStatus == Order.Status.Cancelled)
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = await _context.Products.FindAsync(orderItem.ProductId);
                        if (product != null)
                        {
                            await _productManager.UpdateProductQuantity(
                                orderItem.ProductId,
                                product.Quantity + orderItem.Quantity
                            );
                        }
                    }
                }

                order.OrderStatus = (Order.Status)newStatus;
                await _context.SaveChangesAsync();
                return order;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while updating order status: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not InvalidOperationException && ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Error updating status for order with ID {id}: {ex.Message}", ex);
            }
        }
    }
}
