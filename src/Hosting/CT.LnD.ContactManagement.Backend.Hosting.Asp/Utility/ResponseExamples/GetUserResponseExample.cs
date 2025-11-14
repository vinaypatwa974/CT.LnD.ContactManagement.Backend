using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class GetUserResponseExample : IExamplesProvider<List<GetUserResponse>>
    {
        public List<GetUserResponse> GetExamples()
        {
            return
            [
                new GetUserResponse
                {
                    Id = 101,
                    FirstName = "Abhinav",
                    LastName = "Arora",
                    Role = "User",
                    Status = "Active",
                    Email = "developerabhinv02@gmail.com",
                    LastLogin = DateTime.UtcNow.AddHours(-5),
                    CreatedAt = new DateTime(2022, 11, 5, 10, 0, 0)
                }
            ];
        }
    }
}
