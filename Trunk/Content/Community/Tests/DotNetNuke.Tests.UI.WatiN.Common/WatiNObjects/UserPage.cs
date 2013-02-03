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
using System.Text.RegularExpressions;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The user accounts page object.
    /// </summary>
    public class UserPage : WatiNBase
    {
        #region Constructors

        public UserPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public UserPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }
       
        #endregion

        #region Public Properties

        #region Links
        /// <summary>
        /// The add new user link on the user accounts page.
        /// </summary>
        public Link AddNewUserStartLink 
        { 
            get { return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Add New User"))); } 
        }

        /// <summary>
        /// The add new user link on the add user form.
        /// </summary>
        public Link AddNewUserLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Add New User")));
                }
                return IEInstance.Link(Find.ByText(s => s.Contains("Add New User")));
            }
        }

        public Link AllDisplayLink
        {
            get { return ContentPaneDiv.Link(Find.ByText("All")); }
        }

        public Link UnauthorizeUserLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("UnAuthorize User"));
                }
                return IEInstance.Link(Find.ByTitle("UnAuthorize User"));
            }
        }

        public Link UnauthorizedDisplayLink
        {
            get { return IEInstance.Link(Find.ByText("Unauthorized")); }
        }

        public Link CancelLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Cancel"));
                } 
                return IEInstance.Link(Find.ByTitle("Cancel"));
            }
        }

        public Link AuthorizeUserLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Authorize User"));
                }
                return IEInstance.Link(Find.ByTitle("Authorize User"));
            }
        }

        public Link ForcePasswordChangeLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Force Password Change"));
                }
                return IEInstance.Link(Find.ByTitle("Force Password Change"));
            }
        }

        public Link ManageProfilePropertiesLink
        {
            get { return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Manage Profile Properties"))); }
        }

        public Link UserSettingsLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("User Settings"))); }
        }

        public Link ManagePasswordTabLink
        {
            get { return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Manage Password"))); }
        }

        public Link ChangePasswordLink
        {
            get { return ContentPaneDiv.Link(Find.ByTitle("Change Password")); }
        }

        public Link RegisterButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText(s => s.Contains("Register")));
                }
                return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Register")));
            }
        }

        #endregion

        #region TextFields
        public TextField UserNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$Username")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_userName_userName_TextBox")));
                }
               // return IEInstance.TextField(Find.ByName(s => s.EndsWith("$Username")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_userName_userName_TextBox")));
            }
        }
        public TextField FirstNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                   // return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$FirstName")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_firstName_firstName_TextBox")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$FirstName")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_firstName_firstName_TextBox")));
            }
        }
        public TextField LastNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$LastName")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_lastName_lastName_TextBox")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$LastName")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_lastName_lastName_TextBox")));
            }
        }
        public TextField DisplayNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$DisplayName")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_displayName_displayName_TextBox")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$DisplayName")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_displayName_displayName_TextBox")));
            }
        }
        public TextField EmailField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$Email")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_email_email_TextBox")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$Email")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_userForm_email_email_TextBox")));
            }
        }
        public TextField PasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    //return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$txtPassword")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_txtPassword")));
                }
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$txtPassword")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_txtPassword")));
            }
        }
        public TextField ConfirmPasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                   // return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$txtConfirm")));
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_txtConfirm")));
                } 
                //return IEInstance.TextField(Find.ByName(s => s.EndsWith("$txtConfirm")));
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_User_txtConfirm")));
            }
        }
        public TextField NewPasswordTextField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_Password_txtNewPassword")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_Password_txtNewPassword")));
            }
        }
        public TextField ConfirmNewPasswordField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ManageUsers_Password_txtNewConfirm")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("ManageUsers_Password_txtNewConfirm")));
            }
        }
        #endregion

        #region CheckBoxes
        public CheckBox AuthorizeCheckBox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById("ManageUsers_User_ChkAuthorize"));
                }
                return ContentPaneDiv.CheckBox(Find.ById("ManageUsers_User_ChkAuthorize"));
            }
        }
        public CheckBox NotifyCheckBox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById("ManageUsers_User_ChkNotify"));
                }
                return ContentPaneDiv.CheckBox(Find.ById("ManageUsers_User_ChkNotify"));
            }
        }
        #endregion

        #region Tables
        public Table UserTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("grdUsers"))); }
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds the delete button for the user.
        /// </summary>
        /// <param name="displayName">The users display name as it appears in the user table.</param>
        /// <returns>The delete image/button for the user.</returns>
        public virtual Image GetUserDeleteButton(string displayName)
        {
            //Returns the Delete Button for the user specified 
            Image result = null;
            foreach (TableRow row in UserTable.TableRows)
            {
                if (row.TableCells[7].InnerHtml.Contains(displayName))
                {
                    result = row.Image(Find.ByTitle("Delete"));
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the edit button for the user.
        /// </summary>
        /// <param name="displayName">The users display name as it appears in the user table.</param>
        /// <returns>The edit image/button for the user.</returns>
        public virtual Image GetUserEditButton(string displayName)
        {
            //Returns the Edit Button for the user specified 
            Image result = null;
            foreach (TableRow row in UserTable.TableRows)
            {
                if (row.TableCells[7].InnerHtml.Contains(displayName))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the roles button for the user.
        /// </summary>
        /// <param name="displayName">The users display name as it appears in the user table.</param>
        /// <returns>The roles image/button for the user.</returns>
        public virtual Image GetUserRolesButton(string displayName)
        {
            //Returns the Roles Button for the user specified 
            Image result = null;
            foreach (TableRow row in UserTable.TableRows)
            {
                if (row.TableCells[7].InnerHtml.Contains(displayName))
                {
                    result = row.Image(Find.ByTitle("Manage Roles"));
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the table row for the user.
        /// </summary>
        /// <param name="displayName">The users display name as it appears in the user table.</param>
        /// <returns>The table row for the user.</returns>
        public virtual TableRow GetUserRow(string displayName)
        {
            //Returns the user Row specified from the user table
            TableRow result = null;
            foreach (TableRow row in UserTable.TableRows)
            {
                if (row.InnerHtml.Contains(displayName))
                {
                    result = row;
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Checks if a user is displayed as being deleted in the user table.
        /// </summary>
        /// <param name="displayName">The users display name as it appears in the user table.</param>
        /// <returns>True if the user is displayed as deleted. 
        /// False if any of the cells in the user row do not have the class "NormalDeleted".</returns>
        public virtual bool CheckUserDisplaysAsDeleted(string displayName)
        {
            bool deleted = true;
            TableCellCollection userRowCells = GetUserRow(displayName).TableCells;
            foreach(TableCell cell in userRowCells)
            {
                if(cell.ClassName != "NormalDeleted")
                {
                    deleted = false;
                }
            }
            return deleted;
        }

        /// <summary>
        /// Adds a user.
        /// Clicks the Add New User link
        /// Fills in the Add User form
        /// Clicks the Add New User link
        /// </summary>
        /// <param name="userName">The users username.</param>
        /// <param name="password">The users password.</param>
        /// <param name="firstName">The users first name.</param>
        /// <param name="lastName">The users last name.</param>
        /// <param name="email">The users email address.</param>
        /// <param name="displayName">The users display name.</param>
        public void AddUser(string userName, string password, string firstName, string lastName, string email, string displayName)
        {

            AddNewUserStartLink.Click();
            UserNameField.Value = userName;
            FirstNameField.Value = firstName;
            LastNameField.Value = lastName;
            DisplayNameField.Value = displayName;
            EmailField.Value = email;
            PasswordField.Value = password;
            ConfirmPasswordField.Value = password;
            AddNewUserLink.ClickNoWait();
        }

        /// <summary>
        /// Registers a user.
        /// Clicks the register link
        /// Fills in the Register User form
        /// Clicks the register button
        /// </summary>
        /// <param name="userName">The users username.</param>
        /// <param name="password">The users password.</param>
        /// <param name="firstName">The users first name.</param>
        /// <param name="lastName">The users last name.</param>
        /// <param name="email">The users email address.</param>
        /// <param name="displayName">The users display name.</param>
        public void RegisterUser(string userName, string password, string firstName, string lastName, string email, string displayName)
        {
            RegisterLink.Click();
            UserNameField.Value = userName;
            FirstNameField.Value = firstName;
            LastNameField.Value = lastName;
            DisplayNameField.Value = displayName;
            EmailField.Value = email;
            PasswordField.Value = password;
            ConfirmPasswordField.Value = password;
            RegisterButton.Click();
        }

        
        #endregion
    }
}