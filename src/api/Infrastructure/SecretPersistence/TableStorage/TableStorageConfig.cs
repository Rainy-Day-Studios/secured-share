namespace Infrastructure.SecretPersistence.TableStorage;

public class TableStorageConfig
{
    public bool UseMSIAuth { get; set; }
    public string StorageAccountUri { get; set; }
    public string ConnectionString {get;set;}
}