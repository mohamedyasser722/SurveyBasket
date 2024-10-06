namespace SurveyBasket.Api.Contracts.Roles;

public class RollRequestValidator : AbstractValidator<RollRequest>
{
    public RollRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(3, 200)
            .WithMessage("Role name is required");

        RuleFor(x => x.Permissions)
            .NotNull()
            .NotEmpty()
            .WithMessage("Role permissions are required");

        RuleFor(x => x.Permissions)
            .Must(x => x.Distinct().Count() == x.Count) // Check for duplicate permissions
            .WithMessage("You can't add duplicate permissions for the same role")
            .When(x => x.Permissions != null);
    }
}
