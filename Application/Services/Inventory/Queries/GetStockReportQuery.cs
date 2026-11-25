using Application.DTO.Response.Inventory;
using Domain.Entities.Inventory;
using MediatR;

namespace Application.Services.Inventory.Queries
{
    public record GetStockInReportQuery(
        int PageNumber = 1,
        int PageSize = 20,
        Guid? ProductId = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null) : IRequest<PagedResponse<InventoryMovementResponseDto>>;

    public record GetStockOutReportQuery(
        int PageNumber = 1,
        int PageSize = 20,
        Guid? ProductId = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null) : IRequest<PagedResponse<InventoryMovementResponseDto>>;

    public record GetAdjustmentReportQuery(
        int PageNumber = 1,
        int PageSize = 20,
        Guid? ProductId = null,
        DateTime? DateFrom = null,
        DateTime? DateTo = null) : IRequest<PagedResponse<InventoryMovementResponseDto>>;
}
