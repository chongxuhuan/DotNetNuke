namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMarkUpResolver
    {
        /// <summary>
        /// Returns the markup associated with the device
        /// </summary>
        /// <param name="device">The device.</param>
        /// <returns></returns>
        MarkUp ResolveMarkUp(IDevice device);
    }
}