using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.Contact
{
    public class UpdateContactPhysicalAddressExample : IExamplesProvider<PhysicalAddressDto>
    {
        public PhysicalAddressDto GetExamples()
        {
            return new PhysicalAddressDto
            {
                TypeId = 1,
                Zipcode = "560001",
                Street = "MG Road, Near Trinity Metro Station",
                City = "Bengaluru",
                State = "Karnataka",
                Country = "India"
            };
        }
    }
}
