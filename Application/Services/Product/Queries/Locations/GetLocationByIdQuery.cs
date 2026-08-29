using Application.DTO.Response.Products;
using MediatR;
using System;

namespace Application.Services.Product.Queries.Locations
{
    public record GetLocationByIdQuery(Guid Id) : IRequest<GetLocationResponseDto>;
}
