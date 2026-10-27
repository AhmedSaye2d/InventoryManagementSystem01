using Application.DTO.Response.Products;
using Application.Interfaces.Repositories;
using Application.Services.Product.Queries.Products;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class GetProductHandler : IRequestHandler<GetProductQuery, IEnumerable<GetProductResponseDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly Application.Interfaces.ICacheService _cacheService;

        public GetProductHandler(IProductRepository productRepository, Application.Interfaces.ICacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<GetProductResponseDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "products:all";
            var cachedProducts = await _cacheService.GetAsync<IEnumerable<GetProductResponseDto>>(cacheKey);
            
            if (cachedProducts != null)
            {
                return cachedProducts;
            }

            var products = await _productRepository.GetProductsWithCategoryAndLocationAsync();

            var response = products.Select(product => new GetProductResponseDto
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
            }).ToList();

            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(10));

            return response;
        }
    }
}
