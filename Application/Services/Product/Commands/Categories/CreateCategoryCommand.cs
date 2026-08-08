using Application.DTO.Request.Product;
using Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Product.Commands.Categories
{
    public record CreateCategoryCommand(AddCategoryRequestDto Categorymodel) : IRequest<ServicesResponse>;
  
}
