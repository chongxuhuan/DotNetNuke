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
using System.Text.RegularExpressions;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The Host Site Management page object.
    /// </summary>
    public class HostSiteManagementPage : WatiNBase
    {
        #region Constructors

        public HostSiteManagementPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public HostSiteManagementPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Tables
        public Table PortalsTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("Portals_grdPortals"))); }
        }

        #endregion

        #region TextFields
        public TextField SiteAliasField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtPortalAlias")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtPortalAlias")));
            }
        }
        public TextField SiteNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtPortalName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtPortalName")));
            }
        }
        public TextField AdminUserNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtUsername")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtUsername")));
            }
        }

        public TextField AdminFirstNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtFirstName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtFirstName")));
            }
        }

        public TextField AdminLastNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtLastName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtLastName")));
            }
        }

        public TextField AdminEmailField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtEmail")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtEmail")));
            }
        }

        public TextField AdminPasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtPassword")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtPassword")));
            }
        }

        public TextField AdminConfirmPasswordField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("Signup_txtConfirm")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("Signup_txtConfirm")));
            }
        }
        #endregion

        #region SelectLists
        public SelectList PortalTemplateSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("Signup_cboTemplate")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("Signup_cboTemplate")));
            }
        }
        #endregion

        #region Links
        public Link CreateSiteLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Create Site"));
                }
                return IEInstance.Link(Find.ByTitle("Create Site"));
            }
        }
        /// <summary>
        /// The link that appears in the error message span if email hasn't been set up properly for the site.
        /// </summary>
        public Link MessageLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ById(new Regex(@"dnn_ctr\d\d\d_ContentPane"))).Div(Find.ById(s => s.EndsWith("dnnSkinMessage"))).Link(Find.Any);
                }
                return IEInstance.Div(Find.ById(new Regex(@"dnn_ctr\d\d\d_ContentPane"))).Div(Find.ById(s => s.EndsWith("dnnSkinMessage"))).Link(Find.Any);
                
            }
        }
        public Link AddNewSiteLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Add New Site"))); }
        }
        #endregion

        #region RadioButtons
        public RadioButton ChildRadioButton
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("C"));
                }
                return IEInstance.RadioButton(Find.ByValue("C"));
            }
        }
        #endregion

        #region Checkboxes
        public CheckBox UseCurrentUserCheckbox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("Signup_useCurrent")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("Signup_useCurrent")));
            }
        }

        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a child site to the isntallation, and clicks through the message span stating there was an error sending the emails (if it appears).
        /// </summary>
        /// <param name="appName">The name of the app. Used to enter a valid portal alias: (appName)/(portalAlias).</param>
        /// <param name="portalTitle">The title for the site.</param>
        /// <param name="portalAlias">The alias for the site. This will be appended to the app name.</param>
        public void AddChildPortal(string appName, string portalTitle, string portalAlias)
        {
            AddNewSiteLink.Click();
            ChildRadioButton.Click();
            System.Threading.Thread.Sleep(4000);
            SiteAliasField.Value = ("localhost/" + appName + "/" + portalAlias);
            SiteNameField.Value = portalTitle;
            PortalTemplateSelectList.Refresh();
            PortalTemplateSelectList.Select("Default Website");
            System.Threading.Thread.Sleep(3000);
            AdminUserNameField.Value = TestUsers.Admin2.UserName;
            AdminFirstNameField.Value = TestUsers.Admin2.FirstName;
            AdminLastNameField.Value = TestUsers.Admin2.LastName;
            AdminEmailField.Value = TestUsers.Admin2.Email;
            AdminPasswordField.Value = TestUsers.Admin2.Password;
            AdminConfirmPasswordField.Value = TestUsers.Admin2.Password;
            CreateSiteLink.Click();
            System.Threading.Thread.Sleep(6000);
            if (MessageSpan != null && MessageSpan.InnerHtml.Contains("There was an error"))
            {
                MessageLink.Click();
            }
        }

        /// <summary>
        /// Adds a child site to the isntallation. This method will not click through any error message spans if they appear. 
        /// </summary>
        /// <param name="appName">The name of the app. Used to enter a valid portal alias: (appName)/(portalAlias).</param>
        /// <param name="portalTitle">The title for the site.</param>
        /// <param name="portalAlias">The alias for the site. This will be appended to the app name.</param>
        public void AddChildPortalDontClickMsgLink(string appName, string portalTitle, string portalAlias)
        {
            AddNewSiteLink.Click();
            ChildRadioButton.Checked = true;
            System.Threading.Thread.Sleep(3000);
            SiteAliasField.Value = ("localhost/" + appName + "/" + portalAlias);
            SiteNameField.Value = portalTitle;
            PortalTemplateSelectList.Select("Default Website");
            System.Threading.Thread.Sleep(2000);
            AdminUserNameField.Value = TestUsers.Admin2.UserName;
            AdminFirstNameField.Value = TestUsers.Admin2.FirstName;
            AdminLastNameField.Value = TestUsers.Admin2.LastName;
            AdminEmailField.Value = TestUsers.Admin2.Email;
            AdminPasswordField.Value = TestUsers.Admin2.Password;
            AdminConfirmPasswordField.Value = TestUsers.Admin2.Password;
            CreateSiteLink.Click();
        }

        /// <summary>
        /// Finds the table cell containing the site alias for the site.
        /// </summary>
        /// <param name="portalName">The name of the portal.</param>
        /// <returns>The table cell containing the site alias.</returns>
        public TableCell GetPortalAliasCellByPortalName(string portalName)
        {
            //Returns the Portal Title Cell for the specified portal 
            TableCell result = null;
            foreach (TableRow row in PortalsTable.TableRows)
            {
                if (row.TableCells[3].InnerHtml.Contains(portalName))
                {
                    result = row.TableCells[4];
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the site delete button for the site.
        /// </summary>
        /// <param name="portalName">The name of the site.</param>
        /// <returns>The delete image/button for the site.</returns>
        public Image GetPortalDeleteImage(string portalName)
        {
            Image result = null;
            foreach (TableRow row in PortalsTable.TableRows)
            {
                if (row.TableCells[3].InnerHtml.Contains(portalName))
                {
                    result = row.Image(Find.ByTitle("Delete"));
                    break;
                }
                continue;
            }
            return result;
        }
        
        /// <summary>
        /// Finds the table cell containing the site number.
        /// </summary>
        /// <param name="portalName">The name of the site.</param>
        /// <returns>The table cell containing the site number.</returns>
        public TableCell GetPortalNumberCellByPortalName(string portalName)
        {
            //Returns the Portal Number Cell for the specified portal 
            TableCell result = null;
            foreach (TableRow row in PortalsTable.TableRows)
            {
                if (row.TableCells[3].InnerHtml.Contains(portalName))
                {
                    result = row.TableCells[2];
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the table cell containing the site title.
        /// </summary>
        /// <param name="portalName">The name of the site.</param>
        /// <returns>The table cell containing the site title.</returns>
        public TableCell GetPortalTitleCellByPortalName(string portalName)
        {
            //Returns the Portal Title Cell for the specified portal 
            TableCell result = null;
            foreach (TableRow row in PortalsTable.TableRows)
            {
                if (row.TableCells[3].InnerHtml.Contains(portalName))
                {
                    result = row.TableCells[3];
                    break;
                }
                continue;
            }
            return result;
        }
        
        #endregion
    }
}