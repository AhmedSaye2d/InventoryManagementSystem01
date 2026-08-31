using Application.DTO.Response.Products;
using Application.Services.Product.Queries.Categories;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Reposito.Product.Handlers.Category
{
    public class GetAllCategoryHandler : IRequestHandler<GetAllCategoryQuery, IEnumerable<GetCategoryResponseDto>>
    {
        private readonly AppDbContext _context;

        public GetAllCategoryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetCategoryResponseDto>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
        {
            var data = await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
            return data.Adapt<List<GetCategoryResponseDto>>();
        }
    }
}
