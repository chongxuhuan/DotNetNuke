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
using WatiN.Core.DialogHandlers;

namespace DotNetNuke.Website.Specs
{
	[Binding]
    public class SearchResultSteps : WatiNTest
	{
	    public SchedulePage SchedulePage
	    {
	        get
	        {
	            return GetPage<SchedulePage>();
	        }
	    }

        [When(@"I run scheduler (.*)")]
        public void WhenIRunScheduler(string name)
        {
            SchedulePage.GetScheduleItemEditButton(name).Click();
            HomePage.PopUpFrame.Link(Find.ByTitle("Run Now")).Click();
            HomePage.PopUpFrame.Link(Find.ByTitle("Run Now")).WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            IEInstance.Refresh();
            Thread.Sleep(25000);
        }

        [When(@"I visit child portal")]
        public void WhenIVisitChildPortal()
        {
            IEInstance.GoTo(SiteUrl + "/Child");
        }

        [When(@"I input search text (.*) in search box and click search button")]
        public void WhenIInputSearchTextInSearchBoxAndClickSearchButton(string word)
        {
            IEInstance.TextField(Find.ById(i => i.EndsWith("dnnSearch_txtSearchNew"))).Value = word;
            IEInstance.Link(Find.ById(i => i.EndsWith("dnnSearch_cmdSearchNew"))).Click();
        }

        [Then(@"I should not see result from parent portal")]
        public void ThenIShouldNotSeeResultFromParentPortal()
        {
            Assert.IsFalse(IEInstance.Span(s => s.ClassName == "scrPath" && !s.InnerHtml.Contains(SiteUrl + "/child")).Exists);
        }

	}
}
