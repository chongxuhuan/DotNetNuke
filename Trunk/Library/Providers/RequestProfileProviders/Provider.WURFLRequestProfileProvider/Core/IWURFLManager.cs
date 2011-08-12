using System.Web;
using DotNetNuke.Services.Devices.Core.Request;

namespace DotNetNuke.Services.Devices.Core
{
    /// <summary>
    /// 
    /// </summary>
    public interface IWURFLManager
    {
        /// <summary>
        /// Gets the device for request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        IDevice GetDeviceForRequest(IWURFLRequest request);

        /// <summary>
        /// Gets the device for request.
        /// </summary>
        /// <param name="reqeuest">The reqeuest.</param>
        /// <returns></returns>
        IDevice GetDeviceForRequest(HttpRequest reqeuest);

        /// <summary>
        /// Gets the device for request.
        /// </summary>
        /// <param name="userAgent">The user agent.</param>
        /// <returns></returns>
        IDevice GetDeviceForRequest(string userAgent);

        /// <summary>
        /// Get a generic device
        /// </summary>
        /// <returns></returns>
        IDevice GetGenericDevice();
    }
}