using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.Contact
{
    public class UpdateBasicContactDetailsRequestExample : IExamplesProvider<BasicContactDetailsUpdateRequest>
    {
        public BasicContactDetailsUpdateRequest GetExamples()
        {
            return new BasicContactDetailsUpdateRequest
            {
                FirstName = "Rakesh",
                LastName = "Arora",
                Company = "Tata Consultancy Services",
                Notes = "Key decision maker for South Zone IT procurement.",
                Tags = "Lead, IT, SouthZone"
            };
        }
    }
}
