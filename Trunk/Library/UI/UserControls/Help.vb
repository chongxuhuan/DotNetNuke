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
Imports System.IO
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Services.Installer.Packages
Imports DotNetNuke.Entities.Modules.Actions

Namespace DotNetNuke.UI.UserControls

    Public MustInherit Class Help
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Controls"

        Protected lblHelp As System.Web.UI.WebControls.Label
        Protected WithEvents cmdCancel As System.Web.UI.WebControls.LinkButton
        Protected cmdHelp As System.Web.UI.WebControls.HyperLink
        Protected lblInfo As System.Web.UI.WebControls.Label

#End Region

#Region "Private Members"

        Private _key As String
        Private MyFileName As String = "Help.ascx"

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Dim FriendlyName As String = ""

            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModule(ModuleId, TabId, False)
            If Not objModule Is Nothing Then
                FriendlyName = objModule.DesktopModule.FriendlyName
            End If

            Dim ModuleControlId As Integer = Null.NullInteger

            If Not (Request.QueryString("ctlid") Is Nothing) Then
                ModuleControlId = Int32.Parse(Request.QueryString("ctlid"))
            End If

            Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControl(ModuleControlId)
            If Not objModuleControl Is Nothing Then
                Dim FileName As String = Path.GetFileName(objModuleControl.ControlSrc)
                Dim LocalResourceFile As String = objModuleControl.ControlSrc.Replace(FileName, Services.Localization.Localization.LocalResourceDirectory & "/" & FileName)
                If Services.Localization.Localization.GetString(ModuleActionType.HelpText, LocalResourceFile) <> "" Then
                    lblHelp.Text = Services.Localization.Localization.GetString(ModuleActionType.HelpText, LocalResourceFile)
                Else
                    lblHelp.Text = Services.Localization.Localization.GetString("lblHelp.Text", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                End If

                _key = objModuleControl.ControlKey

                Dim helpUrl As String = GetOnLineHelp(objModuleControl.HelpURL, ModuleConfiguration)
                If Not String.IsNullOrEmpty(helpUrl) Then
                    cmdHelp.NavigateUrl = FormatHelpUrl(helpUrl, PortalSettings, FriendlyName)
                    cmdHelp.Visible = True
                Else
                    cmdHelp.Visible = False
                End If

                ' display module info to Host users
                If UserInfo.IsSuperUser Then
                    Dim strInfo As String = Services.Localization.Localization.GetString("lblInfo.Text", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                    strInfo = strInfo.Replace("[CONTROL]", objModuleControl.ControlKey)
                    strInfo = strInfo.Replace("[SRC]", objModuleControl.ControlSrc)
                    Dim objModuleDefinition As ModuleDefinitionInfo = ModuleDefinitionController.GetModuleDefinitionByID(objModuleControl.ModuleDefID)
                    If Not objModuleDefinition Is Nothing Then
                        strInfo = strInfo.Replace("[DEFINITION]", objModuleDefinition.FriendlyName)
                        Dim objDesktopModule As DesktopModuleInfo = DesktopModuleController.GetDesktopModule(objModuleDefinition.DesktopModuleID, PortalId)
                        If Not objDesktopModule Is Nothing Then
                            Dim objPackage As PackageInfo = PackageController.GetPackage(objDesktopModule.PackageID)
                            If Not objPackage Is Nothing Then
                                strInfo = strInfo.Replace("[ORGANIZATION]", objPackage.Organization)
                                strInfo = strInfo.Replace("[OWNER]", objPackage.Owner)
                                strInfo = strInfo.Replace("[EMAIL]", objPackage.Email)
                                strInfo = strInfo.Replace("[URL]", objPackage.Url)
                                strInfo = strInfo.Replace("[MODULE]", objPackage.Name)
                                strInfo = strInfo.Replace("[VERSION]", objPackage.Version.ToString())
                            End If
                        End If
                    End If
                    lblInfo.Text = Server.HtmlDecode(strInfo)
                End If
            End If

            If Page.IsPostBack = False Then
                If Not Request.UrlReferrer Is Nothing Then
                    ViewState("UrlReferrer") = Convert.ToString(Request.UrlReferrer)
                Else
                    ViewState("UrlReferrer") = ""
                End If
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' cmdCancel_Click runs when the cancel Button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(Convert.ToString(ViewState("UrlReferrer")), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

    End Class

End Namespace