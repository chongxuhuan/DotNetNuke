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

Imports System.Xml
Imports System.IO

Imports DotNetNuke.Services.Mail
Imports DotNetNuke.Security.Membership
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Modules.Admin.Portals

    Partial Class Signup
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'Customise the Control Title
            If IsHostMenu Then
                Me.ModuleConfiguration.ModuleTitle = Services.Localization.Localization.GetString("AddPortal", Me.LocalResourceFile)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	5/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim strFolder As String
                Dim strFileName As String
                Dim strMessage As String

                ' ensure portal signup is allowed
                If (Not IsHostMenu Or UserInfo.IsSuperUser = False) AndAlso Not Host.DemoSignup Then
                    Response.Redirect(NavigateURL("Access Denied"), True)
                End If

                valEmail2.ValidationExpression = glbEmailRegEx

                If Not Page.IsPostBack Then
                    strFolder = Common.Globals.HostMapPath
                    If System.IO.Directory.Exists(strFolder) Then
                        ' admin.template and a portal template are required at minimum
                        Dim fileEntries As String() = System.IO.Directory.GetFiles(strFolder, "*.template")
                        For Each strFileName In fileEntries
                            If Path.GetFileNameWithoutExtension(strFileName) <> "admin" Then
                                cboTemplate.Items.Add(Path.GetFileNameWithoutExtension(strFileName))
                            End If
                        Next

                        If cboTemplate.Items.Count = 0 Then
                            lblMessage.Text = Localization.GetString("PortalMissing", Me.LocalResourceFile)
                            cmdUpdate.Enabled = False
                        End If
                        cboTemplate.Items.Insert(0, New ListItem(Localization.GetString("None_Specified"), "-1"))
                        cboTemplate.SelectedIndex = 0
                    End If

                    If UserInfo.IsSuperUser = True Then
                        rowType.Visible = True
                        optType.SelectedValue = "P"
                    Else
                        optType.SelectedValue = "C"
                        txtPortalName.Text = GetDomainName(Request) & "/"
                        rowType.Visible = False
                        strMessage = String.Format(Localization.GetString("DemoMessage", Me.LocalResourceFile), Convert.ToString(IIf(Not String.IsNullOrEmpty(Host.DemoPeriod), " for " & Host.DemoPeriod & " days", "")), GetDomainName(Request))
                        lblInstructions.Text = strMessage
                        btnCustomizeHomeDir.Visible = False
                    End If

                    txtHomeDirectory.Text = "Portals/[PortalID]"
                    txtHomeDirectory.Enabled = False

                    If MembershipProviderConfig.RequiresQuestionAndAnswer Then
                        trQuestion.Visible = True
                        trAnswer.Visible = True
                    End If
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
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
            txtConfirm.Attributes("value") = txtConfirm.Text
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the Cancel button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	5/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                If IsHostMenu Then
                    Response.Redirect(NavigateURL(), True)
                Else
                    Response.Redirect(GetPortalDomainName(PortalAlias.HTTPAlias, Request), True)
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdUpdate_Click runs when the Update button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	5/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click

            If Page.IsValid Then
                Try
                    Dim blnChild As Boolean
                    Dim strMessage As String = String.Empty
                    Dim strPortalAlias As String
                    Dim intPortalId As Integer
                    Dim strServerPath As String
                    Dim strChildPath As String = String.Empty

                    Dim objPortalController As New PortalController

                    ' check template validity
                    Dim messages As New ArrayList
                    Dim schemaFilename As String = Server.MapPath(String.Concat(Me.AppRelativeTemplateSourceDirectory, "portal.template.xsd"))
                    Dim xmlFilename As String = Common.Globals.HostMapPath & cboTemplate.SelectedItem.Text & ".template"
                    Dim xval As New PortalTemplateValidator
                    If Not xval.Validate(xmlFilename, schemaFilename) Then
                        strMessage = Localization.GetString("InvalidTemplate", Me.LocalResourceFile)
                        lblMessage.Text = String.Format(strMessage, cboTemplate.SelectedItem.Text & ".template")
                        messages.AddRange(xval.Errors)
                        lstResults.Visible = True
                        lstResults.DataSource = messages
                        lstResults.DataBind()
                        Exit Sub
                    End If

                    'Set Portal Name
                    txtPortalName.Text = txtPortalName.Text.ToLowerInvariant
                    txtPortalName.Text = txtPortalName.Text.Replace("http://", "")

                    'Validate Portal Name
                    If PortalSettings.ActiveTab.ParentId <> PortalSettings.SuperTabId Then
                        blnChild = True
                        strPortalAlias = txtPortalName.Text
                    Else
                        blnChild = (optType.SelectedValue = "C")

                        If blnChild Then
                            strPortalAlias = Mid(txtPortalName.Text, InStrRev(txtPortalName.Text, "/") + 1)
                        Else
                            strPortalAlias = txtPortalName.Text
                        End If
                    End If
                    If Not PortalAliasController.ValidateAlias(strPortalAlias, blnChild) Then
                        strMessage &= "<br>" & Localization.GetString("InvalidName", Me.LocalResourceFile)
                    End If

                    'Validate Password
                    If txtPassword.Text <> txtConfirm.Text Then
                        strMessage &= "<br>" & Localization.GetString("InvalidPassword", Me.LocalResourceFile)
                    End If

                    strServerPath = GetAbsoluteServerPath(Request)

                    'Set Portal Alias for Child Portals
                    If strMessage = "" Then
                        If blnChild Then
                            strChildPath = strServerPath & strPortalAlias

                            If System.IO.Directory.Exists(strChildPath) Then
                                strMessage = Localization.GetString("ChildExists", Me.LocalResourceFile)
                            Else
                                If PortalSettings.ActiveTab.ParentId <> PortalSettings.SuperTabId Then
                                    strPortalAlias = GetDomainName(Request, True) & "/" & strPortalAlias
                                Else
                                    strPortalAlias = txtPortalName.Text
                                End If
                            End If
                        End If
                    End If

                    'Get Home Directory
                    Dim HomeDir As String
                    If txtHomeDirectory.Text <> "Portals/[PortalID]" Then
                        HomeDir = txtHomeDirectory.Text
                    Else
                        HomeDir = ""
                    End If

                    'Validate Home Folder
                    If Not String.IsNullOrEmpty(HomeDir) Then
                        Dim objFolderController As New Services.FileSystem.FolderController
                        If String.IsNullOrEmpty(objFolderController.GetMappedDirectory(Common.Globals.ApplicationPath + "/" + HomeDir + "/")) Then
                            strMessage = Localization.GetString("InvalidHomeFolder", Me.LocalResourceFile)
                        End If
                        If HomeDir.Contains("admin") OrElse HomeDir.Contains("DesktopModules") OrElse HomeDir.ToLowerInvariant() = "portals/" Then
                            strMessage = Localization.GetString("InvalidHomeFolder", Me.LocalResourceFile)
                        End If
                    End If

                    'Validate Portal Alias
                    If Not String.IsNullOrEmpty(strPortalAlias) Then
                        Dim PortalAlias As PortalAliasInfo = PortalAliasController.GetPortalAliasLookup(strPortalAlias.ToLower)
                        If PortalAlias IsNot Nothing Then
                            strMessage = Localization.GetString("DuplicatePortalAlias", Me.LocalResourceFile)
                        End If
                    End If

                    'Create Portal
                    If strMessage = "" Then
                        Dim strTemplateFile As String = cboTemplate.SelectedItem.Text & ".template"

                        'Attempt to create the portal
                        Dim objAdminUser As New UserInfo
                        Try
                            objAdminUser.FirstName = txtFirstName.Text
                            objAdminUser.LastName = txtLastName.Text
                            objAdminUser.Username = txtUsername.Text
                            objAdminUser.DisplayName = txtFirstName.Text + " " + txtLastName.Text
                            objAdminUser.Email = txtEmail.Text
                            objAdminUser.IsSuperUser = False

                            objAdminUser.Membership.Approved = True
                            objAdminUser.Membership.Password = txtPassword.Text
                            objAdminUser.Membership.PasswordQuestion = txtQuestion.Text
                            objAdminUser.Membership.PasswordAnswer = txtAnswer.Text

                            objAdminUser.Profile.FirstName = txtFirstName.Text
                            objAdminUser.Profile.LastName = txtLastName.Text

                            intPortalId = objPortalController.CreatePortal(txtTitle.Text, objAdminUser, txtDescription.Text, txtKeyWords.Text, Common.Globals.HostMapPath, strTemplateFile, HomeDir, strPortalAlias, strServerPath, strChildPath, blnChild)
                        Catch ex As Exception
                            intPortalId = Null.NullInteger
                            strMessage = ex.Message
                        End Try

                        If intPortalId <> -1 Then

                            'Create a Portal Settings object for the new Portal
                            Dim objPortal As PortalInfo = objPortalController.GetPortal(intPortalId)
                            Dim newSettings As New PortalSettings
                            newSettings.PortalAlias = New PortalAliasInfo
                            newSettings.PortalAlias.HTTPAlias = strPortalAlias
                            newSettings.PortalId = intPortalId
                            newSettings.DefaultLanguage = objPortal.DefaultLanguage
                            Dim webUrl As String = AddHTTP(strPortalAlias)

                            Try
                                If PortalSettings.ActiveTab.ParentId <> PortalSettings.SuperTabId Then
                                    strMessage = Mail.SendMail(PortalSettings.Email, txtEmail.Text, PortalSettings.Email & ";" & Host.HostEmail, _
                                        Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_SUBJECT", objAdminUser), _
                                        Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_BODY", objAdminUser), _
                                        "", "", "", "", "", "")
                                Else
                                    strMessage = Mail.SendMail(Host.HostEmail, txtEmail.Text, Host.HostEmail, _
                                        Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_SUBJECT", objAdminUser), _
                                        Localization.GetSystemMessage(newSettings, "EMAIL_PORTAL_SIGNUP_BODY", objAdminUser), _
                                        "", "", "", "", "", "")
                                End If
                            Catch ex As Exception
                                strMessage = String.Format(Localization.GetString("UnknownSendMail.Error", Me.LocalResourceFile), webUrl)
                            End Try

                            Dim objEventLog As New Services.Log.EventLog.EventLogController
                            objEventLog.AddLog(objPortalController.GetPortal(intPortalId), PortalSettings, UserId, "", Services.Log.EventLog.EventLogController.EventLogType.PORTAL_CREATED)

                            ' Redirect to this new site
                            If strMessage = Null.NullString Then
                                Response.Redirect(webUrl, True)
                            Else
                                strMessage = String.Format(Localization.GetString("SendMail.Error", Me.LocalResourceFile), strMessage, webUrl)
                            End If
                        End If
                    End If

                    lblMessage.Text = "<br>" & strMessage & "<br><br>"

                Catch exc As Exception    'Module failed to load
                    ProcessModuleLoadException(Me, exc)
                End Try
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' optType_SelectedIndexChanged runs when the Portal Type is changed
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	5/10/2004	Updated to reflect design changes for Help, 508 support
        '''                       and localisation
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub optType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles optType.SelectedIndexChanged
            Try
                If optType.SelectedValue = "C" Then
                    txtPortalName.Text = GetDomainName(Request) & "/"
                Else
                    txtPortalName.Text = ""
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub btnCustomizeHomeDir_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCustomizeHomeDir.Click
            Try
                If txtHomeDirectory.Enabled Then
                    btnCustomizeHomeDir.Text = Services.Localization.Localization.GetString("Customize", LocalResourceFile)
                    txtHomeDirectory.Text = "Portals/[PortalID]"
                    txtHomeDirectory.Enabled = False
                Else
                    btnCustomizeHomeDir.Text = Services.Localization.Localization.GetString("AutoGenerate", LocalResourceFile)
                    txtHomeDirectory.Text = ""
                    txtHomeDirectory.Enabled = True
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cboTemplate_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTemplate.SelectedIndexChanged
            Try
                Dim filename As String

                If cboTemplate.SelectedIndex > 0 Then
                    filename = Common.Globals.HostMapPath & cboTemplate.SelectedItem.Text & ".template"
                    Dim xmldoc As New XmlDocument
                    Dim node As XmlNode
                    xmldoc.Load(filename)
                    node = xmldoc.SelectSingleNode("//portal/description")
                    If Not node Is Nothing AndAlso node.InnerXml <> "" Then
                        lblTemplateDescription.Visible = True
                        lblTemplateDescription.Text = Server.HtmlDecode(node.InnerXml)
                    Else
                        lblTemplateDescription.Visible = False
                    End If
                Else
                    lblTemplateDescription.Visible = False
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

#End Region

    End Class

End Namespace
