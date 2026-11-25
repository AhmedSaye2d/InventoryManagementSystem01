using Application.Interfaces.Repositories;
using Domain.Entities.Product;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetProductsWithCategoryAndLocationAsync()
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Location)
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithCategoryAndLocationByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
