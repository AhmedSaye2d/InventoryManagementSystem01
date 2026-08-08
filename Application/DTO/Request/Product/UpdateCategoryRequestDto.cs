namespace Application.DTO.Request.Product
{
    public class UpdateCategoryRequestDto : AddCategoryRequestDto
    {
        public Guid Id { get; set; }
    }
}
