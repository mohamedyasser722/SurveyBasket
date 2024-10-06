namespace SurveyBasket.Api.Contracts.Questions;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{

    public QuestionRequestValidator()
    {


        RuleFor(x => x.Content)
            .NotEmpty()
            .Length(3, 1000)
            .When(x => x.Content != null);

        RuleFor(x => x.Answers)
            .NotEmpty()
            .Must(x => x.Count > 1)
            .WithMessage("Question must have at least 2 answers")
            .When(x => x.Answers != null);


        RuleFor(x => x.Answers)
            .NotEmpty()
           .Must(answers => answers.Distinct().Count() == answers.Count)
           .WithMessage("You can't add duplicated answers for the same question")
           .When(x => x.Answers != null);
    }

}
