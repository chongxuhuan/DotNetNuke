#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
namespace DotNetNuke.Tests.UI.WatiN.Common
{
    /// <summary>
    /// A collection of user credentials to be used for testing.
    /// </summary>
    public static class TestUsers
    {
        public static class Admin
        {
            public const string UserName = "admin";
            public const string Password = "dnnhost";
            public const string DisplayName = "Administrator Account";
            public const string Email = "admin@dnn.com";
            public const string UpdatedPassword = "password";
        }

        public static class AdminUpdatedPassword
        {
            public const string UserName = "admin";
            public const string Password = "password";
            public const string DisplayName = "Administrator Account";
            public const string Email = "admin@dnn.com";
        }

        public static class Host
        {
            public const string UserName = "host";
            public const string Password = "dnnhost";
            public const string DisplayName = "SuperUser Account";
            public const string Email = "host@dnn.com";
        }

        public static class Admin2
        {
            public const string UserName = "admin2";
            public const string FirstName = "Admin2FN";
            public const string LastName = "Admin2LN";
            public const string Email = "admin2@dnn.com";
            public const string Password = "dnnadmin2";
            public const string DisplayName = FirstName + " " + LastName;
        }

        public static class User1
        {
            public const string UserName = "atestuser";
            public const string FirstName = "testuser1FN";
            public const string LastName = "testuser1LN";
            public const string DisplayName = "Testuser DN";
            public const string EmailAddress = "testuser1@dnn.com";
            public const string Password = "password";
            public const string NewPassword = "dnnuser1";
        }

        public static class User2
        {
            public const string UserName = "atestuser2";
            public const string FirstName = "user2FN";
            public const string LastName = "user2LN";
            public const string DisplayName = "User2 DN";
            public const string EmailAddress = "user2@dnn.com";
            public const string Password = "password";
        }

        public static class User3
        {
            public const string UserName = "atestUser3";
            public const string FirstName = "user3FN";
            public const string LastName = "user3LN";
            public const string DisplayName = "User3 DN";
            public const string EmailAddress = "user3@dnn.com";
            public const string Password = "password";
        }

        public static class TestHost
        {
            public const string UserName = "aHost";
            public const string FirstName = "hostFN";
            public const string LastName = "hostLN";
            public const string DisplayName = "Host DN";
            public const string EmailAddress = "ahost@dnn.com";
            public const string Password = "password";
            public const string NewPassword = "different";
        }

        public static class Register
        {
            public const string UserName = "register";
            public const string FirstName = "regFN";
            public const string LastName = "regLN";
            public const string DisplayName = "register DN";
            public const string EmailAddress = "register@dnn.com";
            public const string Password = "dnnregister";
            public const string NewPassword = "different";
        }

        public static class PaymentUser
        {
            public const string UserName = "paymentTest";
            public const string Password = "password";
        }

        public static class Invalid
        {
            public const string UserName = "badUser";
            public const string Password = "badUser";
        }

        public static class DBUser
        {
            public const string UserName = "dnntest";
            public const string Password = "dnntest";
        }
    }
}
