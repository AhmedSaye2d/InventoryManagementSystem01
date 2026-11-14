using FluentValidation;

namespace Application.DTO.Request.Product.Validators
{
    public class AddLocationRequestDtoValidator : AbstractValidator<AddLocationRequestDto>
    {
        public AddLocationRequestDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
