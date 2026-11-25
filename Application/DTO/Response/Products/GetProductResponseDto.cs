namespace Application.DTO.Response.Products
{
    public class GetProductResponseDto : ProductBaseDto
    {
        public Guid Id { get; set; }
        public GetCategoryResponseDto Category { get; set; }
        public GetLocationResponseDto Location { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.MinValue;
    }
}
