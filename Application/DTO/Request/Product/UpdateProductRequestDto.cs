using Application.DTO.Response.Products;
namespace Application.DTO.Request.Product
{
    public class UpdateProductRequestDto : ProductBaseDto
    {
        public Guid Id { get; set; }
    }
}