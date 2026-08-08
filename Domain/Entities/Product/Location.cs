namespace Domain.Entities.Product
{
    public class Location : ProductBase
    {
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
