using System;

namespace API.Models;

public class SecurityMetadataResponse
{
    public bool RequiresPassword { get; set; }
    public DateTime Expiration { get; set; }
}