namespace Application.DTO.Response.Order
{
    public record GetOrderCountResponsDto(int Processing, int Delivered, int Cancelled, int Delivering);
}


