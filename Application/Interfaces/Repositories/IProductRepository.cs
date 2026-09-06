using Domain.Entities.Product;

namespace Application.Interfaces.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAndLocationAsync();
        Task<Product?> GetProductWithCategoryAndLocationByIdAsync(Guid id);
    }
}
