namespace SurveyBasket.Api.Abstraction.Consts;

public static class RateLimiterPolicyNames
{
    public const string ipLimit = nameof(ipLimit);
    public const string userLimit = nameof(userLimit);
    public const string concurrency = nameof(concurrency);
    public const string tokenBucket = nameof(tokenBucket);
    public const string fixedWindow = nameof(fixedWindow);
    public const string slidingWindow = nameof(slidingWindow);

}
