namespace Application.DTO.Request.Inventory
{
    public class StockInRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid? LocationId { get; set; } // optional: if null use Product.LocationId
        public string? ReferenceId { get; set; }
    }
}
