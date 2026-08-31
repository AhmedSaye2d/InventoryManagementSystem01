using Application.DTO.Response;
using Application.Services.Order.Commands;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Reposito.Order
{
    public class UpdateClientOrderHandler : IRequestHandler<UpdateOrderCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;

        public UpdateClientOrderHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == request.model.OrderId, cancellationToken);
                if (order == null)
                {
                    return new ServicesResponse(false, "Order not found");
                }

                if (!string.IsNullOrEmpty(request.model.OrderStatus))
                {
                    order.OrderStatus = request.model.OrderStatus;
                }

                if (request.model.Delivering.HasValue)
                {
                    order.DeliveryDate = request.model.Delivering.Value;
                }

                _context.Orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);

                return new ServicesResponse(true, "Order updated successfully");
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
