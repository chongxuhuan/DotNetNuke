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
using System.Text.RegularExpressions;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The site menu object.
    /// </summary>
    public class SiteMenu : WatiNBase
    {
        #region Constructors

        public SiteMenu(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public SiteMenu(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Spans
        public Span BreadCrumbSpan
        {
            get { return IEInstance.Span(Find.ById(new Regex("dnn_dnnBreadcrumb_lblBreadCrumb", RegexOptions.IgnoreCase))); }
        }
        public Span HomeButton
        {
            get { return MainMenu.Span(Find.ByText("Home")); }
        }
        #endregion

        #region Links
        public Link SearchLink
        {
            get { return IEInstance.Link(Find.ById("dnn_dnnSearch_cmdSearchNew")); }
        }
        #endregion

        #region TextFields
        public TextField SearchField
        {
            get { return IEInstance.TextField(Find.ById("dnn_dnnSearch_txtSearchNew")); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The outer div in the site menu.
        /// </summary>
        public Div MainMenu
        {
            get
            {
                if(IEInstance.Div(Find.ByClass("menu_style")).Exists)
                {
                    return IEInstance.Div(Find.ByClass("menu_style"));
                }
                if(IEInstance.Div(Find.ByClass("tabmenu")).Exists)
                {
                    return IEInstance.Div(Find.ByClass("tabmenu"));
                }
                return IEInstance.Div(Find.ById("Nav"));
            }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the span containing the menu button at the specified index.
        /// </summary>
        /// <param name="position">The index of the menu item. To access the first span in the menu use 0.</param>
        /// <returns>The span containing the menu button.</returns>
        public Span getMenuButton(int position)
        {
            //Returns the span containing the menu button at the specified location
            //The position will be 0 indexed begining with the home page
            if (!(position < 0))
                return MainMenu.Spans[position];
            else
                return null; 
        }

        #endregion
    }
}
