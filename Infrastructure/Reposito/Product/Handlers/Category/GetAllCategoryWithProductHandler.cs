using Application.DTO.Response.Products;
using Application.Interfaces.Repositories;
using Application.Services.Product.Queries.Categories;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class GetAllCategoryWithProductHandler : IRequestHandler<GetAllCategoryWithProduct, IEnumerable<GetCategoryResponseDto>>
    {
        private readonly ICategoryRepository _categoryRepository;

        public GetAllCategoryWithProductHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<GetCategoryResponseDto>> Handle(GetAllCategoryWithProduct request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetCategoriesWithProductsAsync();

            return categories.Select(category => new GetCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products != null ? category.Products.Select(product => new GetProductResponseDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    SerialNumber = product.SerialNumber,
                    Description = product.Description,
                    Base64Image = product.Base64Image,
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    DateAdded = product.DateAdd,
                    LocationId = product.LocationId,
                    Quantity = product.Quantity
                }).ToList() : new List<GetProductResponseDto>()
            });
        }
    }
}
