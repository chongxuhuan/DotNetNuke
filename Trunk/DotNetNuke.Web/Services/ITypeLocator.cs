using System;
using System.Collections.Generic;

namespace DotNetNuke.Web.Services
{
    internal interface ITypeLocator
    {
        IEnumerable<Type> GetAllMatchingTypes(Predicate<Type> predicate);
    }
}