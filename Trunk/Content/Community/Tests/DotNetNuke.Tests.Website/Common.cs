using System;

namespace DotNetNuke.Tests.Website
{
    public class Common
    {
        public static TimeSpan DriverTimeout;
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
