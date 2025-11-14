using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.User
{
    public class UserUpdateNameRequestExample : IExamplesProvider<UserUpdateNameRequest>
    {
        public UserUpdateNameRequest GetExamples()
        {
            return new UserUpdateNameRequest
            {
                FirstName = "Aarav",
                LastName = "Sharma"
            };
        }
    }
}
