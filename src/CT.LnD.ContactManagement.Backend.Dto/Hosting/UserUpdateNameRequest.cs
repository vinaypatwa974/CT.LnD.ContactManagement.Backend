using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Dto.Hosting
{
    public class UserUpdateNameRequest
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

    }
}
