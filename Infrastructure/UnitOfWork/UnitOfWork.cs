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

        public IProductRepository Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new ProductRepository(_context);
                }

                return _products;
            }
        }

        public ICategoryRepository Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new CategoryRepository(_context);
                }

                return _categories;
            }
        }

        public ILocationRepository Locations
        {
            get
            {
                if (_locations == null)
                {
                    _locations = new LocationRepository(_context);
                }

                return _locations;
            }
        }

        public IOrderRepository Orders
        {
            get
            {
                if (_orders == null)
                {
                    _orders = new OrderRepository(_context);
                }

                return _orders;
            }
        }

        public IInventoryMovementRepository InventoryMovements
        {
            get
            {
                if (_inventoryMovements == null)
                {
                    _inventoryMovements =
                        new InventoryMovementRepository(_context);
                }

                return _inventoryMovements;
            }
        }

        public IGenericRepository<Tracker> ActivityTrackers
        {
            get
            {
                if (_activityTrackers == null)
                {
                    _activityTrackers =
                        new GenericRepository<Tracker>(_context);
                }

                return _activityTrackers;
            }
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}