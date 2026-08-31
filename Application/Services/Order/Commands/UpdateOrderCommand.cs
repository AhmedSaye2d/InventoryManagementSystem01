using Application.DTO.Request.Order;
using Application.DTO.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Order.Commands
{
    public record UpdateOrderCommand(UpdateClientOrderRequestDto model) : IRequest<ServicesResponse>;
   
}
