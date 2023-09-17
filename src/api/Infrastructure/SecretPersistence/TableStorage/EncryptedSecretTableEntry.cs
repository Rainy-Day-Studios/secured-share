using Azure;
using Azure.Data.Tables;

namespace Infrastructure.SecretPersistence.TableStorage;

public class EncryptedSecretTableEntry : ITableEntity
{
    public string Name { get; set; }

    public string EncryptedValue { get; set; }
    public string InternalIV { get; set; }

    public string Password { get; set; }
    public string Salt { get; set; }
    public DateTime Expiration { get; set; }
    public int? MaxAccessCount { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}