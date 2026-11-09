namespace Application.DTO.Response.Order
{
    public record GetOrderCountResponsDto(int Pending,int Processing, int Delivered, int Delivering ,int Cancelled,int cancel);
}
