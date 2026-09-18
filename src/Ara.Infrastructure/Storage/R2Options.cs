namespace Ara.Infrastructure.Storage;

public class R2Options
{
    public const string SectionName = "R2";

    public string AccountId { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;

    /// <summary>Key prefix ("folder") the hero images live under in the bucket.</summary>
    public string HeroImagePrefix { get; set; } = "homepage/";

    /// <summary>Key prefix ("folder") the sign-up page photo lives under in the bucket.</summary>
    public string SignUpImagePrefix { get; set; } = "sign-up/";
}
