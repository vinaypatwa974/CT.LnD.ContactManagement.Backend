using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Dto.Hosting
{
    public class PhoneNumberDto
    {
        [Required]
        public int TypeId { get; set; }

        [MaxLength(10)]
        public string? CountryCode { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Phone number must be between 6 and 15 digits")]
        public string? Number { get; set; }
    }
}
