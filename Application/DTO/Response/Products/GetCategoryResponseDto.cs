using Application.DTO.Request.Product;
using System.Text.Json.Serialization;

namespace Application.DTO.Response.Products
{
    public class GetCategoryResponseDto : UpdateCategoryRequestDto
    {
        [JsonIgnore]
        public virtual ICollection<GetProductResponseDto> Products { get; set; }
    }
}
