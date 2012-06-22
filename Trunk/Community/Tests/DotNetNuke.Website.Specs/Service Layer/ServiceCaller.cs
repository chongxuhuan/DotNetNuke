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

using System;
using System.Configuration;
using System.Net;

namespace DotNetNuke.Website.Specs.ServiceLayer
{
    public class ServiceCaller
    {
        private readonly WebClient _webClient = new WebClient();

        public ServiceCaller()
        {
            _webClient.BaseAddress = DetermineBaseAddress();
            _webClient.UseDefaultCredentials = false;
            UserName = "host";
            Password = "dnnhost";
            AuthMode = AuthMode.None;
        }

        private static string DetermineBaseAddress()
        {
            var setting = ConfigurationManager.AppSettings["SiteURL"];
            return setting.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ? setting : "http://" + setting;
        }

        public string Module { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public AuthMode AuthMode { get; set; }

        public string CallService()
        {
            string path = string.Format("/DesktopModules/{0}/API", Module);

            if (!String.IsNullOrEmpty(Controller))
            {
                path += "/" + Controller;

                if (!String.IsNullOrEmpty(Action))
                {
                    path += "/" + Action;
                }
            }

            if (AuthMode != AuthMode.None)
            {
                var cache = new CredentialCache();
                cache.Add(new Uri(_webClient.BaseAddress), AuthMode.ToString(), new NetworkCredential(UserName, Password));
                _webClient.Credentials = cache;
            }

            return _webClient.DownloadString(path);
        }
    }
}