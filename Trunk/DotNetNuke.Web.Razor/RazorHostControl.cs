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

#region Usings

using System;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.WebPages;

using DotNetNuke.UI.Modules;
using DotNetNuke.Web.Razor.Helpers;

#endregion

namespace DotNetNuke.Web.Razor
{
    public class RazorHostControl : ModuleControlBase
    {
        private readonly string _razorScriptFile;

        public RazorHostControl(string scriptFile)
        {
            _razorScriptFile = scriptFile;
        }

        protected HttpContextBase HttpContext
        {
            get { return new HttpContextWrapper(System.Web.HttpContext.Current); }
        }

        protected virtual string RazorScriptFile
        {
            get { return _razorScriptFile; }
        }

        private object CreateWebPageInstance()
        {
            Type type = BuildManager.GetCompiledType(RazorScriptFile);
            object instance = null;

            if (type != null)
            {
                instance = Activator.CreateInstance(type);
            }

            return instance;
        }

        private void InitHelpers(DotNetNukeWebPage webPage)
        {
            webPage.Dnn = new DnnHelper(ModuleContext);
            webPage.Html = new HtmlHelper(ModuleContext, LocalResourceFile);
            webPage.Url = new UrlHelper(ModuleContext);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!(string.IsNullOrEmpty(RazorScriptFile)))
            {
                object instance = CreateWebPageInstance();
                if (instance == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                      "The webpage found at '{0}' was not created.",
                                                                      RazorScriptFile));
                }

                DotNetNukeWebPage webPage = instance as DotNetNukeWebPage;

                if (webPage == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                                                      "The webpage at '{0}' must derive from DotNetNukeWebPage.",
                                                                      RazorScriptFile));
                }

                webPage.Context = HttpContext;
                webPage.VirtualPath = VirtualPathUtility.GetDirectory(RazorScriptFile);
                InitHelpers(webPage);

                StringWriter writer = new StringWriter();
                webPage.ExecutePageHierarchy(new WebPageContext(HttpContext, webPage, null), writer, webPage);

                Controls.Add(new LiteralControl(HttpContext.Server.HtmlDecode(writer.ToString())));
            }
        }
    }
}