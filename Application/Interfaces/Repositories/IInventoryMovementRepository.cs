using Application.DTO.Request.Inventory;
using Application.DTO.Response.Inventory;
using Domain.Entities.Inventory;

namespace Application.Interfaces.Repositories
{
    public interface IInventoryMovementRepository : IGenericRepository<InventoryMovement>
    {
        Task<InventoryMovement?> GetMovementByIdWithProductAsync(Guid id);
        Task<InventoryMovementResponseDto?> GetMovementByIdResponseAsync(Guid id);
        Task<PagedResponse<InventoryMovementResponseDto>> GetFilteredMovementsAsync(InventoryMovementFilterRequestDto filter);
        Task<PagedResponse<InventoryMovementResponseDto>> GetProductHistoryAsync(Guid productId, DateTime? dateFrom, DateTime? dateTo, int pageNumber, int pageSize);
        Task<PagedResponse<InventoryMovementResponseDto>> GetReportAsync(MovementType type, Guid? productId, DateTime? from, DateTime? to, int pageNumber, int pageSize);
    }
}
