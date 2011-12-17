using System;

namespace DotNetNuke.Web.Services.Internal
{
    //interface to allowing mocking of System.Reflection.Assembly
    public interface IAssembly
    {
        Type[] GetTypes();
    }
}