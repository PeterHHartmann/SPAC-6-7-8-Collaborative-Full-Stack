using Inventory_Management.Context;
using Inventory_Management.DTO_S;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Management.Managers
{
    public class CategoryManager
    {
        private readonly InventoryDbContext _context;

        public CategoryManager(InventoryDbContext context)
        {
            _context = context;
        }

        // Returns a list of all categories with their details
        public async Task<(List<CategoryDTO> Categories, int TotalCount)> GetAllCategoriesAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Validate page number and size
                if (pageNumber <= 0)
                {
                    throw new ArgumentException("Page number must be greater than zero", nameof(pageNumber));
                }
                if (pageSize <= 0)
                {
                    throw new ArgumentException("Page size must be greater than zero", nameof(pageSize));
                }
                // Get total count
                int totalCount = await _context.Categories.CountAsync();

                // Fetch paginated categories from the database and map them to CategoryDTO
                var categories = await _context.Categories
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new CategoryDTO
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    })
                    .ToListAsync();

                return (categories, totalCount);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Invalid pagination parameters: {ex.Message}", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Database error while retrieving categories", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving categories: {ex.Message}", ex);
            }
        }

        // Returns a single category by its ID
        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            try
            {
                // Check if the category ID is valid
                if (id <= 0)
                {
                    throw new ArgumentException("Category ID must be greater than zero", nameof(id));
                }

                // Fetch a single category by ID from the database and map it to CategoryDTO
                var category = await _context.Categories
                    .Where(c => c.CategoryId == id)
                    .Select(c => new CategoryDTO
                    {
                        CategoryId = c.CategoryId,
                        CategoryName = c.CategoryName
                    })
                    .FirstOrDefaultAsync(); // Asynchronous call to fetch a single category

                if (category == null)
                {
                    // Return null instead of throwing an exception to indicate "not found"
                    return null;
                }

                return category;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException($"Database error while retrieving category with ID {id}", ex);
            }
            catch (Exception ex) when (ex is not ArgumentException)
            {
                throw new InvalidOperationException($"An error occurred while retrieving category with ID {id}: {ex.Message}", ex);
            }
        }
    }
}
