using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Dto.Hosting
{
    public class PhysicalAddressDto
    {
        [Required]
        public int TypeId { get; set; }

        [MaxLength(20)]
        public string Zipcode { get; set; }

        [MaxLength(200)]
        public string Street { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        [MaxLength(100)]
        public string State { get; set; }
    }
}
