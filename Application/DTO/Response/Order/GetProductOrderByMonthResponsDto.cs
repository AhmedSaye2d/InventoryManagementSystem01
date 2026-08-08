namespace Application.DTO.Response.Order
{
    public class GetProductOrderByMonthResponsDto
    {
        public string MonthName { get; set; }
        public int TotalAmount { get; set; }
        public string FormattedTotalAmount => TotalAmount.ToString("N0");
    }
}
