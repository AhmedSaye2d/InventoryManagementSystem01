using Application.DTO.Response;
namespace Application.Services.Product.Commands.Locations
{
    public record DeleteLocationCommand(Guid Id) : MediatR.IRequest<ServicesResponse>;
}
