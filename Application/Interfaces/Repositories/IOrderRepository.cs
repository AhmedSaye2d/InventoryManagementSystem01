using Application.DTO.Response.Order;
using Domain.Entities.Order;

namespace Application.Interfaces.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<GetOrderResponsDto>> GetAllOrdersWithDetailsAsync();
        Task<IEnumerable<GetOrderResponsDto>> GetOrdersByUserIdWithDetailsAsync(string userId);
        Task<GetOrderCountResponsDto> GetOrderCountReportAsync(bool isAdmin, string? userId);
        Task<GetOrderedProductsWithQuantityResponseDto> GetTopOrderedProductAsync(string? userId);
        Task<IEnumerable<GetProductOrderByMonthResponsDto>> GetProductOrderByMonthReportAsync(string? userId);
    }
}
