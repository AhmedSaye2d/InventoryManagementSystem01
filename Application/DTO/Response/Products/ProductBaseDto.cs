using NetcodeHub.Packages.Extensions.Attributes.RequiredGuid;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Response.Products
{
    public class ProductBaseDto
    {
        [Required]
        public string Name { get; set; }

        [RegularExpression(@"^[{(]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[)}]?$")]
        public Guid LocationId { get; set; }

        [RegularExpression(@"^[{(]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[)}]?$")]
        public Guid CategoryId { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        //public DateTime DateAdd { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [RegularExpression(@"^\d+(\.\d{1,2})?$")]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "Base64 Image")]
        public string Base64Image { get; set; } = null;
    }
}
