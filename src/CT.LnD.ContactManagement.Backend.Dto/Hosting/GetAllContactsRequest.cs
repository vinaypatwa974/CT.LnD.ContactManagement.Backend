using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Dto.Hosting
{
    public class GetAllContactsRequest
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Filter { get; set; }

        [Required]
        public string PageNumber { get; set; }
    }
}
