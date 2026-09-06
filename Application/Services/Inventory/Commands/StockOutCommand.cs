using Application.DTO.Request.Inventory;
using Application.DTO.Response.Inventory;
using MediatR;

namespace Application.Services.Inventory.Commands
{
    public record StockOutCommand(StockOutRequestDto Request, string UserId) : IRequest<StockOperationResponseDto>;
}
