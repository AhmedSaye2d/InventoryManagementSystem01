using Application.DTO.Response.Order;
using Application.Services.Order.Queries;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEntity = Domain.Entities.Order.Order;

namespace Infrastructure.Reposito.Order
{
    public class GetAllOrderHandler : IRequestHandler<GetAllOrderQuery, IEnumerable<GetOrderResponsDto>>
    {
        private readonly AppDbContext _context;

        public GetAllOrderHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetOrderResponsDto>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .Join(
                    _context.Products,
                    order => order.ProductId,
                    product => product.Id,
                    (order, product) => new { order, product }
                )
                .Join(
                    _context.Users,
                    op => op.order.ClientId,
                    user => user.Id,
                    (op, user) => new GetOrderResponsDto
                    {
                        Id = op.order.Id,
                        ProductId = op.order.ProductId,
                        Quantity = op.order.Quantity,
                        Status = op.order.OrderStatus,
                        ProductName = op.product.Name,
                        ProductDescription = op.product.Description,
                        price = op.product.Price,
                        SerialNumber = op.product.SerialNumber,
                        OrderDate = op.order.DateOrder,
                        DeliveringDate = op.order.DeliveryDate,
                        ClientId = op.order.ClientId,
                        ClientName = user.UserName ?? string.Empty
                    }
                )
                .ToListAsync(cancellationToken);

            return orders;
        }
    }
}
