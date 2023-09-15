using System.Security.Cryptography;
using System.Text;
namespace UseCases;

public static class Encryptor
{
    public static EncryptionResult Encrypt(string key, string value)
    {
        using var aesAlg = Aes.Create();
        aesAlg.Key = Encoding.UTF8.GetBytes(key);
        aesAlg.GenerateIV();

        // Create an encryptor to perform the stream transform.
        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        // Create the streams used for encryption.
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            //Write all data to the stream.
            swEncrypt.Write(value);
        }
        
        return new EncryptionResult
        {
            Value = Convert.ToBase64String(msEncrypt.ToArray()),
            IV = Convert.ToBase64String(aesAlg.IV)
        };
    }

    public static string Decrypt(string key, string iv, string value)
    {
        using var aesEncryptor = Aes.Create();
        aesEncryptor.Key = Encoding.UTF8.GetBytes(key);
        aesEncryptor.IV = Convert.FromBase64String(iv);

        ICryptoTransform decryptor = aesEncryptor.CreateDecryptor(aesEncryptor.Key, aesEncryptor.IV);

        using var msDecrypt = new MemoryStream(Convert.FromBase64String(value));
        using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

        using var swDecrypt = new StreamReader(csDecrypt);
        var result = swDecrypt.ReadToEnd();

        return result;
    }
}

public class EncryptionResult
{
    public string Value { get; set; }
    public string IV { get; set; }
}