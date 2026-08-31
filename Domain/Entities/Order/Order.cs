namespace Domain.Entities.Order
{
    public class Order
    {
        public Guid Id { get; set; }
        public DateTime DateOrder { get; set; }
        public Guid ProductId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string ClientId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Price { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public static class OrderStatus
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Delivering = "Delivering";
        public const string Delivered = "Delivered";
        public const string Cancelled = "Cancelled";
        public const string cancel = "Cancelled";
    }
}
