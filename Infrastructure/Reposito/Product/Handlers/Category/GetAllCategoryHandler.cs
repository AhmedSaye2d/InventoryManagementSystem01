using Application.DTO.Response.Products;
using Application.Interfaces.Repositories;
using Application.Services.Product.Queries.Categories;
using Mapster;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class GetAllCategoryHandler : IRequestHandler<GetAllCategoryQuery, IEnumerable<GetCategoryResponseDto>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public GetAllCategoryHandler(ICategoryRepository categoryRepository, Application.Interfaces.ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<GetCategoryResponseDto>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "categories:all";
            var cachedCategories = await _cacheService.GetAsync<IEnumerable<GetCategoryResponseDto>>(cacheKey);
            if (cachedCategories != null)
            {
                return cachedCategories;
            }

            var data = await _categoryRepository.GetAllAsync();
            var response = data.Adapt<List<GetCategoryResponseDto>>();
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            return response;
        }
    }
}
