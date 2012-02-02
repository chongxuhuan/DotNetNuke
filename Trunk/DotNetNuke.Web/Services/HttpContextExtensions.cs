using System;
using System.Web;
using DotNetNuke.Common.Utilities;

namespace DotNetNuke.Web.Services
{
    public static class HttpContextExtensions
    {
        private const string TabIdKey = "TabId";

        public static int FindTabId(this HttpContextBase context)
        {
            string value = context.Request.Headers[TabIdKey];

            if(String.IsNullOrEmpty(value))
            {
                value = context.Request.Params[TabIdKey];
            }

            int id;
            if(Int32.TryParse(value, out id))
            {
                return id;
            }

            return Null.NullInteger;
        }
    }
}