using Application.DTO.Response.Order;
using Application.Interfaces.Repositories;
using Application.Services.Order.Queries;
using MediatR;

namespace Infrastructure.Reposito.Order
{
    public class GetOrderProductWithQuentity : IRequestHandler<GetOrderPorductWithQuantityQuery, GetOrderedProductsWithQuantityResponseDto>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderProductWithQuentity(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<GetOrderedProductsWithQuantityResponseDto> Handle(GetOrderPorductWithQuantityQuery request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetTopOrderedProductAsync(request.UserId);
        }
    }
}
