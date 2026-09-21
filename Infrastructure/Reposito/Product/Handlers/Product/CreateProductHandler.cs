using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Products;
using Mapster;
using MediatR;
using ProductEntity = Domain.Entities.Product.Product;

namespace Infrastructure.Reposito.Product.Handlers.Products
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public CreateProductHandler(IUnitOfWork unitOfWork, Application.Interfaces.ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<ServicesResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _unitOfWork.Products.ExistsAsync(x => x.Name == request.productmodel.Name);
                if (exists)
                {
                    return GeneralDbResponses.ItemAlreadyExist(request.productmodel.Name);
                }

                var data = request.productmodel.Adapt(new ProductEntity());

                await _unitOfWork.Products.AddAsync(data);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync("products:all");

                return GeneralDbResponses.ItemCreated(request.productmodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
