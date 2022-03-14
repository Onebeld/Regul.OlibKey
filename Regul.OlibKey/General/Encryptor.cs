using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Regul.OlibKey.General
{
    public static class Encryptor
    {
        private static readonly RNGCryptoServiceProvider Rand = new RNGCryptoServiceProvider();

        public static int RandomInteger(int min, int max)
        {
            uint scale = uint.MaxValue;
            while (scale == uint.MaxValue)
            {
                byte[] fourBytes = new byte[4];
                Rand.GetBytes(fourBytes);
                scale = BitConverter.ToUInt32(fourBytes, 0);
            }

            return (int)(min + ((max - min) * (scale / (double)uint.MaxValue)));
        }

        private static byte[] GetRandomBytes()
        {
            byte[] ba = new byte[SaltLength];
            RandomNumberGenerator.Create().GetBytes(ba);
            return ba;
        }

        private static readonly int SaltLength = 50;

        private static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes, int iterations)
        {
            byte[] saltBytes = new byte[SaltLength];
            for (int i = 0; i < SaltLength; i++)
                saltBytes[i] = (byte)(i + 1);

            using (MemoryStream ms = new MemoryStream())
            {
                using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations))
                {
                    using (RijndaelManaged aes = new RijndaelManaged
                               { KeySize = 256, BlockSize = 128, Mode = CipherMode.CBC })
                    {
                        aes.Key = key.GetBytes((int)(aes.KeySize * 0.125));
                        aes.IV = key.GetBytes((int)(aes.BlockSize * 0.125));

                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                            cs.Close();
                        }
                    }
                }

                return ms.ToArray();
            }
        }

        private static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes, int iterations)
        {
            byte[] saltBytes = new byte[SaltLength];
            for (int i = 0; i < SaltLength; i++)
                saltBytes[i] = (byte)(i + 1);

            using (MemoryStream ms = new MemoryStream())
            {
                using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, iterations))
                {
                    using (RijndaelManaged aes = new RijndaelManaged
                               { KeySize = 256, BlockSize = 128, Mode = CipherMode.CBC })
                    {
                        aes.Key = key.GetBytes(aes.KeySize / 8);
                        aes.IV = key.GetBytes(aes.BlockSize / 8);

                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                            cs.Close();
                        }
                    }
                }

                return ms.ToArray();
            }
        }

        public static string EncryptString(string text, string masterPassword,
            int iterations, int numberOfEncryptionProcedures)
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
}