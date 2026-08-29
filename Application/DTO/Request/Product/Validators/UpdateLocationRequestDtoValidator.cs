using FluentValidation;

namespace Application.DTO.Request.Product.Validators
{
    public class UpdateLocationRequestDtoValidator : AbstractValidator<UpdateLocationRequestDto>
    {
        public UpdateLocationRequestDtoValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}
