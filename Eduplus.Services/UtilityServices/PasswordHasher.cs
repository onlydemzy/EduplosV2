using System;
using System.Security.Cryptography;
using System.Text;

namespace Eduplus.Services.Implementations
{
    public static class PasswordMaker
    {
        public static string GeneratePassword()
        {
            string password = Guid.NewGuid().ToString().Substring(0, 6);
            return password;
        }
        public static string HashPassword(string password)
        {
            string salt = GenerateSalt();
            string hash = Sha512Hex(salt + password);

            return salt + hash;
        }
        private static string GenerateSalt()
        {
            RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
            byte[] salt = new byte[64];//512 bits
            random.GetBytes(salt);
            return BytesToHex(salt);

        }
        public static string Sha512Hex(string toHash)
        {
            SHA512Managed sha = new SHA512Managed();
            byte[] utf8 = UTF8Encoding.UTF8.GetBytes(toHash);
            return BytesToHex(sha.ComputeHash(utf8));
        }


        private static string BytesToHex(byte[] toConvert)
        {
            StringBuilder sbuilder = new StringBuilder(toConvert.Length * 2);
            foreach (byte b in toConvert)
            {
                sbuilder.Append(b.ToString("x2"));
            }
            return sbuilder.ToString();
        }

        public static bool ValidateUserPassword(string password, string correctHash)
        {
            if (correctHash.Length < 256)
            {
                throw new ArgumentException("Correct hash from db must be 256");
            }
            string saltComponent = correctHash.Substring(0, 128);
            string hashComponent = correctHash.Substring(128, 128);

            string inputtedPasswordHash = Sha512Hex(saltComponent + password);

            return string.Compare(hashComponent, inputtedPasswordHash) == 0;
        }
    }
}
