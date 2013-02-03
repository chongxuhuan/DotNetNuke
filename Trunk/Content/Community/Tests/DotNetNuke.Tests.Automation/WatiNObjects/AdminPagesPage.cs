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
using System.Collections.Specialized;
using System.Threading;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The Admin Pages page object. 
    /// </summary>
    public class AdminPagesPage : WatiNBase
    {
        #region Constructors

        public AdminPagesPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public AdminPagesPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Images
        [Obsolete("This object is no longer present in the pages module.")]  
        public Image AddNewPageImage
        {
            get { return ContentPaneDiv.Image(Find.ByTitle("Add New Page")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image DeleteSelectedPageImage
        {
            get { return ContentPaneDiv.Image(Find.ByTitle("Delete Selected Page")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image EditSelectedPageImage
        {
            get { return ContentPaneDiv.Image(Find.ByTitle("Edit Selected Page")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image MovePageBottomButton
        {
            get { return IEInstance.Image(Find.ByTitle("Move Selected Page to the Bottom of Current Level")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image MovePageDownButton
        {
            get { return IEInstance.Image(Find.ByTitle("Move Selected Page Down In Current Level")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image MovePageLeftButton
        {
            get { return IEInstance.Image(Find.ByTitle("Move Selected Page Up One Hierarchical Level")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image MovePageRightButton
        {
            get { return IEInstance.Image(Find.ByTitle("Move Selected Page Down One Hierarchical Level")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image MovePageTopButton
        {
            get { return IEInstance.Image(Find.ByTitle("Move Selected Page to the Top of Current Level")); }
        }
        [Obsolete("This object is no longer present in the pages module.")] 
        public Image MovePageUpButton
        {
            get { return IEInstance.Image(Find.ByTitle("Move Selected Page Up In Current Level")); }
        }

        #endregion

        #region SelectList
        [Obsolete("This object is no longer present in new pages module.")] 
        public SelectList PagesSelectList
        {
            get { return ContentPaneDiv.SelectList(Find.ById(s => s.EndsWith("Tabs_lstTabs"))); }
        }
        #endregion

        #region Div
        /// <summary>
        /// The div conatining the pages module.
        /// </summary>
        public Div PagesPanel
        {
            get { return ContentPaneDiv.Div(Find.ById("dnnTabsModule")); }
        }
        #endregion

        #region Span Collections
        /// <summary>
        /// The inner spans for all the pages listed in the module.
        /// This will include all of the admin pages, and the top most "My Website" item.
        /// To view the action links for one of these pages, find the required page, and call {myPageSpan}.FireEvent("oncontextmenu")
        /// </summary>
        public SpanCollection PageSpans
        {
            get { return IEInstance.Spans.Filter(Find.ByClass("rtIn")); }
        }
        #endregion

        #region Link Collections
        /// <summary>
        /// The collection of page function links within the page context menu. 
        /// </summary>
        public LinkCollection PageFunctionLinks
        {
            get { return IEInstance.Links.Filter(Find.ByClass(s => s.Contains("rmLink"))); }
        }
        #endregion

        #region TextFields
        /// <summary>
        /// The field used to add pages (in bulk) from the pages module.
        /// This field will only be visible after the add page link in a page context menu has been clicked.
        /// </summary>
        public TextField BulkAddPageField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("Tabs_txtBulk"))); }
        }
        #endregion

        #region Links
        /// <summary>
        /// The Add Page(s) link within the page context menu.
        /// This link will only be available after the context menu for a page has been displayed.
        /// To do this first find the page span for the required page, then use {myPageSpan}.FireEvent("oncontextmenu").
        /// </summary>
        public Link AddPageLink
        {
            get { return PageFunctionLinks.Filter(Find.ByText(s => s.Contains("Add Page(s)")))[0]; }
        }
        /// <summary>
        /// The Delete Page link within the page context menu.
        /// This link will only be available after the context menu for a page has been displayed.
        /// To do this first find the page span for the required page, then use {myPageSpan}.FireEvent("oncontextmenu").
        /// </summary>
        public Link DeletePageLink
        {
            get { return PageFunctionLinks.Filter(Find.ByText(s => s.Contains("Delete Page")))[0]; }
        }
        /// <summary>
        /// The Disable link in navigation link within the page context menu.
        /// This link will only be available after the context menu for a page has been displayed.
        /// To do this first find the page span for the required page, then use {myPageSpan}.FireEvent("oncontextmenu").
        /// </summary>
        public Link DisableNavigationLink
        {
            get { return PageFunctionLinks.Filter(Find.ByText(s => s.Contains("Disable link in navigation")))[0]; }
        }
        /// <summary>
        /// The Hide from navigation link within the page context menu.
        /// This link will only be available after the context menu for a page has been displayed.
        /// To do this first find the page span for the required page, then use {myPageSpan}.FireEvent("oncontextmenu").
        /// </summary>
        public Link HidePageLink
        {
            get { return PageFunctionLinks.Filter(Find.ByText(s => s.Contains("Hide from navigation")))[0]; }
        }
        /// <summary>
        /// The Make Homepage link within the page context menu.
        /// This link will only be available after the context menu for a page has been displayed.
        /// To do this first find the page span for the required page, then use {myPageSpan}.FireEvent("oncontextmenu").
        /// </summary>
        public Link MakeHomepageLink
        {
            get { return PageFunctionLinks.Filter(Find.ByText(s => s.Contains("Make Homepage")))[0]; }
        }
        /// <summary>
        /// The Page Settings link within the page context menu.
        /// This link will only be available after the context menu for a page has been displayed.
        /// To do this first find the page span for the required page, then use {myPageSpan}.FireEvent("oncontextmenu").
        /// </summary>
        public Link PageSettingsLink
        {
            get { return PageFunctionLinks.Filter(Find.ByText(s => s.Contains("Page Settings")))[0]; }
        }
        /// <summary>
        /// The View Page link within the page context menu.
        /// This link will only be available after the context menu for a page has been displayed.
        /// To do this first find the page span for the required page, then use {myPageSpan}.FireEvent("oncontextmenu").
        /// </summary>
        public Link ViewPageLink
        {
            get { return PageFunctionLinks.Filter(Find.ByText(s => s.Contains("View Page")))[0]; }
        }

        public Link CreatePageLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("Tabs_btnBulkCreate"))); }
        }

        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a child page to the site. 
        /// The parent page of the child must already exist on the site. 
        /// </summary>
        /// <param name="childName">The name for the child page.</param>
        /// <param name="parentName">The name of the parent page.</param>
        public void AddChildPage(string childName, string parentName)
        {
            Span homeSpan = PageSpans.Filter(Find.ByText(s => s.Contains(parentName)))[0];
            // Fires the left click event
            homeSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            AddPageLink.Click();
            BulkAddPageField.Value = childName;
            CreatePageLink.Click();
        }

        /// <summary>
        /// Adds a page that is hidden, or not in the menu. 
        /// </summary>
        /// <param name="pageName">The name of the page. </param>
        public void AddHiddenPage(string pageName)
        {
            AddPage(pageName);
            Thread.Sleep(2000);
            Span pageSpan = PageSpans.Filter(Find.ByText(s => s.Contains(pageName)))[0];
            // Fires the left click event
            pageSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            HidePageLink.Click();
        }

        /// <summary>
        /// Adds a page using the Pages bulk add page field.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        public void AddPage(string pageName)
        {
            Span homeSpan = PageSpans.Filter(Find.ByText(s => s.Contains("My Website")))[0];
            // Fires the left click event
            homeSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            AddPageLink.Click();
            Thread.Sleep(2500);
            BulkAddPageField.Value = pageName;
            CreatePageLink.Click();

        }

        /// <summary>
        /// Adds a page using the Pages bulk add page field.
        /// Gives the user view permissions for that page
        /// </summary>
        /// <param name="pageName">The name of the page</param>
        /// <param name="userName">The username of the user that will be given view permissions</param>
        public void AddPageGiveUserViewPermission(string pageName, string userName)
        {
            AddPage(pageName);
            EditPageGiveUserViewPermissions(pageName, userName);
        }

        /// <summary>
        /// Adds a page using the Pages bulk add page field.
        /// Sets the template for the page to the template specified
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="template">The name of the template.</param>
        public void AddPageSelectTemplate(string pageName, string template)
        {
            AddPage(pageName);
            Thread.Sleep(2500);
            Span pageSpan = PageSpans.Filter(Find.ByText(s => s.Contains(pageName)))[0];
            // Fires the left click event
            pageSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            PageSettingsLink.Click();
            Thread.Sleep(1500);
            PageSettingsPage _pageSettings = new PageSettingsPage(this);
            _pageSettings.TemplateFolderSelectList.Select(template);
            Thread.Sleep(1500);
            _pageSettings.UpdateLink.Click();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Gives a user view permissions for a page that has already been created.
        /// </summary>
        /// <param name="pageName">The name of the page that the user wil get view permissions for.</param>
        /// <param name="userName">The name of the user that will be given view permissions.</param>
        public void EditPageGiveUserViewPermissions(string pageName, string userName)
        {
            Span homeSpan = PageSpans.Filter(Find.ByText(s => s.Contains(pageName)))[0];
            // Fires the left click event
            homeSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            PageSettingsLink.Click();
            Thread.Sleep(1500);
            PageSettingsPage _pageSettings = new PageSettingsPage(this);
            _pageSettings.PermissionsTabLink.Click();
            _pageSettings.UserPermissionField.Value = userName;
            _pageSettings.AddUserPermissionLink.Click();
            Thread.Sleep(1500);
            _pageSettings.UpdatePageLink.Click();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Gives a user edit permissions for a page that has already been created.
        /// </summary>
        /// <param name="pageName">The name of the page that the user will get edit permissions for.</param>
        /// <param name="userName">The name of the user that will be given edit permissions.</param>
        /// <param name="displayName">The display name of the user.</param>
        public void EditPageGiveUserEditPermissions(string pageName, string userName, string displayName)
        {
            Span homeSpan = PageSpans.Filter(Find.ByText(s => s.Contains(pageName)))[0];
            // Fires the left click event
            homeSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            PageSettingsLink.Click();
            Thread.Sleep(1500);
            PageSettingsPage _pageSettings = new PageSettingsPage(this);
            _pageSettings.PermissionsTabLink.Click();
            Thread.Sleep(1500);
            _pageSettings.GiveUserEditPermissionsForPage(userName, displayName);
            Thread.Sleep(1500);
            _pageSettings.UpdatePageLink.Click();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Gives a role view permissions for a page that has already been created.
        /// </summary>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="roleName">The name of the role.</param>
        public void EditPageGiveViewPermissionForRole(string pageName, string roleName)
        {
            Span homeSpan = PageSpans.Filter(Find.ByText(s => s.Contains(pageName)))[0];
            // Fires the left click event
            homeSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            PageSettingsLink.Click();
            Thread.Sleep(1500);
            PageSettingsPage _pageSettings = new PageSettingsPage(this);
            _pageSettings.PermissionsTabLink.Click();
            _pageSettings.SetPermissionForRole("Grant", "View", roleName);
            _pageSettings.UpdatePageLink.Click();
            Thread.Sleep(1500);
        }

        /// <summary>
        /// Gives a role permissions for a page that has already been created.
        /// </summary>
        /// <param name="setting">Grant, Deny, Unchecked</param>
        /// <param name="permission">The name of the Permission such as View, Edit, Add, Add Content etc.</param>
        /// <param name="pageName">The name of the page.</param>
        /// <param name="roleName">The name of the role.</param>
        public void SetPagePermissionForRole(string setting, string permission, string pageName, string roleName)
        {
            Span homeSpan = PageSpans.Filter(Find.ByText(s => s.Contains(pageName)))[0];
            // Fires the left click event
            homeSpan.FireEvent("oncontextmenu");
            Thread.Sleep(1000);
            PageSettingsLink.Click();
            Thread.Sleep(1500);
            PageSettingsPage _pageSettings = new PageSettingsPage(this);
            _pageSettings.PermissionsTabLink.Click();
            _pageSettings.SetPermissionForRole(setting, permission, roleName);
            _pageSettings.UpdatePageLink.Click();
            Thread.Sleep(1500);
        }

        #endregion
    }
}
