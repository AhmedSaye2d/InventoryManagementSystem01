using Application.DTO.Request.Product;
using Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Product.Commands.Categories
{
    public record UpdateCategoryCommand(Guid Id, UpdateCategoryRequestDto Categorymodel) : IRequest<ServicesResponse>;
}
