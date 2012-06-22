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
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.HttpModules.Services.Internal;

namespace DotNetNuke.HttpModules.Services
{
    public class ServicesModule : IHttpModule
    {
        static readonly Regex ServiceApi = new Regex(@"DesktopModules/[^/]+/API");

        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += CheckForReal401;
            context.BeginRequest += InitDnn;
        }

        private void InitDnn(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app != null && ServiceApi.IsMatch(app.Context.Request.RawUrl))
            {
                Initialize.Init(app);
            }
        }

        private static void CheckForReal401(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if(app != null)
            {
                CheckForReal401(new ServicesContextWrapper(app.Context));
            }
        }

        internal static void CheckForReal401(IServicesContext context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (context.DoA401)
            {
                var response = context.BaseContext.Response;
                response.ClearContent();
                response.ClearHeaders();
                response.StatusCode = 401;

                if (context.SupportBasicAuth)
                {
                    response.AppendHeader("WWW-Authenticate", "Basic realm=\"DNNAPI\"");
                }

                if (context.SupportDigestAuth)
                {
                    var stale = context.IsStale.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
                    var value = string.Format("Digest realm=\"DNNAPI\", nonce=\"{0}\",  opaque=\"0000000000000000\", stale={1}, algorithm=MD5, qop=\"auth\"", CreateNewNonce(), stale);
                    response.AppendHeader("WWW-Authenticate", value);
                }
            }
        }

        //This nonce must be compatible with DotNetNuke.Web.Service.DigestAuthentication expectataions
        private static string CreateNewNonce()
        {
            DateTime nonceTime = DateTime.Now + TimeSpan.FromMinutes(1);
            string expireStr = nonceTime.ToString("G");

            byte[] expireBytes = Encoding.Default.GetBytes(expireStr);
            string nonce = Convert.ToBase64String(expireBytes);

            nonce = nonce.TrimEnd(new[] { '=' });
            return nonce;
        }

        public void Dispose()
        {
        }
    }
}