using Application.DTO.Response.Order;
using Application.Interfaces.Repositories;
using Application.Services.Order.Queries;
using MediatR;

namespace Infrastructure.Reposito.Order
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderById, IEnumerable<GetOrderResponsDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderByIdHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<GetOrderResponsDto>> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetOrdersByUserIdWithDetailsAsync(request.UserId);
        }
    }
}
