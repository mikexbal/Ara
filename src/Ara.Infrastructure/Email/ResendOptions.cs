namespace Ara.Infrastructure.Email;

public class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; set; } = string.Empty;

    public string FromAddress { get; set; } = "onboarding@resend.dev";
}
