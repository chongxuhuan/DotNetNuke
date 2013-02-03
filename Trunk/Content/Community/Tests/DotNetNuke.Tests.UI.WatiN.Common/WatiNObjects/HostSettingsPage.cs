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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The host settings page object.
    /// </summary>
    public class HostSettingsPage : WatiNBase
    {
        #region Constructors

        public HostSettingsPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public HostSettingsPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Images
        [Obsolete("Element no longer exists in 6.X")]
        public Image ExpandSMTPButton
        {
            get { return IEInstance.Image(Find.ById(s => s.EndsWith("HostSettings_dshSMTP_imgIcon"))); }
        }
        [Obsolete("Element no longer exists in 6.X")]
        public Image OtherSettingsExpandButton
        {
            get { return IEInstance.Image(Find.ById(s => s.EndsWith("HostSettings_dshOther_imgIcon"))); }
        }
        [Obsolete("Element no longer exists in 6.X")]
        public Image PaymentOptionsExpandButton
        {
            get { return IEInstance.Image(Find.ById(s => s.EndsWith("HostSettings_dshPayment_imgIcon"))); }
        }
        #endregion

        #region TextFields
        public TextField AllowableFileExtensionsField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtFileExtensions"))); }
        }
        public TextField AutoUnlockAccountsTimeField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtAutoAccountUnlock"))); }
        }
        public TextField DemoPeriodField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtDemoPeriod"))); }
        }
        public TextField HelpUrlField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtHelpURL"))); }
        }
        public TextField HostEmailTextField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtHostEmail"))); }
        }
        public TextField HostingFeeField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtHostFee"))); }
        }
        public TextField HostingSpaceField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtHostSpace"))); }
        }
        public TextField HostTitleTextField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtHostTitle"))); }
        }
        public TextField HostURLTextField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtHostURL"))); }
        }
        public TextField PageQuotaField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtPageQuota"))); }
        }
        public TextField ProcessorPasswordField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtPassword"))); }
        }
        public TextField ProcessorUserIdField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtUserId"))); }
        }
        public TextField SiteLogHistoryField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtSiteLogHistory"))); }
        }
        public TextField SMTPServerField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtSMTPServer"))); }
        }         
        public TextField UserQuotaField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("HostSettings_txtUserQuota"))); }
        }
        #endregion
        
        #region RadioButtons
        public RadioButton SMTPAnonymousAuthenticationRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("HostSettings_optSMTPAuthentication_0"))); }
        }
        public RadioButton SMTPBasicAuthenticationRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("HostSettings_optSMTPAuthentication_1"))); }
        }
        public RadioButton SMTPNTLMAuthenticationRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("HostSettings_optSMTPAuthentication_2"))); }
        }
        #endregion

        #region CheckBoxes
        public CheckBox AnonymousDemoSignupCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("HostSettings_chkDemoSignup"))); }
        }
        public CheckBox AllowContentLocalizationCheckbox
        {
            get { return ContentPaneDiv.CheckBox(Find.ById(s => s.EndsWith("HostSettings_chkEnableContentLocalization"))); }
        }
        public CheckBox ShowCopyrightCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("HostSettings_chkCopyright"))); }
        }
        public CheckBox EnableModuleOnlineHelpCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("HostSettings_chkEnableHelp"))); }
        }
        public CheckBox EnableRememberMeCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("HostSettings_chkRemember"))); }
        }
        public CheckBox EnableUsersOnlineCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("HostSettings_chkUsersOnline"))); }
        }
        public CheckBox SMTPEnableSSLCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("HostSettings_chkSMTPEnableSSL"))); }
        }        
        #endregion

        #region Links
        public Link GoToPaymentProcessorLink
        {
            get { return IEInstance.Link(Find.ByTitle("Go To Payment Processor WebSite")); }
        }
        public Link RestartApplicationLink
        {
            get { return ContentPaneDiv.Link(Find.ByTitle("Restart Application")); }
        }
        public Link SMTPTestLink
        {
            get { return IEInstance.Link(Find.ByTitle("Test SMTP Settings")); }
        }
        public new Link UpdateLink
        {
            get { return IEInstance.Div(Find.ById("dnnHostSettings")).Link(Find.ByTitle("Update")); }
        }
        public Link AdvancedSettingsTab
        {
            get { return IEInstance.Link(Find.ByText("Advanced Settings")); }
        }
        public Link SMTPSettingsSectionLink
        {
            get { return IEInstance.Link(Find.ByText("SMTP Server Settings")); }
        }
        public Link OtherSettingsTab
        {
            get { return IEInstance.Link(Find.ByText("Other Settings")); }
        }
        #endregion

        #region SelectLists
        public SelectList ControlPanelStyleSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_cboControlPanel"))); }
        }
        public SelectList EditContainerSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_editContainerCombo"))); }
        }
        public SelectList EditSkinSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_editSkinCombo"))); }
        }
        public SelectList HostContainerSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_hostContainerCombo"))); }
        }
        public SelectList HostPortalSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_hostPortalsCombo"))); }
        }
        public SelectList HostSkinSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_hostSkinCombo"))); }
        }
        public SelectList HostingCurrencySelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_currencyCombo"))); }
        }
        public SelectList PaymentOptionsSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_processorCombo"))); }
        }
        public SelectList SchedularModeSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("HostSettings_cboSchedulerMode"))); }
        }
        #endregion

        #region Tables
        [Obsolete("Element no longer exists in 6.X")]
        public Table OtherSettingsTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("HostSettings_tblOther"))); }
        }
        #endregion


        #endregion

        #region Public Methods
        /// <summary>
        /// Enters a valid email address for the host, then sets the SMTP server and clicks the SMTP test button.
        /// All other SMTP settings will be set to the default.
        /// </summary>
        /// <param name="hostEmail">A valid email address for the host.</param>
        /// <param name="smtpServer">The SMTP server to test.</param>
        public void SetAndTestSMTPSettings(string hostEmail, string smtpServer)
        {
            HostEmailTextField.Value = hostEmail;
            //ExpandSMTPButton.Click();
            AdvancedSettingsTab.Click();
            SMTPServerField.Value = smtpServer;
            UpdateLink.Click();
            System.Threading.Thread.Sleep(2000);
            AdvancedSettingsTab.Click();
            //ExpandSMTPButton.Click();
            SMTPTestLink.Click();
        }

        /// <summary>
        /// Enters a valid email address for the host and then sets the schedular mode to Timer Method.
        /// </summary>
        /// <param name="hostEmail">A valid email address for the host.</param>
        public void SetSchedularToTimerMethod(string hostEmail)
        {
            HostEmailTextField.Value = "test@dnn.com";
            //OtherSettingsExpandButton.Click();
            AdvancedSettingsTab.Click();
            SchedularModeSelectList.Select("Timer Method");
            UpdateLink.Click();
        }

        /// <summary>
        /// Enters a valid email address for the host and then sets the SMTP server.
        /// All other SMTP settings will be set to the default.
        /// </summary>
        /// <param name="hostEmail">A valid email address for the host.</param>
        /// <param name="smtpServer">The SMTP server to test.</param>
        public void SetSMTPSettings(string hostEmail, string smtpServer)
        {
            HostEmailTextField.Value = hostEmail;
            //ExpandSMTPButton.ClickNoWait();
            AdvancedSettingsTab.Click();
            SMTPSettingsSectionLink.Click();
            SMTPServerField.Value = smtpServer;
            UpdateLink.Click();
            System.Threading.Thread.Sleep(2000);
        }
        
        /// <summary>
        /// Updates the control panel style.
        /// </summary>
        /// <param name="style">The style to be used for the control panel.</param>
        public void UpdateControlPanelStyle(string style)
        {
            OtherSettingsTab.Click();
            ControlPanelStyleSelectList.Select(style);
            UpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(2000);
        }

        /// <summary>
        /// Updates the control panel style to Classic.
        /// </summary>
        public void UpdateControlPanelToClassic()
        {
            ControlPanelStyleSelectList.Select("CLASSIC");
            UpdateLink.Click();
            System.Threading.Thread.Sleep(2000);
           
        }

        #endregion
    }
}
