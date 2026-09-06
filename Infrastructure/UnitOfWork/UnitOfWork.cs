using Application.Interfaces.Repositories;
using Domain.Entities.ActivityTracker;
using Infrastructure.DataAccess;
using Infrastructure.Repositories;

namespace Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        private IProductRepository? _products;
        private ICategoryRepository? _categories;
        private ILocationRepository? _locations;
        private IOrderRepository? _orders;
        private IInventoryMovementRepository? _inventoryMovements;
        private IGenericRepository<Tracker>? _activityTrackers;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products =>
            _products ??= new ProductRepository(_context);

        public ICategoryRepository Categories =>
            _categories ??= new CategoryRepository(_context);

        public ILocationRepository Locations =>
            _locations ??= new LocationRepository(_context);

        public IOrderRepository Orders =>
            _orders ??= new OrderRepository(_context);

        public IInventoryMovementRepository InventoryMovements =>
            _inventoryMovements ??= new InventoryMovementRepository(_context);

        public IGenericRepository<Tracker> ActivityTrackers =>
            _activityTrackers ??= new GenericRepository<Tracker>(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
