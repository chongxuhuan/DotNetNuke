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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;
using System.Text.RegularExpressions;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The security roles page object.
    /// </summary>
    public class SecurityRolesPage : WatiNBase
    {
        #region Constructors

        public SecurityRolesPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public SecurityRolesPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Tables
        public Table RolesTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("Roles_grdRoles"))); }
        }
        #endregion

        #region Links
        public Link AddNewRoleLink
        {
            get { return IEInstance.Link(Find.ByTitle("Add New Role")); }
        }
        public Link AddNewRoleGroupLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Add New Role Group"))); }
        }
        /// <summary>
        /// The Add User to Role link on the Manage Users in Role page.
        /// </summary>
        public Link AddUserToRoleLink
        {
            get { return PageContentDiv.Div(Find.ById(s => s.EndsWith("SecurityRoles_pnlRoles"))).Link(Find.ByTitle("Add User to Role")); }
        }

        public Link ManageUsersInRoleLink
        {
            get
            {
                return FindElement<Link>(Find.ByTitle("Manage Users in this Role"));
            }
        }
        #endregion

        #region TextFields
        public TextField RoleNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditRoles_txtRoleName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditRoles_txtRoleName")));
            }
        }
        public TextField RoleDescription
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditRoles_txtDescription")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditRoles_txtDescription")));
            }
        }
        public TextField RoleGroupNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditGroups_txtRoleGroupName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditGroups_txtRoleGroupName")));
            }
        }
        public TextField RoleGroupDescriptionField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditGroups_txtDescription")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditGroups_txtDescription")));
            }
        }
        #endregion

        #region SelectLists
        public SelectList RoleGroupSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("EditRoles_cboRoleGroups")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("EditRoles_cboRoleGroups")));
            }
        }
        /// <summary>
        /// The filter by role group selectlist.
        /// Only visible if at least one role group is added.
        /// </summary>
        public SelectList FilterByGroupSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("Roles_cboRoleGroups"))); }
        }
        public SelectList AddUserToRoleSelectList
        {
            get { return PageContentDiv.SelectList(Find.ById(s => s.EndsWith("SecurityRoles_cboUsers"))); }
        }

        public SelectList RoleStatusSelectList
        {
            get
            {
                return FindElement<SelectList>(Find.ById(s => s.EndsWith("statusList")));
            }
        }
        #endregion

        #region CheckBoxes
        /// <summary>
        /// The public role checkbox from the edit role page.
        /// Will not find any of the public role checkboxes in the roles table.
        /// </summary>
        public CheckBox PublicRoleCheckBox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("EditRoles_chkIsPublic")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("EditRoles_chkIsPublic")));
            }
        }
        /// <summary>
        /// The Auto Assign role checkbox from the edit role page.
        /// Will not find any of the Auto Assign role checkboxes in the roles table.
        /// </summary>
        public CheckBox AutoAssignCheckBox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("EditRoles_chkAutoAssignment")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("EditRoles_chkAutoAssignment")));
            }
        }
        #endregion

        #region Images
        public Image DeleteRoleGroupButton
        {
            get { return IEInstance.Image(Find.ById(s => s.EndsWith("Roles_cmdDelete"))); }
        }
        public Image EditRoleGroupButton
        {
            get { return IEInstance.Image(Find.ById(s => s.EndsWith("Roles_imgEditGroup"))); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds a link on the page that contains the username text.
        /// </summary>
        /// <param name="userName">The username</param>
        /// <returns>A link with the innertext specified.</returns>
        public Link GetUserLinkFromUserRolesPage(string userName)
        {
            return IEInstance.Link(Find.ByText(userName));
        }

        /// <summary>
        /// Creates a role group.
        /// Clicks the Add New Role Group link
        /// Enters the group name
        /// Clicks the update link
        /// </summary>
        /// <param name="groupName">The name for the role group.</param>
        public void AddRoleGroup(string groupName)
        {
            AddNewRoleGroupLink.Click();
            RoleGroupNameField.Value = groupName;
            UpdateLink.Click();
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>
        /// Creates a new role.
        /// Clicks the Add New Role link
        /// Enters the role name
        /// Clicks the update link
        /// </summary>
        /// <param name="roleName">The name for the new role.</param>
        public void AddRole(string roleName)
        {
            AddNewRoleLink.Click();
            RoleNameField.Value = roleName;
            UpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
        }

        /// <summary>
        /// Adds a user to an existing role.
        /// Clicks the Manage Users link for the role
        /// Selects the users display name from the User selectlist
        /// Clicks the Add User to Role link
        /// </summary>
        /// <param name="roleName">The role name as it appears in the security roles table.</param>
        /// <param name="userDisplayName">The users display name.</param>
        public void AddUserToRole(string roleName, string userDisplayName)
        {
            TableRow roleRow = GetRoleRow(roleName);
            roleRow.Image(Find.ByTitle("Manage Users")).Click();

            AddUserToRoleSelectList.Select(new Regex(userDisplayName + "(.*)"));

            System.Threading.Thread.Sleep(1000);
            AddUserToRoleLink.Click();
        }

        /// <summary>
        /// Finds the manage users button for the role in the Security Roles table.
        /// </summary>
        /// <param name="roleName">The role name as it appears in the security roles table.</param>
        /// <returns>The manage users image/button for the role.</returns>
        public virtual Image GetManageUsersButton(string roleName)
        {
            //Returns the Manage Users Button for the role specified 
            Image result = null;
            foreach (TableRow row in RolesTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(roleName))
                {
                    result = row.Image(Find.ByTitle("Manage Users"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the edit button for the role in the security roles table.
        /// </summary>
        /// <param name="roleName">The role name as it appears in the security roles table.</param>
        /// <returns>The edit role image/button for the role.</returns>
        public virtual Image GetRoleEditButton(string roleName)
        {
            //Returns the Edit Button for the role specified 
            Image result = null;
            foreach (TableRow row in RolesTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(roleName))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the row for the role in the security roles table.
        /// </summary>
        /// <param name="roleName">The role name as it appears in the security roles table.</param>
        /// <returns>The table row for the role.</returns>
        public virtual TableRow GetRoleRow(string roleName)
        {
            //Returns the user Row specified from the user table
            TableRow result = null;
            foreach (TableRow row in RolesTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(roleName))
                {
                    result = row;
                    break;
                }
                continue;
            }
            return result;
        }

        #endregion
    }
}
