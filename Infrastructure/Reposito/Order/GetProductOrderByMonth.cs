using Application.DTO.Response.Order;
using Application.Interfaces.Repositories;
using Application.Services.Order.Queries;
using MediatR;

namespace Infrastructure.Reposito.Order
{
    public class GetProductOrderByMonth : IRequestHandler<GetProductOrderByMonthQuery, IEnumerable<GetProductOrderByMonthResponsDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public GetProductOrderByMonth(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<GetProductOrderByMonthResponsDto>> Handle(GetProductOrderByMonthQuery request, CancellationToken cancellationToken)
        {
            return await _orderRepository.GetProductOrderByMonthReportAsync(request.UserId);
        }
    }
}
