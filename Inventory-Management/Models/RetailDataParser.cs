using CsvHelper;
using CsvHelper.Configuration;
using Inventory_Management.Context;
using Inventory_Management.Model;
using System.Formats.Asn1;
using System.Globalization;

namespace Inventory_Management.Models
{
    public class RetailDataParser
    {
        private readonly InventoryDbContext _context;

        public RetailDataParser(InventoryDbContext context)
        {
            _context = context;
        }

        public Order.Status GenerateRandomStatus()
        {
            var statuses = new List<Order.Status> { Order.Status.Pending, Order.Status.Completed, Order.Status.Cancelled };
            Random random = new Random();
            int index = random.Next(statuses.Count);
            return statuses[index];
        }

        public int GenerateRandomQuantity()
        {
            Random random = new Random();
            return random.Next(1, 100); // Random quantity between 1 and 100
        }

        public void ParseAndSaveData(string csvFilePath)
        {
            // Dictionary to track orders by a composite key (customer_id + order_date)
            var orderTracker = new Dictionary<string, Order>();
            // Dictionary to track products by product_id
            var productTracker = new Dictionary<int, Product>();
            // Dictionary to track categories by category_id
            var categoryTracker = new Dictionary<int, Category>();
            // Dictionary to track customers by customer_id
            var customerTracker = new Dictionary<int, Customer>();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null
            };

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, config))
            {
                // Read all records from the CSV
                var records = csv.GetRecords<CsvRetailRecord>().ToList();

                foreach (var record in records)
                {
                    // Process category if it doesn't exist in our tracker
                    if (!categoryTracker.ContainsKey(record.category_id))
                    {
                        var category = new Category
                        {
                            CategoryId = record.category_id,
                            CategoryName = record.category_name
                        };
                        categoryTracker[record.category_id] = category;
                    }

                    // Process product if it doesn't exist in our tracker
                    if (!productTracker.ContainsKey(record.product_id))
                    {
                        var product = new Product
                        {                
                            ProductName = record.product_name,
                            Price = (decimal)record.price,
                            CategoryId = record.category_id,
                            Category = categoryTracker[record.category_id],
                            Quantity = GenerateRandomQuantity()
                        };
                        productTracker[record.product_id] = product;
                    }

                    // Process customer if it doesn't exist in our tracker
                    if (!customerTracker.ContainsKey(record.customer_id))
                    {
                        var customer = new Customer
                        {
                            CustomerId = record.customer_id,
                            City = record.city
                        };
                        customerTracker[record.customer_id] = customer;
                    }

                    // Generate a composite key for orders
                    string orderKey = $"{record.customer_id}_{record.order_date:yyyy-MM-dd}";

                    // Process order if it doesn't exist in our tracker
                    if (!orderTracker.ContainsKey(orderKey))
                    {
                        var order = new Order
                        {
                            CustomerId = record.customer_id,
                            Customer = customerTracker[record.customer_id],
                            OrderDate = record.order_date,
                            PaymentMethod = record.payment_method,
                            OrderStatus = GenerateRandomStatus(),
                        };
                        orderTracker[orderKey] = order;
                    }

                    // Create and add the order item
                    var orderItem = new OrderItem
                    {
                        Order = orderTracker[orderKey],
                        ProductId = record.product_id,
                        Product = productTracker[record.product_id],
                        Quantity = record.quantity
                    };

                    // Add the order item to the order
                    orderTracker[orderKey].OrderItems.Add(orderItem);
                }

                // Save all entities to the database
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        // Add categories
                        _context.Categories.AddRange(categoryTracker.Values);
                        _context.SaveChanges();

                        // Add products
                        _context.Products.AddRange(productTracker.Values);
                        _context.SaveChanges();

                        // Add customers
                        _context.Customers.AddRange(customerTracker.Values);
                        _context.SaveChanges();

                        // Add orders and order items
                        _context.Orders.AddRange(orderTracker.Values);
                        _context.SaveChanges();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Error saving data to database", ex);
                    }
                }
            }
        }
    }
}
