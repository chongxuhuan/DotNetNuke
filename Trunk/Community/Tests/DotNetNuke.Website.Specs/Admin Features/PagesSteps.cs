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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;
using WatiN.Core.Native.InternetExplorer;

using mshtml;

namespace DotNetNuke.Website.Specs.Steps
{
	[Binding]
    public class PagesSteps : WatiNTest
    {
        #region Properties

        public AdminPagesPage PagesPage
		{
			get
			{
                return GetPage<AdminPagesPage>();
			}
		}
        
	    public PageSettingsPage PageSettingsPage
	    {
	        get
	        {
	            return GetPage<PageSettingsPage>();
	        }
	    }

	    public RibbonBar RibbonBar
	    {
	        get
	        {
	            return GetPage<RibbonBar>();
	        }
	    }

	    public ExportImportPage ExportImportPage
	    {
	        get
	        {
	            return GetPage<ExportImportPage>();
	        }
	    }

	    public SiteSettingsPage SiteSettingsPage
	    {
	        get
	        {
	            return GetPage<SiteSettingsPage>();
	        }
	    }

        #endregion

        #region Scenario "Click help icon"

        [When(@"I click on a page in the page treeview")]
		public void WhenIClickOnAPage()
		{
			PagesPage.PageSpans.Filter(Find.ByText("Home")).First().ClickNoWait();
			Thread.Sleep(2000);
		}


		[When(@"I click help icon")]
		public void WhenIClickHelpIcon()
		{
			var label = IEInstance.Spans.Filter(Find.ById(s => s.EndsWith("lblName_lblLabel"))).First();
			PositionMousePointerInMiddleOfElement(label, IEInstance);
		}

		[Then(@"I should see help text")]
		public void ThenIShouldSeeHelpText()
		{
			Assert.AreEqual("block", IEInstance.Divs.Filter(Find.ById(d => d.EndsWith("lblName_pnlHelp"))).First().Style.Display);
		}

        #endregion

        #region Scenario "Page create with template should apply permisson from template"

        [BeforeScenario(@"MustNotHaveTheTestPages")]
        public void MustNotHaveTestPages()
        {
            var tabController = new TabController();
            var tabId = Null.NullInteger;

            tabId = TabController.GetTabByTabPath(PortalId, "//TestA", Null.NullString);
            if (tabId != Null.NullInteger)
            {
                tabController.DeleteTab(tabId, PortalId);
            }

            tabId = TabController.GetTabByTabPath(PortalId, "//TestB", Null.NullString);
            if (tabId != Null.NullInteger)
            {
                tabController.DeleteTab(tabId, PortalId);
            }
        }

        [Given("I clean the cache")]
        public void CleanCache()
        {
            RibbonBar.ClearCacheLink.Click();
        }
        [Given(@"I export template of the page(.*)")]
        public void GivenIExportTemplateOfThePage(string pageName)
        {
            NavigateToPage(pageName);
            RibbonBar.ExportPageLink.Click();
            ExportImportPage.TemplateDescriptionField.Value = pageName;
            ExportImportPage.ExportLink.Click();
            ExportImportPage.ExportLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [Then(@"The page (.*) should have (.*) permisson on role (.*)")]
        public void ThenThePageTestBShouldHaveViewPermissonOnRoleRegisteredUsers(string pageName, string permisson, string roleName)
        {
            NavigateToPage(pageName);
            RibbonBar.EditCurrentPageLink.Click();
            switch (permisson)
            {
                case "View":
                    Assert.IsTrue(PageSettingsPage.GetViewPermissionsForRole(roleName));
                    break;
                case "Edit":
                    Assert.IsTrue(PageSettingsPage.GetEditPermissionsForRole(roleName));
                    break;
            }
        }


        #endregion

        #region Scenario "Should not allow to add page when page quota is exceed"

        [When(@"I update page quota to (.*)")]
        public void WhenIUpdatePageQuota(string pageQuota)
        {
            SiteSettingsPage.PageQuotaField.Value = pageQuota;
            SiteSettingsPage.UpdateLink.Click();
            SiteSettingsPage.UpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [Then(@"the add page button should disabled in Ribbon bar")]
        public void ThenTheAddPageButtonShouldDisabledInRibbonBar()
        {
            
        }

        [AfterScenario("ResetPageQuota")]
        public void ResetPageQuota()
        {
            IEInstance.GoTo(SiteUrl + "/logoff.aspx");
            GetPage<LoginPage>().LoginUser(TestUsers.Host.UserName, TestUsers.Host.Password);
            IEInstance.GoTo(string.Format("{0}/Admin/SiteSettings.aspx", SiteUrl));
            SiteSettingsPage.PageQuotaField.Value = "0";
            SiteSettingsPage.UpdateLink.Click();
            SiteSettingsPage.UpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        #endregion

        #region Scenario "Help Methods"

        public static void PositionMousePointerInMiddleOfElement(Element button, Document ie)
		{
			var left = Position(button, "Left");
			var width = int.Parse(button.GetAttributeValue("clientWidth"));
			var top = Position(button, "Top");
			var height = int.Parse(button.GetAttributeValue("clientHeight"));

			var window = (IHTMLWindow3)((IEDocument)ie.NativeDocument).HtmlDocument.parentWindow;

			left = left + window.screenLeft;
			top = top + window.screenTop;

			var currentPt = new System.Drawing.Point(left + (width / 2), top + (height / 2));
			System.Windows.Forms.Cursor.Position = currentPt;
		}

		private static int Position(Element element, string attributename)
		{

			var ieElement = (IEElement)element.NativeElement;
			var pos = 0;
			var offsetParent = ieElement.AsHtmlElement.offsetParent;

			if (offsetParent != null)
			{
				var domContainer = element.DomContainer;
				pos = Position(new Element(domContainer, new IEElement(offsetParent)), attributename);
			}

			if (WatiN.Core.Comparers.StringComparer.AreEqual(element.TagName, "table", true))
			{
				pos = pos + int.Parse(element.GetAttributeValue("client" + attributename));
			}
			return pos + int.Parse(element.GetAttributeValue("offset" + attributename));
        }

        private void NavigateToPage(string pageName)
        {
            pageName = pageName.Replace(" ", "");
            IEInstance.GoTo(SiteUrl + "/" + pageName + ".aspx");
        }

        #endregion
    }
}
