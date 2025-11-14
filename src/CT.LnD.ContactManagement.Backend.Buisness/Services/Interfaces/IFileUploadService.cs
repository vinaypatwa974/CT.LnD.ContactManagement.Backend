using Microsoft.AspNetCore.Http;

namespace CT.LnD.ContactManagement.Backend.Business.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task UploadImage(IFormFile file, string contactId);

        Task ProcessCSV(IFormFile file, string userId);
    }
}