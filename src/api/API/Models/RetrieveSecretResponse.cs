namespace API.Models;

public class RetrieveSecretResponse
{
    public string SecretName { get; set; }
    public string EncryptedSecret { get; set; }
    public string ClientIV { get; set; }
}