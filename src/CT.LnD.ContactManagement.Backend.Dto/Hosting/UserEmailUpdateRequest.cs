using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Dto.Hosting
{
    public class UserEmailUpdateRequest
    {
        [Required]

        public string Email { get; set; }

    }
}
