using Microsoft.IdentityModel.Tokens;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces
{
    public interface IEmailService
    {
        void SendVerificationEmail(string to, SecurityToken token);
    }
}
