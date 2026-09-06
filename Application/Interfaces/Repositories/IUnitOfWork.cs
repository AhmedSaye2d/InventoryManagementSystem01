using Domain.Entities.ActivityTracker;

namespace Application.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICategoryRepository Categories { get; }
        ILocationRepository Locations { get; }
        IOrderRepository Orders { get; }
        IInventoryMovementRepository InventoryMovements { get; }
        IGenericRepository<Tracker> ActivityTrackers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
