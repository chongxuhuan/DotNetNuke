using System.Collections.Generic;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDevice
    {
        string ID { get; }
        string UserAgent { get; }
        string FallBack { get; }
        bool IsActualDeviceRoot { get; }

        /// <summary>
        /// Gets the capabilities.
        /// </summary>
        /// <value>The capabilities.</value>
        IDictionary<string, string> Capabilities { get; }

        /// <summary>
        /// Gets the mark up.
        /// </summary>
        /// <value>The mark up.</value>
        MarkUp MarkUp { get; }

        /// <summary>
        /// Gets the capability.
        /// </summary>
        /// <param name="capabilityName">Name of the capability.</param>
        /// <returns></returns>
        string GetCapability(string capabilityName);
    }


    public enum MarkUp
    {
        XHTML_ADVANCED,
        XHTML_SIMPLE,
        CHTML,
        WML
    }
}