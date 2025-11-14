using CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.Interfaces;
using Swashbuckle.AspNetCore.Filters;

namespace CT.LnD.ContactManagement.Backend.Hosting.Asp.Utility.ResponseExamples
{
    public class SuccessExample : ISuccessHttpResponse, IExamplesProvider<ISuccessHttpResponse>
    {
        public int StatusCode => 204;
        public string Message => "Patch Request Successfull";

        public object[] Data => [];
        public ISuccessHttpResponse GetExamples()
        {
            return new PatchSuccessExample();
        }
    }
}
