using System.Web;

namespace DotNetNuke.HttpModules.Services.Internal
{
    public interface IServicesContext
    {
        bool DoA401 { get; set; }
        bool SupportBasicAuth { get; }
        bool SupportDigestAuth { get; }
        bool IsStale { get; set; }
        HttpContextBase BaseContext { get; }
    }
}