using System.Collections.Generic;

namespace DotNetNuke.Services.ClientCapability.Hanldlers
{
    internal interface IHandlersFactory<IHandler>
    {
        IList<IHandler> Create();
    }
}