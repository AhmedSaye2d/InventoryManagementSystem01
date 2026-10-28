using Application.DTO.Request.Product;
using Application.DTO.Response;
using MediatR;

namespace Application.Services.Product.Commands.Categories
{
    public record UpdateCategoryCommand(UpdateCategoryRequestDto Categorymodel) : IRequest<ServicesResponse>;
}
