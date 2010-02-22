using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Tests.UI
{
    public static class TestUsers
    {
        public static class Admin
        {
            public const string UserName = "admin";
            public const string Password = "password";
            public const string DisplayName = "Administrator Account";
        }

        public static class Host
        {
            public const string UserName = "host";
            public const string Password = "dnnhost";
            public const string DisplayName = "SuperUser Account";
        }
    }
}
