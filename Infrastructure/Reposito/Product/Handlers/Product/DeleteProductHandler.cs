using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Products;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public DeleteProductHandler(IUnitOfWork unitOfWork, Application.Interfaces.ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
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

                await _cacheService.RemoveAsync("products:all");
                await _cacheService.RemoveAsync($"product:{request.Id}");

                return GeneralDbResponses.ItemDeleted(product.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}