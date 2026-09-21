using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Categories;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public DeleteCategoryHandler(IUnitOfWork unitOfWork, Application.Interfaces.ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<ServicesResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.Id);

                if (category == null)
                {
                    return GeneralDbResponses.ItemNotFound("Category");
                }

                _unitOfWork.Categories.Delete(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync("categories:all");
                await _cacheService.RemoveAsync($"category:{request.Id}");

                return GeneralDbResponses.ItemDeleted(category.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
