using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Regul.OlibKey.General;

public static class Encryptor
{
    private static byte[] GetRandomBytes()
    {
        byte[] ba = new byte[SaltLength];
        RandomNumberGenerator.Create().GetBytes(ba);
        return ba;
    }

    private const int SaltLength = 50;

    private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes, int iterations)
    {
        byte[] saltBytes = new byte[SaltLength];
        for (int i = 0; i < SaltLength; i++)
            saltBytes[i] = (byte)(i + 1);

        using MemoryStream memoryStream = new();
        using (Rfc2898DeriveBytes key = new(passwordBytes, saltBytes, iterations))
        {
            using (RijndaelManaged aes = new() { KeySize = 256, BlockSize = 128, Mode = CipherMode.CBC })
            {
                aes.Key = key.GetBytes((int)(aes.KeySize * 0.125));
                aes.IV = key.GetBytes((int)(aes.BlockSize * 0.125));

                using (CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                    cryptoStream.Close();
                }
            }
        }

        return memoryStream.ToArray();
    }

    private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, int iterations)
    {
        byte[] saltBytes = new byte[SaltLength];
        for (int i = 0; i < SaltLength; i++)
            saltBytes[i] = (byte)(i + 1);

        using MemoryStream memoryStream = new();
        using (Rfc2898DeriveBytes key = new(passwordBytes, saltBytes, iterations))
        {
            using (RijndaelManaged aes = new() { KeySize = 256, BlockSize = 128, Mode = CipherMode.CBC })
            {
                aes.Key = key.GetBytes((int)(aes.KeySize * 0.125));
                aes.IV = key.GetBytes((int)(aes.BlockSize * 0.125));

                using (CryptoStream cryptoStream = new(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                    cryptoStream.Close();
                }
            }
        }

        return memoryStream.ToArray();
    }

    public static string EncryptString(string text, string masterPassword, int iterations, int numberOfEncryptionProcedures)
    {
        string encryptString = text;
        byte[] baPwd = Encoding.UTF8.GetBytes(masterPassword);
        byte[] baPwdHash = SHA256.Create().ComputeHash(baPwd);

        for (int d = 0; d < numberOfEncryptionProcedures; d++)
        {
            byte[] baText = Encoding.UTF8.GetBytes(encryptString);
            byte[] baSalt = GetRandomBytes();
            byte[] baEncrypted = new byte[baSalt.Length + baText.Length];
            for (int i = 0; i < baSalt.Length; i++) baEncrypted[i] = baSalt[i];
            for (int i = 0; i < baText.Length; i++) baEncrypted[i + baSalt.Length] = baText[i];
            baEncrypted = AES_Encrypt(baEncrypted, baPwdHash, iterations);

            encryptString = Convert.ToBase64String(baEncrypted);
        }

        return encryptString;
    }

    public static string DecryptString(string text, string masterPassword, int iterations,
        int numberOfEncryptionProcedures)
    {
        string result = text;

        byte[] baPwdHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(masterPassword));

        for (int d = 0; d < numberOfEncryptionProcedures; d++)
        {
            byte[] baText = Convert.FromBase64String(result);
            byte[] baDecrypted = AES_Decrypt(baText, baPwdHash, iterations);
            byte[] baResult = new byte[baDecrypted.Length - SaltLength];
            for (int i = 0; i < baResult.Length; i++) baResult[i] = baDecrypted[i + SaltLength];

            result = Encoding.UTF8.GetString(baResult);
        }

        return result;
    }
}