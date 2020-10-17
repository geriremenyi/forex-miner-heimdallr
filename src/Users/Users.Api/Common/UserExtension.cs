namespace ForexMiner.Heimdallr.Users.Api.Common
{
    using ForexMiner.Heimdallr.Common.Data.Database.Models;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public static class UserExtension
    {
        public static void UpdatePassword(this User user, string plainPassword)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            var password = HashPassword(plainPassword, salt);

            user.PasswordSalt = salt.ToString();
            user.PasswordHash = password;
        }

        public static bool IsPasswordCorrect(this User user, string plainPassword)
        {
            var hashedPassword = HashPassword(plainPassword, Encoding.ASCII.GetBytes(user.PasswordSalt));

            return hashedPassword.Equals(user.PasswordHash);
        }

        private static string HashPassword(string plainPassword, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: plainPassword,
               salt: salt,
               prf: KeyDerivationPrf.HMACSHA1,
               iterationCount: 10000,
               numBytesRequested: 256 / 8)
            );
        }
    }
}
