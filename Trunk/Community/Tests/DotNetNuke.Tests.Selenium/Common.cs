using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Tests.Selenium
{
    public class Common
    {

        public static int DriverTimeout;
        public static string TestSite;

        public class BrowserType
        {
            public const string firefox = "firefox";
            public const string chrome = "chrome";
            public const string ie = "ie";
            public const string opera = "opera";
        }
    }
}
