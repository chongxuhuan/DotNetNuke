using System.Collections.Generic;

namespace DotNetNuke.Services.Devices.Core.Hanldlers
{
    internal interface IHandlersFactory<IHandler>
    {
        IList<IHandler> Create();
    }
}