using Application.DTO.Response.Products;
using Application.Interfaces.Repositories;
using Application.Services.Product.Queries.Categories;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, GetCategoryResponseDto>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public GetCategoryByIdHandler(ICategoryRepository categoryRepository, Application.Interfaces.ICacheService cacheService)
        {
            _categoryRepository = categoryRepository;
            _cacheService = cacheService;
        }

        public async Task<GetCategoryResponseDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"category:{request.Id}";
            var cachedCategory = await _cacheService.GetAsync<GetCategoryResponseDto>(cacheKey);
            if (cachedCategory != null)
            {
                return cachedCategory;
            }

            var category = await _categoryRepository.GetByIdAsync(request.Id);

            if (category == null) return null!;

            var response = new GetCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };

            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            return response;
        }
    }
}
