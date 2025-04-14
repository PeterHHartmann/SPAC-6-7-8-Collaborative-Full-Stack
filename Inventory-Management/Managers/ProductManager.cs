using Inventory_Management.Context;
using Inventory_Management.DTO_S;
using Inventory_Management.Factories;
using Inventory_Management.Interfaces;
using Inventory_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.Managers
{
    public class ProductManager
    {
        private readonly InventoryDbContext _context;
        private readonly IProductFactory _productFactory;
        private readonly IProductStockObserver _productStockObserver;

        public ProductManager(InventoryDbContext context, IProductFactory productFactory, IProductStockObserver productStockObserver)
        {
            _context = context;
            _productFactory = productFactory;
            _productStockObserver = productStockObserver;
        }

        // Returns a list of all products with their details and category information 
        public async Task<(List<ProductDTO> Products, int TotalCount)> GetAllProductsAsync(
            string? sortBy,
            bool isDescending,
            ProductsFilterModel? filter = null,
            int pageNumber = 1,
            int pageSize = 40)
        {
            try
            {
                // Start with the base query including the Category
                IQueryable<Product> productsQuery = _context.Products
                    .Include(p => p.Category);

                // Apply filters
                productsQuery = ApplyFilters(productsQuery, filter);

                // Get total count before pagination
                int totalCount = await productsQuery.CountAsync();

                // Apply pagination before the Select transformation
                productsQuery = productsQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                // Convert to DTOs and apply sorting
                var productDtos = await ConvertToProductDTOs(productsQuery);
                productDtos = ApplySorting(productDtos, sortBy, isDescending);

                return (productDtos, totalCount);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while retrieving products: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving products: {ex.Message}", ex);
            }
        }

        private IQueryable<Product> ApplyFilters(IQueryable<Product> query, ProductsFilterModel? filter)
        {
            if (filter == null)
                return query;

            if (!string.IsNullOrWhiteSpace(filter.ProductName))
            {
                query = query.Where(p => p.ProductName.Contains(filter.ProductName));
            }

            if (!string.IsNullOrWhiteSpace(filter.CategoryName))
            {
                query = query.Where(p => p.Category.CategoryName.Contains(filter.CategoryName));
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);
            }

            if (filter.Category != null)
            {
                query = query.Where(p => p.CategoryId == filter.Category.CategoryId);
            }

            return query;
        }

        private async Task<List<ProductDTO>> ConvertToProductDTOs(IQueryable<Product> query)
        {
            try
            {
                return await query
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        Category = new CategoryDTO
                        {
                            CategoryId = p.Category.CategoryId,
                            CategoryName = p.Category.CategoryName
                        },
                        Quantity = p.Quantity
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error converting products to DTOs: {ex.Message}", ex);
            }
        }

        private List<ProductDTO> ApplySorting(List<ProductDTO> products, string? sortBy, bool isDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return products;

            try
            {
                return sortBy.ToLowerInvariant() switch
                {
                    "productname" => isDescending
                        ? products.OrderByDescending(p => p.ProductName).ToList()
                        : products.OrderBy(p => p.ProductName).ToList(),

                    "price" => isDescending
                        ? products.OrderByDescending(p => p.Price).ToList()
                        : products.OrderBy(p => p.Price).ToList(),

                    "categoryname" => isDescending
                        ? products.OrderByDescending(p => p.Category.CategoryName).ToList()
                        : products.OrderBy(p => p.Category.CategoryName).ToList(),

                    "productid" => isDescending
                        ? products.OrderByDescending(p => p.ProductId).ToList()
                        : products.OrderBy(p => p.ProductId).ToList(),

                    "id" => isDescending
                        ? products.OrderByDescending(p => p.ProductId).ToList()
                        : products.OrderBy(p => p.ProductId).ToList(),

                    "categoryid" => isDescending
                        ? products.OrderByDescending(p => p.Category.CategoryId).ToList()
                        : products.OrderBy(p => p.Category.CategoryId).ToList(),

                    "quantity" => isDescending
                        ? products.OrderByDescending(p => p.Quantity).ToList()
                        : products.OrderBy(p => p.Quantity).ToList(),

                    _ => throw new ArgumentException($"Invalid sort parameter: {sortBy}")
                };
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Error sorting products: {ex.Message}", ex);
            }
        }

        // Returns a single product by its ID with category information
        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Product ID must be greater than zero", nameof(id));
                }

                // Fetch a single product by ID from the database and map it to ProductDTO
                var product = await _context.Products
                    .Where(p => p.ProductId == id)
                    .Include(p => p.Category)
                    .Select(p => new ProductDTO
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        Category = new CategoryDTO
                        {
                            CategoryId = p.Category.CategoryId,
                            CategoryName = p.Category.CategoryName
                        }
                    })
                    .FirstOrDefaultAsync(); // Asynchronous call to fetch a single product

                return product;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error retrieving product with ID {id}: {ex.Message}", ex);
            }
        }

        // Creates a new inventory item and saves it to the database
        public async Task<IProduct> CreateProduct(int productId, int categoryId,
                                                 string productName, decimal price, int quantity)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productName))
                {
                    throw new ArgumentException("Product name cannot be empty", nameof(productName));
                }

                if (price < 0)
                {
                    throw new ArgumentException("Price cannot be negative", nameof(price));
                }

                // Verify that the category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
                if (!categoryExists)
                {
                    throw new ArgumentException($"Category with ID {categoryId} does not exist", nameof(categoryId));
                }

                // Generate a new product ID by finding the highest existing ID and adding 1
                int newProductId = 1;
                if (await _context.Products.AnyAsync())
                {
                    newProductId = await _context.Products.MaxAsync(p => p.ProductId) + 1;
                }


                // Create domain object from factory
                var item = _productFactory.CreateProductItem(newProductId, categoryId, productName, price, quantity);

                // Map from domain model to database entity
                var productEntity = new Product
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    CategoryId = item.CategoryId,
                    Quantity = 0 // Initialize quantity to zero for new products
                };

                // Check if the product already exists in the database
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.ProductName == productEntity.ProductName);

                if (existingProduct != null)
                {
                    // If the product already exists, return the existing product
                    return (IProduct)existingProduct;
                }

                // Add entity to database
                _context.Products.Add(productEntity);
                await _context.SaveChangesAsync();

                // Return domain object
                return item;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while creating product: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new InvalidOperationException($"Error creating product: {ex.Message}", ex);
            }
        }

        // Updates an existing product in the database
        public async Task<Product> UpdateProduct(int productId, string productName, decimal price, int? categoryId, int quantity)
        {
            try
            {
                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID must be greater than zero", nameof(productId));
                }

                if (string.IsNullOrWhiteSpace(productName))
                {
                    throw new ArgumentException("Product name cannot be empty", nameof(productName));
                }

                if (price < 0)
                {
                    throw new ArgumentException("Price cannot be negative", nameof(price));
                }

                if (categoryId <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than zero", nameof(categoryId));
                }

                if (quantity < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative", nameof(quantity));
                }

                // Fetch the product from the database
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {productId} not found");
                }

                // Verify that the category exists if one is provided
                if (categoryId.HasValue && categoryId.Value > 0)
                {
                    var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == categoryId.Value);
                    if (!categoryExists)
                    {
                        throw new ArgumentException($"Category with ID {categoryId.Value} does not exist", nameof(categoryId));
                    }
                    product.CategoryId = categoryId.Value;
                }

                // Update the product details
                product.ProductName = productName;
                product.Price = price;
                product.Quantity = quantity;
                UpdateProductQuantity(productId, quantity).Wait(); // Update the quantity in the database

                // Save changes to the database
                await _context.SaveChangesAsync();
                return product; // Update successful
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while updating product: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error updating product: {ex.Message}", ex);
            }
        }

        // Deletes a product from the database
        public async Task DeleteProduct(int productId)
        {
            try
            {
                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID must be greater than zero", nameof(productId));
                }

                // Fetch the product from the database
                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {productId} not found");
                }

                // Check if product has related records
                bool hasRelatedOrderItems = await _context.OrderItems.AnyAsync(oi => oi.ProductId == productId);
                bool hasRelatedTransactions = await _context.InventoryTransactions.AnyAsync(it => it.ProductId == productId);

                if (hasRelatedOrderItems || hasRelatedTransactions)
                {
                    throw new InvalidOperationException($"Cannot delete product with ID {productId} because it has related order items or inventory transactions");
                }

                // Remove the product from the database
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while deleting product: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error deleting product: {ex.Message}", ex);
            }
        }

        public async Task UpdateProductQuantity(int productId, int newQuantity)
        {
            try
            {
                if (productId <= 0)
                {
                    throw new ArgumentException("Product ID must be greater than zero", nameof(productId));
                }

                if (newQuantity < 0)
                {
                    throw new ArgumentException("Quantity cannot be negative", nameof(newQuantity));
                }

                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Product with ID {productId} not found");
                }

                product.AttachObserver(_productStockObserver);
                product.Quantity = newQuantity;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while updating product quantity: {ex.Message}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException && ex is not InvalidOperationException)
            {
                throw new InvalidOperationException($"Error updating product quantity: {ex.Message}", ex);
            }
        }
    }
}
