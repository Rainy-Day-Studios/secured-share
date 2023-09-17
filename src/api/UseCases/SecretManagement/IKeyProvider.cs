namespace UseCases.SecretManagement;

public interface IKeyProvider
{
    Task<string> GetSecretEncryptionKey();
}