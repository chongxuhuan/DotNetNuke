'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2009
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
'

Imports System.IO
Imports System.Net

Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Mail
Imports System.Xml
Imports System.Xml.XPath
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.Installer
Imports System.Collections.Generic
Imports DotNetNuke.Application
Imports DotNetNuke.Services.Cache
Imports System.Linq

Namespace DotNetNuke.Modules.Admin.Host

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The HostSettings PortalModuleBase is used to edit the host settings
    ''' for the application.
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class HostSettings
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData fetches the data from the database and updates the controls
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()

            lblProduct.Text = DotNetNukeContext.Current.Application.Description
            lblVersion.Text = FormatVersion(DotNetNukeContext.Current.Application.Version, True)

            trBeta.Visible = (DotNetNukeContext.Current.Application.Status <> ReleaseMode.Stable)
            chkBetaNotice.Checked = Entities.Host.Host.DisplayBetaNotice

            chkUpgrade.Checked = Entities.Host.Host.CheckUpgrade
            hypUpgrade.ImageUrl = Upgrade.Upgrade.UpgradeIndicator(DotNetNukeContext.Current.Application.Version, Request.IsLocal, Request.IsSecureConnection)
            If hypUpgrade.ImageUrl = "" Then
                hypUpgrade.Visible = False
            Else
                hypUpgrade.NavigateUrl = Upgrade.Upgrade.UpgradeRedirect()
            End If
            lblDataProvider.Text = ProviderConfiguration.GetProviderConfiguration("data").DefaultProvider
            lblFramework.Text = Common.Globals.NETFrameworkVersion.ToString(2)

            If Not Upgrade.Upgrade.IsNETFrameworkCurrent Then
                Skin.AddModuleMessage(Me, Localization.GetString("FrameworkDownLevel", Me.LocalResourceFile), Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
            End If

            lblIdentity.Text = System.Security.Principal.WindowsIdentity.GetCurrent.Name
            lblHostName.Text = Dns.GetHostName()
            lblIPAddress.Text = Dns.GetHostEntry(lblHostName.Text).AddressList(0).ToString
            lblPermissions.Text = Framework.SecurityPolicy.Permissions
            If String.IsNullOrEmpty(lblPermissions.Text) Then
                lblPermissions.Text = Localization.GetString("None", Me.LocalResourceFile)
            End If
            If String.IsNullOrEmpty(Common.ApplicationPath) Then
                lblApplicationPath.Text = "/"
            Else
                lblApplicationPath.Text = Common.ApplicationPath
            End If
            lblApplicationMapPath.Text = Common.ApplicationMapPath
            lblServerTime.Text = Now.ToString()
            lblGUID.Text = DotNetNuke.Entities.Host.Host.GUID

            Dim objPortals As New PortalController
            cboHostPortal.DataSource = objPortals.GetPortals
            cboHostPortal.DataBind()
            If Entities.Host.Host.HostPortalID > Null.NullInteger Then
                If Not cboHostPortal.Items.FindByValue(Entities.Host.Host.HostPortalID.ToString()) Is Nothing Then
                    cboHostPortal.Items.FindByValue(Entities.Host.Host.HostPortalID.ToString()).Selected = True
                End If
            End If
            txtHostTitle.Text = DotNetNuke.Entities.Host.Host.HostTitle
            txtHostURL.Text = DotNetNuke.Entities.Host.Host.HostURL
            txtHostEmail.Text = DotNetNuke.Entities.Host.Host.HostEmail
            valHostEmail.ValidationExpression = glbEmailRegEx

            Dim objSkins As New UI.Skins.SkinController

            ctlHostSkin.Width = "252px"
            ctlHostSkin.SkinRoot = SkinController.RootSkin
            ctlHostSkin.SkinSrc = Entities.Host.Host.DefaultPortalSkin

            ctlHostContainer.Width = "252px"
            ctlHostContainer.SkinRoot = SkinController.RootContainer
            ctlHostContainer.SkinSrc = Entities.Host.Host.DefaultPortalContainer

            ctlAdminSkin.Width = "252px"
            ctlAdminSkin.SkinRoot = SkinController.RootSkin
            ctlAdminSkin.SkinSrc = Entities.Host.Host.DefaultAdminSkin

            ctlAdminContainer.Width = "252px"
            ctlAdminContainer.SkinRoot = SkinController.RootContainer
            ctlAdminContainer.SkinSrc = Entities.Host.Host.DefaultAdminContainer

            For Each kvp As KeyValuePair(Of String, ModuleControlInfo) In ModuleControlController.GetModuleControlsByModuleDefinitionID(Null.NullInteger)
                If kvp.Value.ControlType = SecurityAccessLevel.ControlPanel Then
                    cboControlPanel.Items.Add(New ListItem(kvp.Value.ControlKey.Replace("CONTROLPANEL:", ""), kvp.Value.ControlSrc))
                End If
            Next
            If String.IsNullOrEmpty(Entities.Host.Host.ControlPanel) Then
                If Not cboControlPanel.Items.FindByValue(glbDefaultControlPanel) Is Nothing Then
                    cboControlPanel.Items.FindByValue(glbDefaultControlPanel).Selected = True
                End If
            Else
                If Not cboControlPanel.Items.FindByValue(Entities.Host.Host.ControlPanel) Is Nothing Then
                    cboControlPanel.Items.FindByValue(Entities.Host.Host.ControlPanel).Selected = True
                End If
            End If

            Dim ctlList As New Common.Lists.ListController
            Dim colProcessor As Common.Lists.ListEntryInfoCollection = ctlList.GetListEntryInfoCollection("Processor", "")

            cboProcessor.DataSource = colProcessor
            cboProcessor.DataBind()
            cboProcessor.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", ""))

            If Not cboProcessor.Items.FindByText(Entities.Host.Host.PaymentProcessor) Is Nothing Then
                cboProcessor.Items.FindByText(Entities.Host.Host.PaymentProcessor).Selected = True
            End If
            txtUserId.Text = Entities.Host.Host.ProcessorUserId
            txtPassword.Attributes.Add("value", Entities.Host.Host.ProcessorPassword)

            txtHostFee.Text = Entities.Host.Host.HostFee

            Dim colCurrency As Common.Lists.ListEntryInfoCollection = ctlList.GetListEntryInfoCollection("Currency", "")

            cboHostCurrency.DataSource = colCurrency
            cboHostCurrency.DataBind()
            If Not cboHostCurrency.Items.FindByValue(Entities.Host.Host.HostCurrency) Is Nothing Then
                cboHostCurrency.Items.FindByValue(Entities.Host.Host.HostCurrency).Selected = True
            Else
                cboHostCurrency.Items.FindByValue("USD").Selected = True
            End If
            If Not cboSchedulerMode.Items.FindByValue(Entities.Host.Host.SchedulerMode) Is Nothing Then
                cboSchedulerMode.Items.FindByValue(Entities.Host.Host.SchedulerMode).Selected = True
            Else
                cboSchedulerMode.Items.FindByValue("1").Selected = True
            End If

            txtHostSpace.Text = Entities.Host.Host.HostSpace
            txtPageQuota.Text = Entities.Host.Host.PageQuota
            txtUserQuota.Text = Entities.Host.Host.UserQuota
            If Entities.Host.Host.SiteLogStorage = "" Then
                optSiteLogStorage.Items.FindByValue("D").Selected = True
            Else
                optSiteLogStorage.Items.FindByValue(Entities.Host.Host.SiteLogStorage).Selected = True
            End If
            txtSiteLogBuffer.Text = Entities.Host.Host.SiteLogBuffer.ToString()
            txtSiteLogHistory.Text = Entities.Host.Host.SiteLogHistory.ToString()

            cboPageState.Items.FindByValue(Entities.Host.Host.PageStatePersister).Selected = True


            BindModuleCacheProviderList()
            BindPageCacheProviderList()
            If Not cboPerformance.Items.FindByValue(Entities.Host.Host.PerformanceSetting) Is Nothing Then
                cboPerformance.Items.FindByValue(Entities.Host.Host.PerformanceSetting).Selected = True
            Else
                cboPerformance.Items.FindByValue("3").Selected = True
            End If
            cboCacheability.Items.FindByValue(Entities.Host.Host.AuthenticatedCacheability).Selected = True
            If Not cboCompression.Items.FindByValue(Entities.Host.Host.HttpCompressionAlgorithm) Is Nothing Then
                cboCompression.Items.FindByValue(Entities.Host.Host.HttpCompressionAlgorithm).Selected = True
            Else
                cboCompression.Items.FindByValue("0").Selected = True
            End If
            chkWhitespace.Checked = Entities.Host.Host.WhitespaceFilter

            Dim docTypesetting As String = String.Empty
            If DotNetNuke.Common.Globals.DataBaseVersion IsNot Nothing Then
                DotNetNuke.Entities.Host.Host.GetHostSettingsDictionary.TryGetValue("DefaultDocType", docTypesetting)
            End If
            If Not String.IsNullOrEmpty(docTypesetting) Then
                If cboHostDefaultDocType.Items.FindByValue(docTypesetting) IsNot Nothing Then
                    cboHostDefaultDocType.Items.FindByValue(docTypesetting).Selected = True
                Else
                    cboHostDefaultDocType.Items.FindByValue("0").Selected = True
                End If
            Else
                cboHostDefaultDocType.Items.FindByValue("0").Selected = True
            End If

            chkRemember.Checked = Entities.Host.Host.RememberCheckbox

            Dim filePath As String = Common.Globals.ApplicationMapPath + "\Compression.config"
            If File.Exists(filePath) Then
                Dim fileReader As FileStream = New FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim doc As XPathDocument = New XPathDocument(fileReader)
                For Each nav As XPathNavigator In doc.CreateNavigator.Select("compression/excludedPaths/path")
                    txtExcludedPaths.Text += nav.Value.ToLower & vbCrLf
                Next
                txtWhitespaceFilter.Text = doc.CreateNavigator.SelectSingleNode("compression/whitespace").Value
            End If

            txtDemoPeriod.Text = Entities.Host.Host.DemoPeriod.ToString
            chkDemoSignup.Checked = Entities.Host.Host.DemoSignup
            chkCopyright.Checked = Entities.Host.Host.DisplayCopyright
            chkUsersOnline.Checked = Entities.Host.Host.EnableUsersOnline
            chkAJAX.Checked = Entities.Host.Host.EnableAJAX
            txtUsersOnlineTime.Text = Entities.Host.Host.UsersOnlineTimeWindow.ToString
            txtAutoAccountUnlock.Text = Entities.Host.Host.AutoAccountUnlockDuration.ToString
            txtProxyServer.Text = Entities.Host.Host.ProxyServer
            txtProxyPort.Text = Entities.Host.Host.ProxyPort
            txtProxyUsername.Text = Entities.Host.Host.ProxyUsername
            txtProxyPassword.Attributes.Add("value", Entities.Host.Host.ProxyPassword)
            txtWebRequestTimeout.Text = Entities.Host.Host.WebRequestTimeout.ToString
            txtSMTPServer.Text = Entities.Host.Host.SMTPServer
            If Not String.IsNullOrEmpty(Entities.Host.Host.SMTPAuthentication) Then
                optSMTPAuthentication.Items.FindByValue(Entities.Host.Host.SMTPAuthentication).Selected = True
            Else
                optSMTPAuthentication.Items.FindByValue("0").Selected = True
            End If
            ShowHideSMTPCredentials()

            chkSMTPEnableSSL.Checked = Entities.Host.Host.EnableSMTPSSL
            txtSMTPUsername.Text = Entities.Host.Host.SMTPUsername
            txtSMTPPassword.Attributes.Add("value", Entities.Host.Host.SMTPPassword)
            txtFileExtensions.Text = Entities.Host.Host.FileExtensions

            chkUseCustomErrorMessages.Checked = Entities.Host.Host.UseCustomErrorMessages
            chkUseFriendlyUrls.Checked = Entities.Host.Host.UseFriendlyUrls
            rowFriendlyUrls.Visible = chkUseFriendlyUrls.Checked

            chkEnableRequestFilters.Checked = Entities.Host.Host.EnableRequestFilters
            rowRequestFilters.Visible = chkEnableRequestFilters.Checked

            chkLogBuffer.Checked = Entities.Host.Host.EventLogBuffer
            txtHelpURL.Text = Entities.Host.Host.HelpURL
            chkEnableHelp.Checked = Entities.Host.Host.EnableModuleOnLineHelp
            chkAutoSync.Checked = Entities.Host.Host.EnableFileAutoSync
            chkContentLocalization.Checked = Entities.Host.Host.ContentLocalization

            chkWebFarm.Checked = CachingProvider.Instance().IsWebFarm

            'jQuery Support
            jQueryVersion.Text = jQuery.Version
            chkJQueryDebugVersion.Checked = jQuery.UseDebugScript
            chkJQueryUseHosted.Checked = jQuery.UseHostedScript
            txtJQueryHostedUrl.Text = jQuery.HostedUrl

            ViewState.Item("SelectedSchedulerMode") = cboSchedulerMode.SelectedItem.Value
            ViewState.Item("SelectedLogBufferEnabled") = chkLogBuffer.Checked
            ViewState.Item("SelectedUsersOnlineEnabled") = chkUsersOnline.Checked

            ' Get the name of the data provider
            Dim objProviderConfiguration As Framework.Providers.ProviderConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data")

            ' get list of script files
            Dim strProviderPath As String = DataProvider.Instance.GetProviderPath()
            Dim arrScriptFiles As New ArrayList
            Dim strFile As String
            Dim arrFiles As String() = Directory.GetFiles(strProviderPath, "*." & objProviderConfiguration.DefaultProvider)
            For Each strFile In arrFiles
                arrScriptFiles.Add(Path.GetFileNameWithoutExtension(strFile))
            Next
            arrScriptFiles.Sort()

            cboVersion.DataSource = arrScriptFiles
            cboVersion.DataBind()

        End Sub

        Private Sub BindModuleCacheProviderList()
            cboModuleCacheProvider.DataSource = GetFilteredProviders(ModuleCache.ModuleCachingProvider.GetProviderList(), "ModuleCachingProvider")
            cboModuleCacheProvider.DataBind()

            Dim defaultModuleCache As ModuleCache.ModuleCachingProvider = DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of ModuleCache.ModuleCachingProvider)()
            Dim providerKey As String = (From provider In ModuleCache.ModuleCachingProvider.GetProviderList() _
                                        Where provider.Value.Equals(defaultModuleCache) _
                                        Select provider.Key).SingleOrDefault

            If Not String.IsNullOrEmpty(Entities.Host.Host.ModuleCachingMethod) Then
                If cboModuleCacheProvider.Items.FindByValue(Entities.Host.Host.ModuleCachingMethod) IsNot Nothing Then
                    cboModuleCacheProvider.Items.FindByValue(Entities.Host.Host.ModuleCachingMethod).Selected = True
                Else
                    'maybe the specified output cache provider has been removed
                    cboModuleCacheProvider.Items.FindByValue(providerKey).Selected = True
                End If
            Else
                cboModuleCacheProvider.Items.FindByValue(providerKey).Selected = True
            End If
        End Sub

        Private Sub BindPageCacheProviderList()
            cboPageCacheProvider.DataSource = GetFilteredProviders(OutputCache.OutputCachingProvider.GetProviderList(), "OutputCachingProvider")
            cboPageCacheProvider.DataBind()

            Dim defaultPageCache As OutputCache.OutputCachingProvider = DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of OutputCache.OutputCachingProvider)()
            Dim providerKey As String = (From provider In OutputCache.OutputCachingProvider.GetProviderList() _
                                         Where provider.Value.Equals(defaultPageCache) _
                                         Select provider.Key).SingleOrDefault


            If defaultPageCache IsNot Nothing Then
                PageCacheRow.Visible = True

                If Not String.IsNullOrEmpty(Entities.Host.Host.PageCachingMethod) Then
                    If cboPageCacheProvider.Items.FindByValue(Entities.Host.Host.PageCachingMethod) IsNot Nothing Then
                        cboPageCacheProvider.Items.FindByValue(Entities.Host.Host.PageCachingMethod).Selected = True
                    Else
                        'maybe the specified output cache provider has been removed
                        cboPageCacheProvider.Items.FindByValue(providerKey).Selected = True
                    End If
                Else
                    cboPageCacheProvider.Items.FindByValue(providerKey).Selected = True
                End If
            Else
                PageCacheRow.Visible = False
            End If
        End Sub

        Private Function SkinChanged(ByVal SkinRoot As String, ByVal PortalId As Integer, ByVal SkinType As DotNetNuke.UI.Skins.SkinType, ByVal PostedSkinSrc As String) As Boolean
            Dim objSkins As New UI.Skins.SkinController
            Dim objSkinInfo As UI.Skins.SkinInfo = Nothing
            Dim strSkinSrc As String = Null.NullString
            'objSkinInfo = SkinController.GetSkin(SkinRoot, PortalId, SkinType.Admin)
            If Not objSkinInfo Is Nothing Then strSkinSrc = objSkinInfo.SkinSrc
            If strSkinSrc Is Nothing Then strSkinSrc = ""
            Return strSkinSrc <> PostedSkinSrc
        End Function

        Private Sub ShowHideSMTPCredentials()
            If optSMTPAuthentication.SelectedValue = "1" Then
                trSMTPPassword.Visible = True
                trSMTPUserName.Visible = True
            Else
                trSMTPPassword.Visible = False
                trSMTPUserName.Visible = False
            End If
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        '''     [VMasanas]  9/28/2004   Changed redirect to Access Denied
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ' Verify that the current user has access to access this page
                If Not UserInfo.IsSuperUser Then
                    Response.Redirect(NavigateURL("Access Denied"), True)
                End If

                ' If this is the first visit to the page, populate the site data
                If Page.IsPostBack = False Then
                    BindData()
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' chkUseFriendlyUrls_CheckedChanged runs when the use friendly urls checkbox's
        ''' value is changed.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	07/06/2006 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub chkUseFriendlyUrls_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkUseFriendlyUrls.CheckedChanged
            rowFriendlyUrls.Visible = chkUseFriendlyUrls.Checked
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' chkEnableRequestFilters_CheckedChanged runs when the use friendly urls checkbox's
        ''' value is changed.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	07/06/2006 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub chkEnableRequestFilters_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkEnableRequestFilters.CheckedChanged
            rowRequestFilters.Visible = chkEnableRequestFilters.Checked
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdEmail_Click runs when the test email button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdEmail_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEmail.Click
            Try
                If txtHostEmail.Text <> "" Then
                    txtSMTPPassword.Attributes.Add("value", Entities.Host.Host.SMTPPassword)

                    Dim strMessage As String = Mail.SendMail(txtHostEmail.Text, txtHostEmail.Text, "", "", MailPriority.Normal, _
                        Services.Localization.Localization.GetSystemMessage(PortalSettings, "EMAIL_SMTP_TEST_SUBJECT"), MailFormat.Text, _
                        System.Text.Encoding.UTF8, "", "", txtSMTPServer.Text, optSMTPAuthentication.SelectedItem.Value, _
                        txtSMTPUsername.Text, txtSMTPPassword.Text, chkSMTPEnableSSL.Checked)

                    If strMessage <> "" Then
                        lblEmail.Text = "<br>" & String.Format(Services.Localization.Localization.GetString("EmailErrorMessage", Me.LocalResourceFile), strMessage)
                        lblEmail.CssClass = "NormalRed"
                    Else
                        lblEmail.Text = "<br>" & Services.Localization.Localization.GetString("EmailSentMessage", Me.LocalResourceFile)
                        lblEmail.CssClass = "NormalBold"
                    End If
                Else
                    lblEmail.Text = "<br>" & Services.Localization.Localization.GetString("SpecifyHostEmailMessage", Me.LocalResourceFile)
                    lblEmail.CssClass = "NormalRed"
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdProcessor_Click runs when the processor Go button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdProcessor_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdProcessor.Click
            Try
                Response.Redirect(AddHTTP(cboProcessor.SelectedItem.Value), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdEmail_Click runs when the clear cache button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCache_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCache.Click
            ' clear entire cache
            DataCache.ClearCache()

            Response.Redirect(Request.RawUrl, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Upgrade button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdUpdate.Click
            If Page.IsValid Then
                Try
                    Dim objHostSettings As New Entities.Host.HostSettingsController

                    objHostSettings.UpdateHostSetting("CheckUpgrade", Convert.ToString(IIf(chkUpgrade.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("DisplayBetaNotice", Convert.ToString(IIf(chkBetaNotice.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("HostPortalId", cboHostPortal.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("HostTitle", txtHostTitle.Text, False, False)
                    objHostSettings.UpdateHostSetting("HostURL", txtHostURL.Text, False, False)
                    objHostSettings.UpdateHostSetting("HostEmail", txtHostEmail.Text, False, False)
                    objHostSettings.UpdateHostSetting("PaymentProcessor", cboProcessor.SelectedItem.Text, False, False)
                    objHostSettings.UpdateHostSetting("ProcessorUserId", txtUserId.Text, True, False)
                    objHostSettings.UpdateHostSetting("ProcessorPassword", txtPassword.Text, True, False)
                    objHostSettings.UpdateHostSetting("HostFee", txtHostFee.Text, False, False)
                    objHostSettings.UpdateHostSetting("HostCurrency", cboHostCurrency.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("HostSpace", txtHostSpace.Text, False, False)
                    objHostSettings.UpdateHostSetting("PageQuota", txtPageQuota.Text, False, False)
                    objHostSettings.UpdateHostSetting("UserQuota", txtUserQuota.Text, False, False)
                    objHostSettings.UpdateHostSetting("SiteLogStorage", optSiteLogStorage.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("SiteLogBuffer", txtSiteLogBuffer.Text, False, False)
                    objHostSettings.UpdateHostSetting("SiteLogHistory", txtSiteLogHistory.Text, False, False)
                    objHostSettings.UpdateHostSetting("DemoPeriod", txtDemoPeriod.Text, False, False)
                    objHostSettings.UpdateHostSetting("DemoSignup", Convert.ToString(IIf(chkDemoSignup.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("Copyright", Convert.ToString(IIf(chkCopyright.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("DefaultDocType", cboHostDefaultDocType.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("RememberCheckbox", Convert.ToString(IIf(chkRemember.Checked, "Y", "N")), False, False)

                    Dim OriginalUsersOnline As Boolean
                    OriginalUsersOnline = CType(ViewState.Item("SelectedUsersOnlineEnabled"), Boolean)
                    If OriginalUsersOnline <> chkUsersOnline.Checked Then
                        Dim objScheduleItem As Services.Scheduling.ScheduleItem
                        objScheduleItem = Services.Scheduling.SchedulingProvider.Instance.GetSchedule("DotNetNuke.Entities.Users.PurgeUsersOnline, DOTNETNUKE", Null.NullString)
                        If Not objScheduleItem Is Nothing Then
                            If chkUsersOnline.Checked Then
                                If Not objScheduleItem.Enabled Then
                                    objScheduleItem.Enabled = True
                                    Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem)
                                    If CType(cboSchedulerMode.SelectedItem.Value, Services.Scheduling.SchedulerMode) = Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                                        Services.Scheduling.SchedulingProvider.Instance.ReStart("Host Settings")
                                    End If
                                End If
                            Else
                                If objScheduleItem.Enabled Then
                                    objScheduleItem.Enabled = False
                                    Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem)
                                    If CType(cboSchedulerMode.SelectedItem.Value, Services.Scheduling.SchedulerMode) = Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                                        Services.Scheduling.SchedulingProvider.Instance.ReStart("Host Settings")
                                    End If
                                End If
                            End If
                        End If
                    End If
                    objHostSettings.UpdateHostSetting("DisableUsersOnline", Convert.ToString(IIf(chkUsersOnline.Checked, "N", "Y")), False, False)
                    objHostSettings.UpdateHostSetting("EnableAJAX", Convert.ToString(IIf(chkAJAX.Checked, "Y", "N")), False, False)

                    objHostSettings.UpdateHostSetting("AutoAccountUnlockDuration", txtAutoAccountUnlock.Text, False, False)
                    objHostSettings.UpdateHostSetting("UsersOnlineTime", txtUsersOnlineTime.Text, False, False)
                    objHostSettings.UpdateHostSetting("ProxyServer", txtProxyServer.Text, False, False)
                    objHostSettings.UpdateHostSetting("ProxyPort", txtProxyPort.Text, False, False)
                    objHostSettings.UpdateHostSetting("ProxyUsername", txtProxyUsername.Text, True, False)
                    objHostSettings.UpdateHostSetting("ProxyPassword", txtProxyPassword.Text, True, False)
                    objHostSettings.UpdateHostSetting("WebRequestTimeout", txtWebRequestTimeout.Text, False, False)
                    objHostSettings.UpdateHostSetting("SMTPServer", txtSMTPServer.Text, False, False)
                    objHostSettings.UpdateHostSetting("SMTPAuthentication", optSMTPAuthentication.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("SMTPUsername", txtSMTPUsername.Text, True, False)
                    objHostSettings.UpdateHostSetting("SMTPPassword", txtSMTPPassword.Text, True, False)
                    objHostSettings.UpdateHostSetting("SMTPEnableSSL", Convert.ToString(IIf(chkSMTPEnableSSL.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("FileExtensions", txtFileExtensions.Text, False, False)
                    objHostSettings.UpdateHostSetting("UseCustomErrorMessages", Convert.ToString(IIf(chkUseCustomErrorMessages.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("UseFriendlyUrls", Convert.ToString(IIf(chkUseFriendlyUrls.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("EnableRequestFilters", Convert.ToString(IIf(chkEnableRequestFilters.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("ControlPanel", cboControlPanel.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("SchedulerMode", cboSchedulerMode.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("PerformanceSetting", cboPerformance.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("AuthenticatedCacheability", cboCacheability.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("PageStatePersister", cboPageState.SelectedItem.Value)
                    objHostSettings.UpdateHostSetting("ModuleCaching", cboModuleCacheProvider.SelectedItem.Value, False, False)
                    If PageCacheRow.Visible Then
                        objHostSettings.UpdateHostSetting("PageCaching", cboPageCacheProvider.SelectedItem.Value, False, False)
                    End If
                    objHostSettings.UpdateHostSetting("HttpCompression", cboCompression.SelectedItem.Value, False, False)
                    objHostSettings.UpdateHostSetting("WhitespaceFilter", Convert.ToString(IIf(chkWhitespace.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("EnableModuleOnLineHelp", Convert.ToString(IIf(chkEnableHelp.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("EnableFileAutoSync", Convert.ToString(IIf(chkAutoSync.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("ContentLocalization", Convert.ToString(IIf(chkContentLocalization.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("HelpURL", txtHelpURL.Text, False, False)

                    Dim OriginalLogBuffer As Boolean
                    OriginalLogBuffer = CType(ViewState.Item("SelectedLogBufferEnabled"), Boolean)
                    If OriginalLogBuffer <> chkLogBuffer.Checked Then
                        Dim objScheduleItem As Services.Scheduling.ScheduleItem
                        objScheduleItem = Services.Scheduling.SchedulingProvider.Instance.GetSchedule("DotNetNuke.Services.Log.EventLog.PurgeLogBuffer, DOTNETNUKE", Null.NullString)
                        If Not objScheduleItem Is Nothing Then
                            If chkLogBuffer.Checked Then
                                If Not objScheduleItem.Enabled Then
                                    objScheduleItem.Enabled = True
                                    Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem)
                                    If CType(cboSchedulerMode.SelectedItem.Value, Services.Scheduling.SchedulerMode) = Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                                        Services.Scheduling.SchedulingProvider.Instance.ReStart("Host Settings")
                                    End If
                                End If
                            Else
                                If objScheduleItem.Enabled Then
                                    objScheduleItem.Enabled = False
                                    Services.Scheduling.SchedulingProvider.Instance.UpdateSchedule(objScheduleItem)
                                    If CType(cboSchedulerMode.SelectedItem.Value, Services.Scheduling.SchedulerMode) = Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                                        Services.Scheduling.SchedulingProvider.Instance.ReStart("Host Settings")
                                    End If
                                End If
                            End If
                        End If
                    End If
                    objHostSettings.UpdateHostSetting("EventLogBuffer", Convert.ToString(IIf(chkLogBuffer.Checked, "Y", "N")), False, False)

                    objHostSettings.UpdateHostSetting("DefaultPortalSkin", ctlHostSkin.SkinSrc, False, False)
                    objHostSettings.UpdateHostSetting("DefaultAdminSkin", ctlAdminSkin.SkinSrc, False, False)
                    objHostSettings.UpdateHostSetting("DefaultPortalContainer", ctlHostContainer.SkinSrc, False, False)
                    objHostSettings.UpdateHostSetting("DefaultAdminContainer", ctlAdminContainer.SkinSrc, False, False)

                    'jQuery Settings
                    objHostSettings.UpdateHostSetting("jQueryDebug", Convert.ToString(IIf(chkJQueryDebugVersion.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("jQueryHosted", Convert.ToString(IIf(chkJQueryUseHosted.Checked, "Y", "N")), False, False)
                    objHostSettings.UpdateHostSetting("jQueryUrl", txtJQueryHostedUrl.Text, False, False)

                    Dim OriginalSchedulerMode As Services.Scheduling.SchedulerMode
                    OriginalSchedulerMode = CType(ViewState.Item("SelectedSchedulerMode"), Services.Scheduling.SchedulerMode)

                    If CType(cboSchedulerMode.SelectedItem.Value, Services.Scheduling.SchedulerMode) = Services.Scheduling.SchedulerMode.DISABLED Then
                        If OriginalSchedulerMode <> Services.Scheduling.SchedulerMode.DISABLED Then
                            Services.Scheduling.SchedulingProvider.Instance.Halt("Host Settings")
                        End If
                    ElseIf CType(cboSchedulerMode.SelectedItem.Value, Services.Scheduling.SchedulerMode) = Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                        If OriginalSchedulerMode = Services.Scheduling.SchedulerMode.DISABLED Or OriginalSchedulerMode = Services.Scheduling.SchedulerMode.REQUEST_METHOD Then
                            Dim newThread As New Threading.Thread(AddressOf Services.Scheduling.SchedulingProvider.Instance.Start)
                            newThread.IsBackground = True
                            newThread.Start()
                        End If
                    ElseIf CType(cboSchedulerMode.SelectedItem.Value, Services.Scheduling.SchedulerMode) <> Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                        If OriginalSchedulerMode = Services.Scheduling.SchedulerMode.TIMER_METHOD Then
                            Services.Scheduling.SchedulingProvider.Instance.Halt("Host Settings")
                        End If
                    End If

                    ' this is needed in order to fully flush the cache after changing FriendlyURL
                    Response.Redirect(Request.RawUrl, True)
                Catch exc As Exception    'Module failed to load
                    ProcessModuleLoadException(Me, exc)
                Finally
                    ' clear host settings cache
                    DataCache.ClearHostCache(False)
                End Try
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpgrade_Click runs when the Upgrade Log Go button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpgrade_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpgrade.Click
            Try
                ' get path to provider
                Dim strProviderPath As String = DataProvider.Instance.GetProviderPath()

                If File.Exists(strProviderPath & cboVersion.SelectedItem.Text & ".log") Then
                    Dim objStreamReader As StreamReader
                    objStreamReader = File.OpenText(strProviderPath & cboVersion.SelectedItem.Text & ".log")
                    Dim upgradeText As String = objStreamReader.ReadToEnd
                    If upgradeText.Trim = "" Then
                        upgradeText = Localization.GetString("LogEmpty", Me.LocalResourceFile)
                    End If
                    lblUpgrade.Text = Replace(upgradeText, ControlChars.Lf, "<br>")
                    objStreamReader.Close()
                Else
                    lblUpgrade.Text = Localization.GetString("NoLog", Me.LocalResourceFile)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdUploadSkinContainer(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUploadContainer.Click, cmdUploadSkin.Click
            Response.Redirect(Util.InstallURL(ModuleContext.TabId, ""), True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdRestart_Click runs when the Restart button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	9/27/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdRestart_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRestart.Click
            Dim objEv As New EventLogController
            Dim objEventLogInfo As New LogInfo
            objEventLogInfo.BypassBuffering = True
            objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.HOST_ALERT.ToString
            objEventLogInfo.AddProperty("Message", Localization.GetString("UserRestart", Me.LocalResourceFile))
            objEv.AddLog(objEventLogInfo)

            Config.Touch()
            Response.Redirect(NavigateURL(), True)
        End Sub

        Protected Sub cmdUpdateCompression_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdateCompression.Click

            'Create XML Document
            Dim xmlCompression As New XmlDocument

            'Root Element
            Dim nodeRoot As XmlNode = xmlCompression.CreateElement("compression")

            'ExcludedPaths Element
            Dim nodeExcludedPaths As XmlNode = xmlCompression.CreateElement("excludedPaths")
            nodeRoot.AppendChild(nodeExcludedPaths)

            'Add ExcludedPaths
            For Each strItem As String In txtExcludedPaths.Text.Split(vbCrLf)
                If strItem.Trim <> "" Then
                    XmlUtils.AppendElement(xmlCompression, nodeExcludedPaths, "path", strItem.Trim, False)
                End If
            Next

            'Whitespace Element
            XmlUtils.AppendElement(xmlCompression, nodeRoot, "whitespace", txtWhitespaceFilter.Text, False, True)

            'Add Root element to document
            xmlCompression.AppendChild(nodeRoot)

            'Create XML declaration. 
            Dim xmlDeclaration As XmlDeclaration
            xmlDeclaration = xmlCompression.CreateXmlDeclaration("1.0", "utf-8", Nothing)
            xmlCompression.InsertBefore(xmlDeclaration, nodeRoot)

            'Save Compression file
            Dim strFile As String = Common.Globals.ApplicationMapPath + "\Compression.config"
            File.SetAttributes(strFile, FileAttributes.Normal)
            xmlCompression.Save(strFile)

        End Sub

        Protected Sub optSMTPAuthentication_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optSMTPAuthentication.SelectedIndexChanged
            ShowHideSMTPCredentials()
            lblEmail.Text = ""
        End Sub

        ''' <summary>
        ''' GetFilteredProviders takes a Dictionary and a regular expression and returns an IEnumerable list
        ''' where the key is modified by the regular expression.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="providerList">A dictionary object containing the list of objects</param>
        ''' <param name="keyFilter">A regular expression used for filtering the key name</param>
        ''' <returns>An enumeration with the modified keys and the associated values.</returns>
        ''' <remarks></remarks>
        ''' <history>
        '''     [jbrinkman]    11/17/2009  Initial release
        ''' </history>
        Private Function GetFilteredProviders(Of T)(ByVal providerList As Dictionary(Of String, T), ByVal keyFilter As String) As IEnumerable
            Dim providers = From provider In providerList _
                            Let filteredkey = provider.Key.Replace(keyFilter, String.Empty) _
                            Select filteredkey, provider.Key

            Return Providers
        End Function
#End Region

    End Class

End Namespace

