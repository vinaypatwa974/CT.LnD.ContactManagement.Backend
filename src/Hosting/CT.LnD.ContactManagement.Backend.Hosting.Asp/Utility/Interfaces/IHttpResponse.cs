namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces
{
    public interface IHttpResponse
    {
        int StatusCode { get; }
        string Status { get; }

        string Message { get; }

        DateTime TimeStamp { get; }

    }
}
