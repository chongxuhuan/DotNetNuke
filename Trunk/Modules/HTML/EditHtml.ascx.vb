'
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2011
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

Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Portals
Imports Telerik.Web.UI
Imports DotNetNuke.Common.Utilities

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The EditHtml PortalModuleBase is used to manage Html
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Public Class EditHtml
        Inherits Entities.Modules.PortalModuleBase

        Private _htmlTextController As New HtmlTextController
        Private _workflowStateController As New WorkflowStateController
        Private _htmlTextLogController As New HtmlTextLogController
        Private _moduleController As New ModuleController

        Private Enum WorkflowType
            DirectPublish = 1
            ContentStaging = 2
        End Enum

#Region "Private Properties"

        Private ReadOnly Property WorkflowID() As Integer
            Get
                Dim _workflowID As Integer = -1

                If ViewState("WorkflowID") Is Nothing Then
                    _workflowID = _htmlTextController.GetWorkflow(ModuleId, TabId, PortalId).Value
                    ViewState.Add("WorkflowID", _workflowID)
                Else
                    _workflowID = Integer.Parse(ViewState("WorkflowID"))
                End If

                Return _workflowID
            End Get
        End Property

      Private Property LockedByUserID() As Integer
            Get
                Dim _userID As Integer = -1
                If (Me.Settings("Content_LockedBy")) IsNot Nothing Then
                    _userID = Integer.Parse(Me.Settings("Content_LockedBy"))
                End If

                Return _userID
            End Get
            Set(ByVal value As Integer)
                Me.Settings("Content_LockedBy") = value
            End Set
        End Property

        Private Property TempContent() As String
            Get
                Dim _content As String = ""
                If Not ViewState("TempContent") Is Nothing Then _content = ViewState("TempContent")
                Return _content
            End Get
            Set(ByVal value As String)
                ViewState("TempContent") = value
            End Set
        End Property

        Private Property CurrentWorkflowType() As WorkflowType
            Get
                Dim _currentWorkflowType As WorkflowType
                If Not ViewState("_currentWorkflowType") Is Nothing Then
                    _currentWorkflowType = [Enum].Parse(_currentWorkflowType.GetType(), ViewState("_currentWorkflowType"))
                End If

                Return _currentWorkflowType
            End Get
            Set(ByVal value As WorkflowType)
                ViewState("_currentWorkflowType") = value
            End Set
        End Property


#End Region

