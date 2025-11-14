using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class ForbiddenExample : IHttpResponse, IExamplesProvider<IHttpResponse>
    {


        public int StatusCode => 403;
        public string Status => "Forbidden";

        public string Message => " You dont have enough permission to access this resource ";
        public DateTime TimeStamp => DateTime.Now;

        public IHttpResponse GetExamples()
        {
            return new ForbiddenExample();
        }
    }
}
