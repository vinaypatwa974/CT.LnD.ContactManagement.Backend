using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class PatchSuccessExample : ISuccessHttpResponse, IExamplesProvider<ISuccessHttpResponse>
    {
        public int StatusCode => 200;
        public string Message => "Request Successfull";

        public object[] Data => [];
        public ISuccessHttpResponse GetExamples()
        {
            return new PatchSuccessExample();
        }
    }
}
