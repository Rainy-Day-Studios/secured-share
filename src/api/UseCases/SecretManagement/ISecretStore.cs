using Entities;

namespace UseCases.SecretManagement;

public interface ISecretStore
{
    Task<SecuredSecret> CreateSecret(SecuredSecret newSecret);
    SecuredSecret GetSecret(string secretId);
    List<AccessEvent> GetAccessHistory(string secretId);
    
    Task<AccessEvent> CreateAccessEvent(AccessEvent accessEvent, SecuredSecret secret);
}