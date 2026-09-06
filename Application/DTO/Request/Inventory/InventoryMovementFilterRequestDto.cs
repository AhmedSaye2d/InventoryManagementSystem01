using Domain.Entities.Inventory;

namespace Application.DTO.Request.Inventory
{
    public class InventoryMovementFilterRequestDto
    {
        public Guid? ProductId { get; set; }
        public Guid? LocationId { get; set; }
        public MovementType? MovementType { get; set; }
        public string? UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        // Sorting (optional)
        public string? SortBy { get; set; } // CreatedAt, Quantity, MovementType
        public bool SortDescending { get; set; } = true;
    }
}
