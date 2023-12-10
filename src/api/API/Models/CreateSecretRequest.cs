using System;

namespace API.Models;

public class CreateSecretRequest
{
    public string Name { get; set; }
    public string EncryptedValue { get; set; }
    public string ClientIV { get; set; }

    public string Password { get; set; }
    public DateTime Expiration { get; set; }
    public int? MaxAccessCount { get; set; }
}