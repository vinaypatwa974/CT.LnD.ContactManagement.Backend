using CT.LnD.ContactManagement.Backend.Dto.Business;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Dtos;

namespace CT.LnD.ContactManagement.Backend.Business.Mappers
{
    public class ContactToFlatContactMapper
    {
        public static List<ContactToCSVModel> Map(List<ContactResponse> contacts)
        {
            List<ContactToCSVModel> res = [];

            foreach (ContactResponse contact in contacts)
            {
                ContactToCSVModel flattenedContact = new()
                {
                    FirstName = contact.Contact.FirstName,
                    LastName = contact.Contact.LastName
                };

                if (contact.Contact.Notes != null)
                {
                    flattenedContact.Notes = contact.Contact.Notes;
                }

                if (contact.Contact.Tags != null)
                {
                    flattenedContact.Tags = contact.Contact.Tags;
                }

                if (contact.Contact.Company != null)
                {
                    flattenedContact.Company = contact.Contact.Company;
                }

                foreach (PhoneNumberDto phoneNumber in contact.PhoneNumbers)
                {
                    int typeId = phoneNumber.TypeId;

                    switch (typeId)
                    {
                        case 1:
                            flattenedContact.PrimaryPhoneCountryCode = phoneNumber.CountryCode;
                            flattenedContact.PrimaryPhoneNumber = phoneNumber.Number;
                            break;
                        case 2:
                            flattenedContact.SecondaryPhoneCountryCode = phoneNumber.CountryCode;
                            flattenedContact.SecondaryPhoneNumber = phoneNumber.Number;
                            break;
                        case 3:
                            flattenedContact.WorkPhoneCountryCode = phoneNumber.CountryCode;
                            flattenedContact.WorkPhoneNumber = phoneNumber.Number;
                            break;
                        default:
                            throw new Exception("Invalid contact Type");
                    }
                }

                foreach (PhysicalAddressDto physicalAddress in contact.PhysicalAddresses)
                {
                    int typeId = physicalAddress.TypeId;

                    switch (typeId)
                    {
                        case 1:
                            flattenedContact.HomeAddressCountry = physicalAddress.Country;
                            flattenedContact.HomeAddressCity = physicalAddress.City;
                            flattenedContact.HomeAddressState = physicalAddress.State;
                            flattenedContact.HomeAddressStreet = physicalAddress.Street;
                            flattenedContact.HomeAddressZipcode = physicalAddress.Zipcode;
                            break;
                        case 2:
                            flattenedContact.WorkAddressCountry = physicalAddress.Country;
                            flattenedContact.WorkAddressCity = physicalAddress.City;
                            flattenedContact.WorkAddressState = physicalAddress.State;
                            flattenedContact.WorkAddressStreet = physicalAddress.Street;
                            flattenedContact.WorkAddressZipcode = physicalAddress.Zipcode;
                            break;
                        case 3:
                            flattenedContact.OtherAddressCountry = physicalAddress.Country;
                            flattenedContact.OtherAddressCity = physicalAddress.City;
                            flattenedContact.OtherAddressState = physicalAddress.State;
                            flattenedContact.OtherAddressStreet = physicalAddress.Street;
                            flattenedContact.OtherAddressZipcode = physicalAddress.Zipcode;
                            break;
                        default:
                            throw new Exception("Invalid Address Type");
                    }
                }

                foreach (EmailAddressDto emailAddress in contact.EmailAddresses)
                {
                    int typeId = emailAddress.TypeId;

                    switch (typeId)
                    {
                        case 1:
                            flattenedContact.PrimaryEmailAddress = emailAddress.Email;
                            break;
                        case 2:
                            flattenedContact.SecondaryEmailAddress = emailAddress.Email;
                            break;
                        case 3:
                            flattenedContact.WorkEmailAddress = emailAddress.Email;
                            break;
                        default:
                            throw new Exception("Invalid Email Type");
                    }
                }

                res.Add(flattenedContact);
            }

            return res;
        }
    }
}
