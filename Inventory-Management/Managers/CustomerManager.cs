using Inventory_Management.Context;
using Inventory_Management.Model;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.Managers
{
    public class CustomerManager
    {
        private readonly InventoryDbContext _context;

        public CustomerManager(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    throw new ArgumentException("Customer ID must be greater than zero", nameof(customerId));
                }
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    throw new InvalidOperationException($"Customer with ID {customerId} not found");
                }
                return customer;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error while retrieving customer", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving customer: {ex.Message}", ex);
            }
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            try
            {
                var customers = await _context.Customers.ToListAsync();
                if (customers == null || !customers.Any())
                {
                    throw new InvalidOperationException("No customers found");
                }
                return customers;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error while retrieving customers", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving customers: {ex.Message}", ex);
            }
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
                }
                await _context.Customers.AddAsync(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error while creating customer", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while creating customer: {ex.Message}", ex);
            }
        }

        public async Task<Customer> UpdateCustomerAsync(int customerId, Customer updatedCustomer)
        {
            try
            {
                if (customerId <= 0)
                {
                    throw new ArgumentException("Customer ID must be greater than zero", nameof(customerId));
                }
                if (updatedCustomer == null)
                {
                    throw new ArgumentNullException(nameof(updatedCustomer), "Updated customer cannot be null");
                }
                var existingCustomer = await _context.Customers.FindAsync(customerId);
                if (existingCustomer == null)
                {
                    throw new InvalidOperationException($"Customer with ID {customerId} not found");
                }
                existingCustomer.City = updatedCustomer.City;
                _context.Customers.Update(existingCustomer);
                await _context.SaveChangesAsync();
                return existingCustomer;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error while updating customer", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while updating customer: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try
            {
                if (customerId <= 0)
                {
                    throw new ArgumentException("Customer ID must be greater than zero", nameof(customerId));
                }
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer == null)
                {
                    throw new InvalidOperationException($"Customer with ID {customerId} not found");
                }
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error while deleting customer", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting customer: {ex.Message}", ex);
            }
        }
    }
}
