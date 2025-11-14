using CT.LnD.ContactManagement.Backend.DataAccess.Database.Dtos;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class GetContactByIdExample : IExamplesProvider<ContactResponse>
    {
        public ContactResponse GetExamples()
        {
            return new ContactResponse
            {
                Contact = new Contact
                {
                    Id = 2001,
                    FirstName = "Ritika",
                    LastName = "Verma",
                    UserId = 101,
                    Company = "Infosys Ltd.",
                    Notes = "Lead contact from Pune office.",
                    Tags = "india,client,priority",
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
                },
                EmailAddresses =
                [
                    new EmailAddressDto
                    {
                        TypeId = 1,
                        Email = "ritika.verma@infosys.com"
                    },
                    new EmailAddressDto
                    {
                        TypeId = 2,
                        Email = "ritika.personal@gmail.com"
                    }
                ],
                PhoneNumbers =
                [
                    new PhoneNumberDto
                    {
                        TypeId = 1,
                        CountryCode = "+91",
                        Number = "9876543210"
                    },
                    new PhoneNumberDto
                    {
                        TypeId = 2,
                        CountryCode = "+91",
                        Number = "08023456789"
                    }
                ],
                PhysicalAddresses =
                [
                    new PhysicalAddressDto
                    {
                        TypeId = 1,
                        Zipcode = "560001",
                        Street = "MG Road, Near Trinity Metro Station",
                        City = "Bengaluru",
                        State = "Karnataka",
                        Country = "India"
                    }
                ]
            };
        }
    }
}
