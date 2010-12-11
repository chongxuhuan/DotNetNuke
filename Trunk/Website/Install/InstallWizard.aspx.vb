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

Imports System.Data.Common
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath

Imports DotNetNuke.Application
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Controllers

Namespace DotNetNuke.Services.Install

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The InstallWizard class provides the Installation Wizard for DotNetNuke
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	01/23/2007 Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class InstallWizard
        Inherits PageBase : Implements DotNetNuke.UI.Utilities.IClientAPICallbackEventHandler

#Region "Private Members"

        Private connectionString As String = Null.NullString
        Private dataProvider As DataProvider = dataProvider.Instance
        Private _DataBaseVersion As System.Version
        Private _localesFile As String = "/Install/App_LocalResources/Locales.xml"

        Private _installTemplate As XmlDocument

#End Region

#Region "Protected Members"

        Protected ReadOnly Property ApplicationVersion() As System.Version
            Get
                Return DotNetNukeContext.Current.Application.Version
            End Get
        End Property

        Protected ReadOnly Property DatabaseVersion() As System.Version
            Get
                If _DataBaseVersion Is Nothing Then
                    _DataBaseVersion = dataProvider.Instance().GetVersion
                End If
                Return _DataBaseVersion
            End Get
        End Property

        Protected ReadOnly Property BaseVersion() As System.Version
            Get
                Return Services.Upgrade.Upgrade.GetInstallVersion(InstallTemplate)
            End Get
        End Property

        Protected ReadOnly Property InstallTemplate() As XmlDocument
            Get
                If _installTemplate Is Nothing Then
                    _installTemplate = New XmlDocument
                    Services.Upgrade.Upgrade.GetInstallTemplate(_installTemplate)
                End If
                Return _installTemplate
            End Get
        End Property

        Protected Shadows LocalResourceFile As String = "~/Install/App_LocalResources/InstallWizard.aspx.resx"

        Protected Property PermissionsValid() As Boolean
            Get
                Dim _Valid As Boolean = False
                If Not ViewState("PermissionsValid") Is Nothing Then
                    _Valid = Convert.ToBoolean(ViewState("PermissionsValid"))
                End If
                Return _Valid
            End Get
            Set(ByVal value As Boolean)
                ViewState("PermissionsValid") = value
            End Set
        End Property

        Protected Property PortalId() As Integer
            Get
                Dim _PortalId As Integer = Null.NullInteger
                If Not ViewState("PortalId") Is Nothing Then
                    _PortalId = Convert.ToInt32(ViewState("PortalId"))
                End If
                Return _PortalId
            End Get
            Set(ByVal value As Integer)
                ViewState("PortalId") = value
            End Set
        End Property

        Protected Property Versions() As String
            Get
                Dim _Versions As String = Null.NullString
                If Not ViewState("Versions") Is Nothing Then
                    _Versions = Convert.ToString(ViewState("Versions"))
                End If
                Return _Versions
            End Get
            Set(ByVal value As String)
                ViewState("Versions") = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindAuthSystems binds the Authentication Systems checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/28/2008 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindAuthSystems()
            BindPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "NoAuthSystems", lblAuthSystemsError)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindConnectionString binds the connection String info
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/23/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindConnectionString()
            Dim connection As String = Config.GetConnectionString()
            Dim connectionParams As String() = connection.Split(";")
            For Each connectionParam As String In connection.Split(";")
                Dim index As Integer = connectionParam.IndexOf("=")
                If index > 0 Then
                    Dim key As String = connectionParam.Substring(0, index)
                    Dim value As String = connectionParam.Substring(index + 1)
                    Select Case key.ToLower
                        Case "server", "data source", "address", "addr", "network address"
                            txtServer.Text = value
                        Case "database", "initial catalog"
                            txtDatabase.Text = value
                        Case "uid", "user id", "user"
                            txtUserId.Text = value
                        Case "pwd", "password"
                            txtPassword.Text = value
                        Case "integrated security"
                            chkIntegrated.Checked = (value.ToLower = "true")
                        Case "attachdbfilename"
                            txtFile.Text = value.Replace("|DataDirectory|", "")
                    End Select
                End If
            Next

            If chkIntegrated.Checked Then
                chkOwner.Checked = True
            End If
            chkOwner.Enabled = Not chkIntegrated.Checked
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the userid for the upgradeConnectionString
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[smehaffie]	07/13/2008 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetUpgradeConnectionStringUserID() As String
            Dim dbUser As String = ""
            Dim connection As String = Config.GetUpgradeConnectionString()
            Dim connectionParams As String()

            'If connection string does not use integrated security, then get user id.
            If connection.ToLower().Contains("user id") Or connection.ToLower().Contains("uid") Or connection.ToLower().Contains("user") Then
                connectionParams = connection.Split(";"c)

                For Each connectionParam As String In connectionParams
                    Dim index As Integer = connectionParam.IndexOf("=")
                    If index > 0 Then
                        Dim key As String = connectionParam.Substring(0, index)
                        Dim value As String = connectionParam.Substring(index + 1)
                        If "user id|uuid|user".Contains(key.Trim.ToLower()) Then
                            dbUser = value.Trim()
                        End If
                    End If
                Next
            End If

            Return dbUser
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindDatabases binds the supported databases
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/23/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindDatabases()

            If (Config.GetDefaultProvider("data").Name = "SqlDataProvider") Then

                Dim connection As String = Config.GetConnectionString()
                If connection.ToLower().Contains("attachdbfilename") Then
                    rblDatabases.Items.FindByValue("SQLFile").Selected = True
                Else
                    rblDatabases.Items.FindByValue("SQLDatabase").Selected = True
                End If
            End If

            If (Config.GetDefaultProvider("data").Name = "OracleDataProvider") Then
                rblDatabases.Items.Add(New ListItem(LocalizeString("Oracle"), "Oracle"))
                rblDatabases.SelectedIndex = 2
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindLanguages binds the languages checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/20/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindLanguages()
            BindPackageItems("Language", lstLanguages, lblNoLanguages, "NoLanguages", lblLanguagesError)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindModules binds the modules checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindModules()
            BindPackageItems("Module", lstModules, lblNoModules, "NoModules", lblModulesError)
        End Sub

        Private Sub BindPackageItems(ByVal packageType As String, ByVal list As CheckBoxList, ByVal noItemsLabel As Label, ByVal noItemsKey As String, ByVal errorLabel As Label)
            Dim arrFiles As String()
            Dim strFile As String

            Dim InstallPath As String = ApplicationMapPath & "\Install\" + packageType
            list.Items.Clear()
            If Directory.Exists(InstallPath) Then
                arrFiles = Directory.GetFiles(InstallPath)
                Dim iFile As Integer = 0
                For Each strFile In arrFiles
                    Dim strResource As String = strFile.Replace(InstallPath + "\", "")
                    If strResource.ToLower.EndsWith(".zip") OrElse strResource.ToLower.EndsWith(".resources") Then
                        Dim packageItem As ListItem = New ListItem()
                        ' *.zip packages are installed by default
                        If strResource.ToLower.EndsWith(".zip") Then
                            packageItem.Selected = True
                            packageItem.Enabled = False
                        Else ' *.resources packages will be optional
                            packageItem.Selected = False
                            packageItem.Enabled = True
                        End If
                        packageItem.Value = strResource
                        strResource = strResource.Replace(".zip", "")
                        strResource = strResource.Replace(".resources", "")
                        strResource = strResource.Replace("_Install", ")")
                        strResource = strResource.Replace("_Source", ")")
                        strResource = strResource.Replace("_", " (")
                        packageItem.Text = strResource

                        list.Items.Add(packageItem)
                    End If
                Next
            End If

            If list.Items.Count > 0 Then
                noItemsLabel.Visible = False
            Else
                noItemsLabel.Visible = True
                noItemsLabel.Text = LocalizeString(noItemsKey)
            End If
            If errorLabel IsNot Nothing Then
                errorLabel.Text = Null.NullString
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindPermissions binds the permissions checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/23/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindPermissions(ByVal test As Boolean)
            PermissionsValid = True

            lstPermissions.Items.Clear()

            Dim verifier As New FileSystemPermissionVerifier(Server.MapPath("~"))

            'FolderCreate
            Dim permissionItem As New ListItem()
            If test Then
                permissionItem.Selected = verifier.VerifyFolderCreate()
                PermissionsValid = PermissionsValid AndAlso permissionItem.Selected
            End If
            permissionItem.Enabled = False
            permissionItem.Text = LocalizeString("FolderCreate")
            lstPermissions.Items.Add(permissionItem)

            'FileCreate
            permissionItem = New ListItem()
            If test Then
                permissionItem.Selected = verifier.VerifyFileCreate()
                PermissionsValid = PermissionsValid AndAlso permissionItem.Selected
            End If
            permissionItem.Enabled = False
            permissionItem.Text = LocalizeString("FileCreate")
            lstPermissions.Items.Add(permissionItem)

            'FileDelete
            permissionItem = New ListItem()
            If test Then
                permissionItem.Selected = verifier.VerifyFileDelete()
                PermissionsValid = PermissionsValid AndAlso permissionItem.Selected
            End If
            permissionItem.Enabled = False
            permissionItem.Text = LocalizeString("FileDelete")
            lstPermissions.Items.Add(permissionItem)

            'FolderDelete
            permissionItem = New ListItem()
            If test Then
                permissionItem.Selected = verifier.VerifyFolderDelete()
                PermissionsValid = PermissionsValid AndAlso permissionItem.Selected
            End If
            permissionItem.Enabled = False
            permissionItem.Text = LocalizeString("FolderDelete")
            lstPermissions.Items.Add(permissionItem)

            If test Then
                If PermissionsValid Then
                    lblPermissionsError.Text = LocalizeString("PermissionsOk")
                Else
                    lblPermissionsError.Text = LocalizeString("PermissionsError").Replace("{0}", ApplicationMapPath)
                End If
            End If


        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindPortal binds the portal information
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindPortal()
            Dim node As XmlNode = InstallTemplate.SelectSingleNode("//dotnetnuke/portals/portal")
            If Not node Is Nothing Then
                Dim adminNode As XmlNode = node.SelectSingleNode("administrator")
                usrAdmin.FirstName = XmlUtils.GetNodeValue(adminNode, "firstname")
                usrAdmin.LastName = XmlUtils.GetNodeValue(adminNode, "lastname")
                usrAdmin.UserName = XmlUtils.GetNodeValue(adminNode, "username")
                usrAdmin.Email = XmlUtils.GetNodeValue(adminNode, "email")
                txtPortalTitle.Text = XmlUtils.GetNodeValue(node, "portalname")

                Dim strTemplate As String = XmlUtils.GetNodeValue(node, "templatefile")
                Dim strFolder As String = Common.Globals.HostMapPath
                If System.IO.Directory.Exists(strFolder) Then
                    cboPortalTemplate.Items.Clear()
                    Dim fileEntries As String() = System.IO.Directory.GetFiles(strFolder, "*.template")

                    For Each strFileName As String In fileEntries
                        If Path.GetFileNameWithoutExtension(strFileName) = "admin" Then
                            'lblMessage.Text = ""
                        Else
                            cboPortalTemplate.Items.Add(Path.GetFileNameWithoutExtension(strFileName))
                        End If
                    Next

                    If cboPortalTemplate.Items.Count = 0 Then
                        'lblMessage.Text = Localization.GetString("PortalMissing", Me.LocalResourceFile)
                    End If

                    If Not cboPortalTemplate.Items.FindByValue(strTemplate.Replace(".template", "")) Is Nothing Then
                        cboPortalTemplate.Items.FindByValue(strTemplate.Replace(".template", "")).Selected = True
                    Else
                        cboPortalTemplate.SelectedIndex = 0
                    End If
                End If
            End If
            lblPortalError.Text = Null.NullString
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindProviders binds the Providers checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	06/24/2008 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindProviders()
            BindPackageItems("Provider", lstProviders, lblNoProviders, "NoProviders", lblProvidersError)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindSkins binds the skins checkbox list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/16/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindSkins()
            BindPackageItems("Skin", lstSkins, lblNoSkins, "NoSkins", lblSkinsError)

            BindPackageItems("Container", lstContainers, lblNoContainers, "NoContainers", Nothing)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindSiperUser binds the superuser information
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/16/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindSuperUser(ByVal customButton As LinkButton)
            Dim superUser As UserInfo = Services.Upgrade.Upgrade.GetSuperUser(InstallTemplate, False)

            If Not superUser Is Nothing Then
                usrHost.FirstName = superUser.FirstName
                usrHost.LastName = superUser.LastName
                usrHost.UserName = superUser.Username
                usrHost.Email = superUser.Email
            End If

            'ShowButton(customButton, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' EnableButton enables/Disables a Navigation Button
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/28/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub EnableButton(ByVal button As LinkButton, ByVal enabled As Boolean)
            If Not button Is Nothing Then
                button.OnClientClick = "return !checkDisabled(this);"
                If enabled Then
                    button.CssClass = "WizardButton"
                Else
                    button.CssClass = "WizardButtonDisabled"
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetInstallerLocales gets an ArrayList of the Locales
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/23/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetInstallerLocales() As ArrayList

            Dim supportedLocales As ArrayList = New ArrayList
            Dim filePath As String = Common.Globals.ApplicationMapPath & _localesFile.Replace("/", "\")

            If File.Exists(filePath) Then
                Dim doc As New XPathDocument(filePath)
                For Each nav As XPathNavigator In doc.CreateNavigator.Select("root/language")
                    If nav.NodeType <> XmlNodeType.Comment Then
                        Dim objLocale As New Locale
                        objLocale.Text = nav.GetAttribute("name", "")
                        objLocale.Code = nav.GetAttribute("key", "")
                        objLocale.Fallback = nav.GetAttribute("fallback", "")

                        supportedLocales.Add(objLocale)
                    End If
                Next
            Else
                Dim objLocale As New Locale
                objLocale.Text = "English"
                objLocale.Code = "en-US"
                objLocale.Fallback = ""
                supportedLocales.Add(objLocale)
            End If

            Return supportedLocales
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetNextScriptVersion gets the next script to Install
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/15/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetNextScriptVersion(ByVal strProviderPath As String, ByVal currentVersion As System.Version) As String
            Dim strNextVersion As String = "Done"

            If currentVersion Is Nothing Then
                strNextVersion = GetBaseDataBaseVersion()
            Else
                Dim strScriptVersion As String = Null.NullString
                Dim arrScripts As ArrayList = Services.Upgrade.Upgrade.GetUpgradeScripts(strProviderPath, currentVersion)

                If arrScripts.Count > 0 Then
                    'First Script is next script
                    strScriptVersion = Path.GetFileNameWithoutExtension(CType(arrScripts(0), String))
                End If

                If Not String.IsNullOrEmpty(strScriptVersion) Then
                    strNextVersion = strScriptVersion
                End If
            End If

            Return strNextVersion
        End Function

        Private Function GetVersion(ByVal scriptFile As String) As System.Version
            Return New System.Version(Path.GetFileNameWithoutExtension(scriptFile))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWizardButton gets a wizard button from the template
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/28/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetWizardButton(ByVal containerID As String, ByVal buttonID As String) As LinkButton
            Dim navContainer As Control = wizInstall.FindControl(containerID)
            Dim button As LinkButton = Nothing
            If Not navContainer Is Nothing Then
                button = TryCast(navContainer.FindControl(buttonID), LinkButton)
            End If
            Return button
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Initialise configures the first Wizard page
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/15/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Initialise()
            If TestDataBaseInstalled() Then
                'running current version, so redirect to site home page
                Response.Redirect("~/Default.aspx", True)
            Else
                If DatabaseVersion > New System.Version(0, 0, 0) Then
                    'Upgrade
                    tblLanguage.Visible = False
                    lblStep0Title.Text = String.Format(LocalizeString("UpgradeTitle"), ApplicationVersion.ToString(3))
                    lblStep0Detail.Text = String.Format(LocalizeString("Upgrade"), Services.Upgrade.Upgrade.GetStringVersion(DatabaseVersion))
                Else
                    'Install
                    UpdateMachineKey()
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallLanguages installs the Optional Languages
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/20/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallAuthSystems() As Boolean
            Return InstallPackageItems("AuthSystem", lstAuthSystems, lblNoAuthSystems, "InstallAuthSystemError")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallDatabase intsalls the base Database scripts
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/14/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallDatabase() As String
            Dim strErrorMessage As String = Null.NullString

            Dim strProviderPath As String = dataProvider.GetProviderPath()
            If Not strProviderPath.StartsWith("ERROR:") Then
                'Install Base Version
                strErrorMessage = Services.Upgrade.Upgrade.InstallDatabase(BaseVersion, strProviderPath, InstallTemplate, False)
            Else
                'provider error
                strErrorMessage = strProviderPath
            End If

            If String.IsNullOrEmpty(strErrorMessage) Then
                'Get Next Version
                strErrorMessage = GetNextScriptVersion(strProviderPath, BaseVersion)
            ElseIf Not strErrorMessage.StartsWith("ERROR:") Then
                strErrorMessage = "ERROR: " + String.Format(LocalizeString("ScriptError"), Services.Upgrade.Upgrade.GetLogFile(strProviderPath, BaseVersion))
            End If

            Return strErrorMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallHost sets up the Host settings
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/16/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallHost() As Boolean
            Dim success As Boolean = False
            Dim strErrorMessage As String = usrHost.Validate()

            If Not String.IsNullOrEmpty(strErrorMessage) Then
                Dim strError As String = LocalizeString(strErrorMessage)
                If strErrorMessage = "PasswordLength" Then
                    strError = String.Format(strError, MembershipProviderConfig.MinPasswordLength)
                End If
                lblHostUserError.Text = String.Format(LocalizeString("HostUserError"), strError)
            Else
                Try
                    'Initialise Host Settings
                    Services.Upgrade.Upgrade.InitialiseHostSettings(InstallTemplate, False)

                    'Create Host User
                    Dim objSuperUserInfo As UserInfo = Services.Upgrade.Upgrade.GetSuperUser(InstallTemplate, False)
                    objSuperUserInfo.FirstName = usrHost.FirstName
                    objSuperUserInfo.LastName = usrHost.LastName
                    objSuperUserInfo.Username = usrHost.UserName
                    objSuperUserInfo.DisplayName = usrHost.FirstName + " " + usrHost.LastName
                    objSuperUserInfo.Membership.Password = usrHost.Password
                    objSuperUserInfo.Email = usrHost.Email
                    UserController.CreateUser(objSuperUserInfo)

                    Services.Upgrade.Upgrade.InstallFiles(InstallTemplate, False)

                    If Not String.IsNullOrEmpty(txtSMTPServer.Text) Then
                        HostController.Instance.Update("SMTPServer", txtSMTPServer.Text)
                        HostController.Instance.Update("SMTPAuthentication", optSMTPAuthentication.SelectedItem.Value)
                        HostController.Instance.Update("SMTPUsername", txtSMTPUsername.Text)
                        HostController.Instance.Update("SMTPPassword", txtSMTPPassword.Text)
                        HostController.Instance.Update("SMTPEnableSSL", Convert.ToString(IIf(chkSMTPEnableSSL.Checked, "Y", "N")))
                    End If

                    'Clear Host Cache
                    DataCache.ClearHostCache(False)

                    success = True
                Catch ex As Exception
                    lblHostUserError.Text = String.Format(LocalizeString("HostUserError"), ex.Message)
                End Try
            End If

            Return success

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallLanguages installs the Optional Languages
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/20/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallLanguages() As Boolean
            Return InstallPackageItems("Language", lstLanguages, lblLanguagesError, "InstallLanguageError")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallModules installs the Optional Modules
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallModules() As Boolean
            Return InstallPackageItems("Module", lstModules, lblModulesError, "InstallModuleError")
        End Function

        Private Function InstallPackageItems(ByVal packageType As String, ByVal list As CheckBoxList, ByVal errorLabel As Label, ByVal errorKey As String) As Boolean
            Dim success As Boolean = False
            Dim strErrorMessage As String = Null.NullString

            'Get current Script time-out
            Dim scriptTimeOut As Integer = Server.ScriptTimeout

            Try
                'Set Script timeout to MAX value
                Server.ScriptTimeout = Integer.MaxValue

                Dim InstallPath As String = ApplicationMapPath & "\Install\" + packageType
                For Each packageItem As ListItem In list.Items
                    If packageItem.Selected Then
                        If (File.Exists(InstallPath + "\" + packageItem.Value)) Then
                            success = Upgrade.Upgrade.InstallPackage(InstallPath + "\" + packageItem.Value, packageType, False)
                            If Not success Then
                                strErrorMessage += String.Format(LocalizeString(errorKey), packageItem.Text)
                            End If
                        End If
                    End If
                Next

                success = String.IsNullOrEmpty(strErrorMessage)
            Catch ex As Exception
                strErrorMessage = ex.StackTrace
            Finally
                'restore Script timeout
                Server.ScriptTimeout = scriptTimeOut
            End Try

            If Not success Then
                errorLabel.Text += strErrorMessage
            End If

            Return success
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallPortal installs the Host Portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallPortal() As Boolean
            Dim success As Boolean = False
            Dim strErrorMessage As String = usrAdmin.Validate()

            If Not String.IsNullOrEmpty(strErrorMessage) Then
                Dim strError As String = LocalizeString(strErrorMessage)
                If strErrorMessage = "PasswordLength" Then
                    strError = String.Format(strError, MembershipProviderConfig.MinPasswordLength)
                End If
                lblPortalError.Text = String.Format(LocalizeString("AdminUserError"), strError)
            Else
                Try
                    Dim objPortalController As New PortalController
                    Dim strServerPath As String = ApplicationMapPath & "\"
                    Dim strPortalAlias As String = GetDomainName(HttpContext.Current.Request, True).Replace("/Install", "")
                    Dim strTemplate As String = cboPortalTemplate.SelectedValue + ".template"

                    'Create Portal
                    PortalId = objPortalController.CreatePortal(txtPortalTitle.Text, usrAdmin.FirstName, usrAdmin.LastName, _
                        usrAdmin.UserName, usrAdmin.Password, usrAdmin.Email, "", "", Common.Globals.HostMapPath, strTemplate, _
                        "", strPortalAlias, strServerPath, "", False)

                    success = (PortalId > Null.NullInteger)
                Catch ex As Exception
                    success = False
                    strErrorMessage = ex.Message
                End Try

                If Not success Then
                    lblPortalError.Text = String.Format(LocalizeString("InstallPortalError"), strErrorMessage)
                End If
            End If

            Return success
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallProviders installs the Optional Providers
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	06/24/2008 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallProviders() As Boolean
            Return InstallPackageItems("Provider", lstProviders, lblProvidersError, "InstallProviderError")
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallSkins installs the Optional Skins
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/19/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallSkins() As Boolean
            Dim skinSuccess As Boolean = InstallPackageItems("Skin", lstSkins, lblSkinsError, "InstallSkinError")
            Dim containerSuccess As Boolean = InstallPackageItems("Container", lstContainers, lblSkinsError, "InstallContainerError")
            Return skinSuccess And containerSuccess
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InstallVersion intsalls the a single version script
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/16/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function InstallVersion(ByVal strVersion As String) As String
            Dim strErrorMessage As String = Null.NullString
            Dim version As System.Version = New System.Version(strVersion)
            Dim strScriptFile As String = Null.NullString
            Dim strProviderPath As String = dataProvider.GetProviderPath()

            If Not strProviderPath.StartsWith("ERROR:") Then
                'Install Version
                strScriptFile = Services.Upgrade.Upgrade.GetScriptFile(strProviderPath, version)
                strErrorMessage += Services.Upgrade.Upgrade.UpgradeVersion(strScriptFile, False)

                Versions += "," + strVersion

                'If version IsNot Nothing Then
                '    strErrorMessage += Services.Upgrade.Upgrade.UpgradeApplication(version, False)

                '    ' delete files which are no longer used
                '    strErrorMessage += Services.Upgrade.Upgrade.DeleteFiles(version, False)

                '    'execute config file updates
                '    strErrorMessage += Services.Upgrade.Upgrade.UpdateConfig(version, False)
                'End If
            Else
                'provider error
                strErrorMessage = strProviderPath
            End If

            If String.IsNullOrEmpty(strErrorMessage) Then
                'Get Next Version
                strErrorMessage = GetNextScriptVersion(strProviderPath, version)
            Else
                strErrorMessage = "ERROR: (see " & Path.GetFileName(strScriptFile).Replace("." & Upgrade.Upgrade.DefaultProvider, ".log") & " for more information)"
            End If

            Return strErrorMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LocalizePage sets up the Localized Text
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/09/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LocalizePage()

            'Main Title
            Me.Title = LocalizeString("Title") + " - " + LocalizeString("Page" + wizInstall.ActiveStepIndex.ToString() + ".Title")

            'Page Titles
            For i As Integer = 0 To wizInstall.WizardSteps.Count - 1
                wizInstall.WizardSteps(i).Title = LocalizeString("Page" + i.ToString() + ".Title")
            Next

            'Wizard Buttons
            wizInstall.StartNextButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + LocalizeString("Next")
            wizInstall.FinishPreviousButtonText = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + LocalizeString("Previous")
            wizInstall.FinishCompleteButtonText = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + LocalizeString("Finished")

            'Page 0 - Introduction
            lblStep0Title.Text = String.Format(LocalizeString("IntroTitle"), FormatVersion(ApplicationVersion))
            lblStep0Detail.Text = LocalizeString("IntroDetail")
            lblChooseInstall.Text = LocalizeString("ChooseInstall")
            lblChooseLanguage.Text = LocalizeString("ChooseLanguage")

            rblInstall.Items(0).Text = LocalizeString("Full")
            rblInstall.Items(1).Text = LocalizeString("Typical")
            rblInstall.Items(2).Text = LocalizeString("Auto")

            'Page 1 - File Permissions
            lblStep1Title.Text = LocalizeString("PermissionsTitle")
            lblStep1Detail.Text = LocalizeString("PermissionsDetail")
            lblPermissions.Text = LocalizeString("Permissions")
            'File Permissions
            BindPermissions(False)

            'Page 2 - Database Configuration
            lblStep2Title.Text = LocalizeString("DatabaseConfigTitle")
            lblStep2Detail.Text = LocalizeString("DatabaseConfigDetail")
            lblChooseDatabase.Text = LocalizeString("ChooseDatabase")
            lblServerHelp.Text = LocalizeString("ServerHelp")
            lblServer.Text = LocalizeString("Server")
            lblFile.Text = LocalizeString("DatabaseFile")
            lblDatabaseFileHelp.Text = LocalizeString("DatabaseFileHelp")
            lblDataBase.Text = LocalizeString("Database")
            lblDatabaseHelp.Text = LocalizeString("DatabaseHelp")
            lblIntegrated.Text = LocalizeString("Integrated")
            lblIntegratedHelp.Text = LocalizeString("IntegratedHelp")
            lblUserId.Text = LocalizeString("UserId")
            lblUserHelp.Text = LocalizeString("UserHelp")
            lblPassword.Text = LocalizeString("Password")
            lblPasswordHelp.Text = LocalizeString("PasswordHelp")
            lblOwner.Text = LocalizeString("Owner")
            lblOwnerHelp.Text = LocalizeString("OwnerHelp")
            lblQualifier.Text = LocalizeString("Qualifier")
            lblQualifierHelp.Text = LocalizeString("QualifierHelp")

            rblDatabases.Items(0).Text = LocalizeString("SQLServerXPress")
            rblDatabases.Items(1).Text = LocalizeString("SQLServer")

            'Page 3 - Database Installation
            lblStep3Title.Text = LocalizeString("DatabaseInstallTitle")
            lblStep3Detail.Text = LocalizeString("DatabaseInstallDetail")

            'Page 4 - Host User
            lblStep4Title.Text = LocalizeString("HostUserTitle")
            lblStep4Detail.Text = LocalizeString("HostUserDetail")
            usrHost.FirstNameLabel = LocalizeString("FirstName")
            usrHost.LastNameLabel = LocalizeString("LastName")
            usrHost.UserNameLabel = LocalizeString("UserName")
            usrHost.PasswordLabel = LocalizeString("Password")
            usrHost.ConfirmLabel = LocalizeString("Confirm")
            usrHost.EmailLabel = LocalizeString("Email")
            lblSMTPSettings.Text = LocalizeString("SMTPSettings")
            lblSMTPSettingsHelp.Text = LocalizeString("SMTPSettingsHelp")
            lblSMTPServer.Text = LocalizeString("SMTPServer")
            lblSMTPAuthentication.Text = LocalizeString("SMTPAuthentication")
            lblSMTPEnableSSL.Text = LocalizeString("SMTPEnableSSL")
            lblSMTPUsername.Text = LocalizeString("SMTPUsername")
            lblSMTPPassword.Text = LocalizeString("SMTPPassword")

            'Page 5 - Modules
            lblStep5Title.Text = LocalizeString("ModulesTitle")
            lblStep5Detail.Text = LocalizeString("ModulesDetail")
            lblModules.Text = LocalizeString("Modules")

            'Page 6 - Skins/Containers
            lblStep6Title.Text = LocalizeString("SkinsTitle")
            lblStep6Detail.Text = LocalizeString("SkinsDetail")
            lblSkins.Text = LocalizeString("Skins")
            lblContainers.Text = LocalizeString("Containers")

            'Page 7 - Languages
            lblStep7Title.Text = LocalizeString("LanguagesTitle")
            lblStep7Detail.Text = LocalizeString("LanguagesDetail")
            lblLanguages.Text = LocalizeString("Languages")

            'Page 8 - Auth Systems
            lblStep8Title.Text = LocalizeString("AuthSystemsTitle")
            lblStep8Detail.Text = LocalizeString("AuthSystemsDetail")
            lblAuthSystems.Text = LocalizeString("AuthSystems")

            'Page 9 - Providers
            lblStep9Title.Text = LocalizeString("ProvidersTitle")
            lblStep9Detail.Text = LocalizeString("ProvidersDetail")
            lblProviders.Text = LocalizeString("Providers")

            'Page 10 - Portal
            lblStep10Title.Text = LocalizeString("PortalTitle")
            lblStep10Detail.Text = LocalizeString("PortalDetail")
            usrAdmin.FirstNameLabel = LocalizeString("FirstName")
            usrAdmin.LastNameLabel = LocalizeString("LastName")
            usrAdmin.UserNameLabel = LocalizeString("UserName")
            usrAdmin.PasswordLabel = LocalizeString("Password")
            usrAdmin.ConfirmLabel = LocalizeString("Confirm")
            usrAdmin.EmailLabel = LocalizeString("Email")
            lblAdminUser.Text = LocalizeString("AdminUser")
            lblPortal.Text = LocalizeString("Portal")
            lblPortalTitle.Text = LocalizeString("PortalTitle")
            lblPortalTemplate.Text = LocalizeString("PortalTemplate")

            'Page 12 - Complete
            lblCompleteTitle.Text = LocalizeString("CompleteTitle")
            lblCompleteDetail.Text = LocalizeString("CompleteDetail")

        End Sub

        Private Sub SetupDatabasePage()
            Dim nextButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton")
            Dim customButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton")
            SetupDatabasePage(customButton, nextButton)
        End Sub

        Private Sub SetupDatabasePage(ByVal customButton As LinkButton, ByVal nextButton As LinkButton)
            If rblDatabases.SelectedIndex > Null.NullInteger Then
                Dim isSQLFile As Boolean = (rblDatabases.SelectedValue = "SQLFile")
                Dim isSQLDb As Boolean = (rblDatabases.SelectedValue = "SQLDatabase")
                Dim isOracle As Boolean = (rblDatabases.SelectedValue = "Oracle")
                tblDatabase.Visible = True
                trFile.Visible = isSQLFile
                trDatabase.Visible = isSQLDb
                trIntegrated.Visible = Not isOracle
                trUser.Visible = Not chkIntegrated.Checked OrElse isOracle
                trPassword.Visible = Not chkIntegrated.Checked OrElse isOracle
                If isSQLDb Then
                    chkOwner.Enabled = True
                Else
                    chkOwner.Enabled = False
                End If
                chkOwner.Checked = (Config.GetDataBaseOwner() = "dbo.")
                txtqualifier.Text = Config.GetObjectQualifer()
            Else
                tblDatabase.Visible = False
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SetupPage updates the WizardPage
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/23/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetupPage()

            Dim nextButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton")
            Dim prevButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "StepPreviousButton")
            Dim customButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton")
            EnableButton(nextButton, True)
            EnableButton(prevButton, True)
            ShowButton(customButton, False)

            Select Case wizInstall.ActiveStepIndex
                Case 0
                    'Page 0 - Welcome
                    lblPermissionsError.Text = ""
                Case 1
                    lblDataBaseError.Text = ""
                Case 2
                    lblPermissionsError.Text = ""

                    '    'Page 2 - Database Configuration
                    SetupDatabasePage(customButton, nextButton)
                Case 3
                    lblDataBaseError.Text = ""
                    lblInstallError.Text = ""
                    lblInstallError.Visible = False
                    EnableButton(nextButton, False)
                    ShowButton(prevButton, False)
                Case 4
                    'Page 4 - SuperUser Configuration
                    BindSuperUser(customButton)
                    ShowButton(prevButton, False)
                Case 5
                    'Page 5 - Modules
                    BindModules()
                    ShowButton(prevButton, False)
                Case 6
                    'Page 6 - Skins/Conatiners
                    BindSkins()
                    ShowButton(prevButton, False)
                Case 7
                    'Page 7 - Languages
                    BindLanguages()
                    ShowButton(prevButton, False)
                Case 8
                    'Page 8 - Auth Systems
                    BindAuthSystems()
                    ShowButton(prevButton, False)
                Case 9
                    'Page 9 - Providers
                    BindProviders()
                    ShowButton(prevButton, False)
                Case 10
                    'Page 10 - Portal
                    BindPortal()
                    ShowButton(prevButton, False)
            End Select

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ShowButton shows/hides a Navigation Button
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/28/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ShowButton(ByVal button As LinkButton, ByVal enabled As Boolean)
            If Not button Is Nothing Then
                button.Visible = enabled
            End If
        End Sub

        Private Sub ShowHideSMTPCredentials()
            If optSMTPAuthentication.SelectedValue = "1" Then
                trSMTPPassword.Visible = True
                trSMTPUserName.Visible = True
            Else
                trSMTPPassword.Visible = False
                trSMTPUserName.Visible = False
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TestDatabaseConnection checks the Database connection properties provided
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/23/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function TestDatabaseConnection() As Boolean
            Dim success As Boolean = False

            If String.IsNullOrEmpty(rblDatabases.SelectedValue) Then
                connectionString = "ERROR:" + LocalizeString("ChooseDbError")
            Else
                Dim isSQLFile As Boolean = (rblDatabases.SelectedValue = "SQLFile")
                Dim builder As DbConnectionStringBuilder = dataProvider.GetConnectionStringBuilder()
                If Not String.IsNullOrEmpty(txtServer.Text) Then
                    builder("Data Source") = txtServer.Text
                End If
                If Not String.IsNullOrEmpty(txtDatabase.Text) And Not isSQLFile Then
                    builder("Initial Catalog") = txtDatabase.Text
                End If
                If String.IsNullOrEmpty(txtDatabase.Text) And Not isSQLFile Then
                    lblDataBaseError.Text = LocalizeString("DbNameError")
                    Return False
                End If
                If Not String.IsNullOrEmpty(txtFile.Text) And isSQLFile Then
                    builder("attachdbfilename") = "|DataDirectory|" + txtFile.Text
                End If
                If chkIntegrated.Checked Then
                    builder("integrated security") = "true"
                End If
                If Not String.IsNullOrEmpty(txtUserId.Text) Then
                    builder("uid") = txtUserId.Text
                End If
                If Not String.IsNullOrEmpty(txtPassword.Text) Then
                    builder("pwd") = txtPassword.Text
                End If
                If isSQLFile Then
                    builder("user instance") = "true"
                End If

                Dim owner As String = txtUserId.Text + "."
                If chkOwner.Checked Then
                    owner = "dbo."
                End If
                connectionString = dataProvider.Instance.TestDatabaseConnection(builder, owner, txtqualifier.Text)
            End If

            If connectionString.StartsWith("ERROR:") Then
                lblDataBaseError.Text = String.Format(LocalizeString("ConnectError"), connectionString.Replace("ERROR:", ""))
            Else
                success = True
                lblDataBaseError.Text = LocalizeString("ConnectSuccess")
            End If
            Return success
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' TestDataBaseInstalled checks whether the Database is the current version
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/16/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function TestDataBaseInstalled() As Boolean
            Dim success As Boolean = True
            If DatabaseVersion Is Nothing OrElse _
             DatabaseVersion.Major <> ApplicationVersion.Major OrElse _
             DatabaseVersion.Minor <> ApplicationVersion.Minor OrElse _
             DatabaseVersion.Build <> ApplicationVersion.Build Then
                success = False
            End If
            If Not success Then
                lblInstallError.Text = LocalizeString("Install.Error")
            End If

            Return success
        End Function

        Private Function TestSMTPSettings() As Boolean

            If Not String.IsNullOrEmpty(usrHost.Email) Then

                Try
                    Dim emailMessage As New System.Net.Mail.MailMessage(usrHost.Email, usrHost.Email, LocalizeString("EmailTestMessageSubject"), String.Empty)

                    Dim smtpClient As New System.Net.Mail.SmtpClient(txtSMTPServer.Text)

                    Dim smtpHostParts As String() = txtSMTPServer.Text.Split(":"c)
                    If smtpHostParts.Length > 1 Then
                        smtpClient.Host = smtpHostParts(0)
                        smtpClient.Port = Convert.ToInt32(smtpHostParts(1))
                    End If


                    Select Case optSMTPAuthentication.SelectedItem.Value
                        Case "", "0" ' anonymous
                        Case "1" ' basic
                            If txtSMTPUsername.Text <> "" And txtSMTPPassword.Text <> "" Then
                                smtpClient.UseDefaultCredentials = False
                                smtpClient.Credentials = New System.Net.NetworkCredential(txtSMTPUsername.Text, txtSMTPPassword.Text)
                            End If
                        Case "2" ' NTLM
                            smtpClient.UseDefaultCredentials = True
                    End Select

                    smtpClient.EnableSsl = chkSMTPEnableSSL.Checked

                    smtpClient.Send(emailMessage)

                    lblHostUserError.Text = String.Format(LocalizeString("SMTPSuccess"), LocalizeString("EmailSentMessage"))
                    Return True

                Catch ex As Exception
                    lblHostUserError.Text = String.Format(LocalizeString("SMTPError"), String.Format(LocalizeString("EmailErrorMessage"), ex.Message))
                    Return False

                End Try
            Else
                lblHostUserError.Text = String.Format(LocalizeString("SMTPError"), LocalizeString("SpecifyHostEmailMessage"))
                Return False
            End If




        End Function

        Private Sub UpdateMachineKey()
            Dim installationDate As String = Config.GetSetting("InstallationDate")

            If installationDate Is Nothing Or installationDate = "" Then
                Dim strError As String = Config.UpdateMachineKey()

                If strError = "" Then
                    ' send a new request to the application to initiate step 2
                    Response.Redirect(HttpContext.Current.Request.RawUrl, True)
                Else
                    '403-3 Error - Redirect to ErrorPage
                    Dim strURL As String = "~/ErrorPage.aspx?status=403_3&error=" & strError
                    HttpContext.Current.Response.Clear()
                    HttpContext.Current.Server.Transfer(strURL)
                End If
            End If

        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetBaseDataBaseVersion gets the Base Database Version as a string
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	03/02/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function GetBaseDataBaseVersion() As String
            Return Upgrade.Upgrade.GetStringVersion(BaseVersion)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LocalizeString is a helper function for localizing strings
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/23/2007 Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function LocalizeString(ByVal key As String) As String
            Return Services.Localization.Localization.GetString(key, Me.LocalResourceFile, cboLanguages.SelectedValue.ToLower)
        End Function

        Protected Overrides Sub OnError(ByVal e As System.EventArgs)
            HttpContext.Current.Response.Clear()
            HttpContext.Current.Server.Transfer("~/ErrorPage.aspx")
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the Page is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/14/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            DotNetNuke.UI.Utilities.ClientAPI.HandleClientAPICallbackEvent(Me)

            'Attach Event(s) to the Buttons
            Dim button As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton")
            If Not button Is Nothing Then
                AddHandler button.Click, AddressOf wizInstall_CustomButtonClick
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the Page loads
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'register variable ActionCallback with script necessary to perform callback
            '[ACTIONTOKEN] will be replaced on the client side with real action
            DotNetNuke.UI.Utilities.ClientAPI.RegisterClientVariable(Me.Page, "ActionCallback", DotNetNuke.UI.Utilities.ClientAPI.GetCallbackEventReference(Me, "[ACTIONTOKEN]", "successFunc", "this", "errorFunc"), True)

            lblHostWarning.Visible = Not Regex.IsMatch(Request.Url.Host, "^([a-zA-Z0-9.-]+)$", RegexOptions.IgnoreCase)

            If Not Page.IsPostBack Then
                rblInstall.Items.Clear()
                rblInstall.Items.Add(New ListItem(LocalizeString("Full"), "Full"))
                rblInstall.Items.Add(New ListItem(LocalizeString("Typical"), "Typical"))
                rblInstall.Items.Add(New ListItem(LocalizeString("Auto"), "Auto"))
                rblInstall.SelectedIndex = 1

                rblDatabases.Items.Clear()
                rblDatabases.Items.Add(New ListItem(LocalizeString("SQLServerXPress"), "SQLFile"))
                rblDatabases.Items.Add(New ListItem(LocalizeString("SQLServer"), "SQLDatabase"))

                'Parse the conneection String to the form
                BindConnectionString()

                'Database Choices
                BindDatabases()

                If TestDatabaseConnection() Then
                    Initialise()

                    rblInstall.Items(2).Enabled = True
                    lblDataBaseWarning.Visible = False
                Else
                    'Install but connection string not configured to point at a valid SQL Server
                    UpdateMachineKey()

                    rblInstall.Items(2).Enabled = False
                    lblDataBaseWarning.Visible = True
                End If

                cboLanguages.DataSource = GetInstallerLocales()
                cboLanguages.DataBind()

                LocalizePage()

                wizInstall.ActiveStepIndex = 0

                SetupPage()
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_PreRender runs just before the page is rendered
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/15/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            'Make sure that the password is not cleared on pastback
            txtPassword.Attributes("value") = txtPassword.Text
            txtSMTPPassword.Attributes("value") = txtSMTPPassword.Text
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Selected Language is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub cboLanguages_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboLanguages.SelectedIndexChanged
            LocalizePage()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Integrated Security Checkbox value is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/23/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub chkIntegrated_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkIntegrated.CheckedChanged
            trUser.Visible = Not chkIntegrated.Checked
            trPassword.Visible = Not chkIntegrated.Checked
            If chkIntegrated.Checked Then
                chkOwner.Checked = True
            End If
            chkOwner.Enabled = Not chkIntegrated.Checked
        End Sub

        Protected Sub optSMTPAuthentication_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optSMTPAuthentication.SelectedIndexChanged
            ShowHideSMTPCredentials()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when there is a ClientCallback
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ProcessAction(ByVal someAction As String) As String Implements DotNetNuke.UI.Utilities.IClientAPICallbackEventHandler.RaiseClientAPICallbackEvent
            'First check that we are not being targetted by an AJAX HttpPost, so get the current DB version
            Dim strProviderPath As String = dataProvider.GetProviderPath()
            Dim nextVersion As String = GetNextScriptVersion(strProviderPath, DatabaseVersion)
            If someAction <> nextVersion Then
                LogException(New Exception("Attempt made to run a Database Script - Possible Attack"))
                Return "Error: Possible Attack"
            End If

            If someAction = GetBaseDataBaseVersion() Then
                Dim result As String = InstallDatabase()
                If result = "Done" Then
                    'Complete Installation
                    Services.Upgrade.Upgrade.UpgradeApplication()
                End If
                Return result
            ElseIf someAction.Contains(".") Then
                'Upgrade Database
                Dim result As String = InstallVersion(someAction)
                If result = "Done" Then
                    Dim arrVersions As ArrayList = Services.Upgrade.Upgrade.GetUpgradeScripts(strProviderPath, BaseVersion)
                    Dim strErrorMessage As String = Null.NullString
                    For i As Integer = 0 To arrVersions.Count - 1
                        Dim strVersion = Path.GetFileNameWithoutExtension(CType(arrVersions(i), String))
                        Dim version As System.Version = New System.Version(strVersion)

                        If version IsNot Nothing Then
                            strErrorMessage += Services.Upgrade.Upgrade.UpgradeApplication(strProviderPath, version, False)

                            ' delete files which are no longer used
                            strErrorMessage += Services.Upgrade.Upgrade.DeleteFiles(strProviderPath, version, False)

                            'execute config file updates
                            strErrorMessage += Services.Upgrade.Upgrade.UpdateConfig(strProviderPath, version, False)
                        End If
                    Next
                    'Complete Installation
                    Services.Upgrade.Upgrade.UpgradeApplication()
                    If Not String.IsNullOrEmpty(strErrorMessage) Then
                        result = "ERROR: " + strErrorMessage
                    End If
                End If
                Return result
            Else
                Return "Done"
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Selected Database is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/23/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub rblDatabases_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles rblDatabases.SelectedIndexChanged
            BindConnectionString()
            SetupDatabasePage()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Active Wizard Step has changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/09/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizInstall_ActiveStepChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles wizInstall.ActiveStepChanged

            'Main Title
            Me.Title = LocalizeString("Title") + " - " + LocalizeString("Page" + wizInstall.ActiveStepIndex.ToString() + ".Title")
            If wizInstall.ActiveStepIndex > 0 Then
                Dim nextButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton")
                Dim prevButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "StepPreviousButton")
                nextButton.Text = "<img src=""" + ApplicationPath + "/images/rt.gif"" border=""0"" /> " + LocalizeString("Next")
                prevButton.Text = "<img src=""" + ApplicationPath + "/images/lt.gif"" border=""0"" /> " + LocalizeString("Previous")
            End If

            Select Case wizInstall.ActiveStepIndex
                Case 1
                    'Page 1 - File Permissions
                    BindPermissions(True)
                Case 2
                    ''Page 2 - Database
                    'Dim customButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton")
                    'customButton.Text = "<img src=""" + ApplicationPath + "/images/icon_sql_16px.gif"" border=""0"" /> " + LocalizeString("TestDB")
                Case 4
                    'Page 4 - SMTP Settings
                    Dim customButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton")
                    customButton.Text = "<img src=""" + ApplicationPath + "/images/icon_bulkmail_16px.gif"" border=""0"" /> " + LocalizeString("TestSMTP")
                    ShowHideSMTPCredentials()
                Case 5
                    'Page 5 - Modules
                    If rblInstall.SelectedValue = "Typical" Then
                        BindModules()

                        If InstallModules() Then
                            'Skip Modules Page
                            wizInstall.ActiveStepIndex = 6
                        End If
                    End If
                Case 6
                    'Page 6 - Skins/Conatiners
                    If rblInstall.SelectedValue = "Typical" Then
                        BindSkins()

                        If InstallSkins() Then
                            'Skip Skins Page
                            wizInstall.ActiveStepIndex = 7
                        End If
                    End If
                Case 7
                    'Page 7 - Languages
                    If rblInstall.SelectedValue = "Typical" Then
                        BindLanguages()

                        If InstallLanguages() Then
                            'Skip Languages Page
                            wizInstall.ActiveStepIndex = 8
                        End If
                    End If
                Case 8
                    'Page 8 - Auth Systems
                    If rblInstall.SelectedValue = "Typical" Then
                        BindAuthSystems()

                        If InstallAuthSystems() Then
                            'Skip Auth Systems Page
                            wizInstall.ActiveStepIndex = 9
                        End If
                    End If
                Case 9
                    'Page 9 - Providers
                    If rblInstall.SelectedValue = "Typical" Then
                        BindProviders()

                        If InstallProviders() Then
                            'Skip Providers Page
                            wizInstall.ActiveStepIndex = 10
                        End If
                    End If
            End Select

            SetupPage()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Wizard's Custom button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/28/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizInstall_CustomButtonClick(ByVal sender As Object, ByVal e As System.EventArgs)

            'Select Case wizInstall.ActiveStepIndex
            '    Case 1
            '        'Page 1 - File Permissions
            '        BindPermissions(True)
            '        If Not PermissionsValid Then
            '            Dim customButton As LinkButton = GetWizardButton("StepNavigationTemplateContainerID", "CustomButton")
            '            customButton.Text = "<img src=""" + ApplicationPath + "/images/icon_filemanager_16px.gif"" border=""0"" /> " + LocalizeString("TestPerm")
            '            ShowButton(GetWizardButton("StepNavigationTemplateContainerID", "CustomButton"), True)
            '            EnableButton(GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton"), False)
            '        Else
            '            ShowButton(GetWizardButton("StepNavigationTemplateContainerID", "CustomButton"), False)
            '            EnableButton(GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton"), True)
            '        End If
            '    Case 2
            '        'Page 2 - Database Connection String
            '        If TestDatabaseConnection() Then
            '            EnableButton(GetWizardButton("StepNavigationTemplateContainerID", "StepNextButton"), True)
            '        End If
            '    Case 4
            '        'Page 4 - SMTP Server
            '        TestSMTPSettings()
            'End Select
        End Sub

        Protected Sub wizInstall_FinishButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizInstall.FinishButtonClick
            'Force an App Restart
            Config.Touch()

            'Redirect to site home page
            Response.Redirect("~/Default.aspx", True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Runs when the Wizard's Next button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	02/20/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub wizInstall_NextButtonClick(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.WizardNavigationEventArgs) Handles wizInstall.NextButtonClick

            Dim nextStep As WizardStep = wizInstall.WizardSteps(e.NextStepIndex)

            Select Case e.CurrentStepIndex
                Case 0
                    If rblInstall.SelectedValue = "Auto" Then
                        Response.Redirect("~/Install/Install.aspx?mode=install")
                    End If
                Case 1
                    'Page 1 - File Permissions
                    BindPermissions(True)
                    e.Cancel = Not PermissionsValid
                Case 2
                    'Page 2 - Database Configuration
                    Dim canConnect As Boolean = TestDatabaseConnection()
                    If canConnect Then
                        'Update Connection String
                        Config.UpdateConnectionString(connectionString)

                        Dim dbOwner As String = String.Empty
                        If chkOwner.Checked Then
                            dbOwner = "dbo"
                        Else
                            If (String.IsNullOrEmpty(GetUpgradeConnectionStringUserID())) Then
                                dbOwner = txtUserId.Text
                            Else
                                dbOwner = GetUpgradeConnectionStringUserID()
                            End If
                        End If

                        If rblDatabases.SelectedValue = "Oracle" Then
                            Config.UpdateDataProvider("OracleDataProvider", "", txtqualifier.Text)
                        Else
                            Config.UpdateDataProvider("SqlDataProvider", dbOwner, txtqualifier.Text)
                        End If

                        'Get Base DatabaseVersion
                        GetBaseDataBaseVersion()

                    Else
                        e.Cancel = True
                    End If
                Case 3
                    'Page 3 - Database Installation
                    e.Cancel = Not TestDataBaseInstalled()
                Case 4
                    'Page 4 - Host User
                    'Check if SMTP needs to be tested
                    If Not txtSMTPServer.Text = String.Empty Then
                        If Not TestSMTPSettings() Then
                            e.Cancel = True
                            Return
                        End If
                    End If
                    e.Cancel = Not InstallHost()
                Case 5
                    'Page 5 - Modules
                    e.Cancel = Not InstallModules()
                Case 6
                    'Page 6 - Skins/Containers
                    e.Cancel = Not InstallSkins()
                Case 7
                    'Page 7 - Languages
                    e.Cancel = Not InstallLanguages()
                Case 8
                    'Page 8 - Auth Systems
                    e.Cancel = Not InstallAuthSystems()
                Case 9
                    'Page 9 - Providers
                    e.Cancel = Not InstallProviders()
                Case 10
                    'Page 10 - Portal
                    e.Cancel = Not InstallPortal()
            End Select
        End Sub

#End Region

    End Class
End Namespace

