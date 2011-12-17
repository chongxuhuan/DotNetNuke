using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotNetNuke.Web.Services.Internal;

namespace DotNetNuke.Web.Services
{
    internal class TypeLocator : ITypeLocator, IAssemblyLocator
    {
        private IAssemblyLocator _assemblyLocator;

        internal IAssemblyLocator AssemblyLocator
        {
            get { return _assemblyLocator ?? (_assemblyLocator = this); }
            set { _assemblyLocator = value; }
        }

        public IEnumerable<Type> GetAllMatchingTypes(Predicate<Type> predicate)
        {
            foreach (var assembly in AssemblyLocator.Assemblies)
            {
                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    //some assemblies don't want to be reflected but they still 
                    //expose types in the exception
                    types = ex.Types ?? new Type[0];
                }

                foreach (var type in types)
                {
                    if(type != null)
                    {
                        if(predicate(type))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        IEnumerable<IAssembly> IAssemblyLocator.Assemblies
        {
            //this method is not readily testable as the assemblies in the current app domain
            //will vary depending on the test runner and test configuration
            get
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    yield return new AssemblyWrapper(assembly);
                }
            }
        }
    }
}