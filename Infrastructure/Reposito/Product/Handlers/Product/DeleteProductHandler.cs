using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Products;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(request.Id);
                if (product == null)
                {
                    return GeneralDbResponses.ItemNotFound("Product");
                }

                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemDeleted(product.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}