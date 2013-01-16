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
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using DotNetNuke.Tests.Utilities;

using TechTalk.SpecFlow;

using Table = TechTalk.SpecFlow.Table;

namespace DotNetNuke.Tests.BuildVerification
{
    [Binding]
    public class NewsletterSteps : WatiNTest
    {
        public NewsletterPage NewsletterPage
        {
            get
            {
                return GetPage<NewsletterPage>();
            }
        }

        /// <summary>
        /// Fills in the newsletter form with information from the table.
        /// </summary>
        /// <param name="table">A table containing the newsletter information.
        /// The table must be in the following format:
        /// | Label        | Value                  |
        /// | FromAddress  | {fromAddress}          |
        /// | ReplyTo      | {replyToAddress}       |
        /// | SubjectField | {subject}              |
        /// | ContentField | {content}              |
        /// </param>
        [When(@"I fill in the newsletter form with the following information")]
        public void WhenIFillInTheNewsletterFormWithTheFollowingInformation(Table table)
        {
            NewsletterPage.FromAddressField.Value = table.Rows[0]["Value"];
            NewsletterPage.ReplyToField.Value = table.Rows[1]["Value"];
            NewsletterPage.SubjectField.Value = table.Rows[2]["Value"];
            NewsletterPage.SelectRadioButtonByName("BASIC");
            NewsletterPage.MessageTabLink.Click();
            NewsletterPage.ContentTextBox.Value = table.Rows[3]["Value"];
        }

        /// <summary>
        /// Selects a role to send the newsletter to. 
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        [When(@"I check the role (.*) on the newsletters page")]
        public void WhenICheckTheRoleOnTheNewslettersPage(string roleName)
        {
            NewsletterPage.CheckRoleCheckBoxByName(roleName);
        }

        /// <summary>
        /// Clicks the send email link on the newsletter page.
        /// </summary>
        [When(@"I click Send")]
        public void WhenIClickSend()
        {
            NewsletterPage.SendButton.Click();
            Thread.Sleep(2500);
        }

        /// <summary>
        /// Checks that the a newsletter is sent to the userEmail with the correct information.
        /// This step assumes that emails are being sent to the folder Tests\Packages\TestEmails.
        /// </summary>
        /// <param name="userEmail">The email address of the user that the newsletter should be sent to.</param>
        /// <param name="table">A table conatining the newsletter information. 
        /// The table must be in the following format:
        /// | Label      | Value                  |
        /// | From		 | {fromEmail}            |
        /// | ReplyTo    | {replyToEmail}         |
        /// | Subject	 | {subject}              |
        /// | Content	 | This is the Newsletter |
        /// </param>
        [Then(@"The following newsletter should be sent to (.*)")]
        public void ThenTheFollowingNewsletterShouldBeSentTo(string userEmail, Table table)
        {
            string from = table.Rows[0]["Value"];
            string replyTo = table.Rows[1]["Value"];
            string subject = table.Rows[2]["Value"];
            string content = table.Rows[3]["Value"];

            var findByText = "X-Receiver: " + userEmail;

            MailAssert.MessageSent(findByText, userEmail, "The newsletter sent to " + userEmail + " could not be found.");
            MailAssert.FromLineContains(from, userEmail, findByText);
            MailAssert.ToLineContains(userEmail, userEmail, findByText);
            MailAssert.ReplyToLineContains(replyTo, userEmail, findByText);
            MailAssert.SubjectLineContains(subject, userEmail, findByText);
            MailAssert.ContentLineContains(content, userEmail, findByText);
        }

        /// <summary>
        /// Checks that the a bulk email report is sent to the userEmail with the correct information.
        /// This step assumes that emails are being sent to the folder Tests\Packages\TestEmails.
        /// </summary>
        /// <param name="userEmail">The email address of the host.</param>
        /// <param name="table">A table conatining the report information. 
        /// The table must be in the following format:
        /// | Label      | Value                  |
        /// | From		 | {fromEmail}            |
        /// | Recipients | {numRecipients}		  |
        /// | Messages	 | {numMessages}		  |
        /// | Subject	 | {newsletterSubject}    |
        /// </param>
        [Then(@"The following bulk email report should be sent to (.*)")]
        public void ThenTheFollowingBulkEmailReportShouldBeSentTo(string userEmail, Table table)
        {
            string from = table.Rows[0]["Value"];
            string recipients = table.Rows[1]["Value"];
            string messages = table.Rows[2]["Value"];
            string subject = table.Rows[3]["Value"];

            var findByText = "Subject: Bulk Email Report for '" + subject + "'.";

            MailAssert.MessageSent(findByText, userEmail, "The bulk email report could not be found.");
            MailAssert.FromLineContains(from, userEmail, findByText);
            MailAssert.SubjectLineContains("Subject: Bulk Email Report for '" + subject + "'.", userEmail, findByText);
            MailAssert.Base64EncodedContentLineContains("No errors occured during sending", userEmail, findByText);
        }


    }
}
