using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.User
{
    public class UserRegisterRequestExample : IExamplesProvider<UserRegisterRequest>
    {
        public UserRegisterRequest GetExamples()
        {
            return new UserRegisterRequest
            {
                FirstName = "Aarav",
                LastName = "Arora",
                Email = "aarav.arora@gmail.com",
                Password = "Aarav@123"
            };
        }
    }
}
