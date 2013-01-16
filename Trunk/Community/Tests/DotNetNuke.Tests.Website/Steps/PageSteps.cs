using System;
using System.Threading;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Security.Roles;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace DotNetNuke.Tests.Website.Steps
{
    [Binding]
    public class PageSteps : AutomationBase
    {
        /// <summary>
        /// Ensures that a Page called Designers is not present.  A page called Designers may then
        /// be added by the test.
        /// </summary>
        [BeforeScenario("PageDesignersMustNotExist")]
        public void PageDesignersMustNotExist()
        {
            var tabController = new TabController();
            var tab = tabController.GetTabByName("Designers", 0);
            if (tab != null)
            {
                tabController.DeleteTab(tab.TabID, 0);
                Config.Touch();
            }
        }

        [Given(@"There is a Page called (.*) with these permissions")]
        public void GivenThereIsAPageCalled(string pageName, Table permissions)
        {
            var reset = false;
            var tabController = new TabController();
            var tab = tabController.GetTabByName(pageName, PortalId);
            if (tab == null)
            {
                tab = new TabInfo
                {
                    TabName = pageName,
                    PortalID = 0
                };
                tab.TabID = tabController.AddTab(tab);
                foreach (var row in permissions.Rows)
                {
                    var roleId = -1;
                    var roleController = new RoleController();
                    if (row[0] == "All Users")
                    {
                        roleId = -1;
                    }
                    else
                    {
                        var role = roleController.GetRoleByName(PortalId, row[0]);
                        if (role == null)
                        {
                            if (roleController.GetRoleByName(Null.NullInteger, row[0]) == null)
                            {
                                role = new RoleInfo { RoleName = row[0], RoleGroupID = Null.NullInteger };
                                roleId = roleController.AddRole(role);
                            }
                        }
                    }
                    var permissionController = new PermissionController();
                    var permission = permissionController.GetPermissionByCodeAndKey("SYSTEM_TAB", row[1]);
                    var tabPermission = new TabPermissionInfo
                    {
                        PermissionID = 3,
                        TabID = tab.TabID,
                        AllowAccess = true,
                        RoleID = roleId
                    };
                    tab.TabPermissions.Add(tabPermission);
                }

                tabController.UpdateTab(tab);
                reset = true;
            }
            Page = tab;
            if (reset)
            {
                Config.Touch();
            }
        }

        [Given(@"I go to (.*)")]
        public void IGoTo(string url)
        {
            Driver.Navigate().GoToUrl(url);
        }

        [Then(@"the page will contain '(.*)'")]
        public void ThePageWillContain(string contains)
        {
            Assert.IsTrue(Driver.PageSource.Contains(contains));
        }

        [AfterTestRun]
        public static void TearDown()
        {
            if (CloseBrowser)
            {
                Driver.Quit();
                Driver = null;
            }
        }
    }
}
