namespace Application.DTO.Request.Inventory
{
    public class StockOutRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid? LocationId { get; set; }
        public string? ReferenceId { get; set; }
    }
}
