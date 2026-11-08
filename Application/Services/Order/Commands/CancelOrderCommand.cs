using Application.DTO.Response;
using MediatR;

namespace Application.Services.Order.Commands
{
    public record CancelOrderCommand(Guid orderId) : IRequest<ServicesResponse>;
 
}
