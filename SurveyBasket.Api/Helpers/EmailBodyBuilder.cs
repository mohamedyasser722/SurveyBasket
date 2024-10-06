namespace SurveyBasket.Api.Helpers;

public class EmailBodyBuilder
{
    public static string GenerateEmailBody(string templete, Dictionary<string, string> templateModel)
    {

        var templatePath = $"{Directory.GetCurrentDirectory()}\\Templates\\{templete}.html";

        using var streamReader = new StreamReader(templatePath);
        var body = streamReader.ReadToEnd();

        foreach (var (key, value) in templateModel)
        {
            body = body.Replace(key, value);
        }

        return body;
    }
}
