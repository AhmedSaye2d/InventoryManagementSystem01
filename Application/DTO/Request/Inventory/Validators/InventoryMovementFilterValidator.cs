using FluentValidation;

namespace Application.DTO.Request.Inventory.Validators
{
    public class InventoryMovementFilterRequestDtoValidator : AbstractValidator<InventoryMovementFilterRequestDto>
    {
        public InventoryMovementFilterRequestDtoValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("PageNumber must be greater than 0");
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100).WithMessage("PageSize must be between 1 and 100");
            RuleFor(x => x.DateFrom)
                .LessThanOrEqualTo(x => x.DateTo)
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue)
                .WithMessage("DateFrom must be less than or equal to DateTo");
        }
    }

    public class GetInventoryMovementsQueryValidator : AbstractValidator<Application.Services.Inventory.Queries.GetInventoryMovementsQuery>
    {
        public GetInventoryMovementsQueryValidator()
        {
            RuleFor(x => x.Filter.PageNumber).GreaterThan(0);
            RuleFor(x => x.Filter.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.Filter.DateFrom)
                .LessThanOrEqualTo(x => x.Filter.DateTo)
                .When(x => x.Filter.DateFrom.HasValue && x.Filter.DateTo.HasValue);
        }
    }

    public class GetInventoryMovementByIdValidator : AbstractValidator<Application.Services.Inventory.Queries.GetInventoryMovementByIdQuery>
    {
        public GetInventoryMovementByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
        }
    }

    public class GetProductInventoryHistoryValidator : AbstractValidator<Application.Services.Inventory.Queries.GetProductInventoryHistoryQuery>
    {
        public GetProductInventoryHistoryValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
            RuleFor(x => x.PageNumber).GreaterThan(0);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.DateFrom)
                .LessThanOrEqualTo(x => x.DateTo)
                .When(x => x.DateFrom.HasValue && x.DateTo.HasValue);
        }
    }
}
