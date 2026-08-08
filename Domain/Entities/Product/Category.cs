namespace Domain.Entities.Product
{
    public class Category : ProductBase
    {
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
