using Application.DTO.Response.Order;
using Application.Services.Order.Queries;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Reposito.Order
{
    public class GetProductOrderByMonth : IRequestHandler<GetProductOrderByMonthQuery, IEnumerable<GetProductOrderByMonthResponsDto>>
    {
        private readonly AppDbContext _context;

        public GetProductOrderByMonth(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetProductOrderByMonthResponsDto>> Handle(GetProductOrderByMonthQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Orders.AsNoTracking();

            if (!string.IsNullOrEmpty(request.UserId))
            {
                query = query.Where(x => x.ClientId == request.UserId);
            }

            var orders = await query.ToListAsync(cancellationToken);

            var monthlyData = orders
                .GroupBy(x => x.DateOrder.Month)
                .Select(g => new GetProductOrderByMonthResponsDto
                {
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    TotalAmount = (int)g.Sum(x => x.TotalAmount)
                })
                .ToList();

            return monthlyData;
        }
    }
}
