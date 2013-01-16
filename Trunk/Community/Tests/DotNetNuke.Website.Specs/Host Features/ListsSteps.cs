using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Website.Specs.Host_Features
{
	[Binding]
    public class ListsSteps : WatiNTest
	{
        public HostListPage ListPage
		{
			get
			{
			    return GetPage<HostListPage>();
			}
		}

        [Given(@"I add a new list (.*)")]
        public void GivenIAddANewList(string listName)
        {
            if(ListPage.PageContentDiv.Span(Find.ByText(listName)).Exists)
            {
                return;
            }

            ListPage.AddListButton.Click();
            ListPage.AddListButton.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            ListPage.ListEntryName.Value = listName;
            ListPage.ListEntryText.Value = listName + "Item";
            ListPage.ListEntryValue.Value = listName + "Item";
            ListPage.SaveButton.Click();
            IEInstance.WaitForComplete();
        }

        [Given(@"I add new item (.*) to (.*)")]
        public void GivenIAddNewItemToList(string itemName, string listName)
        {
            ListPage.PageContentDiv.Span(Find.ByText(listName)).Click();
            ListPage.PageContentDiv.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));

            if(ListPage.GetHostListRow(itemName) != null)
            {
                return;
            }

            ListPage.AddEntryButton.Click();
            ListPage.AddEntryButton.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));

            ListPage.ListEntryText.Value = itemName;
            ListPage.ListEntryValue.Value = itemName;
            ListPage.SaveButton.Click();
            ListPage.SaveButton.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [Then(@"I should see delete button of item (.*) in the list")]
        public void ThenIShouldSeeDeleteButtonOfItemInTheList(string itemName)
        {
            Assert.IsTrue(ListPage.GetHostListDeleteButton(itemName).Exists);
        }
	}
}
