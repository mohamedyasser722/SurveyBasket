namespace SurveyBasket.Api.Hangfire;

using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;

public class HangfireCustomBasicAuthenticationFilter : IDashboardAuthorizationFilter
{
    private readonly string _username;
    private readonly string _password;

    public HangfireCustomBasicAuthenticationFilter(string username, string password)
    {
        _username = username;
        _password = password;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Extract the Authorization header
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Basic "))
        {
            Challenge(httpContext);
            return false;
        }

        // Decode and parse the credentials
        var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
        var decodedCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
        var credentials = decodedCredentials.Split(':', 2);

        if (credentials.Length != 2)
        {
            Challenge(httpContext);
            return false;
        }

        var providedUsername = credentials[0];
        var providedPassword = credentials[1];

        // Validate credentials
        if (providedUsername.Equals(_username, StringComparison.InvariantCultureIgnoreCase) &&
            providedPassword.Equals(_password))
        {
            return true;
        }

        Challenge(httpContext);
        return false;
    }

    private void Challenge(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.Headers.Append("WWW-Authenticate", "Basic realm=\"Hangfire Dashboard\"");
    }
}
