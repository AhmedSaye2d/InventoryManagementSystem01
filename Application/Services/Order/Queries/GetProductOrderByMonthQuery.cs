using Application.DTO.Response.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Order.Queries
{
    public record GetProductOrderByMonthQuery(string UserId = null) : IRequest<IEnumerable<GetProductOrderByMonthResponsDto>>;

}
