namespace SurveyBasket.Api.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotEmpty();

        RuleFor(x => x.Password)
        .NotEmpty()
        .Matches(RegexPatterns.Password)
        .WithMessage("Password must contain at least 1 uppercase letter, 1 lowercase letter, 1 number, 1 special character and be at least 8 characters long");

        RuleFor(x => x.FirstName)
            .Length(2, 100)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .Length(2, 100)
            .NotEmpty();

    }
}
