using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Categories;
using Mapster;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public UpdateCategoryHandler(IUnitOfWork unitOfWork, Application.Interfaces.ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<ServicesResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(request.Categorymodel.Id);

                if (category == null)
                {
                    return GeneralDbResponses.ItemNotFound("Category");
                }

                request.Categorymodel.Adapt(category);

                _unitOfWork.Categories.Update(category);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync("categories:all");
                await _cacheService.RemoveAsync($"category:{request.Categorymodel.Id}");

                return GeneralDbResponses.ItemUpdated(request.Categorymodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
