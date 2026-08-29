using Application.DTO.Request.Product;

namespace Application.DTO.Response.Products
{
    public class GetLocationResponseDto:UpdateLocationRequestDto
    {
        public IEnumerable<GetProductResponseDto> Products { get; set; }
    }
}
