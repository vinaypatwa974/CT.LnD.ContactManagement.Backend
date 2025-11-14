using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public int UserId { get; set; }

        public string? Company { get; set; }

        public string? Notes { get; set; }

        public string? Tags { get; set; }

        public string? Avatar { get; set; }

        public User? User { get; set; }
    }
}