namespace Inventory_Management.Models
{
    public class CsvRetailRecord
    {
        public int customer_id { get; set; }
        public DateTime order_date { get; set; }
        public int product_id { get; set; }
        public int category_id { get; set; }
        public string category_name { get; set; }
        public string product_name { get; set; }
        public int quantity { get; set; }
        public double price { get; set; }
        public string payment_method { get; set; }
        public string city { get; set; }
    }
}
