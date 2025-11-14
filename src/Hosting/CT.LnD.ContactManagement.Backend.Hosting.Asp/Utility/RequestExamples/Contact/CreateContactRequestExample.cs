using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.RequestExamples.Contact
{
    public class CreateContactRequestExample : IExamplesProvider<ContactRequest>
    {
        public ContactRequest GetExamples()
        {
            return new ContactRequest
            {
                FirstName = "Rakesh",
                LastName = "Arora",
                UserId = 101,
                Company = "Infosys Ltd.",
                Notes = "Lead developer contact from Infosys",
                Tags = "developer,india,it",

                PhoneNumber =
            [
                new() {
                    TypeId = 1,
                    CountryCode = "+91",
                    Number = "9876543210"
                },
                new PhoneNumberDto
                {
                    TypeId = 2,
                    CountryCode = "+91",
                    Number = "9123456789"
                },
                new PhoneNumberDto
                {
                    TypeId = 3,
                    CountryCode = "+91",
                    Number = "9988776655"
                }
            ],

                EmailAddress =
            [
                new EmailAddressDto
                {
                    TypeId = 1,
                    Email = "rakesh.arora@infosys.com"
                },
                new EmailAddressDto
                {
                    TypeId = 2,
                    Email = "rakesh1972@gmail.com"
                },
                new EmailAddressDto
                {
                    TypeId = 3,
                    Email = "rakesh001@gmail.com"
                }
            ],

                PhysicalAddress =
            [
                new PhysicalAddressDto
                {
                    TypeId = 1,
                    Zipcode = "110001",
                    Street = "Rajpath Road, Connaught Place",
                    City = "New Delhi",
                    State = "Delhi",
                    Country = "India"
                },
                new PhysicalAddressDto
                {
                    TypeId = 2,
                    Zipcode = "560001",
                    Street = "MG Road, Trinity Circle",
                    City = "Bengaluru",
                    State = "Karnataka",
                    Country = "India"
                },
                new PhysicalAddressDto
                {
                    TypeId = 3,
                    Zipcode = "400001",
                    Street = "Nariman Point, Marine Drive",
                    City = "Mumbai",
                    State = "Maharashtra",
                    Country = "India"
                }
            ]
            };
        }
    }
}
