namespace Ara.Infrastructure.Email;

public class ResendOptions
{
    public const string SectionName = "Resend";

    public string FromAddress { get; set; } = "onboarding@resend.dev";
}
