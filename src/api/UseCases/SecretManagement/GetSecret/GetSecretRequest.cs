namespace UseCases.SecretManagement.DTO;

public class GetSecretRequest
{
    public string SecretId { get; set; }
    public string Password { get; set; }
}