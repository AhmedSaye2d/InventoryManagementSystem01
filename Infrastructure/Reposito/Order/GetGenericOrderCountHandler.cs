using Application.DTO.Response.Order;
using Application.Interfaces.Repositories;
using Application.Services.Order.Queries;
using MediatR;

namespace Infrastructure.Reposito.Order
{
    public class GetGenericOrderCountHandler : IRequestHandler<GetGenericOrderCountQuery, GetOrderCountResponsDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetGenericOrderCountHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<GetOrderCountResponsDto> Handle(GetGenericOrderCountQuery request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetOrderCountReportAsync(request.IsAdmin, request.UserId);
        }
    }
}
