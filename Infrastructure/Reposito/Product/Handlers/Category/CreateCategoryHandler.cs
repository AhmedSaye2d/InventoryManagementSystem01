using Application.DTO.Response;
using Application.Interfaces.Repositories;
using Application.Services.Product.Commands.Categories;
using Mapster;
using MediatR;
using CategoryEntity = Domain.Entities.Product.Category;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, ServicesResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public CreateCategoryHandler(IUnitOfWork unitOfWork, Application.Interfaces.ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<ServicesResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exists = await _unitOfWork.Categories.ExistsAsync(x => x.Name.ToLower() == request.Categorymodel.Name.ToLower());
                if (exists)
                {
                    return GeneralDbResponses.ItemAlreadyExist(request.Categorymodel.Name);
                }

                var data = request.Categorymodel.Adapt<CategoryEntity>();

                await _unitOfWork.Categories.AddAsync(data);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _cacheService.RemoveAsync("categories:all");

                return GeneralDbResponses.ItemCreated(request.Categorymodel.Name);
            }
            catch (Exception ex)
            {
                return new ServicesResponse(false, ex.Message);
            }
        }
    }
}
