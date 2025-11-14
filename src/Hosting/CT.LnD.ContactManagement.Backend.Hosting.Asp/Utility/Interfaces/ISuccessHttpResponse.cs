namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces
{
    public interface ISuccessHttpResponse
    {
        int StatusCode { get; }
        string Message { get; }

        object[] Data { get; }
    }
}
