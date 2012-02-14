// // DotNetNuke® - http://www.dotnetnuke.com
// // Copyright (c) 2002-2012
// // by DotNetNuke Corporation
// // 
// // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// // documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// // the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// // to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// // 
// // The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// // of the Software.
// // 
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// // TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// // THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// // CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// // DEALINGS IN THE SOFTWARE.

using System.IO;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using DotNetNuke.Common;

namespace DotNetNuke.Framework
{
    internal class ServicesFrameworkImpl : IServicesFramework
    {
        private const string Key = "dnnAntiForgeryRequested";

        public void RequestAjaxAntiForgerySupport()
        {
            HttpContextSource.Current.Items[Key] = true;
        }

        public bool IsAjaxAntiForgerySupportRequired
        {
            get { return HttpContextSource.Current.Items.Contains(Key); }
        }

        public void RegisterAjaxAntiForgery(Page page)
        {
            var helper = CreateHtmlHelper();
            var field = helper.AntiForgeryToken();
            var ctl = page.FindControl("ClientResourcesFormBottom");
            if(ctl != null)
            {
                ctl.Controls.Add(new LiteralControl(field.ToHtmlString()));
            }
        }

        private static HtmlHelper CreateHtmlHelper()
        {
            //The only "real" element in the HtmlHelper is the HttpContext
            //the rest in not actually used by the antiforgery code
            var controllerContext = new ControllerContext(HttpContextSource.Current, new RouteData(),
                                                          new DummyController());
            IView view = new DummyView();
            var viewData = new ViewDataDictionary();
            var tempData = new TempDataDictionary();
            TextWriter writer = new StringWriter();
            var viewContext = new ViewContext(controllerContext, view, viewData, tempData, writer);
            IViewDataContainer dataContainer = new DummyViewDataContainer();
            var helper = new HtmlHelper(viewContext, dataContainer);
            return helper;
        }

        private class DummyView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new System.NotImplementedException();
            }
        }


        private class DummyViewDataContainer : IViewDataContainer
        {
            /// <summary>
            /// Gets or sets the view data dictionary.
            /// </summary>
            /// <returns>
            /// The view data dictionary.
            /// </returns>
            public ViewDataDictionary ViewData
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }
        }

        private class DummyController : ControllerBase
        {
            protected override void ExecuteCore()
            {
                throw new System.NotImplementedException();
            }
        }
    }
}