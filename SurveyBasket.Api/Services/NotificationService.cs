namespace SurveyBasket.Api.Services;

public class NotificationService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IEmailSender emailSender) : INotificationService
{

    private readonly ApplicationDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task SendNewPollsNotification(int? pollId = null)
    {
        IEnumerable<Poll> polls = [];

        if (pollId.HasValue)
        {
            var poll = await context.Polls.SingleOrDefaultAsync(p => p.Id == pollId && p.IsPublished);

            polls = [poll];

        }
        else
        {

            polls = await _context.Polls
            .Where(p => p.IsPublished
                    && p.StartsAt <= DateTime.UtcNow.Date
                    && p.EndsAt >= DateTime.UtcNow.Date)
                                        .AsNoTracking()
                                        .ToListAsync();

        }

        // TODO: select members only to send notification

        var users = await _userManager.Users.ToListAsync();
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;


        foreach (var poll in polls)
        {
            foreach (var user in users)
            {
                var placeholder = new Dictionary<string, string>
                {
                    { "{name}", $"{user.FirstName} {user.LastName}" },
                    { "{Title}", poll.Title },
                    {"{StartsAt}" ,poll.StartsAt.ToString() },
                    {"{EndsAt}" ,poll.EndsAt.ToString() },
                    {"{PollUrl}" ,$"{origin}/api/polls/{pollId}/vote" }
                };

                var body = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeholder);

                BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, $"🔔 New Poll Available {poll.Title}", body));
            }
        }
    }
}
