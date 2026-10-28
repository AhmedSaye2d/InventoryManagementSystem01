using Application.DTO.Response.Products;
using MediatR;
using System;

namespace Application.Services.Product.Queries.Categories
{
    public record GetCategoryByIdQuery(Guid Id) : IRequest<GetCategoryResponseDto>;
}
