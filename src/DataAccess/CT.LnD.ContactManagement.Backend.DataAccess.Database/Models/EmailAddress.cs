using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Models
{
    public class EmailAddress
    {
        [Key]
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int ContactId { get; set; }
        public string Email { get; set; }

        public Contact Contact { get; set; }
        public EmailType Type { get; set; }
    }
}