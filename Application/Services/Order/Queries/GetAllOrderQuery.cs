using Application.DTO.Response.Order;
using MediatR;

namespace Application.Services.Order.Queries
{
    public record GetAllOrderQuery : IRequest<IEnumerable<GetOrderResponsDto>>;

}
