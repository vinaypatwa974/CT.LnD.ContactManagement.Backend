using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Models
{
    public class PhoneNumber
    {
        [Key]
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int ContactId { get; set; }
        public string CountryCode { get; set; }
        public string Number { get; set; }

        public Contact Contact { get; set; }
        public PhoneType Type { get; set; }
    }
}