using Application.DTO.Response.Products;
using Application.Interfaces.Repositories;
using Application.Services.Product.Queries.Products;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, GetProductResponseDto>
    {
        private readonly IProductRepository _productRepository;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public GetProductByIdHandler(IProductRepository productRepository, Application.Interfaces.ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<GetProductResponseDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"product:{request.Id}";
            var cachedProduct = await _cacheService.GetAsync<GetProductResponseDto>(cacheKey);
            if (cachedProduct != null)
            {
                return cachedProduct;
            }

            var product = await _productRepository.GetProductWithCategoryAndLocationByIdAsync(request.Id);
            if (product == null)
            {
                return null!;
            }

            var response = new GetProductResponseDto
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
                Quantity = product.Quantity,
                Location = product.Location != null ? new GetLocationResponseDto
                {
                    Id = product.Location.Id,
                    Name = product.Location.Name
                } : null,
                Category = product.Category != null ? new GetCategoryResponseDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name
                } : null
            };

            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));
            return response;
        }
    }
}