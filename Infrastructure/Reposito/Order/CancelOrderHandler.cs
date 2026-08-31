using Application.DTO.Response;
using Application.Services.Order.Commands;
using Domain.Entities.Order;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Reposito.Order
{
    public class CancelOrderHandle : IRequestHandler<CancelOrderCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;
        public CancelOrderHandle(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<ServicesResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _context.Orders.Where(c => c.Id == request.orderId).FirstOrDefaultAsync(cancellationToken);

                if (order == null)
                {
                    return new ServicesResponse(false, "Order not found");
                }
                order.OrderStatus = OrderStatus.cancel;

                await _context.SaveChangesAsync(cancellationToken);
                return new ServicesResponse(true, "order cancel successfully");

            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
