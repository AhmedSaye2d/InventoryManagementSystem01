using Application.DTO.Request.Order;
using Application.DTO.Response;
using MediatR;

namespace Application.Services.Order.Commands
{
    public record CreateOrderCommand(CreateOrderRequestDto ordermodel) : IRequest<ServicesResponse>;
}
