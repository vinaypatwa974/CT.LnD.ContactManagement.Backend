using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Dto.Hosting
{
    public class BasicContactDetailsUpdateRequest
    {

        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; } = null!;

        [MaxLength(200)]
        public string? Company { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(200)]
        public string? Tags { get; set; }
    }
}
