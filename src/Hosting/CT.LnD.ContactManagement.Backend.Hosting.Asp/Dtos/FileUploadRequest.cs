using System.ComponentModel.DataAnnotations;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Dtos
{
    public class FileUploadRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
