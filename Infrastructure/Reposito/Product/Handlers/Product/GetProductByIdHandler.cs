using Application.DTO.Response.Products;
using Application.Services.Product.Queries.Products;
using Infrastructure.DataAccess;
using LocationEntity = global::Domain.Entities.Product.Location;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Reposito.Product.Handlers.Product
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, GetProductResponseDto>
    {
        private readonly AppDbContext _context;
        public GetProductByIdHandler(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<GetProductResponseDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products.AsNoTracking().Include(x => x.Category).Include(x => x.Location).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
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
                Location = new GetLocationResponseDto
                {
                    Id = product.LocationId,
                    Name = product.Location.Name

                },
                Category = new GetCategoryResponseDto
                {
                    Id = product.CategoryId,
                    Name = product.Category.Name,
                }
            };
        } 
    }
}