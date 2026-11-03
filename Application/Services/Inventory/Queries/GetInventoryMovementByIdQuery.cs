using Application.DTO.Response.Inventory;
using MediatR;

namespace Application.Services.Inventory.Queries
{
    public record GetInventoryMovementByIdQuery(Guid Id) : IRequest<InventoryMovementResponseDto?>;
}
