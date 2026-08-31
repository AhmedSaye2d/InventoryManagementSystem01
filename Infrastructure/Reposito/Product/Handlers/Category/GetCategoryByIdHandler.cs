using Application.DTO.Response.Products;
using Application.Services.Product.Queries.Categories;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, GetCategoryResponseDto>
    {
        private readonly AppDbContext _context;

        public GetCategoryByIdHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GetCategoryResponseDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

            if (category == null) return null!;

            return new GetCategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }
    }
}
