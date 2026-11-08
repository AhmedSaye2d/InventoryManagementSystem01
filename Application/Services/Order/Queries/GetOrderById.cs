using Application.DTO.Response.Order;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Order.Queries
{
    public  record GetOrderById(string UserId):IRequest<IEnumerable<GetOrderResponsDto>>
    {
    }
}
