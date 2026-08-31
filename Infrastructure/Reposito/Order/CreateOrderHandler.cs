using Application.DTO.Response;
using Application.Services.Order.Commands;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderEntity = Domain.Entities.Order.Order;

namespace Infrastructure.Reposito.Order
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, ServicesResponse>
    {
        private readonly AppDbContext _context;

        public CreateOrderHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServicesResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(x => x.Id == request.ordermodel.ProductId, cancellationToken);

                if (product == null)
                    return new ServicesResponse(false, "Product not found");

                if (product.Quantity < request.ordermodel.Quantity)
                    return new ServicesResponse(false, "Insufficient product quantity in stock");

                var data = request.ordermodel.Adapt<OrderEntity>();
                data.Id = Guid.NewGuid();
                data.DateOrder = DateTime.UtcNow;
                data.DeliveryDate = DateTime.UtcNow.AddDays(7);
                data.Price = product.Price;
                data.TotalAmount = product.Price * request.ordermodel.Quantity;
                data.OrderStatus = "Pending";

                // Reduce product quantity
                product.Quantity -= request.ordermodel.Quantity;
                _context.Products.Update(product);

                _context.Orders.Add(data);
                await _context.SaveChangesAsync(cancellationToken);

                return new ServicesResponse(true, "Order created successfully");
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
