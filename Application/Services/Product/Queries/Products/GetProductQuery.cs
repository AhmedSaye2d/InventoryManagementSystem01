using Application.DTO.Response.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Product.Queries.Products
{
    public record GetProductQuery() : IRequest<IEnumerable<GetProductResponseDto>>;
   
}
