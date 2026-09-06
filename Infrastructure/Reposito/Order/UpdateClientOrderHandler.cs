using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Order.Commands;
using MediatR;

namespace Infrastructure.Reposito.Order
{
    public class UpdateClientOrderHandler : IRequestHandler<UpdateOrderCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateClientOrderHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(request.model.OrderId);
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

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return new ServicesResponse(true, "Order updated successfully");
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
