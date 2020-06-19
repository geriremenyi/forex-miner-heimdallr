namespace ForexMiner.Heimdallr.UserManager.Utilities
{
    using ForexMiner.Heimdallr.UserManager.Database;
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using System;
    using System.Security.Cryptography;

    public static class UserExtension
    {
        public static void UpdatePassword(this User user, string plainPassword)
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            var password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
               password: plainPassword,
               salt: salt,
               prf: KeyDerivationPrf.HMACSHA1,
               iterationCount: 10000,
               numBytesRequested: 256 / 8)
            );

            user.PasswordSalt = salt.ToString();
            user.PasswordHash = password;
        }

    }
}
