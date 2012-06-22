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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    using global::WatiN.Core;
    /// <summary>
    /// The newsletters page object.
    /// </summary>
    public class NewsletterPage : WatiNBase
    {
        #region Constructors

        public NewsletterPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public NewsletterPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region TextFields
        public TextField AdditionalEmailsField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("txtEmail"))); }
        }
        public TextField FromAddressField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("txtFrom"))); }
        }
        public TextField ReplyToField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("txtReplyTo"))); }
        }
        public TextField SubjectField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("txtSubject"))); }
        }
        public TextField ContentTextBox
        {
            get
            {
                if(!IEInstance.TextField(Find.ById(new Regex(".*Message_txtDesktopHTML", RegexOptions.IgnoreCase))).Exists)
                {
                    IEInstance.RadioButton(Find.ById(s => s.EndsWith("teMessage_OptView_0"))).Click();
                    IEInstance.TextField(Find.ById(new Regex(".*Message_txtDesktopHTML", RegexOptions.IgnoreCase))).WaitUntilExists();
                }
                return IEInstance.TextField(Find.ById(new Regex(".*Message_txtDesktopHTML", RegexOptions.IgnoreCase)));
            }
        }
        #endregion

        #region Links
        public Link SendButton
        {
            get { return IEInstance.Link(Find.ByTitle("Send Email")); }
        }
        public Link MessageTabLink
        {
            get { return this.ContentPaneDiv.Link(Find.ByText(s => s.Contains("Message"))); }
        }
        public Link AdvancedSettingsTabLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Advanced Settings"))); }
        }
        #endregion
        
        #region CheckBoxes
        public CheckBox ReplaceTokensCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("Newsletter_chkReplaceTokens"))); }
        }
        #endregion

        #region SelectLists
        public SelectList SendMethodSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("Newsletter_cboSendMethod"))); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds and selects a radio button on the page with the value specified.
        /// </summary>
        /// <param name="selectedValue">The value for the radio button.</param>
        public void SelectRadioButtonByName(string selectedValue)
        {
            var radioButtons = IEInstance.RadioButtons;

            foreach (var rb in radioButtons)
            {
                if (selectedValue != rb.GetAttributeValue("value"))
                {
                    continue;
                }

                rb.Checked = true;
                break;
            }
        }

        /// <summary>
        /// Checks the role checkbox on the newsletters page for the role specified.
        /// </summary>
        /// <param name="roleName">The name of the role.</param>
        public void CheckRoleCheckBoxByName(string roleName)
        {
            //Returns the Edit Button for the user specified
            CheckBox roleCheckBox = null;
            TableRowCollection roleRows = IEInstance.Div(Find.ByClass("dnnRolesGrid")).Table(Find.Any).TableRows;
            foreach (TableRow row in roleRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(roleName))
                {
                    roleCheckBox = row.TableCells[1].CheckBox(Find.Any);
                }
                continue;
            }
            roleCheckBox.Checked = true;
        }
        #endregion

    }
}
