using Application.DTO.Response.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Order.Queries
{
    public record GetOrderPorductWithQuantityQuery(string UserId = null) : IRequest<GetOrderedProductsWithQuantityResponseDto>;
 
}
