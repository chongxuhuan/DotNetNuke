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
    /// The installation page object.
    /// </summary>
    public class WizardPage : WatiNBase
    {
        private int pageNo;

        #region Constructors

        public WizardPage(WatiNBase watinBase, int pageNo) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { this.pageNo = pageNo; }

        public WizardPage(IE ieInstance, string siteUrl, int pageNo, string dbName) : base(ieInstance, siteUrl, dbName) { this.pageNo = pageNo; }

        #endregion

        #region Public Properties

        public int PageNo { 
            get { return pageNo; }
            set { pageNo = value; }
        }

        #region Links
        public Link NextLink 
        { 
            get 
            {
                //string id;
                //if (PageNo == 0)
                //    id = "wizInstall_StartNavigationTemplateContainerID_StartNextButton";
                //else
                //    id = "wizInstall_StepNavigationTemplateContainerID_StepNextButton";
                //return IEInstance.Link(Find.ById(id)); 
                return IEInstance.Link(Find.ById(new Regex("wizInstall_(Step|Start)NavigationTemplateContainerID_(Step|Start)NextButton")));
            } 
        }
        public Link FinishedLink
        {
            get { return IEInstance.Link(Find.ById("wizInstall_FinishNavigationTemplateContainerID_FinishLinkButton")); }
        }
        #endregion

        #region TextFields
        public TextField ServerNameField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_txtServer")); }
        }
        public TextField DataBaseNameField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_txtDatabase")); }
        }
        public TextField DataBaseUserNameField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_txtUserId")); }
        }
        public TextField DataBaseUserPasswordField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_txtPassword")); }
        }
        public TextField DatabaseScriptResults
        {
            get { return IEInstance.TextField(Find.ById("txtFeedback")); }
        }
        public TextField HostUserNameField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrHost_txtUserName")); }
        }
        public TextField HostPasswordField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrHost_txtPassword")); }
        }
        public TextField HostPasswordConfimationField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrHost_txtConfirm")); }
        }
        public TextField HostEmailField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrHost_txtEmail")); }
        }
        public TextField AdminUserNameField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrAdmin_txtUserName")); }
        }
        public TextField AdminPasswordField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrAdmin_txtPassword")); }
        }
        public TextField AdminPasswordConfimationField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrAdmin_txtConfirm")); }
        }
        public TextField AdminEmailField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_usrAdmin_txtEmail")); }
        }
        public TextField ObjectQualifierTextField
        {
            get { return IEInstance.TextField(Find.ById("wizInstall_txtqualifier")); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The div containing the progress of the database installation.
        /// The content in this div can be used to decide whether the database installation has completed or not
        /// </summary>
        public Div DatabaseInstallProgressDiv
        {
            get { return IEInstance.Div(Find.ById("Progress")); }
        }
        #endregion

        #region RadioButtons
        public RadioButton SQLServerRadioButton
        {
            get { return IEInstance.RadioButtons[1]; }
        }
        #endregion

        #region CheckBoxes
        public CheckBox DBIntegratedCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById("wizInstall_chkIntegrated")); }
        }
        #endregion

        #region Spans
        public Span PageTitle
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("Title"))); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Selects all the checkboxes on the current page.
        /// </summary>
        public void SelectAllCheckBoxes()
        {
            WatiNUtil.SelectAllCheckBoxes(IEInstance);
        }

        /// <summary>
        /// Selects the module checkbox.
        /// </summary>
        /// <param name="moduleName">The name of the module.</param>
        public void SelectOptionalModule(string moduleName)
        {
            if(!string.IsNullOrEmpty(moduleName))
                WatiNUtil.SelectCheckBoxByName(IEInstance, moduleName);
        }
        
        /// <summary>
        /// Select the install method radio button.
        /// </summary>
        /// <param name="installMethod">The install method. "Auto", "Custom" or "Typical"</param>
        public void SelectInstallMethod(string installMethod)
        {
            if (!string.IsNullOrEmpty(installMethod))
                WatiNUtil.SelectRadioButtonByName(IEInstance, "wizInstall$rblInstall", installMethod);
        }

        /// <summary>
        /// Select the database type radio button.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        public void SelectDatabaseType(string databaseType)
        {
            if (!string.IsNullOrEmpty(databaseType))
                WatiNUtil.SelectRadioButtonByName(IEInstance, "wizInstall$rblDatabases", databaseType);
        }

        #endregion
    }
}
