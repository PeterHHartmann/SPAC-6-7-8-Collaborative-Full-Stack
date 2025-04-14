namespace Inventory_Management.Models
{
    public class ProductsFilterModel
    {
        public string? CategoryName { get; set; }
        public string? ProductName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public Category? Category { get; set; }
        public ProductsFilterModel(string? categoryName, string? productName, decimal? minPrice, decimal? maxPrice, Category? category)
        {
            CategoryName = categoryName;
            ProductName = productName;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            Category = category;
        }
    }
}
