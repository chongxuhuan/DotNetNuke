using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;

namespace DotNetNuke.Tests.UI
{
    public static class IEExtensions
    {
        public static void GoToDNNUrl(this IE ie, string page)
        {
            if (!page.StartsWith("/"))
            {
                page = "/" + page;
            }

            ie.GoTo(String.Concat(TestEnvironment.PortalUrl, page));
        }
    }
}
