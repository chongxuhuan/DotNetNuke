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

using System.Collections.Generic;

namespace DotNetNuke.Web.Client.ClientResourceManagement
{
    using System.IO;
    using System.Web;
    using System.Web.UI;
    using ClientDependency.Core;
    using ClientDependency.Core.Controls;

    /// <summary>
    /// Provides the ability to request that client resources (JavaScript and CSS) be loaded on the client browser.
    /// </summary>
    public class ClientResourceManager
    {
        private const string DefaultProvider = "PageHeaderProvider";

        private static ClientDependencyLoader GetLoader(Page page)
        {
            return (ClientDependencyLoader)page.FindControl("ClientResourceLoader");
        }

        private static bool FileExists(Page page, string filePath)
        {
            return File.Exists(page.Server.MapPath(filePath));
        }

        /// <summary>
        /// Requests that a JavaScript file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the JavaScript resource.</param>
        public static void RegisterScript(Page page, string filePath)
        {
            RegisterScript(page, filePath, Constants.DefaultPriority);
        }

        /// <summary>
        /// Requests that a JavaScript file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the JavaScript resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded. Established convention starts at 0 and increments by five for each subsequent priority. Default priority is set to 100.</param>
        public static void RegisterScript(Page page, string filePath, int priority)
        {
            var loader = GetLoader(page);
            if (loader != null)
            {
                loader.RegisterDependency(priority, filePath, ClientDependencyType.Javascript);
            }
        }

        /// <summary>
        /// Requests that a CSS file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the CSS resource.</param>
        public static void RegisterStyleSheet(Page page, string filePath)
        {
            RegisterStyleSheet(page, filePath, Constants.DefaultPriority, DefaultProvider);
        }

        /// <summary>
        /// Requests that a CSS file be registered on the client browser. Defaults to rendering in the page header.
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the CSS resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded. Established convention starts at 0 and increments by five for each subsequent priority. Default priority is set to 100.</param>
        public static void RegisterStyleSheet(Page page, string filePath, int priority)
        {
            RegisterStyleSheet(page, filePath, priority, DefaultProvider);
        }

        /// <summary>
        /// Requests that a CSS file be registered on the client browser. Allows for overriding the default provider.
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the CSS resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded. Established convention starts at 0 and increments by five for each subsequent priority. Default priority is set to 100.</param>
        /// <param name="provider">The provider name to be used to render the css file on the page.</param>
        public static void RegisterStyleSheet(Page page, string filePath, int priority, string provider)
        {
            // to do: put some defensive logic around this to prevent excessive file system hits.
            if (FileExists(page, filePath))
            {
                var include = new DnnCssInclude {ForceProvider = provider, Priority = priority, FilePath = filePath};
                var loader = page.FindControl("ClientResourceCssIncludes");

                if (loader != null)
                {
                    loader.Controls.Add(include);
                }
            }
        }
    }
}