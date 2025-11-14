using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces
{
    public interface IJWTService
    {
        SecurityToken GenerateJSONWebToken(IConfiguration configuratoin, string userEmail);

        SecurityToken GenerateJSONWebToken(IConfiguration configuratoin, GetUserResponse userEmail);


        ClaimsPrincipal ValidateToken(IConfiguration configuration, string token);
    }
}
