using Application.Interfaces.Repositories;
using Domain.Entities.Product;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class LocationRepository : GenericRepository<Location>, ILocationRepository
    {
        public LocationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Location>> GetLocationsWithProductsAsync()
        {
           return await _dbSet
            .AsNoTracking()
            .Include(l => l.Products)
            .ToListAsync();
        }

        public async Task<Location?> GetLocationWithProductsByIdAsync(Guid id)
        {
            return await _dbSet.AsNoTracking()  
                .Include(l => l.Products)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}
