using System;
using System.Web;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Web.Services
{
    public static class HttpContextExtensions
    {
        private const string ModuleIdKey = "ModuleId";
        private const string TabIdKey = "TabId";

        public static int FindTabId(this HttpContextBase context)
        {
            return FindInt(context, TabIdKey);
        }

        public static int FindModuleId(this HttpContextBase context)
        {
            return FindInt(context, ModuleIdKey);
        }

        private static int FindInt(HttpContextBase context, string key)
        {
            string value = context.Request.Headers[key];

            if (String.IsNullOrEmpty(value))
            {
                value = context.Request.Params[key];
            }

            int id;
            if (Int32.TryParse(value, out id))
            {
                return id;
            }

            return Null.NullInteger;
        }
    }
}