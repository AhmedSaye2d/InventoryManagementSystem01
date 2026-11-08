using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Order.Commands;
using Domain.Entities.Order;
using MediatR;

namespace Infrastructure.Reposito.Order
{
    public class CancelOrderHandle : IRequestHandler<CancelOrderCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderHandle(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(request.orderId);

                if (order == null)
                {
                    return new ServicesResponse(false, "Order not found");
                }
                order.OrderStatus = OrderStatus.cancel;

                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return new ServicesResponse(true, "order cancel successfully");
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
