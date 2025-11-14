using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class NotFoundExample : IHttpResponse, IExamplesProvider<IHttpResponse>
    {
        public int StatusCode => 404;
        public string Status => "Not Found";

        public string Message => "Required resource not found";


        public DateTime TimeStamp => DateTime.Now;

        public IHttpResponse GetExamples()
        {
            return new NotFoundExample();
        }
    }
}
