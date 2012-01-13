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
using System.Web;

namespace DotNetNuke.HttpModules.Services
{
    public class ServicesModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += CheckForReal401;
        }

        private static void CheckForReal401(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if(app != null)
            {
                CheckForReal401(new HttpContextWrapper(app.Context));
            }
        }

        internal static void CheckForReal401(HttpContextBase context)
        {
            if(context == null)
            {
                throw new ArgumentNullException("context");
            }

            if ((bool?)context.Items["DnnReal401"] ?? false)
            {
                context.Response.ClearContent();
                context.Response.StatusCode = 401;
                context.Response.Headers.Remove("Location");
            }
        }

        public void Dispose()
        {
        }
    }
}