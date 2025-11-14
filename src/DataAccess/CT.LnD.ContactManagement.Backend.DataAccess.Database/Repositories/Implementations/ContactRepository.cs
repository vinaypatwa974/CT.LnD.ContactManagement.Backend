using CT.LnD.ContactManagement.Backend.DataAccess.Database.Configurations;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Constants;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Dtos;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Interfaces;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Repositories.Implementations
{
    public class ContactRepository(ContactManagementDbContext context) : IContactRepository
    {
        public async Task<List<Contact>> GetAllAsync(GetAllContactsRequest getAllContactsRequest)
        {
            string userId = getAllContactsRequest.Id;
            int filter = Convert.ToInt32(getAllContactsRequest.Filter);
            int pageNumber = Convert.ToInt32(getAllContactsRequest.PageNumber);

            int offSet = (filter * pageNumber) - filter;

            return await context.Contacts
                .FromSqlRaw(ContactSqlQueries.GetPaginatedContacts, userId, offSet, filter)
                .ToListAsync();
        }


        public async Task<ContactResponse?> GetByIdAsync(string contactId)
        {
            Contact contact = await context.Contacts
                .FromSqlRaw(ContactSqlQueries.GetContactById, contactId)
                .FirstOrDefaultAsync()
                ?? throw new Exception("No contact found with this ID!");

            List<EmailAddress> emailAddresses = await context.EmailAddresses
                .FromSqlRaw(ContactSqlQueries.GetEmailsByContactId, contactId)
                .ToListAsync();

            List<PhoneNumber> phoneNumbers = await context.PhoneNumbers
                .FromSqlRaw(ContactSqlQueries.GetPhonesByContactId, contactId)
                .ToListAsync();

            List<PhysicalAddress> physicalAddresses = await context.PhysicalAddresses
                .FromSqlRaw(ContactSqlQueries.GetAddressesByContactId, contactId)
                .ToListAsync();

            List<EmailAddressDto> emailDtos = [.. emailAddresses
                .Select(e => new EmailAddressDto
                {
                    TypeId = e.TypeId,
                    Email = e.Email
                })];

            List<PhoneNumberDto> phoneDtos = [.. phoneNumbers
                .Select(p => new PhoneNumberDto
                {
                    TypeId = p.TypeId,
                    CountryCode = p.CountryCode,
                    Number = p.Number
                })];

            List<PhysicalAddressDto> addressDtos = [.. physicalAddresses
                .Select(a => new PhysicalAddressDto
                {
                    TypeId = a.TypeId,
                    Zipcode = a.Zipcode,
                    Street = a.Street,
                    City = a.City,
                    Country = a.Country,
                    State = a.State
                })];

            return new ContactResponse
            {
                Contact = contact,
                EmailAddresses = emailDtos,
                PhoneNumbers = phoneDtos,
                PhysicalAddresses = addressDtos
            };
        }




        public async Task AddAsync(ContactRequest contactRequest)
        {
            int contactId;
            List<PhoneNumberDto> phoneNumbers = contactRequest.PhoneNumber!;
            List<EmailAddressDto> emailAddresses = contactRequest.EmailAddress!;
            List<PhysicalAddressDto> physicalAddresses = contactRequest.PhysicalAddress!;

            IExecutionStrategy strategy = context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync();
                try
                {
                    using (System.Data.Common.DbCommand command = context.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = ContactSqlQueries.InsertContact;

                        _ = command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@FirstName", contactRequest.FirstName));
                        _ = command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@LastName", contactRequest.LastName));
                        _ = command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Company", (object?)contactRequest.Company ?? DBNull.Value));
                        _ = command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Notes", (object?)contactRequest.Notes ?? DBNull.Value));
                        _ = command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Tags", (object?)contactRequest.Tags ?? DBNull.Value));
                        _ = command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@UserId", contactRequest.UserId));

                        if (command.Connection.State != System.Data.ConnectionState.Open)
                        {
                            await command.Connection.OpenAsync();
                        }

                        command.Transaction = transaction.GetDbTransaction();

                        object result = await command.ExecuteScalarAsync();
                        contactId = Convert.ToInt32(result);
                    }

                    foreach (PhoneNumberDto phoneNumber in phoneNumbers!)
                    {
                        PhoneNumber res = await context.PhoneNumbers.FromSqlRaw(ContactSqlQueries.GetPhoneNumberByNumber, phoneNumber.Number).FirstOrDefaultAsync();

                        if (res == null)
                        {
                            _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.InsertPhoneNumber, contactId, phoneNumber.CountryCode!, phoneNumber.Number!, phoneNumber.TypeId);
                        }
                        else
                        {
                            Contact contact = await context.Contacts.FromSqlRaw(ContactSqlQueries.GetContactById, res.ContactId).FirstOrDefaultAsync() ?? new();

                            if (contact.UserId != contactRequest.UserId)
                            {
                                _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.InsertPhoneNumber, contactId, phoneNumber.CountryCode!, phoneNumber.Number!, phoneNumber.TypeId);
                            }
                            else
                            {
                                transaction.Rollback();
                                throw new Exception("Contact with this phone number already exists .");
                            }
                        }
                    }

                    foreach (PhysicalAddressDto physicalAddress in physicalAddresses!)
                    {
                        _ = await context.Database.ExecuteSqlRawAsync(
                            ContactSqlQueries.InsertAddress,
                            contactId, physicalAddress.TypeId, physicalAddress.Street, physicalAddress.Zipcode, physicalAddress.City, physicalAddress.Country, physicalAddress.State);
                    }

                    foreach (EmailAddressDto emailAddress in emailAddresses!)
                    {
                        _ = await context.Database.ExecuteSqlRawAsync(
                            ContactSqlQueries.InsertEmail,
                            contactId, emailAddress.TypeId, emailAddress.Email!);
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception e)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"{e.Message}");
                    throw new Exception("error while creating the contact ", e);
                }
            });
        }
        public async Task UpdateBasicContactDetails(BasicContactDetailsUpdateRequest req, string contactId)
        {
            _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.UpdateBasicContactDetails, req.FirstName, req.LastName, req.Company, req.Notes, req.Tags, contactId);
        }

        public async Task DeleteByIdAsync(string id)
        {

            _ = await context.Contacts.FromSqlRaw(ContactSqlQueries.GetContactById, id).FirstOrDefaultAsync() ?? throw new Exception("No contact found with this Contact ID ..");
            _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.MarkDeletedContact, id);

        }


        public async Task UpdateProfilePhoto(string filePath, string contactId)
        {
            _ = await context.Contacts.FromSqlRaw(ContactSqlQueries.GetContactById, contactId).FirstOrDefaultAsync() ?? throw new Exception("No contact found with this Contact ID ..");
            _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.UpdateAvatar, filePath, contactId);

        }



        public async Task<List<int>> GetAllContactIDs(string userId)
        {
            List<Contact> contacts = await context.Contacts.FromSqlRaw(ContactSqlQueries.GetAllContactsByUserId, userId).ToListAsync();

            List<int> res = [];

            foreach (Contact contact in contacts)
            {
                res.Add(contact.Id);
            }

            return res;
        }


        public async Task UpdateEmail(EmailAddressDto emailAddress, string contactId)
        {
            _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.UpsertEmail, emailAddress.Email!, contactId, emailAddress.TypeId);
        }

        public async Task UpdatePhone(PhoneNumberDto phoneNumber, string contactId)
        {
            _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.UpsertPhone, phoneNumber.CountryCode!, phoneNumber.Number!, contactId, phoneNumber.TypeId);

        }

        public async Task UpdateAddress(PhysicalAddressDto physicalAddress, string contactId)
        {
            _ = await context.Database.ExecuteSqlRawAsync(ContactSqlQueries.UpsertAddress, physicalAddress.Street, physicalAddress.Zipcode, physicalAddress.City, physicalAddress.Country, physicalAddress.State, contactId, physicalAddress.TypeId);

        }


        public async Task<List<Contact>> SearchUsingTag(string tag, string userId)
        {
            return await context.Contacts.FromSqlRaw(ContactSqlQueries.SearchUsingTag, tag, userId).ToListAsync();

        }


        public async Task<List<Contact>> SearchUsingName(string name, string userId)
        {
            return await context.Contacts.FromSqlRaw(ContactSqlQueries.SearchUsingName, name, userId).ToListAsync();
        }


        public async Task DeleteEmailAddress(string typeId, string contactId)
        {
            _ = await context.Database.ExecuteSqlRawAsync("DELETE FROM EmailAddresses WHERE TypeId = {0} AND ContactId = {1}", typeId, contactId);
        }
        public async Task DeletePhoneNumber(string typeId, string contactId)
        {
            _ = await context.Database.ExecuteSqlRawAsync("DELETE FROM PhoneNumbers WHERE TypeId = {0} AND ContactId = {1}", typeId, contactId);


        }
        public async Task DeletePhysicalAddress(string typeId, string contactId)
        {

            _ = await context.Database.ExecuteSqlRawAsync("DELETE FROM PhysicalAddresses WHERE TypeId = {0} AND ContactId = {1}", typeId, contactId);

        }
        public async Task<List<EmailType>> FindAllEmailTypes()
        {
            return await context.EmailType.FromSqlRaw("SELECT * FROM EmailType").ToListAsync();
        }
        public async Task<List<AddressType>> FindAllAddressTypes()
        {
            return await context.AddressType.FromSqlRaw("SELECT * FROM AddressType").ToListAsync();

        }
        public async Task<List<PhoneType>> FindAllPhoneTyps()
        {
            return await context.PhoneType.FromSqlRaw("SELECT * FROM PhoneType").ToListAsync();

        }
    }
}