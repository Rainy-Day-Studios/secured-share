namespace Entities;

public class SecurityMetadata
{
    public string Password { get; set; }
    public bool RequiresPassword
    {
        get
        {
            return !string.IsNullOrWhiteSpace(Password);
        }
    }
    public string Salt { get; set; }

    public DateTime Expiration { get; set; }
    public int? MaxAccessCount { get; set; }

    public bool IsExpired(DateTime utcNow)
    {
        return Expiration < utcNow;
    }
}