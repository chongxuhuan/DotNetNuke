using System;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Internal;

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

        public static ModuleInfo FindModuleInfo(this HttpContextBase context)
        {
            var tabId = context.FindTabId();
            var moduleId = context.FindModuleId();

            if (moduleId != Null.NullInteger && tabId != Null.NullInteger)
            {
                return TestableModuleController.Instance.GetModule(moduleId, tabId);
            }

            return null;  
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