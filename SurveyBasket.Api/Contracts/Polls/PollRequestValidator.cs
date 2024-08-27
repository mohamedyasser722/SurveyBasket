

using Microsoft.EntityFrameworkCore;

namespace SurveyBasket.Api.Contracts.Polls;

public class LoginRequestValidator : AbstractValidator<PollRequest>
{
    private readonly ApplicationDbContext _context;
    public LoginRequestValidator(ApplicationDbContext context)
    {

        _context = context;

        RuleFor(x => x.Title).Cascade(CascadeMode.Stop)     // CascadeMode is to stop the validation on the first failure
            .NotEmpty()
            .Length(3, 100);

        RuleFor(x => x.Summary)
            .NotEmpty()
            .MaximumLength(1500);

        RuleFor(x => x.StartsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddSeconds(30))
            .WithMessage("Start time must be at least 30 seconds in the future. ");

        RuleFor(x => x.EndsAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartsAt);


    }
}
