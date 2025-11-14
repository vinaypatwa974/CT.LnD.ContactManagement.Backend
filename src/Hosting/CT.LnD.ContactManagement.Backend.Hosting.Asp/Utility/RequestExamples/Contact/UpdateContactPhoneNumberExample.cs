using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.Contact
{
    public class UpdateContactPhoneNumberExample : IExamplesProvider<PhoneNumberDto>
    {
        public PhoneNumberDto GetExamples()
        {
            return new PhoneNumberDto
            {
                TypeId = 1,
                CountryCode = "+91",
                Number = "9876543210"
            };
        }
    }
}
