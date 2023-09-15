using Azure;
using Azure.Data.Tables;

namespace Infrastructure.SecretPersistence.TableStorage;

public class AccessEventTableEntry : ITableEntity
{
    public string RelatedRecordId { get; set; }

    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}