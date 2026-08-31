using Application.DTO.Response.Products;
using Application.Services.Product.Queries.Categories;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class GetAllCategoryWithProductHandler : IRequestHandler<GetAllCategoryWithProduct, IEnumerable<GetCategoryResponseDto>>
    {
        private readonly AppDbContext _context;

        public GetAllCategoryWithProductHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetCategoryResponseDto>> Handle(GetAllCategoryWithProduct request, CancellationToken cancellationToken)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Include(x => x.Products)
                .ToListAsync(cancellationToken);

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
