using Application.DTO.Request.Product;
using System.Text.Json.Serialization;

namespace Application.DTO.Response.Products
{
    public class GetLocationResponseDto:UpdateLocationRequestDto
    {
        [JsonIgnore]
        public IEnumerable<GetProductResponseDto> Products { get; set; }
    }
}
