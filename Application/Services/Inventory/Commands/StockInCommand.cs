using Application.DTO.Request.Inventory;
using Application.DTO.Response.Inventory;
using MediatR;

namespace Application.Services.Inventory.Commands
{
    public record StockInCommand(StockInRequestDto Request, string UserId) : IRequest<StockOperationResponseDto>;
}
