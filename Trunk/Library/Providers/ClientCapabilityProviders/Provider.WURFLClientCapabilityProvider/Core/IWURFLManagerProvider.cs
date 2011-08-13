namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="IWURFLManager">The type of the WURFL manager.</typeparam>
    public interface IWURFLManagerProvider<IWURFLManager>
    {
        /// <summary>
        /// Gets the WURFL manager.
        /// </summary>
        /// <value>The WURFL manager.</value>
        IWURFLManager WURFLManager { get; }
    }
}