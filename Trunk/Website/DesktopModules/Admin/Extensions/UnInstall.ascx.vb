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
Imports System.Collections.Generic
Imports ICSharpCode.SharpZipLib.Zip

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.Installer
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.UI.Utilities

Namespace DotNetNuke.Modules.Admin.Extensions

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Class	 : UnInstall
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Supplies the functionality for uninstalling Extensions(packages) from the Portal
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]   07/26/2007    Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class UnInstall
        Inherits ModuleUserControlBase

#Region "Members"

        Private _Package As PackageInfo = Nothing

#End Region

#Region "Public Properties"

        Public Property PackageID() As Integer
            Get
                Dim _PageNo As Integer = 0
                If Not ViewState("PackageID") Is Nothing Then
                    _PageNo = CInt(ViewState("PackageID"))
                End If
                Return _PageNo
            End Get
            Set(ByVal Value As Integer)
                ViewState("PackageID") = Value
            End Set
        End Property

        Public ReadOnly Property Package() As PackageInfo
            Get
                If _Package Is Nothing AndAlso PackageID > Null.NullInteger Then
                    _Package = PackageController.GetPackage(PackageID)
                End If
                Return _Package
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Return Url
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/31/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ReturnURL() As String
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

#End Region

#Region "Private Methods"

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
        ''' This routine uninstalls the package
        ''' </summary>
        ''' <history>
        '''     [cnurse]   07/31/2007    Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UnInstallPackage()
            phPaLogs.Visible = True
            Dim installer As New Installer(Package, Request.MapPath("."))
            installer.UnInstall(chkDelete.Checked)
            phPaLogs.Controls.Add(installer.InstallerInfo.Log.GetLogsTable)
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	01/21/2008	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not (Request.QueryString("packageid") Is Nothing) Then
                PackageID = Int32.Parse(Request.QueryString("packageid"))
            Else
                PackageID = Null.NullInteger
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

                ClientAPI.AddButtonConfirm(cmdUninstall, Services.Localization.Localization.GetString("DeleteItem"))

                If Package IsNot Nothing AndAlso String.IsNullOrEmpty(Package.Manifest) Then
                    chkDelete.Visible = False
                End If

                ''Make Uninstall and Delete Files option unavailable if package cannot be deleted. Also display a message to the user
                If Not PackageController.CanDeletePackage(Package, ModuleContext.PortalSettings) Then
                    cmdUninstall.Visible = False
                    chkDelete.Visible = False
                    lblMessage.CssClass = "NormalRed"
                    Select Case Package.PackageType
                        Case "Skin"
                            lblMessage.Text = Localization.GetString("CannotDeleteSkin.ErrorMessage", Me.LocalResourceFile)
                        Case "Container"
                            lblMessage.Text = Localization.GetString("CannotDeleteContainer.ErrorMessage", Me.LocalResourceFile)
                        Case "Provider"
                            lblMessage.Text = Localization.GetString("CannotDeleteProvider.ErrorMessage", Me.LocalResourceFile)
                    End Select
                Else
                    lblMessage.CssClass = "Normal"
                    lblMessage.Text = ""
                End If

                ctlPackage.EditMode = UI.WebControls.PropertyEditorMode.View
                ctlPackage.DataSource = Package
                ctlPackage.DataBind()

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The cmdUninstall_Click runs when the Uninstall Button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 07/31/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdUninstall_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUninstall.Click
            Try
                UnInstallPackage()

                If phPaLogs.Controls.Count > 0 Then
                    pButtons.Visible = False
                    ctlPackage.Visible = False
                    tblLogs.Visible = True
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The cmdReturn_Click runs when the Return Button is clicked
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''   [cnurse] 16/9/2004  Updated for localization, Help and 508
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdReturn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdReturn1.Click, cmdReturn2.Click
            Response.Redirect(ReturnURL(), True)
        End Sub

#End Region

    End Class

End Namespace
