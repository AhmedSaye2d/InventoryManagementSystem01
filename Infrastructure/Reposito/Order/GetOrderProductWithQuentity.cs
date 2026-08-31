using Application.DTO.Response.Order;
using Application.Services.Order.Queries;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Reposito.Order
{
    public class GetOrderProductWithQuentity : IRequestHandler<GetOrderPorductWithQuantityQuery, GetOrderedProductsWithQuantityResponseDto>
    {
        private readonly AppDbContext _context;

        public GetOrderProductWithQuentity(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetOrderedProductsWithQuantityResponseDto> Handle(GetOrderPorductWithQuantityQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking();

            if (!string.IsNullOrEmpty(request.UserId))
            {
                query = query.Where(x => x.ClientId == request.UserId);
            }

            var topProduct = await query
                .Join(
                    _context.Products,
                    order => order.ProductId,
                    product => product.Id,
                    (order, product) => new { product.Name, order.Quantity }
                )
                .GroupBy(x => x.Name)
                .Select(g => new GetOrderedProductsWithQuantityResponseDto
                {
                    NameProduct = g.Key,
                    QuantityOrdered = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantityOrdered)
                .FirstOrDefaultAsync(cancellationToken);

            return topProduct ?? new GetOrderedProductsWithQuantityResponseDto
            {
                NameProduct = "N/A",
                QuantityOrdered = 0
            };
        }
    }
}
