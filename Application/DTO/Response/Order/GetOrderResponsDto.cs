using Application.DTO.Request.Order;

namespace Application.DTO.Response.Order
{
    public class GetOrderResponsDto : OrderBaseDto
    {
        public Guid Id { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal price { get; set; }
        public decimal TotalAmount => price * Quantity;
        public string SerialNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveringDate { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
    }
}
