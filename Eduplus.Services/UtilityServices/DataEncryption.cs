using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace KS.AES256Encryption
{
    public class DataEncryption
    {
        public static string AESEncryptData(string originalString)
        {

            using (Aes aesEncryption = Aes.Create())
            {
                byte[] key = { 7, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8,
                           1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };
                aesEncryption.KeySize = 256;
                aesEncryption.BlockSize = 128;
                aesEncryption.Mode = CipherMode.CBC;
                aesEncryption.Padding = PaddingMode.PKCS7;
                aesEncryption.IV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 0 };
                aesEncryption.Key = key;
                byte[] originalToByte = Encoding.UTF8.GetBytes(originalString);
                ICryptoTransform crypto = aesEncryption.CreateEncryptor();
                byte[] cipherText = crypto.TransformFinalBlock(originalToByte, 0, originalToByte.Length);
                string chck = Convert.ToBase64String(cipherText);
                return chck;
            }
           

        }
        public static string AESDecryptData(string encryptedData)
        {
            try
            {
                string final = encryptedData.Replace(' ', '+');
                using (Aes aesEncryption = Aes.Create())
                {
                    byte[] key = { 7, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8,
                           1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 8 };
                    aesEncryption.KeySize = 256;
                    aesEncryption.BlockSize = 128;
                    aesEncryption.Mode = CipherMode.CBC;
                    aesEncryption.Padding = PaddingMode.PKCS7;
                    aesEncryption.IV = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 1, 2, 3, 4, 5, 6, 7, 0 };
                    aesEncryption.Key = key;
                    byte[] originalByte = Convert.FromBase64String(final);
                    ICryptoTransform crypto = aesEncryption.CreateDecryptor();
                    byte[] decipheredByte = crypto.TransformFinalBlock(originalByte, 0, originalByte.Length);

                    string chk = Encoding.UTF8.GetString(decipheredByte);
                    return chk;
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }

        
        public static string GetUniqueKey(int maxSize = 15)
        {
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        
        public static string GenerateSHA256(string text)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GenerateSHA512(string text)
        {
            using (SHA512 sha = SHA512.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }

        }

        public static string GenerateRRRSHA512(string text)
        {
            using (SHA512 sha = SHA512.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }

        }

        public static bool ValidateFinalCheckSum(string checksum,string rawData)
        {
            string checksumHash;

            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                checksumHash= builder.ToString();
            }

            //compare checksum
            return string.Compare(checksum, checksumHash) == 0;
        }
    }
}
