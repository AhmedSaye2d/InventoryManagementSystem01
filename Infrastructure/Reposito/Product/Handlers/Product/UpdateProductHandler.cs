using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Products;
using Mapster;
using MediatR;
using ProductEntity = Domain.Entities.Product.Product;

namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ServicesResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(request.productmodel.Id);

                if (product == null)
                {
                    return GeneralDbResponses.ItemNotFound(request.productmodel.Name);
                }

                var adapt = request.productmodel.Adapt(new ProductEntity());
                _unitOfWork.Products.Update(adapt);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return GeneralDbResponses.ItemUpdated(request.productmodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}