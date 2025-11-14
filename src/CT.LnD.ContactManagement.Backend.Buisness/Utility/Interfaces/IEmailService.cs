namespace CT.LnD.ContactManagement.Backend.Business.Utility.Interfaces
{
    public interface IEmailService
    {
        void SendExportedContacts(string to, string blobUrl);


    }
}
