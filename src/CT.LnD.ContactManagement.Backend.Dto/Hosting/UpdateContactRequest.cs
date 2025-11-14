using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Dto.Hosting
{
    public class UpdateContactRequest
    {

        [Required]

        public string Id { get; set; }


        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        [MaxLength(200)]
        public string? Company { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(200)]
        public string? Tags { get; set; }

        public List<PhoneNumberDto>? PhoneNumber { get; set; }

        public List<EmailAddressDto>? EmailAddress { get; set; }

        public List<PhysicalAddressDto>? PhysicalAddress { get; set; }
    }
}
