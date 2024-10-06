namespace SurveyBasket.Api.Health;


public class MailProviderHealthCheck(IOptions<MailSettings> mailSettings) : IHealthCheck
{
    private readonly MailSettings _mailSettings = mailSettings.Value;
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {

        try
        {
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls, cancellationToken); // Add cancellationToken
            await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password, cancellationToken); // Add cancellationToken

            return await Task.FromResult(HealthCheckResult.Healthy("Mail provider is healthy"));
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Mail provider is unhealthy", ex);
        }
    }
}
