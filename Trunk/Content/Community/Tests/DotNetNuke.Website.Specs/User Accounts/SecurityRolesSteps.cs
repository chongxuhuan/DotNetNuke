using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Entities.Users;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

using TableRow = WatiN.Core.TableRow;

namespace DotNetNuke.Website.Specs.Steps
{
    [Binding]
    public class SecurityRolesSteps : WatiNTest
    {
        public SecurityRolesPage SecurityRolesPage
        {
            get
            {
                return GetPage<SecurityRolesPage>();
            }
        }

        [When(@"I click edit button of role (.*)")]
        public void WhenIClickEditButtonOfRole(string roleName)
        {
            SecurityRolesPage.GetRoleEditButton(roleName).Click();
        }

        [When(@"I set role's status to (.*)")]
        public void WhenISetRoleSStatus(string status)
        {
            SecurityRolesPage.RoleStatusSelectList.Select(status);
        }

        [When("I click Update button")]
        public void IClickUpdateButton()
        {
            SecurityRolesPage.UpdateLink.Click();
            SecurityRolesPage.UpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            Thread.Sleep(2000);
        }

        [Then(@"I shouldn't see ""Manage Users in this Role"" button")]
        public void ThenIShouldnTSeeManageUsersInThisRoleButton()
        {
            WatiNAssert.AssertIsTrue(!SecurityRolesPage.ManageUsersInRoleLink.Exists, "ManageUsersInRoleLinkExsits.png");
        }

    }
}
