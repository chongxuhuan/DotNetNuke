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
Imports System.Web

Imports System.Collections.Generic
Imports DotNetNuke.Services
Imports DotNetNuke.Services.Cache
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.HtmlEditor.TelerikEditorProvider
Imports Telerik.Web.UI

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider.Dialogs

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    Partial Class SaveTemplate
        Inherits DotNetNuke.Framework.PageBase

#Region "Event Handlers"

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            If Request.IsAuthenticated = True Then
                Response.Cache.SetCacheability(Web.HttpCacheability.ServerAndNoCache)
            End If

            ManageStyleSheets(False)
            ManageStyleSheets(True)
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            Try
                SetResStrings()
                If Not IsPostBack Then
                    Dim portalID As Integer = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings().PortalId
                    Dim folders As ArrayList = DotNetNuke.Common.Utilities.FileSystemUtils.GetFoldersByUser(portalID, True, True, "ADD")

                    If folders.Count = 0 Then
                        msgError.InnerHtml = GetString("msgNoFolders.Text")
                        divInputArea.Visible = False
                        cmdClose.Visible = True
                    Else
                        FolderList.Items.Clear()

                        FolderList.DataTextField = "DisplayPath"
                        FolderList.DataValueField = "FolderPath"
                        FolderList.DataSource = folders
                        FolderList.DataBind()

                        Dim rootFolder As RadComboBoxItem = FolderList.FindItemByText(String.Empty)
                        If rootFolder IsNot Nothing Then
                            rootFolder.Text = GetString("lblRootFolder.Text")
                        End If
                    End If
                End If
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.LogException(ex)
                Throw ex
            End Try
        End Sub

        Protected Sub Save_OnClick(ByVal sender As Object, ByVal e As EventArgs)
            Try
                If FolderList.Items.Count = 0 Then
                    Exit Sub
                End If

                Dim portalSettings As DotNetNuke.Entities.Portals.PortalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current

                Dim fileContents As String = htmlText2.Text.Trim()
                Dim newFileName As String = FileName.Text
                If Not newFileName.EndsWith(".htmtemplate") Then
                    newFileName = newFileName & ".htmtemplate"
                End If

                Dim rootFolder As String = portalSettings.HomeDirectoryMapPath
                Dim dbFolderPath As String = FolderList.SelectedValue
                Dim virtualFolder As String = FileSystemValidation.ToVirtualPath(dbFolderPath)
                rootFolder = rootFolder + FolderList.SelectedValue
                rootFolder = rootFolder.Replace("/", "\")

                Dim errorMessage As String = String.Empty
                Dim folderCtrl As New FileSystem.FolderController()
                Dim folder As FileSystem.FolderInfo = folderCtrl.GetFolder(portalSettings.PortalId, dbFolderPath, False)

                If (IsNothing(folder)) Then
                    ShowSaveTemplateMessage(GetString("msgFolderDoesNotExist.Text"))
                    Return
                End If

                ' Check file name is valid
                Dim dnnValidator As FileSystemValidation = New FileSystemValidation()
                errorMessage = dnnValidator.OnCreateFile(virtualFolder + newFileName, fileContents.Length)
                If (Not String.IsNullOrEmpty(errorMessage)) Then
                    ShowSaveTemplateMessage(errorMessage)
                    Return
                End If

                Dim fileCtrl As New FileSystem.FileController()
                Dim existingFile As DotNetNuke.Services.FileSystem.FileInfo = fileCtrl.GetFile(newFileName, portalSettings.PortalId, folder.FolderID)

                ' error if file exists
                If (Not Overwrite.Checked AndAlso Not IsNothing(existingFile)) Then
                    ShowSaveTemplateMessage(GetString("msgFileExists.Text"))
                    Return
                End If

                Dim newFile As FileSystem.FileInfo = existingFile
                If (IsNothing(newFile)) Then
                    newFile = New FileSystem.FileInfo()
                End If

                newFile.FileName = newFileName
                newFile.ContentType = "text/plain"
                newFile.Extension = "htmtemplate"
                newFile.Size = fileContents.Length
                newFile.FolderId = folder.FolderID

                errorMessage = DotNetNuke.Common.Utilities.FileSystemUtils.CreateFileFromString(rootFolder, newFile.FileName, fileContents, newFile.ContentType, String.Empty, False)

                If (Not String.IsNullOrEmpty(errorMessage)) Then
                    ShowSaveTemplateMessage(errorMessage)
                    Return
                End If

                existingFile = fileCtrl.GetFile(newFileName, portalSettings.PortalId, folder.FolderID)
                If (newFile.FileId <> existingFile.FileId) Then
                    newFile.FileId = existingFile.FileId
                End If

                If (newFile.FileId <> Null.NullInteger) Then
                    fileCtrl.UpdateFile(newFile.FileId, newFile.FileName, newFile.Extension, newFile.Size, newFile.Width, newFile.Height, newFile.ContentType, folder.FolderPath, folder.FolderID)
                Else
                    fileCtrl.AddFile(portalSettings.PortalId, newFile.FileName, newFile.Extension, newFile.Size, newFile.Width, newFile.Height, newFile.ContentType, folder.FolderPath, folder.FolderID, True)
                End If

                ShowSaveTemplateMessage(String.Empty)
            Catch ex As Exception
                DotNetNuke.Services.Exceptions.LogException(ex)
                Throw ex
            End Try
        End Sub

#End Region

#Region "Properties"

#End Region

#Region "Methods"

        Private Sub ShowSaveTemplateMessage(ByVal errorMessage As String)
            If errorMessage = String.Empty Then
                msgSuccess.Visible = True
                msgError.Visible = False
            Else
                msgSuccess.Visible = False
                msgError.Visible = True
                msgError.InnerHtml += errorMessage
                DotNetNuke.Services.Exceptions.Exceptions.LogException(New FileManagerException("Error creating htmtemplate file [" & errorMessage & "]"))
            End If

            divInputArea.Visible = False
            cmdClose.Visible = True
        End Sub

        Private Sub SetResStrings()
            lblFolders.Text = GetString("lblFolders.Text")
            lblFileName.Text = GetString("lblFileName.Text")
            lblOverwrite.Text = GetString("lblOverwrite.Text")
            cmdSave.Text = GetString("cmdSave.Text")
            cmdCancel.Text = GetString("cmdCancel.Text")
            cmdClose.Text = GetString("cmdClose.Text")
            msgSuccess.InnerHtml = GetString("msgSuccess.Text")
            msgError.InnerHtml = GetString("msgError.Text")
        End Sub

        Protected Function GetString(ByVal key As String) As String
            Dim resourceFile As String = System.IO.Path.Combine(Me.TemplateSourceDirectory & "/", DotNetNuke.Services.Localization.Localization.LocalResourceDirectory & "/SaveTemplate.resx")
            Return DotNetNuke.Services.Localization.Localization.GetString(key, resourceFile)
        End Function

        Private Sub ManageStyleSheets(ByVal PortalCSS As Boolean)

            ' initialize reference paths to load the cascading style sheets
            Dim ID As String

            Dim objCSSCache As Hashtable = CType(DotNetNuke.Common.Utilities.DataCache.GetCache("CSS"), Hashtable)
            If objCSSCache Is Nothing Then
                objCSSCache = New Hashtable
            End If

            If PortalCSS = False Then
                ' default style sheet ( required )
                ID = CreateValidID(DotNetNuke.Common.Globals.HostPath)
                AddStyleSheet(ID, DotNetNuke.Common.Globals.HostPath & "default.css")

                ' skin package style sheet
                ID = CreateValidID(PortalSettings.ActiveTab.SkinPath)
                If objCSSCache.ContainsKey(ID) = False Then
                    If IO.File.Exists(Server.MapPath(PortalSettings.ActiveTab.SkinPath) & "skin.css") Then
                        objCSSCache(ID) = PortalSettings.ActiveTab.SkinPath & "skin.css"
                    Else
                        objCSSCache(ID) = ""
                    End If
                    If Not DotNetNuke.Entities.Host.Host.PerformanceSetting = DotNetNuke.Common.Globals.PerformanceSettings.NoCaching Then
                        DotNetNuke.Common.Utilities.DataCache.SetCache("CSS", objCSSCache)
                    End If
                End If
                If objCSSCache(ID).ToString <> "" Then
                    AddStyleSheet(ID, objCSSCache(ID).ToString)
                End If

                ' skin file style sheet
                ID = CreateValidID(Replace(PortalSettings.ActiveTab.SkinSrc, ".ascx", ".css"))
                If objCSSCache.ContainsKey(ID) = False Then
                    If IO.File.Exists(Server.MapPath(Replace(PortalSettings.ActiveTab.SkinSrc, ".ascx", ".css"))) Then
                        objCSSCache(ID) = Replace(PortalSettings.ActiveTab.SkinSrc, ".ascx", ".css")
                    Else
                        objCSSCache(ID) = ""
                    End If
                    If Not DotNetNuke.Entities.Host.Host.PerformanceSetting = DotNetNuke.Common.Globals.PerformanceSettings.NoCaching Then
                        DotNetNuke.Common.Utilities.DataCache.SetCache("CSS", objCSSCache)
                    End If
                End If
                If objCSSCache(ID).ToString <> "" Then
                    AddStyleSheet(ID, objCSSCache(ID).ToString)
                End If
            Else
                ' portal style sheet
                ID = CreateValidID(PortalSettings.HomeDirectory)
                AddStyleSheet(ID, PortalSettings.HomeDirectory & "portal.css")
            End If
        End Sub

        Public Sub AddStyleSheet(ByVal id As String, ByVal href As String, ByVal isFirst As Boolean)

            'Find the placeholder control
            Dim objCSS As Control = Me.FindControl("CSS")

            If Not objCSS Is Nothing Then
                'First see if we have already added the <LINK> control
                Dim objCtrl As Control = Page.Header.FindControl(id)

                If objCtrl Is Nothing Then
                    Dim objLink As New HtmlLink()
                    objLink.ID = id
                    objLink.Attributes("rel") = "stylesheet"
                    objLink.Attributes("type") = "text/css"
                    objLink.Href = href

                    If isFirst Then
                        'Find the first HtmlLink
                        Dim iLink As Integer
                        For iLink = 0 To objCSS.Controls.Count - 1
                            If TypeOf objCSS.Controls(iLink) Is HtmlLink Then
                                Exit For
                            End If
                        Next
                        objCSS.Controls.AddAt(iLink, objLink)
                    Else
                        objCSS.Controls.Add(objLink)
                    End If
                End If
            End If

        End Sub

        Public Sub AddStyleSheet(ByVal id As String, ByVal href As String)
            AddStyleSheet(id, href, False)
        End Sub

#End Region

    End Class

End Namespace
