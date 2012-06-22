using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core.DialogHandlers;

namespace DotNetNuke.Website.Specs.Modules.Html
{
    [Binding]
    public class WorkflowSteps : WatiNTest
    {
        #region Properties

        public HTMLModule HTMLModule
        {
            get
            {
                return GetPage<HTMLModule>();
            }
        }

        public ModulePage ModulePage
        {
            get
            {
                return GetPage<ModulePage>();
            }
        }

        #endregion

        #region Scenario "The token in email subject should be replace"

        [Given(@"The email subject has been update to include tokens")]
        public void GivenTheEmailSubjectHasBeenUpdateToIncludeTokens()
        {
            var resourceFile = Path.Combine(PhysicalPath, "DesktopModules\\HTML\\App_LocalResources\\SharedResources.resx");
            var content = File.ReadAllText(resourceFile);
            content = content.Replace("<value>Content [ACTION] Notification</value>", "<value>Content Notification: [ACTION] [OLDSTATE] [STATE] [URL]</value>");
            File.WriteAllText(resourceFile, content);
        }

        [When(@"I change workflow to (.*)")]
        public void WhenIChangeWorkflowToContentApproval(string name)
        {
            HTMLModule.WorkflowSelectList.Select(name);
            HTMLModule.WorkflowSelectList.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            HTMLModule.UpdateLink.Click();
        }

        [When(@"I Submit content of Module (.*)")]
        public void WhenISubmitContentOfModule0(int moduleId)
        {
            ModulePage.GetEditModuleContentLink(moduleId).Click();
            if (!HTMLModule.PublishCheckBox.Enabled)
            {
                var dialog = new ConfirmDialogHandler();
                using (new UseDialogOnce(IEInstance.DialogWatcher, dialog))
                {
                    HTMLModule.SaveContentLink.Click();
                    dialog.WaitUntilExists();
                    dialog.OKButton.Click();
                }

                ModulePage.GetEditModuleContentLink(moduleId).Click();
            }
            HTMLModule.PublishCheckBox.Checked = true;
            HTMLModule.SaveContentLink.Click();
            HTMLModule.CommentTextField.Value = "Submit Comment";
            HTMLModule.AddCommentLink.Click();
        }

        [Then(@"Then subject in notification email should replace all tokens")]
        public void ThenThenSubjectInNotificationEmailShouldReplaceAllTokens()
        {
            var user = UserController.GetUserByName(PortalId, TestUsers.Admin.UserName);
            var notification = NotificationsController.Instance.GetNotifications(user.UserID, PortalId, 0, 1)
                .FirstOrDefault();
            WatiNAssert.AssertIsTrue(notification != null, "GetNotification_" + user.UserID + "_Error.jpg");
            Assert.IsTrue(notification.Subject.IndexOf("[", StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        private String FindLineUsingRegex(string linePrefix, string emailContent, string emailFileName)
        {
            string lineRegex = linePrefix + @": .*";
            Match match = Regex.Match(@emailContent, @lineRegex);
            Assert.IsTrue(match.Success, "Could not find " + linePrefix + " Line! Looking in file: " + emailFileName);
            String line = match.Value;
            return line;
        }

        #endregion
    }
}
