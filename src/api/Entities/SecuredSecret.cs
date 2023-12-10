namespace Entities;

public class SecuredSecret
{
    public string Id { get; set; }
    public string Name { get; set; }

    public string EncryptedValue { get; set; }
    public string InternalIV { get; set; }
    public string ClientIV { get; set; }

    public SecurityMetadata Metadata { get; set; }
    public List<AccessEvent> AccessHistory { get; set; } = new List<AccessEvent>();
    public DateTime CreatedDate { get; set; }

    public bool MaxAccessCountExceeded
    {
        get
        {
            if ((Metadata?.MaxAccessCount ?? 0) == 0)
            {
                return false;
            }

            return AccessHistory.Count >= Metadata.MaxAccessCount;
        }
    }
}