using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class AccessDeniedExample : IHttpResponse, IExamplesProvider<IHttpResponse>
    {


        public int StatusCode => 401;
        public string Status => "Access Denied";

        public string Message => "Authentication failed  , please provide valid credentials or refresh token";

        public DateTime TimeStamp => DateTime.Now;

        public IHttpResponse GetExamples()
        {
            return new AccessDeniedExample();
        }
    }
}
