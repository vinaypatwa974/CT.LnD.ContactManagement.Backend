namespace CT.LnD.ContactManagement.Backend.Dto.Business
{
    public class ContactToCSVModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Tags { get; set; }
        public string Notes { get; set; }
        public string Company { get; set; }

        // Phone Numbers
        public string PrimaryPhoneNumber { get; set; }
        public string PrimaryPhoneCountryCode { get; set; }

        public string SecondaryPhoneNumber { get; set; }
        public string SecondaryPhoneCountryCode { get; set; }

        public string WorkPhoneNumber { get; set; }
        public string WorkPhoneCountryCode { get; set; }

        // Email Addresses
        public string PrimaryEmailAddress { get; set; }
        public string SecondaryEmailAddress { get; set; }
        public string WorkEmailAddress { get; set; }

        // Home Address
        public string HomeAddressStreet { get; set; }
        public string HomeAddressCity { get; set; }
        public string HomeAddressState { get; set; }
        public string HomeAddressCountry { get; set; }
        public string HomeAddressZipcode { get; set; }

        // Work Address
        public string WorkAddressStreet { get; set; }
        public string WorkAddressCity { get; set; }
        public string WorkAddressState { get; set; }
        public string WorkAddressCountry { get; set; }
        public string WorkAddressZipcode { get; set; }

        // Other Address
        public string OtherAddressStreet { get; set; }
        public string OtherAddressCity { get; set; }
        public string OtherAddressState { get; set; }
        public string OtherAddressCountry { get; set; }
        public string OtherAddressZipcode { get; set; }
    }
}
