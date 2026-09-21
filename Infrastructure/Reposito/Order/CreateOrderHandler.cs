using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Order.Commands;
using Mapster;
using MediatR;
using OrderEntity = Domain.Entities.Order.Order;

namespace Infrastructure.Reposito.Order
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(request.ordermodel.ProductId);

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

                _unitOfWork.Products.Update(product);
                await _unitOfWork.Orders.AddAsync(data);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ServicesResponse(true, "Order created successfully");
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
