namespace SurveyBasket.Api.Authentication;

public class JwtOptions
{
    public static string SectionName = "Jwt";
    [Required]
    public string Key { get; init; } = string.Empty;
    [Required]
    public string Issuer { get; init; } = string.Empty;
    [Required]
    public string Audience { get; init; } = string.Empty;
    [Required]
    [Range(1, int.MaxValue,ErrorMessage = $"Invalid ExpiryInMinutes it should be 1 or greater")]
    public int ExpiryInMinutes { get; init; }
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = $"Invalid RefreshTokenExpiryDays it should be 1 or greater")]
    public int RefreshTokenExpiryDays { get; init; }
}
