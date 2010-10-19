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
Imports System.Web.UI.WebControls
Imports DotNetNuke
Imports System.Reflection
Imports System.IO
Imports DotNetNuke.Entities.Modules
Imports System.Xml
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Security.Permissions

Namespace DotNetNuke.Modules.Admin.Modules

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class Import
        Inherits Entities.Modules.PortalModuleBase

#Region "Controls"



#End Region

#Region "Private Members"

        Private Shadows ModuleId As Integer = -1
        Private _Module As ModuleInfo

        Private ReadOnly Property [Module]() As ModuleInfo
            Get
                If _Module Is Nothing Then
                    _Module = New ModuleController().GetModule(ModuleId, TabId, False)
                End If
                Return _Module
            End Get
        End Property

#End Region

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Not Request.QueryString("moduleid") Is Nothing Then
                Integer.TryParse(Request.QueryString("moduleid"), ModuleId)
            End If

            ' Verify that the current user has access to edit this module
            If Not ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "IMPORT", [Module]) Then
                Response.Redirect(AccessDeniedURL(), True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Not Page.IsPostBack Then
                    cboFolders.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", "-"))
                    Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(PortalId, False, False, "BROWSE, ADD")
                    For Each folder As FolderInfo In folders
                        Dim FolderItem As New ListItem
                        If folder.FolderPath = Null.NullString Then
                            FolderItem.Text = Localization.GetString("Root", Me.LocalResourceFile)
                        Else
                            FolderItem.Text = folder.DisplayPath
                        End If
                        FolderItem.Value = folder.FolderPath
                        cboFolders.Items.Add(FolderItem)
                    Next
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cboFolders_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboFolders.SelectedIndexChanged
            cboFiles.Items.Clear()
            If cboFolders.SelectedIndex <> 0 Then
                If Not [Module] Is Nothing Then
                    Dim arrFiles As ArrayList = Common.Globals.GetFileList(PortalId, "xml", False, cboFolders.SelectedItem.Value)
                    Dim objFile As FileItem
                    For Each objFile In arrFiles
                        If objFile.Text.IndexOf("content." & CleanName([Module].DesktopModule.ModuleName) & ".") <> -1 Then
                            cboFiles.Items.Add(New ListItem(objFile.Text.Replace("content." & CleanName([Module].DesktopModule.ModuleName) & ".", ""), objFile.Text))
                        End If
                        ' legacy support for files which used the FriendlyName
                        If CleanName([Module].DesktopModule.ModuleName) <> CleanName([Module].DesktopModule.FriendlyName) Then
                            If objFile.Text.IndexOf("content." & CleanName([Module].DesktopModule.FriendlyName) & ".") <> -1 Then
                                cboFiles.Items.Add(New ListItem(objFile.Text.Replace("content." & CleanName([Module].DesktopModule.FriendlyName) & ".", ""), objFile.Text))
                            End If
                        End If
                    Next
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdImport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdImport.Click
            Try
                If Not cboFiles.SelectedItem Is Nothing Then
                    If Not [Module] Is Nothing Then
                        Dim strMessage As String = ImportModule(ModuleId, cboFiles.SelectedItem.Value, cboFolders.SelectedItem.Value)
                        If strMessage = "" Then
                            Response.Redirect(NavigateURL(), True)
                        Else
                            UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        End If
                    End If
                Else
                    UI.Skins.Skin.AddModuleMessage(Me, "Please specify the file to import", UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Private Methods"

        Private Function ImportModule(ByVal ModuleId As Integer, ByVal FileName As String, ByVal Folder As String) As String

            Dim strMessage As String = ""
            If Not [Module] Is Nothing Then
                If FileName.IndexOf("." & CleanName([Module].DesktopModule.ModuleName) & ".") <> -1 Or FileName.IndexOf("." & CleanName([Module].DesktopModule.FriendlyName) & ".") <> -1 Then
                    If [Module].DesktopModule.BusinessControllerClass <> "" And [Module].DesktopModule.IsPortable Then
                        Try
                            Dim objObject As Object = Framework.Reflection.CreateObject([Module].DesktopModule.BusinessControllerClass, [Module].DesktopModule.BusinessControllerClass)

                            If TypeOf objObject Is IPortable Then

                                Dim objStreamReader As StreamReader
                                objStreamReader = File.OpenText(PortalSettings.HomeDirectoryMapPath & Folder & FileName)
                                Dim Content As String = objStreamReader.ReadToEnd
                                objStreamReader.Close()

                                Dim xmlDoc As New XmlDocument
                                Try
                                    xmlDoc.LoadXml(Content)
                                Catch
                                    strMessage = Localization.GetString("NotValidXml", Me.LocalResourceFile)
                                End Try

                                If strMessage = "" Then
                                    Dim strType As String = xmlDoc.DocumentElement.GetAttribute("type").ToString
                                    If strType = CleanName([Module].DesktopModule.ModuleName) Or strType = CleanName([Module].DesktopModule.FriendlyName) Then
                                        Dim strVersion As String = xmlDoc.DocumentElement.GetAttribute("version").ToString

                                        CType(objObject, IPortable).ImportModule(ModuleId, xmlDoc.DocumentElement.InnerXml, strVersion, UserInfo.UserID)

                                        Response.Redirect(NavigateURL(), True)
                                    Else
                                        strMessage = Localization.GetString("NotCorrectType", Me.LocalResourceFile)
                                    End If
                                End If
                            Else
                                strMessage = Localization.GetString("ImportNotSupported", Me.LocalResourceFile)
                            End If
                        Catch
                            strMessage = Localization.GetString("Error", Me.LocalResourceFile)
                        End Try
                    Else
                        strMessage = Localization.GetString("ImportNotSupported", Me.LocalResourceFile)
                    End If
                Else
                    strMessage = Localization.GetString("NotCorrectType", Me.LocalResourceFile)
                End If
            End If

            Return strMessage

        End Function


#End Region

    End Class

End Namespace
