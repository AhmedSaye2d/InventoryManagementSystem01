using Application.DTO.Request.Product;
using Application.DTO.Response;

namespace Application.Services.Product.Commands.Products
{
    public record CreateProductCommand(AddProductRequestDto productmodel) : MediatR.IRequest<ServicesResponse>;

}
