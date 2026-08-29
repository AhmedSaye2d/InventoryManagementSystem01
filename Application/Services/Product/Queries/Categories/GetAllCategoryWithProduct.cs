using Application.DTO.Response.Products;
using MediatR;
namespace Application.Services.Product.Queries.Categories
{
    public record GetAllCategoryWithProduct:IRequest<IEnumerable<GetCategoryResponseDto>>;
   
}
