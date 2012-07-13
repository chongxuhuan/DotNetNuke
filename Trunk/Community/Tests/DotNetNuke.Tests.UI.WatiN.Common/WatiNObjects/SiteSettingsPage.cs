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
    /// The Site Settings Page object.
    /// </summary>
    public class SiteSettingsPage : WatiNBase
    {
        #region Constructors

        public SiteSettingsPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public SiteSettingsPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Span
        public Span PageTitle 
        { 
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("dnnTITLE_titleLabel"))); } 
        }
        /// <summary>
        /// A span containing elements for setting the body background of the site.
        /// </summary>
        public Span SiteBackgroundSettingsSpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("SiteSettings_ctlBackground"))); }
        }
        /// <summary>
        /// A span containing elements for setting the logo of the site.
        /// </summary>
        public Span SiteLogoSettingsSpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("SiteSettings_ctlLogo"))); }
        }
        /// <summary>
        /// A span containing elements for setting the fav icon of the site.
        /// </summary>
        public Span FavIconSettingsSpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("SiteSettings_ctlFavIcon"))); }
        }
        #endregion

        #region Tables
        public Table PortalAliasTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("SiteSettings_portalAliases_dgPortalAlias"))); }
        }
        /// <summary>
        /// The table containing the Control Panel Mode options
        /// </summary>
        public Table ControlPanelModeTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("SiteSettings_optControlPanelMode"))); }
        }
        #endregion

        #region Links
        public Link AddNewAliasLink 
        { 
            get { return IEInstance.Link(Find.ByTitle("Add New Alias")); } 
        }
        public new Link UpdateLink
        {
            get { return SiteSettingsDiv.Link(Find.ByTitle("Update")); }
        }
        public Link FrenchUpdateLink
        {
            get { return IEInstance.Link(Find.ByTitle(" Mettre à jour ")); }
        }
        public Link GoToPaymentProcessorLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Go To Payment Processor WebSite"))); }
        }
        /// <summary>
        /// The Add Module link from the Premium Modules settings
        /// </summary>
        public Link AddModuleLink
        {
            get { return SiteSettingsDiv.Link(Find.ByTitle("Add Module")); }
        }
        /// <summary>
        /// The Remove Module link from the Premium Modules settings
        /// </summary>
        public Link RemoveModuleLink
        {
            get { return IEInstance.Link(Find.ByTitle("Remove Module")); }
        }
        public Link AdvancedSettingsTab
        {
            get { return IEInstance.Link(Find.ByText("Advanced Settings")); }
        }
        public Link SiteAliasesSectionLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Site Aliases"))); }
        }
        public Link SSLSectionLink
        {
            get { return IEInstance.Link(Find.ByText("SSL Settings")); }
        }
        public Link HostSectionLink
        {
            get { return ContentPaneDiv.Link(Find.ByText("Host Settings")); }
        }
        public Link SecuritySectionLink
        {
            get { return ContentPaneDiv.Link(Find.ByText("Security Settings")); }
        }
        public Link AppearanceSectionLink
        {
            get { return IEInstance.Link(Find.ByText("Appearance")); }
        }
        public Link UsabillitySectionLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Usability Settings"))); }
        }
        public Link PaymentSettingsSectionLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Payment Settings"))); }
        }
        /// <summary>
        /// The upload file link for the Body Background.
        /// </summary>
        public Link UploadBackgroundImgLink
        {
            get { return SiteBackgroundSettingsSpan.Link(Find.ByText(s => s.Contains("Upload File"))); }
        }
        /// <summary>
        /// The upload file link for the Logo.
        /// </summary>
        public Link UploadLogoImgLink
        {
            get { return SiteLogoSettingsSpan.Link(Find.ByText(s => s.Contains("Upload File"))); }
        }
        /// <summary>
        /// The save file link for the Body Background.
        /// </summary>
        public Link SaveFileBackgroundLink
        {
            get { return SiteBackgroundSettingsSpan.Link(Find.ByText(s => s.Contains("Save File"))); }
        }
        /// <summary>
        /// The save file link for the Logo.
        /// </summary>
        public Link SaveFileLogoLink
        {
            get { return SiteLogoSettingsSpan.Link(Find.ByText(s => s.Contains("Save File"))); }
        }
        #endregion

        #region TextFields
        /// <summary>
        /// The site alias text box that appears after clicking the Add New Alias link
        /// </summary>
        public TextField AliasTextBox
        { 
            get { return IEInstance.TextField(Find.ById(new Regex(@".*SiteSettings_portalAliases_dgPortalAlias_txtHTTPAlias.*"))); }
        }
        public TextField PageQuotaField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtPageQuota"))); }
        }
        public TextField UserQuotaField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtUserQuota"))); }
        }
        public TextField ExpiryDateField
        {
            get
            {
                return
                    IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_datepickerExpiryDate_dateInput_text")));
            }
        }
        public TextField HostingFeeField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtHostFee"))); }
        }
        public TextField DiskSpaceField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtHostSpace"))); }
        }
        public TextField SiteLogHistoryField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtSiteLogHistory"))); }
        }
        public TextField SiteTitleField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtPortalName"))); }
        }
        public TextField DescriptionField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtDescription"))); }
        }
        public TextField KeywordsField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtKeyWords"))); }
        }
        public TextField CopyrightField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtFooterText"))); }
        }
        public TextField PaymentProcessorUserIdField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtUserId"))); }
        }
        public TextField PaymentProcessorPasswordField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtPassword"))); }
        }
        public TextField PayPalReturnURLField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtPayPalReturnURL"))); }
        }
        public TextField PayPalCancelURLField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtPayPalCancelURL"))); }
        }
        public TextField SiteMapURLField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtSiteMap"))); }
        }
        public TextField VerificationField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_tctVerification"))); }
        }
        public TextField SSLURLField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtSSLURL"))); }
        }
        public TextField StandardURLField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_txtSTDURL"))); }
        }
        public TextField UsernameSecurityValidationField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SiteSettings_validationRegistrationSettings_ctl01__TextBox"))); }
        }

        #endregion

        #region Images
        /// <summary>
        /// The Save button for the add new site alias field.
        /// </summary>
        public Image PortalAliasSaveButtonPortal 
        {
            get { return IEInstance.Image(Find.ById(new Regex(@".*SiteSettings_portalAliases_dgPortalAlias_lnkSave.*"))); }
        }
       // public Image PortalAliasSaveButtonPortal2
        //{
          //  get { return IEInstance.Table(Find.ById(s => s.EndsWith("SiteSettings_portalAliases_dgPortalAlias"))).TableRows[3].TableCells[3].Image(Find.Any); }
        //}
        #endregion

        #region SelectLists
        [Obsolete("Element no longer exists in 6.X")]
        public SelectList DefaultLocaleSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboDefaultLanguage"))); }
        }
        public SelectList SiteSkinSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_portalSkinCombo"))); }
        }
        public SelectList SiteContainerSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_portalContainerCombo"))); }
        }
        public SelectList EditSkinSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_editSkinCombo"))); }
        }
        public SelectList EditContainerSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_editContainerCombo"))); }
        }
        public SelectList BodyBackgroundFileSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_ctlBackground_File"))); }
        }
        public SelectList BodyBackgroundFolderSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_ctlBackground_Folder"))); }
        }
        public SelectList LogoFileSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_ctlLogo_File"))); }
        }
        public SelectList LogoFolderSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_ctlLogo_Folder"))); }
        }
        public SelectList FavIconFileSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSEttings_ctlFavIcon_File"))); }
        }
        public SelectList FavIconFolderSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSEttings_ctlFavIcon_Folder"))); }
        }
        public SelectList LoginPageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboLoginTabId"))); }
        }
        public SelectList UserProfilePageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboUserTabId"))); }
        }
        public SelectList SplashPageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboSplashTabId"))); }
        }
        public SelectList HomePageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboHomeTabId"))); }
        }
        public SelectList RegistrationPageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboRegisterTabId"))); }
        }
        public SelectList SearchResultsPageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboSearchTabId"))); }
        }
        public SelectList PaymentProcessorSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_processorCombo"))); }
        }
        public SelectList CurrencySelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_currencyCombo"))); }
        }
        public SelectList AvailablePremiumModulesSelectList
        {
            get { return IEInstance.SelectList(Find.ByName(s => s.EndsWith("SiteSettings$ctlDesktopModules_Available"))); }
        }
        public SelectList SelectedPremiumModuleSelectList
        {
            get { return IEInstance.SelectList(Find.ByName(s => s.EndsWith("SiteSettings$ctlDesktopModules_Selected"))); }
        }
        public SelectList TimeZoneSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboTimeZone"))); }
        }
        public SelectList SearchEngineSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboSearchEngine"))); }
        }
        public SelectList AdministratorSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_cboAdministratorId"))); }
        }
        public SelectList DefaultAliasSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("SiteSettings_defaultAliasDropDown"))); }
        }
        #endregion

        #region RadioButton
        public RadioButton CanonicalMappingModeRadioButton
        {
            get { return IEInstance.RadioButton(Find.ByValue("CANONICALURL")); }
        }
        public RadioButton RedirectMappingModeRadioButton
        {
            get { return IEInstance.RadioButton(Find.ByValue("REDIRECT")); }
        }
        public RadioButton NoneMappingModeRadioButton
        {
            get { return IEInstance.RadioButton(Find.ByValue("NONE")); }
        }
        public RadioButton ControlPanelMinimisedRadioButton
        {
            get { return IEInstance.RadioButton(Find.ByValue("MIN")); }
        }
        public RadioButton ControlPanelMaximisedRadioButton
        {
            get { return IEInstance.RadioButton(Find.ByValue("MAX")); }
        }
        public RadioButton ModuleEditorsControlPanelRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optControlPanelSecurity_1"))); }
        }
        public RadioButton NoneRegistrationRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optUserRegistration_0"))); }
        }
        public RadioButton PrivateRegistrationRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optUserRegistration_1"))); }
        }
        public RadioButton PublicRegistrationRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optUserRegistration_2"))); }
        }
        public RadioButton VerifiedRegistrationRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optUserRegistration_3"))); }
        }
        public RadioButton PageEditorsControlPanelRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optControlPanelSecurity_0"))); }
        }
        public RadioButton ControlPanelViewModeRadioButton
        {
            get { return ControlPanelModeTable.RadioButton(Find.ByValue("VIEW")); }
        }
        public RadioButton ControlPanelEditModeRadioButton
        {
            get { return ControlPanelModeTable.RadioButton(Find.ByValue("EDIT")); }
        }
        public RadioButton NoneBannersRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optBanners_0"))); }
        }
        public RadioButton SiteBannersRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optBanners_1"))); }
        }
        public RadioButton HostBannersRadioButton
        {
            get { return IEInstance.RadioButton(Find.ById(s => s.EndsWith("SiteSettings_optBanners_2"))); }
        }
        #endregion

        #region CheckBoxes
        public CheckBox PayPalSandbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("chkPayPalSandboxEnabled"))); }
        }
        public CheckBox EnableInlineEditorCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SiteSettings_chkInlineEditor"))); }
        }
        public CheckBox CreateChildFolderForNewAliasCheckBox
        {
            get
            {
                return PortalAliasTable.CheckBox(Find.ById(new Regex(@"dnn_ctr\d\d\d_SiteSettings_portalAliases_dgPortalAlias_chkChild_\d")));
            }
        }
        public CheckBox EnableSkinWidgetsCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SiteSettings_chkSkinWidgestEnabled"))); }
        }
        public CheckBox EnablePopUpsCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SiteSettings_enablePopUpsCheckBox"))); }
        }
        public CheckBox HideSystemFoldersCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SiteSettings_chkHideSystemFolders"))); }
        }
        public CheckBox SSLEnabledCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SiteSettings_chkSSLEnabled"))); }
        }
        public CheckBox SSLEnforcedCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SiteSettings_chkSSLEnforced"))); }
        }
        #endregion

        #region FileUploads
        public FileUpload BackgroundImageFileUpload
        {
            get { return SiteBackgroundSettingsSpan.FileUpload(Find.ByName(s => s.Contains("$SiteSettings$ctlBackground$"))); }
        }
        public FileUpload LogoImageFileUpload
        {
            get { return SiteLogoSettingsSpan.FileUpload(Find.Any); }
        }
        public FileUpload FavIconFileUpload
        {
            get { return FavIconSettingsSpan.FileUpload(Find.Any); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The outer div containing the site settings module.
        /// </summary>
        public Div SiteSettingsDiv
        {
            get { return IEInstance.Div(Find.ById("dnnSiteSettings")); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the delete button for the portal alias.
        /// </summary>
        /// <param name="portalNumber">The index of the portal alias. To access the first portal alias' delete button enter 0.</param>
        /// <returns>The delete image/button for the portal alias.</returns>
        public Image GetPortalDeleteImage(int portalNumber)
        {
            //Returns the Delete image for the portal specified
            return PortalAliasTable.TableRows[portalNumber].Image(Find.ByTitle("Delete"));
        }

        /// <summary>
        /// Finds the edit button for the portal alias.
        /// </summary>
        /// <param name="portalNumber">The index of the portal alias. To access the first portal alias' edit button enter 0.</param>
        /// <returns>The edit image/button for the portal alias.</returns>
        public Image GetPortalEditButton(int portalNumber)
        {
            //Returns the Edit image for the portal specified
            return PortalAliasTable.TableRows[portalNumber].Image(Find.ByTitle("Edit"));
        }


        #endregion
    }
}
