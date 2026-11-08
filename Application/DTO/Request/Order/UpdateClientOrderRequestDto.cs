namespace Application.DTO.Request.Order
{
    public class UpdateClientOrderRequestDto
    {
        public Guid OrderId { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? Delivering { get; set; } = DateTime.Now;
    }
}
