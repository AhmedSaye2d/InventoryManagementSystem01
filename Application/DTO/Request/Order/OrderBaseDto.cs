using NetcodeHub.Packages.Extensions.Attributes.RequiredGuid;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Request.Order
{
    public class OrderBaseDto
    {
        [NetcodeHubRequiredGuid(ErrorMessage = "ProductId Must Be Selected")]
        public Guid ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
