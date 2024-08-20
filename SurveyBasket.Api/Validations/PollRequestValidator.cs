

using Microsoft.EntityFrameworkCore;

namespace SurveyBasket.Api.Validations;

public class PollRequestValidator : AbstractValidator<PollRequest>
{
    private readonly ApplicationDbContext _context;
    public PollRequestValidator(ApplicationDbContext context)
    {

        _context = context;

        RuleFor(x => x.Title).Cascade(CascadeMode.Stop)     // CascadeMode is to stop the validation on the first failure
            .NotEmpty()
            .Length(3,100)
            .Must(BeUniqueTitle).WithMessage("A poll with the same title already exists.");

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MaximumLength(1500);

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddSeconds(30));

        RuleFor(x => x.EndsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartsAt);


    }
    private bool BeUniqueTitle(string title)
    {
        return !_context.Polls.AsNoTracking().Any(p => p.Title == title);
    }
}
