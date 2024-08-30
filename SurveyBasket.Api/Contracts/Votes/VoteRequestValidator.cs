namespace SurveyBasket.Api.Contracts.Votes;

public class VoteRequestValidator : AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x => x.Answers)
            .NotEmpty()
            .WithMessage("Answers are required");

        RuleForEach(x => x.Answers)
            .SetInheritanceValidator(v =>
                v.Add(new VoteAnswerRequestValidator())
            );
    }
}
