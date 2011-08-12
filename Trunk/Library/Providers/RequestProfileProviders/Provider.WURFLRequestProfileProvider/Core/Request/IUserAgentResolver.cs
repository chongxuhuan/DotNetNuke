using System.Web;

namespace DotNetNuke.Services.Devices.Core.Request
{
    public interface IUserAgentResolver
    {
        string resolve(HttpRequest request);
    }
}