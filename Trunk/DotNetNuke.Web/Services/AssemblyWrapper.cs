using System;
using System.Reflection;
using DotNetNuke.Web.Services.Internal;

namespace DotNetNuke.Web.Services
{
    internal class AssemblyWrapper : IAssembly
    {
        private readonly Assembly _assembly;

        public AssemblyWrapper(Assembly assembly)
        {
            _assembly = assembly;
        }

        public Type[] GetTypes()
        {
            return _assembly.GetTypes();
        }
    }
}