using Application.DTO.Response.Products;
using Application.Interfaces.Repositories;
using Application.Services.Product.Queries.Products;
using MediatR;

namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, GetProductResponseDto>
    {
        private readonly IProductRepository _productRepository;

        public GetProductByIdHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<GetProductResponseDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductWithCategoryAndLocationByIdAsync(request.Id);
            if (product == null)
            {
                return null!;
            }

            return new GetProductResponseDto
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
        }
    }
}