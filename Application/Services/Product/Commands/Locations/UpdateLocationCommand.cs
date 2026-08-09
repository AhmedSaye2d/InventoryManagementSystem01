using Application.DTO.Response;

namespace Application.Services.Product.Commands.Locations
{
    public record UpdateLocationCommand(Guid Id) : MediatR.IRequest<ServicesResponse>
    {
    }
}
