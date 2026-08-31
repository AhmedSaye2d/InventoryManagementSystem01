using FluentValidation;

namespace Application.DTO.Request.Product.Validators
{
    public class AddCategoryRequestDtoValidator : AbstractValidator<AddCategoryRequestDto>
    {
        public AddCategoryRequestDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
