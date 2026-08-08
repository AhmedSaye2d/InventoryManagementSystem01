namespace Application.DTO.Response.Products
{
    public class GetLocationResponseDto
    {
        public virtual ICollection<GetProductResponseDto> Products { get; set; }
    }
}
