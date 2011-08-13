using System.Collections.Generic;

namespace DotNetNuke.Services.ClientCapability
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IChainFactory<T>
    {
        /// <summary>
        /// Creates a chain of the given class T
        /// </summary>
        /// <returns></returns>
        ICollection<T> Create();
    }
}