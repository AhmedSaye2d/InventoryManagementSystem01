using Application.DTO.Response.Products;
using MediatR;
using System;
namespace Application.Services.Product.Queries.Locations
{
    public record GetLocationWithProductQuery : IRequest<IEnumerable<GetLocationResponseDto>>;
}
