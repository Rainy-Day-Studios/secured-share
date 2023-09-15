using System.Security.Cryptography;
using System.Text;

namespace UseCases;

public static class Hasher
{
    public static string ComputeHash(string value, string salt)
    {
        var saltedValue = $"{value}|{salt}";
        var hashResult = SHA512.HashData(Encoding.UTF8.GetBytes(saltedValue));

        return Convert.ToBase64String(hashResult);
    }
}