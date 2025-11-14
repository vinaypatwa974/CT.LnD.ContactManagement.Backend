using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Dtos
{
    public class ContactResponse
    {
        public Contact Contact { get; set; }
        public List<EmailAddressDto> EmailAddresses { get; set; }
        public List<PhoneNumberDto> PhoneNumbers { get; set; }
        public List<PhysicalAddressDto> PhysicalAddresses { get; set; }
    }
}