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

using System;


namespace DotNetNuke.Web.Client.ClientResourceManagement
{
    using System.IO;
	using System.Web.UI;
	using ClientDependency.Core;

    /// <summary>
    /// Provides the ability to request that client resources (JavaScript and CSS) be loaded on the client browser.
    /// </summary>
    public class ClientResourceManager
    {
        internal const string DefaultCssProvider = "PageHeaderProvider";
        internal const string DefaultJsProvider = "DnnBodyProvider";

        private static bool FileExists(Page page, string filePath)
        {
            return IsAbsoluteUrl(filePath) || File.Exists(page.Server.MapPath(filePath));
        }

        private static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        /// <summary>
        /// Requests that a JavaScript file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the JavaScript resource.</param>
        public static void RegisterScript(Page page, string filePath)
        {
            RegisterScript(page, filePath, FileOrder.Js.DefaultPriority);
        }

        /// <summary>
        /// Requests that a JavaScript file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the JavaScript resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded.</param>
        public static void RegisterScript(Page page, string filePath, int priority)
        {
            RegisterScript(page, filePath, priority, DefaultJsProvider);
        }

        /// <summary>
        /// Requests that a JavaScript file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the JavaScript resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded.</param>
        public static void RegisterScript(Page page, string filePath, FileOrder.Js priority)
        {
            RegisterScript(page, filePath, (int)priority, DefaultJsProvider);
        }

        /// <summary>
        /// Requests that a JavaScript file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the JavaScript resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded.</param>
        /// <param name="provider">The name of the provider responsible for rendering the script output.</param>
        public static void RegisterScript(Page page, string filePath, FileOrder.Js priority, string provider)
        {
            RegisterScript(page, filePath, (int) priority, provider);
        }

        /// <summary>
        /// Requests that a JavaScript file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the JavaScript resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded.</param>
        /// <param name="provider">The name of the provider responsible for rendering the script output.</param>
        public static void RegisterScript(Page page, string filePath, int priority, string provider)
        {
            var include = new DnnJsInclude { ForceProvider = provider, Priority = priority, FilePath = filePath };
            var loader = page.FindControl("ClientResourceIncludes");
            if (loader != null)
            {
                loader.Controls.Add(include);
            }
        }

        /// <summary>
        /// Requests that a CSS file be registered on the client browser
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the CSS resource.</param>
        public static void RegisterStyleSheet(Page page, string filePath)
        {
            RegisterStyleSheet(page, filePath, Constants.DefaultPriority, DefaultCssProvider);
        }

        /// <summary>
        /// Requests that a CSS file be registered on the client browser. Defaults to rendering in the page header.
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the CSS resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded.</param>
        public static void RegisterStyleSheet(Page page, string filePath, int priority)
        {
            RegisterStyleSheet(page, filePath, priority, DefaultCssProvider);
        }

        /// <summary>
        /// Requests that a CSS file be registered on the client browser. Defaults to rendering in the page header.
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the CSS resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded.</param>
        public static void RegisterStyleSheet(Page page, string filePath, FileOrder.Css priority)
        {
            RegisterStyleSheet(page, filePath, (int)priority, DefaultCssProvider);
        }

        /// <summary>
        /// Requests that a CSS file be registered on the client browser. Allows for overriding the default provider.
        /// </summary>
        /// <param name="page">The current page. Used to get a reference to the client resource loader.</param>
        /// <param name="filePath">The relative file path to the CSS resource.</param>
        /// <param name="priority">The relative priority in which the file should be loaded.</param>
        /// <param name="provider">The provider name to be used to render the css file on the page.</param>
        public static void RegisterStyleSheet(Page page, string filePath, int priority, string provider)
        {
            // to do: put some defensive logic around this to prevent excessive file system hits.
            if (FileExists(page, filePath))
            {
                var include = new DnnCssInclude {ForceProvider = provider, Priority = priority, FilePath = filePath};
                var loader = page.FindControl("ClientResourceIncludes");

                if (loader != null)
                {
                    loader.Controls.Add(include);
                }
            }
        }
    }
}