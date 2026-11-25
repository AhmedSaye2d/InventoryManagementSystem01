using Domain.Entities.Product;

namespace Domain.Entities.Inventory
{
    public class InventoryMovement
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProductId { get; set; }
        public Product.Product Product { get; set; } = null!;

        public Guid LocationId { get; set; }
        public Location Location { get; set; } = null!;

        /// <summary>
        /// Signed quantity: +50 for StockIn, -10 for StockOut, etc.
        /// </summary>
        public int Quantity { get; set; }

        public MovementType MovementType { get; set; }

        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// When the movement happened (UTC)
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Who performed the movement (Identity User Id)
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Optional reference: e.g., OrderId, AdjustmentId, Relocation correlation
        /// </summary>
        public string? ReferenceId { get; set; }

        /// <summary>
        /// Quantity after movement? optional for auditing
        /// </summary>
        public int? QuantityAfter { get; set; }
    }
}
