using Application.DTO.Response;

namespace Application.Services.Product.Commands.Products
{
    public record DeleteProductCommand(Guid Id) : MediatR.IRequest<ServicesResponse>;
}
