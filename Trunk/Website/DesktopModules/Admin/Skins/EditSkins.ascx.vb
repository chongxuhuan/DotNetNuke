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
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Modules.Admin.Skins

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditSkins PortalModuleBase is used to manage the Available Skins
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	9/13/2004	Updated to reflect design changes for Help, 508 support
    '''                       and localisation
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class EditSkins
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

        Protected Property CurrentContainer() As String
            Get
                Dim _CurrentContainer As String = Null.NullString
                If ViewState("CurrentContainer") IsNot Nothing Then
                    _CurrentContainer = Convert.ToString(ViewState("CurrentContainer"))
                End If
                Return _CurrentContainer
            End Get
            Set(ByVal value As String)
                ViewState("CurrentContainer") = value
            End Set
        End Property

        Protected Property CurrentSkin() As String
            Get
                Dim _CurrentSkin As String = Null.NullString
                If ViewState("CurrentSkin") IsNot Nothing Then
                    _CurrentSkin = Convert.ToString(ViewState("CurrentSkin"))
                End If
                Return _CurrentSkin
            End Get
            Set(ByVal value As String)
                ViewState("CurrentSkin") = value
            End Set
        End Property

        Private NotSpecified As String = "<" & Services.Localization.Localization.GetString("Not_Specified") & ">"

#Region "Private Methods"

        Private Sub AddSkinstoCombo(ByVal combo As DropDownList, ByVal strRoot As String)
            Dim strName As String

            If Directory.Exists(strRoot) Then
                For Each strFolder As String In Directory.GetDirectories(strRoot)
                    strName = Mid(strFolder, InStrRev(strFolder, "\") + 1)
                    If strName <> "_default" Then
                        combo.Items.Add(New ListItem(strName, strFolder.Replace(ApplicationMapPath, "").ToLower))
                    End If
                Next
            End If
        End Sub

        Private Function CreateThumbnail(ByVal strImage As String) As String

            Dim blnCreate As Boolean = True

            Dim strThumbnail As String = strImage.Replace(Path.GetFileName(strImage), "thumbnail_" & Path.GetFileName(strImage))

            ' check if image has changed
            If File.Exists(strThumbnail) Then
                Dim d1 As Date = File.GetLastWriteTime(strThumbnail)
                Dim d2 As Date = File.GetLastWriteTime(strImage)
                If File.GetLastWriteTime(strThumbnail) = File.GetLastWriteTime(strImage) Then
                    blnCreate = False
                End If
            End If

            If blnCreate Then

                Dim dblScale As Double
                Dim intHeight As Integer
                Dim intWidth As Integer

                Dim intSize As Integer = 150    ' size of the thumbnail 

                Dim objImage As System.Drawing.Image
                Try
                    objImage = System.Drawing.Image.FromFile(strImage)

                    ' scale the image to prevent distortion
                    If objImage.Height > objImage.Width Then
                        'The height was larger, so scale the width 
                        dblScale = intSize / objImage.Height
                        intHeight = intSize
                        intWidth = CInt(objImage.Width * dblScale)
                    Else
                        'The width was larger, so scale the height 
                        dblScale = intSize / objImage.Width
                        intWidth = intSize
                        intHeight = CInt(objImage.Height * dblScale)
                    End If

                    ' create the thumbnail image
                    Dim objThumbnail As System.Drawing.Image
                    objThumbnail = objImage.GetThumbnailImage(intWidth, intHeight, Nothing, IntPtr.Zero)

                    ' delete the old file ( if it exists )
                    If File.Exists(strThumbnail) Then
                        File.Delete(strThumbnail)
                    End If

                    ' save the thumbnail image 
                    objThumbnail.Save(strThumbnail, objImage.RawFormat)

                    ' set the file attributes
                    File.SetAttributes(strThumbnail, FileAttributes.Normal)
                    File.SetLastWriteTime(strThumbnail, File.GetLastWriteTime(strImage))

                    ' tidy up
                    objImage.Dispose()
                    objThumbnail.Dispose()

                Catch

                    ' problem creating thumbnail

                End Try

            End If

            strThumbnail = Common.Globals.ApplicationPath & "/" & strThumbnail.Substring(strThumbnail.IndexOf("portals\"))
            strThumbnail = strThumbnail.Replace("\", "/")

            ' return thumbnail filename
            Return strThumbnail

        End Function

        Private Function GetSkinPath(ByVal Type As String, ByVal Root As String, ByVal Name As String) As String
            Dim strPath As String = Null.NullString

            Select Case Type
                Case "G"    ' global
                    strPath = Common.Globals.HostPath & Root & "/" & Name
                Case "L"    ' local
                    strPath = PortalSettings.HomeDirectory & Root & "/" & Name
            End Select

            Return strPath
        End Function

        Private Function IsFallbackContainer(ByVal skinPath As String) As Boolean
            Dim strDefaultContainerPath As String = (Common.Globals.HostMapPath & SkinController.RootContainer + SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo).Folder).Replace("/", "\")
            If strDefaultContainerPath.EndsWith("\") Then
                strDefaultContainerPath = strDefaultContainerPath.Substring(0, strDefaultContainerPath.Length - 1)
            End If
            Return skinPath.IndexOf(strDefaultContainerPath, StringComparison.CurrentCultureIgnoreCase) <> -1
        End Function

        Private Function IsFallbackSkin(ByVal skinPath As String) As Boolean
            Dim strDefaultSkinPath As String = (Common.Globals.HostMapPath & SkinController.RootSkin + SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo).Folder).Replace("/", "\")
            If strDefaultSkinPath.EndsWith("\") Then
                strDefaultSkinPath = strDefaultSkinPath.Substring(0, strDefaultSkinPath.Length - 1)
            End If
            Return skinPath.ToLowerInvariant = strDefaultSkinPath.ToLowerInvariant
        End Function

        Private Sub LoadCombos()

            cboSkins.Items.Clear()
            CurrentSkin = NotSpecified
            cboSkins.Items.Add(CurrentSkin)

            ' load host skins
            If chkHost.Checked Then
                AddSkinstoCombo(cboSkins, Request.MapPath(Common.Globals.HostPath & SkinController.RootSkin))
            End If

            ' load portal skins
            If chkSite.Checked Then
                AddSkinstoCombo(cboSkins, PortalSettings.HomeDirectoryMapPath & SkinController.RootSkin)
            End If

            cboContainers.Items.Clear()
            CurrentContainer = NotSpecified
            cboContainers.Items.Add(CurrentContainer)

            ' load host containers
            If chkHost.Checked Then
                AddSkinstoCombo(cboContainers, Request.MapPath(Common.Globals.HostPath & SkinController.RootContainer))
            End If

            ' load portal containers
            If chkSite.Checked Then
                AddSkinstoCombo(cboContainers, PortalSettings.HomeDirectoryMapPath & SkinController.RootContainer)
            End If

        End Sub

        Private Function ParseSkinPackage(ByVal strType As String, ByVal strRoot As String, ByVal strName As String, ByVal strFolder As String, ByVal strParse As String) As String
            Dim strRootPath As String = Null.NullString
            Select Case strType
                Case "G"    ' global
                    strRootPath = Request.MapPath(Common.Globals.HostPath)
                Case "L"    ' local
                    strRootPath = Request.MapPath(PortalSettings.HomeDirectory)
            End Select

            Dim objSkinFiles As New UI.Skins.SkinFileProcessor(strRootPath, strRoot, strName)
            Dim arrSkinFiles As New ArrayList

            Dim strFile As String
            Dim arrFiles As String()

            If Directory.Exists(strFolder) Then
                arrFiles = Directory.GetFiles(strFolder)
                For Each strFile In arrFiles
                    Select Case Path.GetExtension(strFile)
                        Case ".htm", ".html", ".css"
                            If strFile.ToLower.IndexOf(glbAboutPage.ToLower) < 0 Then
                                arrSkinFiles.Add(strFile)
                            End If
                        Case ".ascx"
                            If File.Exists(strFile.Replace(".ascx", ".htm")) = False And File.Exists(strFile.Replace(".ascx", ".html")) = False Then
                                arrSkinFiles.Add(strFile)
                            End If
                    End Select
                Next
            End If

            Select Case strParse
                Case "L"    ' localized
                    Return objSkinFiles.ProcessList(arrSkinFiles, SkinParser.Localized)
                Case "P"    ' portable
                    Return objSkinFiles.ProcessList(arrSkinFiles, SkinParser.Portable)
            End Select

            Return Null.NullString
        End Function

        Private Sub ProcessSkins(ByVal strFolderPath As String, ByVal type As String)
            Dim tbl As HtmlTable
            Dim row As HtmlTableRow = Nothing
            Dim cell As HtmlTableCell

            Dim strFile As String
            Dim strFolder As String
            Dim arrFiles As String()
            Dim strGallery As String = ""
            Dim strSkinType As String = ""
            Dim strRootSkin As String = ""
            Dim strURL As String
            Dim intIndex As Integer = 0
            Dim fallbackSkin As Boolean

            If Directory.Exists(strFolderPath) Then
                If type = "Skin" Then
                    tbl = tblSkins
                    strRootSkin = SkinController.RootSkin.ToLower()
                    fallbackSkin = IsFallbackSkin(strFolderPath)
                Else
                    tbl = tblContainers
                    strRootSkin = SkinController.RootContainer.ToLower()
                    fallbackSkin = IsFallbackContainer(strFolderPath)
                End If

                If strFolderPath.ToLower.IndexOf(Common.Globals.HostMapPath.ToLower) <> -1 Then
                    strSkinType = "G"
                Else
                    strSkinType = "L"
                End If

                Dim canDeleteSkin As Boolean = SkinController.CanDeleteSkin(strFolderPath, PortalSettings.HomeDirectoryMapPath)

                If fallbackSkin Or Not canDeleteSkin Then
                    row = New HtmlTableRow()
                    cell = New HtmlTableCell
                    cell.ColSpan = 3
                    cell.Attributes("class") = "NormalRed"
                    If type = "Skin" Then
                        cell.InnerText = Localization.GetString("CannotDeleteSkin.ErrorMessage", Me.LocalResourceFile)
                    Else
                        cell.InnerText = Localization.GetString("CannotDeleteContainer.ErrorMessage", Me.LocalResourceFile)
                    End If
                    row.Cells.Add(cell)
                    tbl.Rows.Add(row)

                    cmdDelete.Visible = False
                End If

                arrFiles = Directory.GetFiles(strFolderPath, "*.ascx")
                If arrFiles.Length = 0 Then
                    row = New HtmlTableRow()
                    cell = New HtmlTableCell
                    cell.ColSpan = 3
                    cell.Attributes("class") = "NormalRed"
                    If type = "Skin" Then
                        cell.InnerText = Localization.GetString("NoSkin.ErrorMessage", Me.LocalResourceFile)
                    Else
                        cell.InnerText = Localization.GetString("NoContainer.ErrorMessage", Me.LocalResourceFile)
                    End If
                    row.Cells.Add(cell)
                    tbl.Rows.Add(row)
                End If

                strFolder = Mid(strFolderPath, InStrRev(strFolderPath, "\") + 1)
                For Each strFile In arrFiles
                    strFile = strFile.ToLower()
                    intIndex += 1
                    If intIndex = 4 Then
                        intIndex = 1
                    End If
                    If intIndex = 1 Then
                        'Create new row
                        row = New HtmlTableRow()
                        tbl.Rows.Add(row)
                    End If

                    cell = New HtmlTableCell
                    cell.Align = "center"
                    cell.VAlign = "bottom"
                    cell.Attributes("class") = "NormalBold"

                    ' name
                    Dim label As New Label
                    label.Text = Path.GetFileNameWithoutExtension(strFile)
                    cell.Controls.Add(label)
                    cell.Controls.Add(New LiteralControl("<br/>"))

                    ' thumbnail
                    If File.Exists(strFile.Replace(".ascx", ".jpg")) Then
                        Dim imgLink As New HyperLink
                        strURL = strFile.Substring(strFile.LastIndexOf("\portals\"))
                        imgLink.NavigateUrl = ResolveUrl("~" + strURL.Replace(".ascx", ".jpg"))
                        imgLink.Target = "_new"


                        Dim img As New System.Web.UI.WebControls.Image
                        img.ImageUrl = CreateThumbnail(strFile.Replace(".ascx", ".jpg"))
                        img.BorderWidth = New Unit(1)

                        imgLink.Controls.Add(img)
                        cell.Controls.Add(imgLink)
                    Else
                        Dim img As New System.Web.UI.WebControls.Image
                        img.ImageUrl = ResolveUrl("~/images/thumbnail.jpg")
                        img.BorderWidth = New Unit(1)

                        cell.Controls.Add(img)
                    End If
                    cell.Controls.Add(New LiteralControl("<br/>"))

                    '' options 
                    strURL = strFile.Substring(strFile.IndexOf("\" & strRootSkin & "\"))
                    strURL.Replace(".ascx", "")

                    Dim previewLink As New HyperLink()
                    
                    If type = "Skin" Then
                        previewLink.NavigateUrl = NavigateURL(PortalSettings.HomeTabId, Null.NullString, "SkinSrc=" & "[" & strSkinType & "]" & QueryStringEncode(strURL.Replace(".ascx", "").Replace("\", "/")))
                    Else
                        previewLink.NavigateUrl = NavigateURL(PortalSettings.HomeTabId, Null.NullString, "ContainerSrc=" & "[" & strSkinType & "]" & QueryStringEncode(strURL.Replace(".ascx", "").Replace("\", "/")))
                    End If
                    previewLink.CssClass = "CommandButton"
                    previewLink.Target = "_new"
                    previewLink.Text = Localization.GetString("cmdPreview", Me.LocalResourceFile)
                    cell.Controls.Add(previewLink)

                    cell.Controls.Add(New LiteralControl("&nbsp;&nbsp;|&nbsp;&nbsp;"))

                    Dim applyButton As New LinkButton
                    applyButton.Text = Localization.GetString("cmdApply", Me.LocalResourceFile)
                    applyButton.CommandName = "Apply" & type
                    applyButton.CommandArgument = "[" & strSkinType & "]" & strRootSkin & "/" & strFolder & "/" & Path.GetFileName(strFile)
                    applyButton.CssClass = "CommandButton"
                    AddHandler applyButton.Command, AddressOf OnCommand
                    cell.Controls.Add(applyButton)

                    If (UserInfo.IsSuperUser = True Or strSkinType = "L") AndAlso (Not fallbackSkin And canDeleteSkin) Then
                        cell.Controls.Add(New LiteralControl("&nbsp;&nbsp;|&nbsp;&nbsp;"))

                        Dim deleteButton As New LinkButton
                        deleteButton.Text = Localization.GetString("cmdDelete")
                        deleteButton.CommandName = "Delete"
                        deleteButton.CommandArgument = "[" & strSkinType & "]" & strRootSkin & "/" & strFolder & "/" & Path.GetFileName(strFile)
                        deleteButton.CssClass = "CommandButton"
                        AddHandler deleteButton.Command, AddressOf OnCommand
                        cell.Controls.Add(deleteButton)
                    End If

                    row.Cells.Add(cell)
                Next

                If File.Exists(strFolderPath & "/" & glbAboutPage) Then
                    row = New HtmlTableRow()
                    cell = New HtmlTableCell
                    cell.ColSpan = 3
                    cell.Align = "center"
                    cell.BgColor = "#CCCCCC"
                    strFile = strFolderPath & "/" & glbAboutPage
                    strURL = strFile.Substring(strFile.IndexOf("\portals\"))

                    Dim copyrightLink As New HyperLink()
                    copyrightLink.NavigateUrl = ResolveUrl("~" + strURL)
                    copyrightLink.CssClass = "CommandButton"
                    copyrightLink.Target = "_new"
                    copyrightLink.Text = String.Format(Localization.GetString("About", Me.LocalResourceFile), strFolder)
                    cell.Controls.Add(copyrightLink)

                    row.Cells.Add(cell)
                    tbl.Rows.Add(row)
                End If
            End If

        End Sub

        Private Sub SetContainer(ByVal strContainer As String)
            If Not cboContainers.Items.FindByValue(CurrentContainer) Is Nothing Then
                cboContainers.Items.FindByValue(CurrentContainer).Selected = False
            End If
            If Not cboContainers.Items.FindByValue(strContainer) Is Nothing Then
                cboContainers.Items.FindByValue(strContainer).Selected = True
                CurrentContainer = strContainer
            Else
                CurrentContainer = NotSpecified
            End If
        End Sub

        Private Sub SetSkin(ByVal strSkin As String)
            If Not cboSkins.Items.FindByValue(CurrentSkin) Is Nothing Then
                cboSkins.Items.FindByValue(CurrentSkin).Selected = False
            End If
            If Not cboSkins.Items.FindByValue(strSkin) Is Nothing Then
                cboSkins.Items.FindByValue(strSkin).Selected = True
                CurrentSkin = strSkin
            Else
                CurrentSkin = NotSpecified
            End If
        End Sub

        Private Sub ShowContainers()
            tblContainers.Rows.Clear()
            Dim intPortalId As Integer = PortalId

            Dim strContainerPath As String = ApplicationMapPath.ToLower + cboContainers.SelectedItem.Value
            If strContainerPath.ToLowerInvariant.Contains(Common.Globals.HostMapPath.ToLowerInvariant) Then
                intPortalId = Null.NullInteger
            End If
            Dim skinPackage As SkinPackageInfo = SkinController.GetSkinPackage(intPortalId, cboContainers.SelectedItem.Text, "Container")
            If skinPackage Is Nothing AndAlso Not lblLegacy.Visible Then
                lblLegacy.Visible = (cboContainers.SelectedIndex > 0)
            End If

            If cboContainers.SelectedIndex > 0 Then
                ProcessSkins(strContainerPath, "Container")
                pnlSkin.Visible = True
                If UserInfo.IsSuperUser Or strContainerPath.IndexOf(Common.Globals.HostMapPath.ToLower) = -1 Then
                    cmdParse.Visible = True
                    pnlParse.Visible = True
                Else
                    cmdParse.Visible = False
                    pnlParse.Visible = False
                End If
            Else
                pnlSkin.Visible = False
                pnlParse.Visible = False
            End If
        End Sub

        Private Sub ShowSkins()
            tblSkins.Rows.Clear()
            Dim intPortalId As Integer = PortalId

            Dim strSkinPath As String = ApplicationMapPath.ToLower + cboSkins.SelectedItem.Value
            If strSkinPath.ToLowerInvariant.Contains(Common.Globals.HostMapPath.ToLowerInvariant) Then
                intPortalId = Null.NullInteger
            End If
            Dim skinPackage As SkinPackageInfo = SkinController.GetSkinPackage(intPortalId, cboSkins.SelectedItem.Text, "Skin")

            If skinPackage Is Nothing Then
                lblLegacy.Visible = (cboSkins.SelectedIndex > 0)
            End If
            If cboSkins.SelectedIndex > 0 Then
                ProcessSkins(strSkinPath, "Skin")
                pnlSkin.Visible = True
                If UserInfo.IsSuperUser Or strSkinPath.IndexOf(Common.Globals.HostMapPath.ToLower) = -1 Then
                    cmdParse.Visible = True
                    pnlParse.Visible = True
                Else
                    cmdParse.Visible = False
                    pnlParse.Visible = False
                End If
            Else
                pnlSkin.Visible = False
                pnlParse.Visible = False
            End If
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim strSkin As String = Null.NullString
            Dim strContainer As String = Null.NullString

            Try
                cmdDelete.Visible = True

                If Page.IsPostBack = False Then
                    LoadCombos()
                End If

                If PortalSettings.ActiveTab.IsSuperTab Then
                    typeRow.Visible = False
                Else
                    typeRow.Visible = True
                End If

                If Not Page.IsPostBack Then
                    Dim strURL As String

                    If Not Request.QueryString("Name") Is Nothing Then
                        strURL = Request.MapPath(GetSkinPath(Convert.ToString(Request.QueryString("Type")), Convert.ToString(Request.QueryString("Root")), Convert.ToString(Request.QueryString("Name"))))
                        strSkin = strURL.Replace(ApplicationMapPath, "").ToLowerInvariant()
                    Else
                        'Get the current portal skin
                        Dim skinSrc As String
                        If Not String.IsNullOrEmpty(PortalSettings.DefaultPortalSkin) Then
                            skinSrc = PortalSettings.DefaultPortalSkin
                        Else
                            skinSrc = SkinController.GetDefaultPortalSkin
                        End If
                        strURL = Request.MapPath(SkinController.FormatSkinPath(SkinController.FormatSkinSrc(skinSrc, PortalSettings)))
                        strURL = strURL.Substring(0, strURL.LastIndexOf("\"))
                        strSkin = strURL.Replace(ApplicationMapPath, "").ToLowerInvariant()
                    End If

                    If Not String.IsNullOrEmpty(strSkin) Then
                        strContainer = strSkin.Replace("\" & SkinController.RootSkin.ToLowerInvariant & "\", "\" & SkinController.RootContainer.ToLowerInvariant & "\")
                    End If

                    SetSkin(strSkin)
                    SetContainer(strContainer)
                Else
                    strSkin = cboSkins.SelectedValue
                    strContainer = cboContainers.SelectedValue
                    If strSkin <> CurrentSkin Then
                        strContainer = strSkin.Replace("\" & SkinController.RootSkin.ToLowerInvariant & "\", "\" & SkinController.RootContainer.ToLowerInvariant & "\")
                        SetSkin(strSkin)
                        SetContainer(strContainer)
                    ElseIf strContainer <> CurrentContainer Then
                        SetSkin(NotSpecified)
                        SetContainer(strContainer)
                    End If
                End If

                ShowSkins()
                ShowContainers()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub chkHost_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkHost.CheckedChanged
            LoadCombos()

            ShowSkins()
            ShowContainers()
        End Sub

        Private Sub chkSite_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkSite.CheckedChanged
            LoadCombos()

            ShowSkins()
            ShowContainers()
        End Sub

        Private Sub OnCommand(ByVal sender As System.Object, ByVal e As CommandEventArgs)
            Try
                Dim strSrc As String = e.CommandArgument
                Dim redirectUrl As String = NavigateURL(TabId)
                Select Case e.CommandName
                    Case "ApplyContainer"
                        If chkPortal.Checked Then
                            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Portal, strSrc)
                        End If
                        If chkAdmin.Checked Then
                            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Admin, strSrc)
                        End If
                    Case "ApplySkin"
                        If chkPortal.Checked Then
                            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Portal, strSrc)
                        End If
                        If chkAdmin.Checked Then
                            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Admin, strSrc)
                        End If
                        DataCache.ClearPortalCache(PortalId, True)
                    Case "Delete"
                        File.Delete(Request.MapPath(SkinController.FormatSkinSrc(strSrc, PortalSettings)))
                        DataCache.ClearPortalCache(PortalId, True)
                        Dim strType As String = "G"
                        If strSrc.StartsWith("[L]") Then strType = "L"
                        Dim strRoot As String = strSrc.Substring(3, strSrc.IndexOf("/") - 3)

                        Dim strFolder As String = strSrc.Substring(strSrc.IndexOf("/") + 1, InStrRev(strSrc, "/") - strSrc.IndexOf("/") - 2)
                        redirectUrl = NavigateURL(TabId, "", "Type=" & strType, "Root=" & strRoot, "Name=" & strFolder)
                End Select

                Response.Redirect(redirectUrl, True)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdDelete_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdDelete.Click

            Dim failure As Boolean = False
            Dim strSkinPath As String = ApplicationMapPath.ToLower + cboSkins.SelectedItem.Value
            Dim strContainerPath As String = ApplicationMapPath.ToLower + cboContainers.SelectedItem.Value

            Dim strMessage As String

            If UserInfo.IsSuperUser = False And cboSkins.SelectedItem.Value.IndexOf("\portals\_default\", 0) <> -1 Then
                strMessage = Services.Localization.Localization.GetString("SkinDeleteFailure", Me.LocalResourceFile)
                UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                failure = True
            Else
                If cboSkins.SelectedIndex > 0 Then
                    Dim skinPackage As SkinPackageInfo = SkinController.GetSkinPackage(PortalId, cboSkins.SelectedItem.Text, "Skin")
                    If skinPackage IsNot Nothing Then
                        strMessage = Services.Localization.Localization.GetString("UsePackageUnInstall", Me.LocalResourceFile)
                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        Exit Sub
                    Else
                        If Directory.Exists(strSkinPath) Then
                            Common.Globals.DeleteFolderRecursive(strSkinPath)
                        End If
                        If Directory.Exists(strSkinPath.Replace("\" & SkinController.RootSkin.ToLower & "\", "\" & SkinController.RootContainer & "\")) Then
                            Common.Globals.DeleteFolderRecursive(strSkinPath.Replace("\" & SkinController.RootSkin.ToLower & "\", "\" & SkinController.RootContainer & "\"))
                        End If
                    End If
                ElseIf cboContainers.SelectedIndex > 0 Then
                    Dim skinPackage As SkinPackageInfo = SkinController.GetSkinPackage(PortalId, cboContainers.SelectedItem.Text, "Container")
                    If skinPackage IsNot Nothing Then
                        strMessage = Services.Localization.Localization.GetString("UsePackageUnInstall", Me.LocalResourceFile)
                        UI.Skins.Skin.AddModuleMessage(Me, strMessage, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                        Exit Sub
                    Else
                        If Directory.Exists(strContainerPath) Then
                            Common.Globals.DeleteFolderRecursive(strContainerPath)
                        End If
                    End If
                End If
            End If

            If Not failure Then
                LoadCombos()
                ShowSkins()
                ShowContainers()
            End If
        End Sub

        Private Sub cmdParse_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdParse.Click

            Dim strFolder As String
            Dim strType As String
            Dim strRoot As String
            Dim strName As String
            Dim strSkinPath As String = ApplicationMapPath.ToLower + cboSkins.SelectedItem.Value
            Dim strContainerPath As String = ApplicationMapPath.ToLower + cboContainers.SelectedItem.Value
            Dim strParse As String = ""

            If cboSkins.SelectedIndex > 0 Then
                strFolder = strSkinPath
                If strFolder.IndexOf(Common.Globals.HostMapPath.ToLower) <> -1 Then
                    strType = "G"
                Else
                    strType = "L"
                End If
                strRoot = SkinController.RootSkin
                strName = cboSkins.SelectedItem.Text
                strParse += ParseSkinPackage(strType, strRoot, strName, strFolder, optParse.SelectedItem.Value)

                strFolder = strSkinPath.Replace("\" & SkinController.RootSkin.ToLower & "\", "\" & SkinController.RootContainer.ToLower & "\")
                strRoot = SkinController.RootContainer
                strParse += ParseSkinPackage(strType, strRoot, strName, strFolder, optParse.SelectedItem.Value)
                DataCache.ClearPortalCache(PortalId, True)
            End If

            If cboContainers.SelectedIndex > 0 Then
                strFolder = strContainerPath
                If strFolder.IndexOf(Common.Globals.HostMapPath.ToLower) <> -1 Then
                    strType = "G"
                Else
                    strType = "L"
                End If
                strRoot = SkinController.RootContainer
                strName = cboContainers.SelectedItem.Text
                strParse += ParseSkinPackage(strType, strRoot, strName, strFolder, optParse.SelectedItem.Value)
                DataCache.ClearPortalCache(PortalId, True)
            End If

            lblOutput.Text = strParse

            If cboSkins.SelectedIndex > 0 Then
                ShowSkins()
            Else
                If cboContainers.SelectedIndex > 0 Then
                    ShowContainers()
                End If
            End If

        End Sub

        Private Sub cmdRestore_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRestore.Click
            Dim objSkins As New UI.Skins.SkinController
            If chkPortal.Checked Then
                SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Portal, "")
                SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Portal, "")
            End If
            If chkAdmin.Checked Then
                SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Admin, "")
                SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Admin, "")
            End If
            DataCache.ClearPortalCache(PortalId, True)
            Response.Redirect(Request.RawUrl)
        End Sub

#End Region

    End Class

End Namespace