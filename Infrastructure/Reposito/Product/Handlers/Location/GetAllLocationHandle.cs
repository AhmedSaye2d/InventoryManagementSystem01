using Application.DTO.Response.Products;
using Application.Services.Product.Queries.Locations;
using Infrastructure.DataAccess;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reposito.Product.Handlers.Location
{
    public class GetAllLocationHandle : IRequestHandler<GetAllLocationQuery, IEnumerable<GetLocationResponseDto>>
    {
        private readonly AppDbContext _context;
        public GetAllLocationHandle(AppDbContext context)
        {
            this._context = context;
        }
        public async Task<IEnumerable<GetLocationResponseDto>> Handle(GetAllLocationQuery request, CancellationToken cancellationToken)
        {
           var data = await  _context.Locations.AsNoTracking().ToListAsync(cancellationToken);  

          return data.Adapt<List<GetLocationResponseDto>>();
        }

    }
}
