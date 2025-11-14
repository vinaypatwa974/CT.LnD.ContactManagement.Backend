using CT.LnD.ContactManagement.Backend.DataAccess.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Configurations
{
    public class ContactManagementDbContext(DbContextOptions<ContactManagementDbContext> options) : DbContext(options)
    {
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<EmailAddress> EmailAddresses { get; set; }
        public DbSet<PhysicalAddress> PhysicalAddresses { get; set; }
        public DbSet<PhoneNumber> PhoneNumbers { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Status> Statuses { get; set; }

        public DbSet<DeletedContact> DeletedContacts { get; set; }

        public DbSet<EmailType> EmailType { get; set; }

        public DbSet<PhoneType> PhoneType { get; set; }


        public DbSet<AddressType> AddressType { get; set; }


    }
}
