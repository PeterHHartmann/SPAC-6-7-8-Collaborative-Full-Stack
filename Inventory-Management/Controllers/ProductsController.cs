using Inventory_Management.DTO_S;
using Inventory_Management.Factories;
using Inventory_Management.Interfaces;
using Inventory_Management.Managers;
using Inventory_Management.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Management.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly ProductManager _productManager;
        public ProductsController(ProductManager productManager)
        {
            _productManager = productManager;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? sortBy,
            [FromQuery] bool isDescending = false,
            [FromQuery] string? categoryName = null,
            [FromQuery] string? productName = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 40)
        {
            var filter = new ProductsFilterModel(
                categoryName,
                productName,
                minPrice,
                maxPrice,
                null);

            var (products, totalCount) = await _productManager.GetAllProductsAsync(sortBy, isDescending, filter, pageNumber, pageSize);

            if (products == null || !products.Any())
            {
                return NotFound("No Products found");
            }

            return Ok(new
            {
                Products = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            });
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                // Get the product by ID
                var product = await _productManager.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound("No product found with specified Id");
                }
                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/products
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO item)
        {
            // Validate the incoming data
            try
            {
                // Create a new inventory item
                var createdItem = await _productManager.CreateProduct(
                    item.ProductId,
                    item.CategoryId,
                    item.ProductName,
                    item.Price,
                    item.Quantity
                );

                // Check if the item was created successfully, if not return a bad request
                if (createdItem == null)
                {
                    return BadRequest("Failed to create the product");
                }

                return CreatedAtAction(nameof(GetProductById), new { id = createdItem.ProductId }, createdItem); // Return the created item with a 201 status code
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/products/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDTO item)
        {
            try
            {
                var updatedItem = await _productManager.UpdateProduct(id, item.ProductName, item.Price, item.CategoryId, item.Quantity);
                if (updatedItem == null)
                {
                    return NotFound("No product found with specified Id");
                }
                return Ok(updatedItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/products/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _productManager.DeleteProduct(id); // Delete the product
                return Ok($"Product with Id {id} has been deleted"); // Return success message
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Return error message if product not found
            }
        }
    }
}
