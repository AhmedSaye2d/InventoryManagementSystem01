using Application.DTO.Response;
using MediatR;

namespace Application.Services.Product.Commands.Categories
{
    public record DeleteCategoryCommand(Guid Id) : IRequest<ServicesResponse>;

}
