using Application.DTO.Request.Inventory;
using Application.DTO.Response.Inventory;
using MediatR;

namespace Application.Services.Inventory.Queries
{
    public record GetInventoryMovementsQuery(InventoryMovementFilterRequestDto Filter) : IRequest<PagedResponse<InventoryMovementResponseDto>>;
}
