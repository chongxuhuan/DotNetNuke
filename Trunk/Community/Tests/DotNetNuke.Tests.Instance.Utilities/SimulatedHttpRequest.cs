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
using System.Web.Hosting;
using System.Web;
using System.IO;

namespace DotNetNuke.Tests.Instance.Utilities
{
    public static class UnitTestHelper
    {
        /// <summary>
        /// Sets the HTTP context with a valid simulated request
        /// </summary>
        /// <param name="host">Host.</param>
        /// <param name="application">Application.</param>
        /// <param name="appPhysicalDir"></param>
        /// <param name="pageName"></param>
        public static void SetHttpContextWithSimulatedRequest(string host, string application, string appPhysicalDir, string pageName)
        {
            const string appVirtualDir = "/";
            var page = application.Replace("/", string.Empty) + "/" + pageName;
            var query = string.Empty;
            TextWriter output = null;

            var workerRequest = new SimulatedHttpRequest(appVirtualDir, appPhysicalDir, page, query, output, host);
            HttpContext.Current = new HttpContext(workerRequest);
        }
    }

    /// <summary>
    /// Used to simulate an HttpRequest.
    /// </summary>
    public class SimulatedHttpRequest : SimpleWorkerRequest
    {
        readonly string _host;

        /// <summary>
        /// Creates a new <see cref="SimulatedHttpRequest"/> instance.
        /// </summary>
        /// <param name="appVirtualDir">App virtual dir.</param>
        /// <param name="appPhysicalDir">App physical dir.</param>
        /// <param name="page">Page.</param>
        /// <param name="query">Query.</param>
        /// <param name="output">Output.</param>
        /// <param name="host">Host.</param>
        public SimulatedHttpRequest(string appVirtualDir, string appPhysicalDir, string page, string query, TextWriter output, string host): base(appVirtualDir, appPhysicalDir, page, query, output)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException("host", "Host cannot be null nor empty.");
            _host = host;
        }

        /// <summary>
        /// Gets the name of the server.
        /// </summary>
        /// <returns></returns>
        public override string GetServerName()
        {
            return _host;
        }

        /// <summary>
        /// Maps the path to a filesystem path.
        /// </summary>
        /// <param name="virtualPath">Virtual path.</param>
        /// <returns></returns>
        public override string MapPath(string virtualPath)
        {
            var path = "";
            var appPath = GetAppPath();

            if (appPath != null)
                path = Path.Combine(appPath, virtualPath);

            return path;
        }
    }
}