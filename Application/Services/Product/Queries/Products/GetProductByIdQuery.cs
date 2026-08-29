using Application.DTO.Response;
using Application.DTO.Response.Products;
using MediatR;
namespace Application.Services.Product.Queries.Products
{
    public record GetProductByIdQuery(Guid Id) : IRequest<GetProductResponseDto>;
    
}
