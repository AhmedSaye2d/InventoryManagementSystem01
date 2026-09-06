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

        public GetAllCategoryHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<GetCategoryResponseDto>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _categoryRepository.GetAllAsync();
            return data.Adapt<List<GetCategoryResponseDto>>();
        }
    }
}
