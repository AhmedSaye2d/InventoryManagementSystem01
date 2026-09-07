using Application.DTO.Response.Order;
using Application.Interfaces.Repositories;
using Domain.Entities.Order;
using Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Threading.Channels;

namespace Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<GetOrderResponsDto>> GetAllOrdersWithDetailsAsync()
        {
            return await _dbSet
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
                .ToListAsync();
        }

        public async Task<IEnumerable<GetOrderResponsDto>> GetOrdersByUserIdWithDetailsAsync(string userId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(o => o.ClientId == userId)
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
                .ToListAsync();
        }

        public async Task<GetOrderCountResponsDto> GetOrderCountReportAsync(
      bool isAdmin,
      string? userId)
        {
            var query = _dbSet.AsNoTracking();

            if (!isAdmin && !string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.ClientId == userId);
            }

            var result = await query
                .GroupBy(x => 1)
                .Select(g => new GetOrderCountResponsDto(
                    g.Count(x =>
                        x.OrderStatus == OrderStatus.Pending),

                    g.Count(x =>
                        x.OrderStatus == OrderStatus.Processing),

                    g.Count(x =>
                        x.OrderStatus == OrderStatus.Delivered),

                    g.Count(x =>
                        x.OrderStatus == OrderStatus.Delivering),

                    g.Count(x =>
                        x.OrderStatus == OrderStatus.Cancelled),

                    g.Count(x =>
                        x.OrderStatus == OrderStatus.cancel)
                ))
                .FirstOrDefaultAsync();

            return result ?? new GetOrderCountResponsDto(
                Pending: 0,
                Processing: 0,
                Delivered: 0,
                Delivering: 0,
                Cancelled: 0,
                cancel: 0
            );
        }
        public async Task<GetOrderedProductsWithQuantityResponseDto>
            GetTopOrderedProductAsync(string? userId)
        {
            var query = _dbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.ClientId == userId);
            }

            var topProduct = await query
                .Join(
                    _context.Products.AsNoTracking(),

                    order => order.ProductId,

                    product => product.Id,

                    (order, product) => new
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = order.Quantity
                    }
                )
                .GroupBy(x => new
                {
                    x.ProductId,
                    x.ProductName
                })
                .Select(g => new GetOrderedProductsWithQuantityResponseDto
                {
                    NameProduct = g.Key.ProductName,
                    QuantityOrdered = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantityOrdered)
                .FirstOrDefaultAsync();

            return topProduct ??
                new GetOrderedProductsWithQuantityResponseDto
                {
                    NameProduct = "N/A",
                    QuantityOrdered = 0
                };
        }
        public async Task<IEnumerable<GetProductOrderByMonthResponsDto>> GetProductOrderByMonthReportAsync(string? userId)
        {
            var query = _dbSet.AsNoTracking();

            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.ClientId == userId);
            }

            var orders = await query.ToListAsync();

            return orders
                .GroupBy(x => x.DateOrder.Month)
                .Select(g => new GetProductOrderByMonthResponsDto
                {
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key),
                    TotalAmount = (int)g.Sum(x => x.TotalAmount)
                })
                .ToList();
        }
    }
}
