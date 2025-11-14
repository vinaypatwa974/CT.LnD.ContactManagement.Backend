using Azure.Storage.Blobs;
using CsvHelper;
using CsvHelper.Configuration;
using CT.LnD.ContactManagement.Backend.Business.Services.Interfaces;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Interfaces;
using CT.LnD.ContactManagement.Backend.Dto.Business;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace CT.LnD.ContactManagement.Backend.Business.Services.Implementations
{
    public class FileUploadService(IContactRepository contactRepository, IConfiguration configuration) : IFileUploadService
    {
        private readonly IContactRepository _contactRepository = contactRepository;
        public async Task ProcessCSV(IFormFile file, string userId)
        {
            const long MaxFileSizeInBytes = 10 * 1024 * 1024;

            if (file == null || file.Length == 0)
            {
                throw new Exception("File is missing or empty.");
            }

            if (Path.GetExtension(file.FileName) != ".csv")
            {
                throw new Exception("Only CSV Filed are accepted. ");
            }
            if (file.Length > MaxFileSizeInBytes)
            {
                throw new ArgumentException("CSV file size must be 10 MB or less.");
            }

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                MissingFieldFound = null
            };


            using StreamReader reader = new(file.OpenReadStream());
            using CsvReader csv = new(reader, config);


            _ = csv.Read();
            _ = csv.ReadHeader();

            string[] headers = csv.HeaderRecord ?? [];
            string[] requiredHeaders = ["First Name", "Last Name", "Primary Phone Country Code", "Primary Phone Number"];

            foreach (string? header in requiredHeaders)
            {
                if (!headers.Contains(header))
                {
                    throw new Exception($"Missing required header: {header}");
                }
            }


            _ = csv.Context.RegisterClassMap<CSVContactMapper>();

            List<CSVContactModel> contacts;
            try
            {
                contacts = [.. csv.GetRecords<CSVContactModel>()];
            }
            catch (FieldValidationException ex)
            {
                CsvContext context = ex.Context!;
                throw new Exception($"Missing {context.Reader!.HeaderRecord![context.Reader.CurrentIndex]} at row {context.Parser!.RawRow}");

            }
            catch (CsvHelperException ex)
            {
                throw new Exception($"{ex.Message}");
            }
            List<ContactRequest> contactRequests = CSVToContactRequestConvertor(contacts, userId);
            List<Task> tasks = [];
            foreach (ContactRequest contactRequest in contactRequests)
            {
                await _contactRepository.AddAsync(contactRequest);
                //tasks.Add(t);
            }

            //await Task.WhenAll(tasks);

        }


        /// <summary>
        /// Handles uploading and saving a profile image file to the server and updating the contact's photo path in the database.
        /// </summary>
        /// <param name="file">Image file uploaded by the client</param>
        /// <param name="contactId">Contact ID to update the profile image for</param>
        /// <returns>Contact response after updating the image path</returns>
        public async Task UploadImage(IFormFile file, string contactId)
        {

            if (file == null || file.Length == 0)
            {
                throw new Exception("File is missing");
            }

            string[] allowedExtensions = [".jpg", ".jpeg", ".png"];
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();


            if (!allowedExtensions.Contains(extension))
            {
                throw new Exception("Invalid file type. Only JPG , JPEG and PNG are allowed.");
            }


            if (!file.ContentType.StartsWith("image/"))
            {
                throw new Exception("Invalid content type. Must be an image.");
            }

            string tempRoot = Path.GetTempPath();
            string uploadsFolder = Path.Combine(tempRoot, "site", "temp", "exports");

            if (!Directory.Exists(uploadsFolder))
            {
                _ = Directory.CreateDirectory(uploadsFolder);
            }


            string fileName = $"{contactId}-avatar" + extension;
            string filePath = Path.Combine(uploadsFolder, fileName);


            using (FileStream stream = new(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }


            BlobServiceClient blobServiceClient = new(configuration.GetSection("AzureBlob")["Connection_String"]);


            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(configuration.GetSection("AzureBlob")["Container_Name"]);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);


            using FileStream uploadFileStream = File.OpenRead(filePath);
            _ = await blobClient.UploadAsync(uploadFileStream, true);
            uploadFileStream.Close();


            string blobUrl = blobClient.Uri.ToString();



            await _contactRepository.UpdateProfilePhoto(blobUrl, contactId);
        }

        /// <summary>
        /// Converts a list of CSVContactModel objects to ContactRequest object with structured data.
        /// </summary>
        /// <param name="contacts">List of parsed CSV contact records</param>
        /// <param name="userId">User ID to associate with each contact</param>
        /// <returns>List of ContactRequest objects</returns>
        private static List<ContactRequest> CSVToContactRequestConvertor(List<CSVContactModel> contacts, string userId)
        {
            List<ContactRequest> contactRequests = [];

            foreach (CSVContactModel contact in contacts)
            {
                ContactRequest contactRequest = new()
                {
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    UserId = Convert.ToInt32(userId),
                    PhoneNumber = [],
                    EmailAddress = [],
                    PhysicalAddress = []
                };


                if (contact.Notes != null)
                {
                    contactRequest.Notes = contact.Notes;
                }

                if (contact.Tags != null)
                {
                    contactRequest.Notes = contact.Notes;
                }

                if (contact.Company != null)
                {
                    contactRequest.Company = contact.Company;
                }

                if (contact.PrimaryContactNumber != null)
                {
                    contactRequest.PhoneNumber.Add(contact.PrimaryContactNumber);
                }

                if (contact.SecondaryContactNumber != null)
                {
                    contactRequest.PhoneNumber.Add(contact.SecondaryContactNumber);
                }

                if (contact.WorkPhoneNumber != null)
                {
                    contactRequest.PhoneNumber.Add(contact.WorkPhoneNumber);
                }

                if (contact.HomeAddress != null)
                {
                    contactRequest.PhysicalAddress.Add(contact.HomeAddress);
                }

                if (contact.WorkAddress != null)
                {
                    contactRequest.PhysicalAddress.Add(contact.WorkAddress);
                }

                if (contact.OtherAddress != null)
                {
                    contactRequest.PhysicalAddress.Add(contact.OtherAddress);
                }


                if (contact.PrimaryEmailAddress != null)
                {
                    contactRequest.EmailAddress.Add(contact.PrimaryEmailAddress);
                }

                if (contact.SecondaryEmailAddress != null)
                {
                    contactRequest.EmailAddress.Add(contact.SecondaryEmailAddress);
                }

                if (contact.WorkEmailAddress != null)
                {
                    contactRequest.EmailAddress.Add(contact.WorkEmailAddress);
                }

                contactRequests.Add(contactRequest);
            }

            return contactRequests;
        }

    }
}
