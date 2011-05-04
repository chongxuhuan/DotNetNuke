#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.Reflection;
using System.Web.Compilation;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Framework.Providers;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;

#endregion

namespace DotNetNuke.Framework
{
    /// -----------------------------------------------------------------------------
    /// Namespace: DotNetNuke.Framework
    /// Project	 : DotNetNuke
    /// Class	 : Reflection
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// Library responsible for reflection
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[Nik Kalyani]	10/15/2004	Replaced brackets in parameter names
    /// 	[cnurse]	    10/13/2005	Documented
    /// </history>
    /// -----------------------------------------------------------------------------
    public class Reflection
    {
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Creates an object
        /// </summary>
        /// <param name="ObjectProviderType">The type of Object to create (data/navigation)</param>
        /// <returns>The created Object</returns>
        /// <remarks>Overload for creating an object from a Provider configured in web.config</remarks>
        /// <history>
        /// 	[cnurse]	    10/13/2005	Documented
        /// </history>
        /// -----------------------------------------------------------------------------
        public static object CreateObject(string ObjectProviderType)
        {
            return CreateObject(ObjectProviderType, true);
        }

        public static object CreateObject(string ObjectProviderType, bool UseCache)
        {
            return CreateObject(ObjectProviderType, "", "", "", UseCache);
        }

        public static object CreateObject(string ObjectProviderType, string ObjectNamespace, string ObjectAssemblyName)
        {
            return CreateObject(ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, true);
        }

        public static object CreateObject(string ObjectProviderType, string ObjectNamespace, string ObjectAssemblyName, bool UseCache)
        {
            return CreateObject(ObjectProviderType, "", ObjectNamespace, ObjectAssemblyName, UseCache);
        }

        public static object CreateObject(string ObjectProviderType, string ObjectProviderName, string ObjectNamespace, string ObjectAssemblyName)
        {
            return CreateObject(ObjectProviderType, ObjectProviderName, ObjectNamespace, ObjectAssemblyName, true);
        }

        public static object CreateObject(string ObjectProviderType, string ObjectProviderName, string ObjectNamespace, string ObjectAssemblyName, bool UseCache)
        {
            string TypeName = "";
            ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ObjectProviderType);
            if (!String.IsNullOrEmpty(ObjectNamespace) && !String.IsNullOrEmpty(ObjectAssemblyName))
            {
                if (String.IsNullOrEmpty(ObjectProviderName))
                {
                    TypeName = ObjectNamespace + "." + objProviderConfiguration.DefaultProvider + ", " + ObjectAssemblyName + "." + objProviderConfiguration.DefaultProvider;
                }
                else
                {
                    TypeName = ObjectNamespace + "." + ObjectProviderName + ", " + ObjectAssemblyName + "." + ObjectProviderName;
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(ObjectNamespace))
                {
                    if (String.IsNullOrEmpty(ObjectProviderName))
                    {
                        TypeName = ObjectNamespace + "." + objProviderConfiguration.DefaultProvider;
                    }
                    else
                    {
                        TypeName = ObjectNamespace + "." + ObjectProviderName;
                    }
                }
                else
                {
                    if (String.IsNullOrEmpty(ObjectProviderName))
                    {
                        TypeName = ((Provider) objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider]).Type;
                    }
                    else
                    {
                        TypeName = ((Provider) objProviderConfiguration.Providers[ObjectProviderName]).Type;
                    }
                }
            }
            return CreateObject(TypeName, TypeName, UseCache);
        }

        public static object CreateObject(string TypeName, string CacheKey)
        {
            return CreateObject(TypeName, CacheKey, true);
        }

        public static object CreateObject(string TypeName, string CacheKey, bool UseCache)
        {
            return Activator.CreateInstance(CreateType(TypeName, CacheKey, UseCache));
        }

        public static T CreateObject<T>()
        {
            return Activator.CreateInstance<T>();
        }

        public static Type CreateType(string TypeName)
        {
            return CreateType(TypeName, "", true, false);
        }

        public static Type CreateType(string TypeName, bool IgnoreErrors)
        {
            return CreateType(TypeName, "", true, IgnoreErrors);
        }

        public static Type CreateType(string TypeName, string CacheKey, bool UseCache)
        {
            return CreateType(TypeName, CacheKey, UseCache, false);
        }

        public static Type CreateType(string TypeName, string CacheKey, bool UseCache, bool IgnoreErrors)
        {
            if (String.IsNullOrEmpty(CacheKey))
            {
                CacheKey = TypeName;
            }
            Type objType = null;
            if (UseCache)
            {
                objType = (Type) DataCache.GetCache(CacheKey);
            }
            if (objType == null)
            {
                try
                {
                    objType = BuildManager.GetType(TypeName, true, true);
                    if (UseCache)
                    {
                        DataCache.SetCache(CacheKey, objType);
                    }
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                    if (!IgnoreErrors)
                    {
                        Exceptions.LogException(exc);
                    }
                }
            }
            return objType;
        }

        public static object CreateInstance(Type Type)
        {
            if (Type != null)
            {
                return Type.InvokeMember("", BindingFlags.CreateInstance, null, null, null, null);
            }
            else
            {
                return null;
            }
        }

        public static object GetProperty(Type Type, string PropertyName, object Target)
        {
            if (Type != null)
            {
                return Type.InvokeMember(PropertyName, BindingFlags.GetProperty, null, Target, null);
            }
            else
            {
                return null;
            }
        }

        public static void SetProperty(Type Type, string PropertyName, object Target, object[] Args)
        {
            if (Type != null)
            {
                Type.InvokeMember(PropertyName, BindingFlags.SetProperty, null, Target, Args);
            }
        }

        public static void InvokeMethod(Type Type, string PropertyName, object Target, object[] Args)
        {
            if (Type != null)
            {
                Type.InvokeMember(PropertyName, BindingFlags.InvokeMethod, null, Target, Args);
            }
        }

        [Obsolete("This method has been deprecated. Please use CreateObject(ByVal ObjectProviderType As String, ByVal UseCache As Boolean) As Object")]
        internal static object CreateObjectNotCached(string ObjectProviderType)
        {
            string TypeName = "";
            Type objType = null;
            ProviderConfiguration objProviderConfiguration = ProviderConfiguration.GetProviderConfiguration(ObjectProviderType);
            TypeName = ((Provider) objProviderConfiguration.Providers[objProviderConfiguration.DefaultProvider]).Type;
            try
            {
                objType = BuildManager.GetType(TypeName, true, true);
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            return Activator.CreateInstance(objType);
        }
    }
}
