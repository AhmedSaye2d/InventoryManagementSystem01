using Application.DTO.Response.Order;
using Application.Services.Order.Queries;
using Domain.Entities.Order;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Reposito.Order
{
    public class GetGenericOrderCountHandler : IRequestHandler<GetGenericOrderCountQuery, GetOrderCountResponsDto>
    {
        private readonly AppDbContext _context;

        public GetGenericOrderCountHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetOrderCountResponsDto> Handle(GetGenericOrderCountQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking();

            if (!request.IsAdmin && !string.IsNullOrEmpty(request.UserId))
            {
                query = query.Where(x => x.ClientId == request.UserId);
            }

            var orders = await query.ToListAsync(cancellationToken);

            int processing = orders.Count(x => string.Equals(x.OrderStatus, OrderStatus.Processing, StringComparison.OrdinalIgnoreCase) || string.Equals(x.OrderStatus, OrderStatus.Pending, StringComparison.OrdinalIgnoreCase));
            int delivered = orders.Count(x => string.Equals(x.OrderStatus, OrderStatus.Delivered, StringComparison.OrdinalIgnoreCase));
            int cancelled = orders.Count(x => string.Equals(x.OrderStatus, OrderStatus.Cancelled, StringComparison.OrdinalIgnoreCase) || string.Equals(x.OrderStatus, OrderStatus.cancel, StringComparison.OrdinalIgnoreCase));
            int delivering = orders.Count(x => string.Equals(x.OrderStatus, OrderStatus.Delivering, StringComparison.OrdinalIgnoreCase));

            return new GetOrderCountResponsDto(processing, delivered, cancelled, delivering);
        }
    }
}
