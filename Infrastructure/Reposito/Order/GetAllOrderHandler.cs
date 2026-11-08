using Application.DTO.Response.Order;
using Application.Interfaces.Repositories;
using Application.Services.Order.Queries;
using MediatR;

namespace Infrastructure.Reposito.Order
{
    public class GetAllOrderHandler : IRequestHandler<GetAllOrderQuery, IEnumerable<GetOrderResponsDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetAllOrderHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<GetOrderResponsDto>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetAllOrdersWithDetailsAsync();
        }
    }
}
