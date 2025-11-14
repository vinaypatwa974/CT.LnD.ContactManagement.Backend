using Azure.Storage.Blobs;
using CsvHelper;
using CT.LnD.ContactManagement.Backend.Business.Mappers;
using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;
using CT.LnD.ContactManagement.Backend.Business.Utility.Interfaces;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Dtos;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Interfaces;
using CT.LnD.ContactManagement.Backend.Dto.Business;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Microsoft.Extensions.Configuration;
using System.Globalization;
namespace CT.LnD.ContactManagement.Backend.Business.Services.Implementations
{
    public class ContactService(IContactRepository contactRepository, IConfiguration configuration, IUserRepository userRepository, IEmailService emailService) : IContactService
    {
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IUserRepository _userRepository = userRepository;
        public async Task<List<Contact>> GetAllAsync(GetAllContactsRequest getAllContactsRequest)
        {
            return await _contactRepository.GetAllAsync(getAllContactsRequest);

        }
        public async Task<ContactResponse?> GetByIdAsync(string userId)
        {
            return await _contactRepository.GetByIdAsync(userId);
        }
        public async Task AddAsync(ContactRequest contactRequest)
        {
            await _contactRepository.AddAsync(contactRequest);
        }
        public async Task UpdateBasicContactDetails(BasicContactDetailsUpdateRequest req, string contactId)
        {
            await _contactRepository.UpdateBasicContactDetails(req, contactId);
        }
        public async Task DeleteByIdAsync(string id)
        {
            await _contactRepository.DeleteByIdAsync(id);
        }

        public async Task ExportContacts(string userID)
        {

            List<int> contactIds = await _contactRepository.GetAllContactIDs(userID);

            List<ContactResponse> res = [];

            foreach (int contactId in contactIds)
            {
                ContactResponse contactResponse = await _contactRepository.GetByIdAsync(Convert.ToString(contactId));
                res.Add(contactResponse!);
            }

            List<ContactToCSVModel> flattenedContacts = ContactToFlatContactMapper.Map(res);
            string tempRoot = Path.GetTempPath();
            string uploadsFolder = Path.Combine(tempRoot, "site", "temp", "exports");
            //string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "exports");

            bool directoryExists = Directory.Exists(uploadsFolder);
            if (!directoryExists)
            {
                _ = Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid() + ".csv";
            string filePath = Path.Combine(uploadsFolder, fileName);

            using StreamWriter writer = new(filePath);

            using (CsvWriter csv = new(writer, CultureInfo.InvariantCulture))
            {
                _ = csv.Context.RegisterClassMap<FlatContactToCSVMapper>();

                csv.WriteHeader<ContactToCSVModel>();
                csv.NextRecord();
                csv.WriteRecords(flattenedContacts);
            }


            BlobServiceClient blobServiceClient = new(configuration.GetSection("AzureBlob")["Connection_String"]);


            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(configuration.GetSection("AzureBlob")["Exports_Container_Name"]);


            BlobClient blobClient = containerClient.GetBlobClient(fileName);


            using FileStream uploadFileStream = File.OpenRead(filePath);

            _ = await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();


            string blobUrl = blobClient.Uri.ToString();

            GetUserResponse userDetails = await _userRepository.GetByIdAsync(userID);

            emailService.SendExportedContacts(Convert.ToString(userDetails.Email), blobUrl);

        }

        public async Task UpdatePhone(PhoneNumberDto phoneNumber, string contactId)
        {
            await _contactRepository.UpdatePhone(phoneNumber, contactId);
        }

        public async Task UpdateEmail(EmailAddressDto emailAddress, string contactId)
        {
            await _contactRepository.UpdateEmail(emailAddress, contactId);
        }

        public async Task UpdateAddress(PhysicalAddressDto physicalAddress, string contactId)
        {
            await _contactRepository.UpdateAddress(physicalAddress, contactId);
        }


        public async Task<List<Contact>> SearchUsingTag(string tag, string userId)
        {
            return await _contactRepository.SearchUsingTag(tag, userId);
        }

        public async Task<List<Contact>> SearchUsingName(string name, string userId)
        {
            return await _contactRepository.SearchUsingName(name, userId);
        }


        public async Task DeleteEmailAddress(string typeId, string contactId)
        {
            await _contactRepository.DeleteEmailAddress(typeId, contactId);

        }
        public async Task DeletePhoneNumber(string typeId, string contactId)
        {

            await _contactRepository.DeletePhoneNumber(typeId, contactId);
        }
        public async Task DeletePhysicalAddress(string typeId, string contactId)
        {

            await _contactRepository.DeletePhysicalAddress(typeId, contactId);
        }
        public async Task<List<EmailType>> FindAllEmailTypes()
        {
            return await _contactRepository.FindAllEmailTypes();
        }
        public async Task<List<AddressType>> FindAllAddressTypes()
        {
            return await _contactRepository.FindAllAddressTypes();
        }
        public async Task<List<PhoneType>> FindAllPhoneTyps()
        {
            return await _contactRepository.FindAllPhoneTyps();
        }


    }
}