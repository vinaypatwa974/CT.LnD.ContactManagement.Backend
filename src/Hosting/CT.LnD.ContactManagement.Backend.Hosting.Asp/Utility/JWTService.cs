using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility
{
    public class JWTService : IJWTService
    {
        public SecurityToken GenerateJSONWebToken(IConfiguration configuration, string userEmail)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
                new Claim(ClaimTypes.Email, userEmail)
            ];

            JwtSecurityToken token = new(
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials,
            claims: claims
            );
            return token;
        }


        public ClaimsPrincipal ValidateToken(IConfiguration configuration, string token)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));

            TokenValidationParameters validationParameters = new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
            };

            try
            {
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch (SecurityTokenException ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }


        public SecurityToken GenerateJSONWebToken(IConfiguration configuration, GetUserResponse user)
        {
            SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

            Claim[] claims = [
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("Status", user.Status),
            ];

            JwtSecurityToken token = new(
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials,
            claims: claims
            );


            return token;
        }
    }
}
