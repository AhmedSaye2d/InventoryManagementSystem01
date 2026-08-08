namespace Domain.Entities.Product
{
    public class Product : ProductBase
    {
        public Category Category { get; set; }
        public Location Location { get; set; }
        public Guid CategoryId { get; set; }
        public Guid LocationId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Base64Image { get; set; } = string.Empty;
    }
}
