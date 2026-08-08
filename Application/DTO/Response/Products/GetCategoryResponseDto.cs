using Application.DTO.Request.Product;

namespace Application.DTO.Response.Products
{
    public class GetCategoryResponseDto : UpdateCategoryRequestDto
    {
        public virtual ICollection<GetProductResponseDto> Products { get; set; }
    }
}
