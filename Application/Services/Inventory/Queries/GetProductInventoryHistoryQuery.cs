using Application.DTO.Response.Inventory;
using MediatR;

namespace Application.Services.Inventory.Queries
{
    /// <summary>
    /// Get history for a specific product with optional filtering + pagination
    /// </summary>
    public record GetProductInventoryHistoryQuery(
        Guid ProductId,

        int PageNumber = 1,

        int PageSize = 20,

        DateTime? DateFrom = null
        ,
        DateTime? DateTo = null) : IRequest<PagedResponse<InventoryMovementResponseDto>>;
}
