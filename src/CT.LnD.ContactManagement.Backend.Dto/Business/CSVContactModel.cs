using CT.LnD.ContactManagement.Backend.Dto.Hosting;

namespace CT.LnD.ContactManagement.Backend.Dto.Business
{
    public class CSVContactModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? Company { get; set; }

        public string? Notes { get; set; }

        public string? Tags { get; set; }

        public PhoneNumberDto PrimaryContactNumber { get; set; }
        public PhoneNumberDto? SecondaryContactNumber { get; set; }
        public PhoneNumberDto? WorkPhoneNumber { get; set; }

        public EmailAddressDto? PrimaryEmailAddress { get; set; }
        public EmailAddressDto? SecondaryEmailAddress { get; set; }
        public EmailAddressDto? WorkEmailAddress { get; set; }

        public PhysicalAddressDto? HomeAddress { get; set; }
        public PhysicalAddressDto? WorkAddress { get; set; }
        public PhysicalAddressDto? OtherAddress { get; set; }
    }
}
