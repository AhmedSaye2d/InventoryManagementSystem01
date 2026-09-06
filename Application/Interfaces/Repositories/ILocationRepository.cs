using Domain.Entities.Product;

namespace Application.Interfaces.Repositories
{
    public interface ILocationRepository : IGenericRepository<Location>
    {
        Task<IEnumerable<Location>> GetLocationsWithProductsAsync();
        Task<Location?> GetLocationWithProductsByIdAsync(Guid id);
    }
}
