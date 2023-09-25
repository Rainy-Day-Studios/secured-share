using Azure.Data.Tables;
using Azure.Identity;
using Entities;
using UseCases.SecretManagement;

namespace Infrastructure.SecretPersistence.TableStorage;

public class TableStorageAccess : ISecretStore
{
    private readonly TableClient _secretTableClient;
    private readonly TableClient _accessHistoryTableClient;

    public TableStorageAccess(TableStorageConfig config)
    {
        TableServiceClient tableServiceClient;

        if (config.UseMSIAuth)
        {
            var defaultAzureCredential = new DefaultAzureCredential();

            tableServiceClient = new TableServiceClient(
                new Uri(config.StorageAccountUri),
                defaultAzureCredential
            );
        }
        else
        {
            tableServiceClient = new TableServiceClient(config.ConnectionString);
        }

        _secretTableClient = tableServiceClient.GetTableClient("secrets");
        _accessHistoryTableClient = tableServiceClient.GetTableClient("accesshistory");
    }

    public async Task<SecuredSecret> CreateSecret(SecuredSecret newSecret)
    {
        var rowKey = Guid.NewGuid().ToString()[..10].Replace("-", "");
        newSecret.Id = rowKey;

        var tableEntity = MapToTableEntity(newSecret);

        await _secretTableClient.AddEntityAsync(tableEntity);

        return newSecret;
    }

    public SecuredSecret GetSecret(string secretId)
    {
        var queryResults = _secretTableClient.Query<EncryptedSecretTableEntry>(e => e.RowKey == secretId, 1000);

        return MapToEntity(queryResults.FirstOrDefault());
    }

    public async Task<AccessEvent> CreateAccessEvent(AccessEvent accessEvent, SecuredSecret secret)
    {
        var rowKey = Guid.NewGuid().ToString();
        accessEvent.Id = rowKey;
        accessEvent.RelatedRecordId = secret.Id;

        var tableEntity = MapToTableEntity(accessEvent);

        await _accessHistoryTableClient.AddEntityAsync(tableEntity);

        return accessEvent;
    }

    public List<AccessEvent> GetAccessHistory(string secretId)
    {
        var queryResults = _accessHistoryTableClient.Query<AccessEventTableEntry>(e => e.PartitionKey == secretId, 1000);

        return queryResults.Select(MapToEntity).ToList();
    }

    private static EncryptedSecretTableEntry MapToTableEntity(SecuredSecret secret)
    {
        return new EncryptedSecretTableEntry
        {
            Name = secret.Name,
            EncryptedValue = secret.EncryptedValue,
            InternalIV = secret.InternalIV,
            ClientIV = secret.ClientIV,

            Password = secret.Metadata.Password,
            Salt = secret.Metadata.Salt,
            Expiration = new DateTime(secret.Metadata.Expiration.Ticks, DateTimeKind.Utc),
            MaxAccessCount = secret.Metadata.MaxAccessCount,

            RowKey = secret.Id,
            PartitionKey = secret.Id[..2]
        };
    }

    private static SecuredSecret MapToEntity(EncryptedSecretTableEntry secret)
    {
        if (secret == null) return null;

        return new SecuredSecret
        {
            Id = secret.RowKey,
            Name = secret.Name,
            EncryptedValue = secret.EncryptedValue,
            InternalIV = secret.InternalIV,
            ClientIV = secret.ClientIV,
            Metadata = new SecurityMetadata
            {
                Password = secret.Password,
                Salt = secret.Salt,
                Expiration = secret.Expiration,
                MaxAccessCount = secret.MaxAccessCount
            },
            CreatedDate = secret.Timestamp.Value.DateTime
        };
    }

    private static AccessEventTableEntry MapToTableEntity(AccessEvent accessEvent)
    {
        return new AccessEventTableEntry
        {
            RelatedRecordId = accessEvent.RelatedRecordId,

            RowKey = accessEvent.Id,
            PartitionKey = accessEvent.RelatedRecordId
        };
    }

    private static AccessEvent MapToEntity(AccessEventTableEntry accessEvent)
    {
        if (accessEvent == null) return null;

        return new AccessEvent
        {
            Id = accessEvent.RowKey,
            RelatedRecordId = accessEvent.RelatedRecordId,
            EventDate = accessEvent.Timestamp.Value.DateTime
        };
    }

}