#Region "Private Methods"

        ''' <summary>
        ''' Displays the history of an html content item in a grid in the preview section.
        ''' </summary>
        ''' <param name="htmlContent">Content of the HTML.</param>
        Private Sub DisplayHistory(ByVal htmlContent As HtmlTextInfo)
            tblHistory.Visible = Not (CurrentWorkflowType = WorkflowType.DirectPublish)
            dshHistory.Visible = Not (CurrentWorkflowType = WorkflowType.DirectPublish)

            If ((CurrentWorkflowType = WorkflowType.DirectPublish)) Then
                Return
            Else
                Dim htmlLogging As ArrayList = _htmlTextLogController.GetHtmlTextLog(htmlContent.ItemID)
                grdLog.DataSource = htmlLogging
                grdLog.DataBind()

                tblHistory.Visible = Not (htmlLogging.Count = 0)
                dshHistory.Visible = Not (htmlLogging.Count = 0)
            End If
        End Sub

        ''' <summary>
        ''' Displays the versions of the html content in the versions section
        ''' </summary>
        Private Sub DisplayVersions()
            grdVersions.DataSource = _htmlTextController.GetAllHtmlText(ModuleId)
            grdVersions.DataBind()
        End Sub

        ''' <summary>
        ''' Displays the content of the master language if localized content is enabled.
        ''' </summary>
        Private Sub DisplayMasterLanguageContent()
            'Get master language
            Dim objModule As ModuleInfo = New ModuleController().GetModule(ModuleId, TabId)
            If objModule.DefaultLanguageModule IsNot Nothing Then
                Dim masterContent As HtmlTextInfo = _htmlTextController.GetTopHtmlText(objModule.DefaultLanguageModule.ModuleID, False, WorkflowID)
                If masterContent IsNot Nothing Then
                    placeMasterContent.Controls.Add(New LiteralControl(HtmlTextController.FormatHtmlText(objModule.DefaultLanguageModule.ModuleID, FormatContent(masterContent.Content), Settings)))
                End If
            End If

            dshMaster.Visible = objModule.DefaultLanguageModule IsNot Nothing
            tblMaster.Visible = objModule.DefaultLanguageModule IsNot Nothing
        End Sub

        ''' <summary>
        ''' Displays the html content in the preview section.
        ''' </summary>
        ''' <param name="htmlContent">Content of the HTML.</param>
        Private Sub DisplayContent(ByVal htmlContent As HtmlTextInfo)
            lblCurrentWorkflowInUse.Text = GetLocalizedString(htmlContent.WorkflowName)
            lblCurrentWorkflowState.Text = GetLocalizedString(htmlContent.StateName)
            lblCurrentVersion.Text = htmlContent.Version
            txtContent.Text = FormatContent(htmlContent.Content)

            DisplayMasterLanguageContent()
        End Sub

        ''' <summary>
        ''' Displays the content preview in the preview section
        ''' </summary>
        ''' <param name="htmlContent">Content of the HTML.</param>
        Private Sub DisplayPreview(ByVal htmlContent As HtmlTextInfo)
            lblPreviewVersion.Text = htmlContent.Version.ToString()
            lblPreviewWorkflowInUse.Text = GetLocalizedString(htmlContent.WorkflowName)
            lblPreviewWorkflowState.Text = GetLocalizedString(htmlContent.StateName)
            litPreview.Text = HtmlTextController.FormatHtmlText(ModuleId, htmlContent.Content, Settings)
            DisplayHistory(htmlContent)

        End Sub

        ''' <summary>
        ''' Displays the preview in the preview section
        ''' </summary>
        ''' <param name="htmlContent">Content of the HTML.</param>
        Private Sub DisplayPreview(ByVal htmlContent As String)
            litPreview.Text = HtmlTextController.FormatHtmlText(ModuleId, htmlContent, Settings)
            rowPreviewVersion.Visible = False
            rowPreviewWorlflow.Visible = False

            rowPreviewWorkflowState.Visible = True
            lblPreviewWorkflowState.Text = GetLocalizedString("EditPreviewState")
            dshPreview.IsExpanded = True
        End Sub

        ''' <summary>
        ''' Displays the content but hide the editor if editing is locked from the current user
        ''' </summary>
        ''' <param name="htmlContent">Content of the HTML.</param>
        ''' <param name="lastPublishedContent">Last content of the published.</param>
        Private Sub DisplayLockedContent(ByVal htmlContent As HtmlTextInfo, ByVal lastPublishedContent As HtmlTextInfo)

            txtContent.Visible = False
            cmdSave.Visible = False
            cmdPreview.Visible = False
            rowPublish.Visible = False

            rowSubmittedContent.Visible = True
            dshPreview.IsExpanded = True

            lblCurrentWorkflowInUse.Text = GetLocalizedString(htmlContent.WorkflowName)
            lblCurrentWorkflowState.Text = GetLocalizedString(htmlContent.StateName)

            litCurrentContentPreview.Text = HtmlTextController.FormatHtmlText(ModuleId, htmlContent.Content, Settings)
            lblCurrentVersion.Text = htmlContent.Version
            DisplayVersions()

            If Not lastPublishedContent Is Nothing Then
                DisplayPreview(lastPublishedContent)
                DisplayHistory(lastPublishedContent)
            Else
                tblHistory.Visible = False
                dshHistory.Visible = False
                DisplayPreview(htmlContent.Content)
            End If



        End Sub

        ''' <summary>
        ''' Displays the initial content when a module is first added to the page.
        ''' </summary>
        ''' <param name="firstState">The first state.</param>
        Private Sub DisplayInitialContent(ByVal firstState As WorkflowStateInfo)
            txtContent.Text = GetLocalizedString("AddContent")
            litPreview.Text = GetLocalizedString("AddContent")
            lblCurrentWorkflowInUse.Text = firstState.WorkflowName
            lblPreviewWorkflowInUse.Text = firstState.WorkflowName
            rowPreviewVersion.Visible = False

            dshVersions.Visible = False
            tblVersions.Visible = False

            dshHistory.Visible = False
            tblHistory.Visible = False

            rowCurrentWorkflowState.Visible = False
            rowCurrentVersion.Visible = False
            rowPreviewWorkflowState.Visible = False

            lblPreviewWorkflowState.Text = firstState.StateName
        End Sub

#End Region

#Region "Private Functions"

        ''' <summary>
        ''' Formats the content to make it html safe.
        ''' </summary>
        ''' <param name="htmlContent">Content of the HTML.</param>
        ''' <returns></returns>
        Private Function FormatContent(ByVal htmlContent As String) As String
            Dim strContent As String = HttpUtility.HtmlDecode(htmlContent)
            strContent = HtmlTextController.ManageRelativePaths(strContent, PortalSettings.HomeDirectory, "src", PortalId)
            strContent = HtmlTextController.ManageRelativePaths(strContent, PortalSettings.HomeDirectory, "background", PortalId)
            Return HttpUtility.HtmlEncode(strContent)
        End Function

        ''' <summary>
        ''' Gets the localized string from a resource file if it exists.
        ''' </summary>
        ''' <param name="str">The STR.</param>
        ''' <returns></returns>
        Private Function GetLocalizedString(ByVal str As String) As String
            Dim localizedString As String = Localization.GetString(str, Me.LocalResourceFile)
            Return IIf(String.IsNullOrEmpty(localizedString), str, localizedString)
        End Function

        ''' <summary>
        ''' Gets the latest html content of the module
        ''' </summary>
        ''' <returns></returns>
        Private Function GetLatestHTMLContent() As HtmlTextInfo
            Dim htmlContent As HtmlTextInfo = _htmlTextController.GetTopHtmlText(ModuleId, False, WorkflowID)
            If htmlContent Is Nothing Then
                htmlContent = New HtmlTextInfo
                htmlContent.ItemID = -1
                htmlContent.StateID = _workflowStateController.GetFirstWorkflowStateID(WorkflowID)
                htmlContent.WorkflowID = WorkflowID
                htmlContent.ModuleID = ModuleId
            End If

            Return htmlContent
        End Function

        ''' <summary>
        ''' Returns whether or not the user has review permissions to this module
        ''' </summary>
        ''' <param name="htmlContent">Content of the HTML.</param>
        ''' <returns></returns>
        Private Function UserCanReview(ByVal htmlContent As HtmlTextInfo) As Boolean

            If Not htmlContent Is Nothing Then
                Return WorkflowStatePermissionController.HasWorkflowStatePermission(WorkflowStatePermissionController.GetWorkflowStatePermissions(htmlContent.StateID), "REVIEW")
            Else
                Return False
            End If

        End Function

        ''' <summary>
        ''' Gets the last published version of this module
        ''' </summary>
        ''' <param name="publishedStateID">The published state ID.</param>
        ''' <returns></returns>
        Private Function GetLastPublishedVersion(ByVal publishedStateID As Integer) As HtmlTextInfo
            Return (From version In _htmlTextController.GetAllHtmlText(ModuleId) Where version.StateID = publishedStateID Order By version.Version Descending Select version)(0)
        End Function


#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            Try

                Dim htmlContentItemID As Integer = -1
                Dim htmlContent As HtmlTextInfo = _htmlTextController.GetTopHtmlText(ModuleId, False, WorkflowID)

                If Not htmlContent Is Nothing Then
                    htmlContentItemID = htmlContent.ItemID
                End If

                If Not Page.IsPostBack Then
                    Dim WorkflowStates As ArrayList = _workflowStateController.GetWorkflowStates(WorkflowID)
                    Dim MaxVersions As Integer = _htmlTextController.GetMaximumVersionHistory(PortalId)
                    Dim UserCanEdit As Boolean = UserInfo.IsSuperUser Or PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Or UserInfo.UserID = LockedByUserID

                    lblMaxVersions.Text = MaxVersions
                    grdVersions.PageSize = IIf(MaxVersions < 10, MaxVersions, 10)

                    Select Case WorkflowStates.Count
                        Case 1
                            CurrentWorkflowType = WorkflowType.DirectPublish
                        Case 2
                            CurrentWorkflowType = WorkflowType.ContentStaging
                    End Select

                    If htmlContentItemID <> -1 Then
                        DisplayContent(htmlContent)
                        DisplayPreview(htmlContent)
                    Else
                        DisplayInitialContent(WorkflowStates(0))
                    End If

                    rowPublish.Visible = Not (CurrentWorkflowType = WorkflowType.DirectPublish)
                    DisplayVersions()
                End If

            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
            Try
                Response.Redirect(NavigateURL(), True)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
            Dim redirect As Boolean = True

            Try
                ' get content
                Dim htmlContent As HtmlTextInfo = GetLatestHTMLContent()

                Dim pac As New PortalAliasController
                Dim aliases = From pa As PortalAliasInfo In pac.GetPortalAliasByPortalID(PortalSettings.PortalId).Values
                                Select pa.HTTPAlias
                htmlContent.Content = HtmlUtils.AbsoluteToRelativeUrls(txtContent.Text, aliases)

                Dim draftStateID As Integer = _workflowStateController.GetFirstWorkflowStateID(WorkflowID)
                Dim nextWorkflowStateID As Integer = _workflowStateController.GetNextWorkflowStateID(WorkflowID, htmlContent.StateID)
                Dim publishedStateID As Integer = _workflowStateController.GetLastWorkflowStateID(WorkflowID)

                Dim UserCanUpdate As Boolean = UserInfo.IsSuperUser Or PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) Or UserInfo.UserID = LockedByUserID

                Select Case CurrentWorkflowType
                    Case WorkflowType.DirectPublish
                        _htmlTextController.UpdateHtmlText(htmlContent, _htmlTextController.GetMaximumVersionHistory(PortalId))

                    Case WorkflowType.ContentStaging
                        If chkPublish.Checked Then
                            If htmlContent.StateID = publishedStateID Then 'if it's already published set it to draft
                                htmlContent.StateID = draftStateID
                            Else
                                htmlContent.StateID = publishedStateID 'here it's in published mode
                            End If
                        Else
                            If (htmlContent.StateID <> draftStateID) Then 'if it's already published set it back to draft
                                htmlContent.StateID = draftStateID
                            End If
                        End If

                        _htmlTextController.UpdateHtmlText(htmlContent, _htmlTextController.GetMaximumVersionHistory(PortalId))
                End Select

            Catch exc As Exception
                Exceptions.LogException(exc)
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(Me.Page, "Error occurred: ", exc.Message, UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Return
            End Try

            ' redirect back to portal
            If redirect Then
                Response.Redirect(NavigateURL(), True)
            End If

        End Sub

        Private Sub cmdPreview_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdPreview.Click
            Try
                DisplayPreview(txtContent.Text)
            Catch exc As Exception
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub grdLog_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdLog.ItemDataBound
            Dim item As GridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
              item.ItemType = ListItemType.AlternatingItem Or _
              item.ItemType = ListItemType.SelectedItem Then

                'Localize columns
                item.Cells(2).Text = Localization.GetString(item.Cells(2).Text, Me.LocalResourceFile)
                item.Cells(3).Text = Localization.GetString(item.Cells(3).Text, Me.LocalResourceFile)
            End If

        End Sub

        Private Sub grdVersions_ItemCommand(ByVal source As Object, ByVal e As GridCommandEventArgs) Handles grdVersions.ItemCommand
            Try
                Dim htmlContent As HtmlTextInfo = _htmlTextController.GetHtmlText(ModuleId, Integer.Parse(e.CommandArgument.ToString))

                Select Case e.CommandName.ToLower()
                    Case "remove"
                        _htmlTextController.DeleteHtmlText(ModuleId, htmlContent.ItemID)
                    Case "rollback"
                        htmlContent.ItemID = -1
                        htmlContent.ModuleID = ModuleId
                        htmlContent.WorkflowID = WorkflowID
                        htmlContent.StateID = _workflowStateController.GetFirstWorkflowStateID(WorkflowID)
                        _htmlTextController.UpdateHtmlText(htmlContent, _htmlTextController.GetMaximumVersionHistory(PortalId))
                    Case "preview"
                        DisplayPreview(htmlContent)
                        dshHistory.IsExpanded = True
                        dshPreview.IsExpanded = True
                End Select

                If (e.CommandName.ToLower() <> "preview") Then
                    Dim latestContent As HtmlTextInfo = _htmlTextController.GetTopHtmlText(ModuleId, False, WorkflowID)
                    If latestContent Is Nothing Then
                        DisplayInitialContent(_workflowStateController.GetWorkflowStates(WorkflowID)(0))
                    Else
                        DisplayContent(latestContent)
                        DisplayPreview(latestContent)
                        DisplayVersions()
                    End If
                End If

            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub grdVersions_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdVersions.ItemDataBound
            If (e.Item.ItemType = GridItemType.Item Or e.Item.ItemType = GridItemType.AlternatingItem Or _
                e.Item.ItemType = GridItemType.SelectedItem) Then

                Dim item As GridDataItem = e.Item
                Dim htmlContent As HtmlTextInfo = item.DataItem

                Dim createdBy As String = "Default"
                If (htmlContent.CreatedByUserID <> -1) Then
                    Dim createdByByUser As UserInfo = UserController.GetUserById(PortalId, htmlContent.CreatedByUserID)
                    If (Not createdByByUser Is Nothing) Then
                        createdBy = createdByByUser.DisplayName
                    End If
                End If

                For Each cell As TableCell In item.Cells
                    For Each cellControl As Control In cell.Controls
                        If TypeOf cellControl Is ImageButton Then
                            Dim _imageButton As ImageButton = cellControl
                            _imageButton.CommandArgument = htmlContent.ItemID
                            Select Case _imageButton.CommandName.ToLower()
                                Case "rollback"
                                    If grdVersions.CurrentPageIndex = 0 Then 'hide rollback for the first item
                                        If (item.ItemIndex = 0) Then
                                            _imageButton.Visible = False
                                            Exit Select
                                        End If
                                    End If

                                    _imageButton.Visible = True

                                Case "remove"
                                    Dim msg As String = GetLocalizedString("DeleteVersion.Confirm")
                                    msg = msg.Replace("[VERSION]", htmlContent.Version).Replace("[STATE]", htmlContent.StateName).Replace("[DATECREATED]", htmlContent.CreatedOnDate.ToString()).Replace("[USERNAME]", createdBy)
                                    _imageButton.OnClientClick = "return confirm(" + Chr(34) + msg + Chr(34) + ");"
                                    'hide the delete button
                                    Dim showDelete As Boolean = UserInfo.IsSuperUser Or PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName)

                                    If Not showDelete Then
                                            showDelete = htmlContent.IsPublished = False
                                    End If

                                    _imageButton.Visible = showDelete
                            End Select
                        End If
                    Next
                Next
            End If

        End Sub

        Private Sub grdVersions_PageIndexChanged(ByVal source As Object, ByVal e As GridPageChangedEventArgs) Handles grdVersions.PageIndexChanged
            DisplayVersions()
        End Sub

#End Region




    End Class

End Namespace
