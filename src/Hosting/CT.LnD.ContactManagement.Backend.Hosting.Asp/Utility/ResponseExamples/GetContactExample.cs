using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class GetContactExample : IExamplesProvider<List<Contact>>
    {
        public List<Contact> GetExamples()
        {
            return
        [
            new Contact
            {
                Id = 2001,
                FirstName = "Ritika",
                LastName = "Verma",
                UserId = 101,
                Company = "Infosys Ltd.",
                Notes = "Met during the Tech Summit in Pune.",
                Tags = "tech,india,client",
                Avatar = "https://example.com/avatars/ritika.jpg",
                User = new User
                {
                    Id = 101,
                    FirstName = "Amit",
                    LastName = "Kumar",
                    Email = "amit.kumar@company.in",
                    RoleId = 1,
                    StatusId = 4,
                    CreatedAt = DateTime.UtcNow.AddYears(-2),
                    LastLogin = DateTime.UtcNow.AddDays(-2)
                }
            }
        ];
        }
    }
}
