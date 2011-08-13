using System.Web;

namespace DotNetNuke.Services.ClientCapability.Request
{
    public interface IUserAgentResolver
    {
        string resolve(HttpRequest request);
    }
}