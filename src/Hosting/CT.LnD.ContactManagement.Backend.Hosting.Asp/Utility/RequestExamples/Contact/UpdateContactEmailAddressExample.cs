using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.Contact
{
    public class UpdateContactEmailAddressExample : IExamplesProvider<EmailAddressDto>
    {
        public EmailAddressDto GetExamples()
        {
            return new EmailAddressDto
            {
                TypeId = 1,
                Email = "abhinav.arora@gmail.com"
            };
        }
    }
}
