using Application.DTO.Request.Product;
using Application.DTO.Response;
namespace Application.Services.Product.Commands.Products
{
    public record UpdateProductCommand(Guid Id, UpdateProductRequestDto productmodel) : MediatR.IRequest<ServicesResponse>
    {
    }
}
