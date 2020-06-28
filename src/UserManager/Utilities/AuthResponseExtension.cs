namespace ForexMiner.Heimdallr.UserManager.Utilities
{
    using ForexMiner.Heimdallr.Utilities.Data.User;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public static class AuthResponseExtension
    {
        public static void AddNewJwtToken(this AuthenticationResponseDTO authResponse, string encryptionSecret)
        {
            // Token construction
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, authResponse.UserId.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(encryptionSecret)), SecurityAlgorithms.HmacSha256Signature)
            };

            // Extend authentication response with the token created
            authResponse.Token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
    }
}
