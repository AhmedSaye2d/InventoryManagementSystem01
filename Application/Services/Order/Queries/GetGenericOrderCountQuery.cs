using Application.DTO.Response.Order;
using MediatR;
namespace Application.Services.Order.Queries
{
    public record GetGenericOrderCountQuery(string UserId, bool IsAdmin = false) : IRequest<GetOrderCountResponsDto>;
   
}
