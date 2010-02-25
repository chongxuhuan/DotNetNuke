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
Imports DotNetNuke.Services.FileSystem
Imports DotNetNuke.Services.Localization
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
    Partial Class Export
        Inherits Entities.Modules.PortalModuleBase

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
                ModuleId = Int32.Parse(Request.QueryString("moduleid"))
            End If

            ' Verify that the current user has access to edit this module
            If Not ModulePermissionController.HasModuleAccess(SecurityAccessLevel.Edit, "EXPORT", [Module]) Then
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
                If Not Request.QueryString("moduleid") Is Nothing Then
                    ModuleId = Int32.Parse(Request.QueryString("moduleid"))
                End If

                If Not Page.IsPostBack Then
                    cboFolders.Items.Insert(0, New ListItem("<" + Services.Localization.Localization.GetString("None_Specified") + ">", "-"))
                    Dim folders As ArrayList = FileSystemUtils.GetFoldersByUser(PortalId, False, False, "READ, WRITE")
                    For Each folder As FolderInfo In folders
                        Dim FolderItem As New ListItem
                        If folder.FolderPath = Null.NullString Then
                            FolderItem.Text = Localization.GetString("Root", Me.LocalResourceFile)
                        Else
                            FolderItem.Text = folder.FolderPath
                        End If
                        FolderItem.Value = folder.FolderPath
                        cboFolders.Items.Add(FolderItem)
                    Next

                    If Not [Module] Is Nothing Then
                        txtFile.Text = CleanName([Module].ModuleTitle)
                    End If
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
        Private Sub cmdExport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdExport.Click
            Try
                If cboFolders.SelectedIndex <> 0 And txtFile.Text <> "" Then
                    Dim strFile As String = "content." & CleanName([Module].DesktopModule.ModuleName) & "." & CleanName(txtFile.Text) & ".xml"
                    Dim strMessage As String = ExportModule(ModuleId, strFile, cboFolders.SelectedItem.Value)
                    If strMessage = "" Then
                        Response.Redirect(NavigateURL(), True)
                    Else
                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                    End If
                Else
                    UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Validation", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Private Methods"

        Private Function ExportModule(ByVal ModuleID As Integer, ByVal FileName As String, ByVal Folder As String) As String

            Dim strMessage As String = ""

            If Not [Module] Is Nothing Then
                If [Module].DesktopModule.BusinessControllerClass <> "" And [Module].DesktopModule.IsPortable Then
                    Try
                        Dim objObject As Object = Framework.Reflection.CreateObject([Module].DesktopModule.BusinessControllerClass, [Module].DesktopModule.BusinessControllerClass)

                        'Double-check
                        If TypeOf objObject Is IPortable Then

                            Dim Content As String = CType(CType(objObject, IPortable).ExportModule(ModuleID), String)

                            If Content <> "" Then
                                ' add attributes to XML document
                                Content = "<?xml version=""1.0"" encoding=""utf-8"" ?>" & _
                                  "<content type=""" & CleanName([Module].DesktopModule.ModuleName) & """ version=""" & [Module].DesktopModule.Version & """>" & _
                                  Content & _
                                  "</content>"

                                'First check the Portal limits will not be exceeded (this is approximate)
                                Dim objPortalController As New PortalController
                                Dim strFile As String = PortalSettings.HomeDirectoryMapPath & Folder & FileName
                                If objPortalController.HasSpaceAvailable(PortalId, Content.Length) Then
                                    ' save the file
                                    Dim objStream As StreamWriter
                                    objStream = File.CreateText(strFile)
                                    objStream.WriteLine(Content)
                                    objStream.Close()

                                    ' add file to Files table
                                    FileSystemUtils.AddFile(FileName, PortalId, Folder, PortalSettings.HomeDirectoryMapPath, "application/octet-stream")
                                Else
                                    strMessage += "<br>" & String.Format(Localization.GetString("DiskSpaceExceeded"), strFile)
                                End If
                            Else
                                strMessage = Localization.GetString("NoContent", Me.LocalResourceFile)
                            End If
                        Else
                            strMessage = Localization.GetString("ExportNotSupported", Me.LocalResourceFile)
                        End If
                    Catch
                        strMessage = Localization.GetString("Error", Me.LocalResourceFile)
                    End Try
                Else
                    strMessage = Localization.GetString("ExportNotSupported", Me.LocalResourceFile)
                End If
            End If

            Return strMessage

        End Function

        Private Function CleanName(ByVal Name As String) As String

            Dim strName As String = Name
            Dim strBadChars As String = ". ~`!@#$%^&*()-_+={[}]|\:;<,>?/" & Chr(34) & Chr(39)

            Dim intCounter As Integer
            For intCounter = 0 To Len(strBadChars) - 1
                strName = strName.Replace(strBadChars.Substring(intCounter, 1), "")
            Next intCounter

            Return strName

        End Function

#End Region

    End Class

End Namespace
