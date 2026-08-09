using Application.DTO.Request.Product;
using Application.DTO.Response;
namespace Application.Services.Product.Commands.Locations
{
    public record CreateLocationCommand(AddLocationRequestDto locationmodel) : MediatR.IRequest<ServicesResponse>;

}
