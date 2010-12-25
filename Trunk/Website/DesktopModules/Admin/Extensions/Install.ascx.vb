'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2010
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

Imports System.Collections.Generic
Imports System.IO

Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Services.Installer.Installers

Namespace DotNetNuke.Modules.Admin.Extensions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : Install
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies the functionality to Install Extensions(packages) to the Portal
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]   07/26/2007    Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Install
        Inherits ModuleUserControlBase

#Region "Members"

        Private _Installer As Installer
        Private _Package As PackageInfo
        Private _PackageType As PackageType

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the FileName for the Uploade file
        ''' </summary>
        ''' <history>
        '''     [cnurse]   01/20/2009    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property FileName() As String
            Get
                Return CStr(ViewState("FileName"))
            End Get
            Set(ByVal value As String)
                ViewState("FileName") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Installer
        ''' </summary>
        ''' <history>
        '''     [cnurse]   08/13/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property Installer() As Installer
            Get
                Return _Installer
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Path to the Manifest File
        ''' </summary>
        ''' <history>
        '''     [cnurse]   08/13/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property ManifestFile() As String
            Get
                Return CStr(ViewState("ManifestFile"))
            End Get
            Set(ByVal value As String)
                ViewState("ManifestFile") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Package
        ''' </summary>
        ''' <history>
        '''     [cnurse]   08/13/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property Package() As PackageInfo
            Get
                Return _Package
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Package Type
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PackageType() As PackageType
            Get
                If _PackageType Is Nothing Then
                    Dim pType As String = Null.NullString
                    If Not String.IsNullOrEmpty(Request.QueryString("ptype")) Then
                        pType = Request.QueryString("ptype")
                    End If
                    _PackageType = PackageController.GetPackageType(pType)
                End If

                Return _PackageType
            End Get
        End Property

        Protected ReadOnly Property InstallPortalId() As Integer
            Get
                Dim _PortalId As Integer = ModuleContext.PortalId
                If ModuleContext.IsHostMenu Then
                    _PortalId = Null.NullInteger
                End If
                Return _PortalId
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Return Url
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ReturnURL() As String
            Get
                Dim _ReturnUrl As String = Server.UrlDecode(Request.Params("returnUrl"))

                If String.IsNullOrEmpty(_ReturnUrl) Then
                    Dim TabID As Integer = ModuleContext.PortalSettings.HomeTabId

                    If Not Request.Params("rtab") Is Nothing Then
                        TabID = Integer.Parse(Request.Params("rtab"))
                    End If

                    _ReturnUrl = NavigateURL(TabID)
                End If
                Return _ReturnUrl
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Temporary Installation Folder
        ''' </summary>
        ''' <history>
        '''     [cnurse]   08/13/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property TempInstallFolder() As String
            Get
                Return CStr(ViewState("TempInstallFolder"))
            End Get
            Set(ByVal value As String)
                ViewState("TempInstallFolder") = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine binds the package to the Property Editor
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindPackage()
            CreateInstaller()

            If Installer.IsValid Then
                If _Installer.Packages.Count > 0 Then
                    _Package = _Installer.Packages(0).Package
                End If

                'Bind Package Info
                ctlPackage.EditMode = UI.WebControls.PropertyEditorMode.View
                ctlPackage.DataSource = _Package
                ctlPackage.DataBind()

                'Bind License Info
                ctlLicense.EditMode = UI.WebControls.PropertyEditorMode.View
                ctlLicense.DataSource = _Package
                ctlLicense.DataBind()

                'Bind ReleaseNotes Info
                ctlReleaseNotes.EditMode = UI.WebControls.PropertyEditorMode.View
                ctlReleaseNotes.DataSource = _Package
                ctlReleaseNotes.DataBind()
            Else
                'Error reading Manifest
                Select Case wizInstall.ActiveStepIndex
                    Case 0
                        lblLoadMessage.Text = Localization.GetString("InstallError", LocalResourceFile)
                        phLoadLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable)
                    Case 3
                        lblAcceptMessage.Text = Localization.GetString("InstallError", LocalResourceFile)
                        phAcceptLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable)
                End Select
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine checks the Access Security
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CheckSecurity()
            If Not ModuleContext.PortalSettings.UserInfo.IsSuperUser Then
                Response.Redirect(NavigateURL("Access Denied"), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine creates the Installer
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CreateInstaller()
            CheckSecurity()
            _Installer = New Installer(TempInstallFolder, ManifestFile, Request.MapPath("."), False)

            'The Installer is created automatically with a SecurityAccessLevel of Host
            'Check if the User has lowere Security and update as neccessary
            If Not ModuleContext.PortalSettings.UserInfo.IsSuperUser Then
                If ModuleContext.PortalSettings.UserInfo.IsInRole(ModuleContext.PortalSettings.AdministratorRoleName) Then
                    'Admin User
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.Admin
                ElseIf ModulePermissionController.CanAdminModule(ModuleContext.Configuration) Then
                    'Has Edit rights
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.Edit
                ElseIf ModulePermissionController.CanViewModule(ModuleContext.Configuration) Then
                    'Has View rights
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.View
                Else
                    Installer.InstallerInfo.SecurityAccessLevel = SecurityAccessLevel.Anonymous
                End If
            End If

            Installer.InstallerInfo.PortalID = InstallPortalId

            'Read the manifest
            If Installer.InstallerInfo.ManifestFile IsNot Nothing Then
                Installer.ReadManifest(True)
            End If
        End Sub

        Private Sub CreateManifest()
            ManifestFile = Path.Combine(TempInstallFolder, Path.GetFileNameWithoutExtension(FileName) + ".dnn")
            Dim manifestWriter As New StreamWriter(ManifestFile)
            manifestWriter.Write(LegacyUtil.CreateSkinManifest(FileName, rblLegacySkin.SelectedValue, TempInstallFolder))
            manifestWriter.Close()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' This routine installs the uploaded package
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InstallPackage(ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs)
            CreateInstaller()

            If Installer.IsValid Then
                'Reset Log
                Installer.InstallerInfo.Log.Logs.Clear()

                'Set the IgnnoreWhiteList flag
                Installer.InstallerInfo.IgnoreWhiteList = chkIgnoreWhiteList.Checked

                'Set the Repair flag
                Installer.InstallerInfo.RepairInstall = chkRepairInstall.Checked

                'Install
                Installer.Install()

                If Not Installer.IsValid Then
                    lblInstallMessage.Text = Localization.GetString("InstallError", LocalResourceFile)
                End If

                phInstallLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable)
            Else
                'Error reading Manifest
                Select Case e.CurrentStepIndex
                    Case 3
                        lblAcceptMessage.Text = Localization.GetString("InstallError", LocalResourceFile)
                        phAcceptLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable)
                    Case 4
                        lblInstallMessage.Text = Localization.GetString("InstallError", LocalResourceFile)
                        phInstallLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable)
                End Select
                e.Cancel = True
            End If
        End Sub

        Private Function ValidatePackage() As Boolean
            Dim strMessage As String = ""
            Dim isValid As Boolean = Null.NullBoolean

            CreateInstaller()
            If Installer.InstallerInfo.ManifestFile IsNot Nothing Then
                ManifestFile = Path.GetFileName(Installer.InstallerInfo.ManifestFile.TempFileName)
            End If

            If String.IsNullOrEmpty(ManifestFile) Then
                If rblLegacySkin.SelectedValue <> "None" Then
                    'We need to create a manifest file so the installer can continue to run
                    CreateManifest()

                    'Revalidate Package
                    isValid = ValidatePackage()
                Else
                    lblWarningMessage.Visible = True
                    pnlRepair.Visible = False
                    pnlWhitelist.Visible = False
                    pnlLegacy.Visible = True
                    lblWarningMessage.Text = Localization.GetString("NoManifest", LocalResourceFile)
                End If
            ElseIf Installer Is Nothing Then
                lblWarningMessage.Visible = True
                pnlRepair.Visible = False
                pnlWhitelist.Visible = False
                pnlLegacy.Visible = False
                lblWarningMessage.Text = Localization.GetString("ZipCriticalError", LocalResourceFile)
            ElseIf Not Installer.IsValid Then
                lblWarningMessage.Visible = True
                pnlRepair.Visible = False
                pnlWhitelist.Visible = False
                pnlLegacy.Visible = False
                lblWarningMessage.Text = Localization.GetString("ZipError", LocalResourceFile)

                'Error parsing zip
                phLoadLogs.Controls.Add(Installer.InstallerInfo.Log.GetLogsTable)
            ElseIf Not String.IsNullOrEmpty(Installer.LegacyError) Then
                lblWarningMessage.Visible = True
                pnlRepair.Visible = False
                pnlWhitelist.Visible = False
                pnlLegacy.Visible = False
                lblWarningMessage.Text = Localization.GetString(Installer.LegacyError, Me.LocalResourceFile)
            ElseIf Not Installer.InstallerInfo.HasValidFiles AndAlso Not chkIgnoreWhiteList.Checked Then
                lblWarningMessage.Visible = True
                pnlRepair.Visible = False
                pnlWhitelist.Visible = True
                pnlLegacy.Visible = False
                lblWarningMessage.Text = String.Format(Localization.GetString("InvalidFiles", Me.LocalResourceFile), Installer.InstallerInfo.InvalidFileExtensions)
            ElseIf Installer.InstallerInfo.Installed AndAlso Not chkRepairInstall.Checked Then
                lblWarningMessage.Visible = True
                If Installer.InstallerInfo.PortalID = InstallPortalId Then
                    pnlRepair.Visible = True
                End If
                pnlWhitelist.Visible = False
                pnlLegacy.Visible = False
                lblWarningMessage.Text = Localization.GetString("PackageInstalled", Me.LocalResourceFile)
            Else
                isValid = True
            End If

            Return isValid
        End Function

#End Region

#Region "Protected Methods"

        Protected Function GetText(ByVal type As String) As String
            Dim text As String = Null.NullString
            If type = "Title" Then
                text = Localization.GetString(wizInstall.ActiveStep.Title + ".Title", Me.LocalResourceFile)
            ElseIf type = "Help" Then
                text = Localization.GetString(wizInstall.ActiveStep.Title + ".Help", Me.LocalResourceFile)
            End If
            Return text
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Page_Init runs when the page is initialised
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            ' Customise the Control Title
            If (PackageType IsNot Nothing) AndAlso (Not String.IsNullOrEmpty(PackageType.PackageType)) Then
                ModuleContext.Configuration.ModuleTitle = String.Format(Localization.GetString("InstallCustomPackage", LocalResourceFile), PackageType.Description)
            Else
                ModuleContext.Configuration.ModuleTitle = Localization.GetString("InstallPackage", LocalResourceFile)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Page_Load runs when the page loads
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]   07/26/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                CheckSecurity()

                wizInstall.CancelButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Cancel", Me.LocalResourceFile)
                wizInstall.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                wizInstall.StepNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + Localization.GetString("Next", Me.LocalResourceFile)
                wizInstall.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + Localization.GetString("Return", Me.LocalResourceFile)

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub chkIgnoreRestrictedFiles_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkIgnoreWhiteList.CheckedChanged
            If (chkIgnoreWhiteList.Checked) Then
                lblWarningMessage.Text = Localization.GetString("IgnoreRestrictedFilesWarning", Me.LocalResourceFile)
            Else
                lblWarningMessage.Text = ""
            End If
        End Sub

        Protected Sub chkRepairInstall_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkRepairInstall.CheckedChanged
            If (chkRepairInstall.Checked) Then
                lblWarningMessage.Text = Localization.GetString("RepairInstallWarning", Me.LocalResourceFile)
            Else
                lblWarningMessage.Text = Localization.GetString("PackageInstalled", Me.LocalResourceFile)
            End If
        End Sub

        Protected Sub wizInstall_ActiveStepChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles wizInstall.ActiveStepChanged
            CheckSecurity()
            Select Case wizInstall.ActiveStepIndex
                Case 1  'Warning Page
                    If ValidatePackage() Then
                        'Skip Warning Page
                        wizInstall.ActiveStepIndex = 2
                    End If
                Case 2, 3, 4
                    BindPackage()
                Case 5
                    wizInstall.DisplayCancelButton = False
            End Select
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' wizInstall_CancelButtonClick runs when the Cancel Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizInstall_CancelButtonClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles wizInstall.CancelButtonClick
            Try
                If Not String.IsNullOrEmpty(TempInstallFolder) AndAlso Directory.Exists(TempInstallFolder) Then
                    Directory.Delete(TempInstallFolder, True)
                End If
                'Redirect to Definitions page
                Response.Redirect(ReturnURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' wizInstall_FinishButtonClick runs when the Finish Button on the Wizard is clicked.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizInstall_FinishButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizInstall.FinishButtonClick
            Try
                If Installer IsNot Nothing Then
                    Installer.DeleteTempFolder()
                End If

                'Redirect to Definitions page
                Response.Redirect(ReturnURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Wizard_NextButtonClickruns when the next Button is clicked.  It provides
        '''	a mechanism for cancelling the page change if certain conditions aren't met.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	08/13/2007	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizInstall_NextButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizInstall.NextButtonClick

            Select Case e.CurrentStepIndex
                Case 0  'Upload Page
                    Dim postedFile As HttpPostedFile = cmdBrowse.PostedFile
                    Dim strMessage As String = ""

                    FileName = System.IO.Path.GetFileName(postedFile.FileName)
                    Dim strExtension As String = Path.GetExtension(FileName)

                    If String.IsNullOrEmpty(postedFile.FileName) Then
                        strMessage = Localization.GetString("NoFile", LocalResourceFile)
                    ElseIf strExtension.ToLower <> ".zip" Then
                        strMessage += String.Format(Localization.GetString("InvalidExt", LocalResourceFile), FileName)
                    End If

                    If String.IsNullOrEmpty(strMessage) Then
                        _Installer = New Installer(CType(postedFile.InputStream, Stream), Request.MapPath("."), True, False)
                        TempInstallFolder = Installer.TempInstallFolder

                        If Installer.InstallerInfo.ManifestFile IsNot Nothing Then
                            ManifestFile = Path.GetFileName(Installer.InstallerInfo.ManifestFile.TempFileName)
                        End If
                    Else
                        lblLoadMessage.Text = strMessage
                        lblLoadMessage.Visible = True
                        e.Cancel = True
                    End If
                Case 1  'Warning Page
                    e.Cancel = Not ValidatePackage()
                Case 4  'Accept Terms 
                    If chkAcceptLicense.Checked Then
                        InstallPackage(e)
                    Else
                        lblAcceptMessage.Text = Localization.GetString("AcceptTerms", LocalResourceFile)
                        e.Cancel = True

                        'Rebind package
                        BindPackage()
                    End If
            End Select


        End Sub

#End Region

    End Class

End Namespace
