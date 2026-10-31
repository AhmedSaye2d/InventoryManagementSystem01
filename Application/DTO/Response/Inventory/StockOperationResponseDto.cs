using Domain.Entities.Inventory;

namespace Application.DTO.Response.Inventory
{
    public class StockOperationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int OldQuantity { get; set; }
        public int NewQuantity { get; set; }
        public int MovementQuantity { get; set; }
        public MovementType MovementType { get; set; }
        public Guid MovementId { get; set; }
        public Guid LocationId { get; set; }
    }
}
