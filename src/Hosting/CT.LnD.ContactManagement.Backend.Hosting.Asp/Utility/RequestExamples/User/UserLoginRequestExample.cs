using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.User
{
    public class UserLoginRequestExample : IExamplesProvider<UserLoginRequest>
    {
        public UserLoginRequest GetExamples()
        {
            return new UserLoginRequest
            {
                Email = "aarav.arora@gmail.com",
                Password = "Aarav@123"
            };
        }
    }
}
