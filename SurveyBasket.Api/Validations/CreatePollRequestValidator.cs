

namespace SurveyBasket.Api.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title).Cascade(CascadeMode.Stop)     // CascadeMode is to stop the validation on the first failure
            .NotEmpty()
            .Length(3,100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);
    }
}
