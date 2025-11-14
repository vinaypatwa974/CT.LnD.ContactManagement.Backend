using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class InternalServerErrorResponse : IHttpResponse, IExamplesProvider<IHttpResponse>
    {

        public int StatusCode => 500;
        public string Status => "Internal Server Error";

        public DateTime TimeStamp => DateTime.Now;

        public string Message => "An error occured while performing the request";

        public IHttpResponse GetExamples()
        {
            return new InternalServerErrorResponse();
        }
    }
}
