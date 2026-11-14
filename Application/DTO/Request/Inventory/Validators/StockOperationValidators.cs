using Application.Services.Inventory.Commands;
using FluentValidation;

namespace Application.DTO.Request.Inventory.Validators
{
    public class StockInCommandValidator : AbstractValidator<StockInCommand>
    {
        public StockInCommandValidator()
        {
            RuleFor(x => x.Request.ProductId).NotEmpty().WithMessage("ProductId is required");
            RuleFor(x => x.Request.Quantity).GreaterThan(0).WithMessage("Quantity must be > 0");
            RuleFor(x => x.Request.Reason).MaximumLength(500);
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
        }
    }

    public class StockOutCommandValidator : AbstractValidator<StockOutCommand>
    {
        public StockOutCommandValidator()
        {
            RuleFor(x => x.Request.ProductId).NotEmpty();
            RuleFor(x => x.Request.Quantity).GreaterThan(0);
            RuleFor(x => x.Request.Reason).MaximumLength(500);
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
