using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Models
{
    public class PhysicalAddress
    {
        [Key]
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int ContactId { get; set; }

        public string Zipcode { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string State { get; set; }



        public Contact Contact { get; set; }
        public AddressType Type { get; set; }
    }
}