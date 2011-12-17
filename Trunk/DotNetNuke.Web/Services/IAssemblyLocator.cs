using System.Collections.Generic;

namespace DotNetNuke.Web.Services.Internal
{
    public interface IAssemblyLocator
    {
        IEnumerable<IAssembly> Assemblies { get; }
    }
}