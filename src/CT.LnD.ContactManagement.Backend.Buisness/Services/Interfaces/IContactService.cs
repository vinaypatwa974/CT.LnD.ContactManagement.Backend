using CT.LnD.ContactManagement.Backend.DataAccess.Database.Dtos;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;

namespace CT.LnD.ContactManagement.Backend.Business.Services.Interfaces
{
    public interface IContactService
    {
        Task<List<Contact>> GetAllAsync(GetAllContactsRequest getAllContactsRequest);
        Task<ContactResponse?> GetByIdAsync(string id);
        Task AddAsync(ContactRequest contactRequest);
        Task DeleteByIdAsync(string id);


        Task ExportContacts(string userID);

        Task UpdatePhone(PhoneNumberDto phoneNumber, string contactId);

        Task UpdateEmail(EmailAddressDto emailAddress, string contactId);

        Task UpdateAddress(PhysicalAddressDto physicalAddress, string contactId);

        Task UpdateBasicContactDetails(BasicContactDetailsUpdateRequest req, string contactId);

        Task<List<Contact>> SearchUsingTag(string tag, string userId);

        Task<List<Contact>> SearchUsingName(string name, string userId);

        Task DeleteEmailAddress(string typeId, string contactId);


        Task DeletePhoneNumber(string typeId, string contactId);


        Task DeletePhysicalAddress(string typeId, string contactId);


        Task<List<EmailType>> FindAllEmailTypes();
        Task<List<AddressType>> FindAllAddressTypes();
        Task<List<PhoneType>> FindAllPhoneTyps();



    }
}