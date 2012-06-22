using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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

namespace DotNetNuke.Website.Specs.Admin_Features
{
    [Binding]
    public class NewsLetterSteps : WatiNTest
    {
        public NewsletterPage NewsletterPage
        {
            get
            {
                return GetPage<NewsletterPage>();
            }
        }

        public HTMLModule HtmlModule
        {
            get
            {
                return GetPage<HTMLModule>();
            }
        }

        [When(@"I input subject and message body")]
        public void WhenIInputSubjectAndMessageBody()
        {
            NewsletterPage.SubjectField.Value = "Subject";
            ExecuteEditorCommand(string.Format("$find('{0}').set_html('{1}');", NewsletterPage.ContentPaneDiv.Div(Find.ById(i => i.EndsWith("teMessage_teMessage"))).Id, "Message Body"));
        }

        [When(@"I click Preview Email")]
        public void WhenIClickPreviewEmail()
        {
            NewsletterPage.ContentPaneDiv.Link(Find.ByTitle("Preview Email")).Click();
            NewsletterPage.ContentPaneDiv.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [Then(@"I should see Cancel Preview button")]
        public void ThenIShouldSeeCancelPreviewButton()
        {
            Assert.IsTrue(NewsletterPage.ContentPaneDiv.Link(Find.ByTitle("Cancel Preview")).Exists);
        }

        private string ExecuteEditorCommand(string command)
        {
            if (HomePage.PopUpFrame != null)
            {
                return HomePage.PopUpFrame.Eval(command);
            }

            return IEInstance.Eval(command);
        }
    }
}
