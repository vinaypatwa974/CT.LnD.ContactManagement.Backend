using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class BadRequestExample : IHttpResponse, IExamplesProvider<IHttpResponse>
    {
        public int StatusCode => 400;
        public string Status => "Bad Request";

        public string Message => "Request Validation Failed. Please check request parameters and try again";
        public DateTime TimeStamp => DateTime.Now;

        public IHttpResponse GetExamples()
        {

            return new BadRequestExample();
        }
    }
}
