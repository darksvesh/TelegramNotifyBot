using System;
using System.Security.Cryptography;
using System.Text;

public static class EncryptionService
{
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("ZzRFrHnhZZqxgRbP");

    public static string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new System.IO.MemoryStream();
        ms.Write(aes.IV, 0, aes.IV.Length);

        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
        using (var writer = new System.IO.StreamWriter(cs))
        {
            writer.Write(plainText);
        }

        return Convert.ToBase64String(ms.ToArray());
    }

    public static string Decrypt(string cipherText)
    {
        var fullCipher = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = Key;
        var iv = new byte[aes.BlockSize / 8];
        var cipher = new byte[iv.Length];

        Array.Copy(fullCipher, iv, iv.Length);
        Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

        var decryptor = aes.CreateDecryptor(aes.Key, iv);

        using var ms = new System.IO.MemoryStream(cipher);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new System.IO.StreamReader(cs);

        return reader.ReadToEnd();
    }
}
