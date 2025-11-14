using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.User
{
    public class UserEmailUpdateRequestExample : IExamplesProvider<UserEmailUpdateRequest>
    {

        public UserEmailUpdateRequest GetExamples()
        {
            return new UserEmailUpdateRequest { Email = "aarav008@gmail.com" };
        }
    }
}
