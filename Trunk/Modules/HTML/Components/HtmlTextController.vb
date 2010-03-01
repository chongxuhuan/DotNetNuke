'
' DotNetNuke� - http://www.dotnetnuke.com
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

Imports System
Imports System.Collections.Generic
Imports System.Web
Imports System.Xml

Imports DotNetNuke.Common
Imports DotNetNuke.Services.Search
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Security.Roles
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Users
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Security

Namespace DotNetNuke.Modules.Html

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.Html
    ''' Project:    DotNetNuke
    ''' Class:      HtmlTextController
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The HtmlTextController is the Controller class for managing HtmlText information the HtmlText module
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class HtmlTextController
        Implements Entities.Modules.ISearchable
        Implements Entities.Modules.IPortable
        Implements Entities.Modules.IUpgradeable

        Private Const MAX_DESCRIPTION_LENGTH As Integer = 100
        Private Const WORKFLOW_SETTING As String = "WorkflowID"

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetHtmlText gets the HtmlTextInfo object for the Module, Item, and Workflow
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The ID of the Module</param>
        ''' <param name="ItemID">The ID of the Item</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetHtmlText(ByVal ModuleID As Integer, ByVal ItemID As Integer) As HtmlTextInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetHtmlText(ModuleID, ItemID), GetType(HtmlTextInfo)), HtmlTextInfo)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetTopHtmlText gets the most recent HtmlTextInfo object for the Module, Workflow, and State
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The ID of the Module</param>
        ''' <param name="IsPublished">Whether the content has been published or not</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetTopHtmlText(ByVal ModuleID As Integer, ByVal IsPublished As Boolean, ByVal WorkflowID As Integer) As HtmlTextInfo
            Dim objHtmlText As HtmlTextInfo = CType(CBO.FillObject(DataProvider.Instance().GetTopHtmlText(ModuleID, IsPublished), GetType(HtmlTextInfo)), HtmlTextInfo)
            If Not objHtmlText Is Nothing Then
                ' check if workflow has changed
                If IsPublished = False AndAlso objHtmlText.WorkflowID <> WorkflowID Then
                    ' get proper state for workflow
                    Dim objWorkflow As New WorkflowStateController
                    objHtmlText.WorkflowID = WorkflowID
                    objHtmlText.WorkflowName = "[REPAIR_WORKFLOW]"
                    If objHtmlText.IsPublished Then
                        objHtmlText.StateID = objWorkflow.GetLastWorkflowStateID(WorkflowID)
                    Else
                        objHtmlText.StateID = objWorkflow.GetFirstWorkflowStateID(WorkflowID)
                    End If
                    ' update object
                    UpdateHtmlText(objHtmlText, GetMaximumVersionHistory(objHtmlText.PortalID))
                    ' get object again
                    objHtmlText = CType(CBO.FillObject(DataProvider.Instance().GetTopHtmlText(ModuleID, IsPublished), GetType(HtmlTextInfo)), HtmlTextInfo)
                End If
            End If
            Return objHtmlText
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAllHtmlText gets a collection of HtmlTextInfo objects for the Module and Workflow
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The ID of the Module</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetAllHtmlText(ByVal ModuleID As Integer) As List(Of HtmlTextInfo)
            Return CBO.FillCollection(Of HtmlTextInfo)(DataProvider.Instance().GetAllHtmlText(ModuleID))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateHtmlText creates a new HtmlTextInfo object or updates an existing HtmlTextInfo object
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="objHtmlText">An HtmlTextInfo object</param>
        ''' <param name="MaximumVersionHistory">The maximum number of versions to retain</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateHtmlText(ByVal objHtmlText As HtmlTextInfo, ByVal MaximumVersionHistory As Integer)
            Dim objWorkflow As New WorkflowStateController
            Dim blnCreateNewVersion As Boolean = False

            ' determine if we are creating a new version of content or updating an existing version
            If objHtmlText.ItemID <> -1 Then
                If objHtmlText.WorkflowName <> "[REPAIR_WORKFLOW]" Then
                    Dim objContent As HtmlTextInfo = GetTopHtmlText(objHtmlText.ModuleID, False, objHtmlText.WorkflowID)
                    If Not objContent Is Nothing Then
                        If objContent.StateID = objWorkflow.GetLastWorkflowStateID(objHtmlText.WorkflowID) Then
                            blnCreateNewVersion = True
                        End If
                    End If
                End If
            Else
                blnCreateNewVersion = True
            End If

            ' determine if content is published
            If objHtmlText.StateID = objWorkflow.GetLastWorkflowStateID(objHtmlText.WorkflowID) Then
                objHtmlText.IsPublished = True
            Else
                objHtmlText.IsPublished = False
            End If

            If blnCreateNewVersion Then
                ' add content
                objHtmlText.ItemID = DataProvider.Instance().AddHtmlText(objHtmlText.ModuleID, objHtmlText.Content, objHtmlText.StateID, objHtmlText.IsPublished, UserController.GetCurrentUserInfo.UserID, MaximumVersionHistory)
            Else
                ' update content
                DataProvider.Instance().UpdateHtmlText(objHtmlText.ItemID, objHtmlText.Content, objHtmlText.StateID, objHtmlText.IsPublished, UserController.GetCurrentUserInfo.UserID)
            End If

            ' add log history
            Dim objLog As New HtmlTextLogInfo
            objLog.ItemID = objHtmlText.ItemID
            objLog.StateID = objHtmlText.StateID
            objLog.Approved = objHtmlText.Approved
            objLog.Comment = objHtmlText.Comment
            Dim objLogs As New HtmlTextLogController
            objLogs.AddHtmlTextLog(objLog)

            ' create user notifications
            CreateUserNotifications(objHtmlText)

            ' refresh output cache
            ModuleController.SynchronizeModule(objHtmlText.ModuleID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteHtmlText deletes an HtmlTextInfo object for the Module and Item
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The ID of the Module</param>
        ''' <param name="ItemID">The ID of the Item</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub DeleteHtmlText(ByVal ModuleID As Integer, ByVal ItemID As Integer)
            DataProvider.Instance().DeleteHtmlText(ModuleID, ItemID)

            ' refresh output cache
            ModuleController.SynchronizeModule(ModuleID)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CreateUserNotifications creates HtmlTextUser records and optionally sends email notifications to participants in a Workflow
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="objHtmlText">An HtmlTextInfo object</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub CreateUserNotifications(ByVal objHtmlText As HtmlTextInfo)
            Dim objHTMLTextUsers As New HtmlTextUserController
            Dim objHtmlTextUser As HtmlTextUserInfo
            Dim objUser As UserInfo

            ' clean up old user notification records
            objHTMLTextUsers.DeleteHtmlTextUsers()

            ' ensure we have latest htmltext object loaded
            objHtmlText = GetHtmlText(objHtmlText.ModuleID, objHtmlText.ItemID)

            ' build collection of users to notify
            Dim objWorkflow As New WorkflowStateController
            Dim arrUsers As New ArrayList

            ' if not published
            If objHtmlText.IsPublished = False Then
                ' include content owner 
                arrUsers.Add(objHtmlText.CreatedByUserID)
            End If

            ' if not draft and not published
            If objHtmlText.StateID <> objWorkflow.GetFirstWorkflowStateID(objHtmlText.WorkflowID) And objHtmlText.IsPublished = False Then
                ' get users from permissions for state
                Dim objRoles As New RoleController
                For Each objWorkFlowStatePermission As WorkflowStatePermissionInfo In WorkflowStatePermissionController.GetWorkflowStatePermissions(objHtmlText.StateID)
                    If objWorkFlowStatePermission.AllowAccess Then
                        If Null.IsNull(objWorkFlowStatePermission.UserID) Then
                            Dim objRole As RoleInfo = New RoleController().GetRole(objWorkFlowStatePermission.RoleID, objHtmlText.PortalID)
							If Not objRole Is Nothing Then
								For Each objUserRole As UserRoleInfo In objRoles.GetUserRolesByRoleName(objHtmlText.PortalID, objRole.RoleName)
									If Not arrUsers.Contains(objUserRole.UserID) Then
										arrUsers.Add(objUserRole.UserID)
									End If
								Next
							End If
                        Else
                            If Not arrUsers.Contains(objWorkFlowStatePermission.UserID) Then
                                arrUsers.Add(objWorkFlowStatePermission.UserID)
                            End If
                        End If
                    End If
                Next
            End If

            ' process notifications
            If arrUsers.Count > 0 Or ( objHtmlText.IsPublished = True And objHtmlText.Notify = True ) Then
                ' get tabid from module 
                Dim objModules As New ModuleController
                Dim objModule As ModuleInfo = objModules.GetModule(objHtmlText.ModuleID)

                Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                Dim strResourceFile As String = Common.Globals.ApplicationPath & "/DesktopModules/" & objModule.DesktopModule.FolderName & "/" & Localization.LocalResourceDirectory & "/" & Localization.LocalSharedResourceFile
                Dim strSubject As String = Localization.GetString("NotificationSubject", strResourceFile)
                Dim strBody As String = Localization.GetString("NotificationBody", strResourceFile)
                strBody = strBody.Replace("[URL]", NavigateURL(objModule.TabID))
                strBody = strBody.Replace("[STATE]", objHtmlText.StateName)

                ' process user notification collection
                For Each intUserID As Integer In arrUsers

                    ' create user notification record 
                    objHtmlTextUser = New HtmlTextUserInfo
                    objHtmlTextUser.ItemID = objHtmlText.ItemID
                    objHtmlTextUser.StateID = objHtmlText.StateID
                    objHtmlTextUser.ModuleID = objHtmlText.ModuleID
                    objHtmlTextUser.TabID = objModule.TabID
                    objHtmlTextUser.UserID = intUserID
                    objHTMLTextUsers.AddHtmlTextUser(objHtmlTextUser)

                    ' send an email notification to a user if the state indicates to do so
                    If objHtmlText.Notify Then
                        objUser = UserController.GetUserById(objHtmlText.PortalID, intUserID)
                        If Not objUser Is Nothing Then
                            Services.Mail.Mail.SendMail(objPortalSettings.Email, objUser.Email, "", strSubject, strBody, "", "", "", "", "", "")
                        End If
                    End If

                Next

                ' if published and the published state specifies to notify members of the workflow
                If objHtmlText.IsPublished = True And objHtmlText.Notify = True Then
                    ' send email notification to the author
                    objUser = UserController.GetUserById(objHtmlText.PortalID, objHtmlText.CreatedByUserID)
                    If Not objUser Is Nothing Then
                        Services.Mail.Mail.SendMail(objPortalSettings.Email, objUser.Email, "", strSubject, strBody, "", "", "", "", "", "")
                    End If
                End If
            End If            

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatHtmlText formats HtmlText content for display in the browser
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="Content">The HtmlText Content</param>
        ''' <param name="Settings">A Hashtable of Module Settings</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function FormatHtmlText(ByVal ModuleId As Integer, ByVal Content As String, ByVal Settings As Hashtable) As String
            Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings

            ' token replace
            Dim blnReplaceTokens As Boolean = False
            If CType(Settings("HtmlText_ReplaceTokens"), String) <> "" Then
                blnReplaceTokens = CType(Settings("HtmlText_ReplaceTokens"), Boolean)
            End If
            If blnReplaceTokens Then
                Dim tr As New DotNetNuke.Services.Tokens.TokenReplace()
                tr.AccessingUser = UserController.GetCurrentUserInfo
                tr.DebugMessages = Not (objPortalSettings.UserMode = PortalSettings.Mode.View)
                tr.ModuleId = ModuleId
                Content = tr.ReplaceEnvironmentTokens(Content)
            End If

            ' Html decode content
            Content = HttpUtility.HtmlDecode(Content)

            ' manage relative paths
            Content = ManageRelativePaths(Content, objPortalSettings.HomeDirectory, "src", objPortalSettings.PortalId)
            Content = ManageRelativePaths(Content, objPortalSettings.HomeDirectory, "background", objPortalSettings.PortalId)

            Return Content
        End Function

        Public Shared Function ManageRelativePaths(ByVal strHTML As String, ByVal strUploadDirectory As String, ByVal strToken As String, ByVal intPortalID As Integer) As String
            Dim P As Integer
            Dim R As Integer
            Dim S As Integer = 0
            Dim tLen As Integer
            Dim strURL As String
            Dim sbBuff As New StringBuilder("")

            If strHTML <> "" Then
                tLen = strToken.Length + 2
                Dim _UploadDirectory As String = strUploadDirectory.ToLower

                'find position of first occurrance:
                P = strHTML.IndexOf(strToken & "=""", StringComparison.InvariantCultureIgnoreCase)
                While P <> -1
                    sbBuff.Append(strHTML.Substring(S, P - S + tLen)) 'keep charactes left of URL
                    S = P + tLen 'save startpos of URL
                    R = strHTML.IndexOf("""", S) 'end of URL
                    If R >= 0 Then
                        strURL = strHTML.Substring(S, R - S).ToLower
                    Else
                        strURL = strHTML.Substring(S).ToLower
                    End If

                    ' if we are linking internally
                    If strURL.Contains("://") = False Then
                        ' remove the leading portion of the path if the URL contains the upload directory structure
                        Dim strDirectory As String = strUploadDirectory + "/"
                        If strURL.IndexOf(strDirectory) <> -1 Then
                            S = S + strURL.IndexOf(strDirectory) + strDirectory.Length
                            strURL = strURL.Substring(strURL.IndexOf(strDirectory) + strDirectory.Length)
                        End If
                        ' add upload directory
                        If strURL.StartsWith("/") = False Then
                            sbBuff.Append(strUploadDirectory)
                        End If
                    End If
                    'find position of next occurrance
                    P = strHTML.IndexOf(strToken & "=""", S + Len(strURL) + 2, StringComparison.InvariantCultureIgnoreCase)
                End While

                If S > -1 Then sbBuff.Append(strHTML.Substring(S)) 'append characters of last URL and behind
            End If

            Return sbBuff.ToString
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetWorkFlowID retrieves the currently active WorkflowID for the Portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The ID of the Module</param>
        ''' <param name="PortalID">The ID of the Portal</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetWorkflowID(ByVal ModuleID As Integer, ByVal PortalID As Integer) As Integer
            Dim intWorkflowID As Integer = -1

            ' get from module settings
            Dim objModules As New ModuleController
            Dim objSettings As Hashtable = objModules.GetModuleSettings(ModuleID)
            If Not objSettings("WorkflowID") Is Nothing Then
                intWorkflowID = Integer.Parse(objSettings("WorkflowID"))
            End If

            ' if undefined at module level, get from portal settings
            If intWorkflowID = -1 Then
                intWorkflowID = Integer.Parse(PortalController.GetPortalSetting("WorkflowID", PortalID, "-1"))
            End If

            ' if undefined at portal level, set portal default
            If intWorkflowID = -1 Then
                Dim objWorkflow As New WorkflowStateController
                Dim arrWorkflows As ArrayList = objWorkflow.GetWorkflows(PortalID)
                For Each objState As WorkflowStateInfo In arrWorkflows
                    ' use direct publish as default
                    If Null.IsNull(objState.PortalID) And objState.WorkflowName = "Direct Publish" Then
                        intWorkflowID = objState.WorkflowID
                    End If
                Next
            End If

            Return intWorkflowID
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateWorkFlowID updates the currently active WorkflowID for the Portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The ID of the Module</param>
        ''' <param name="PortalID">The ID of the Portal</param>
        ''' <param name="WorkflowID">The ID of the Workflow</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateWorkflowID(ByVal ModuleID As Integer, ByVal PortalID As Integer, ByVal WorkflowID As Integer)

            ' no moduleid indicates a portal level setting
            If Null.IsNull(ModuleID) Then
                Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
                If PortalSecurity.IsInRole(objPortalSettings.AdministratorRoleName) Then
                    PortalController.UpdatePortalSetting(PortalID, "WorkflowID", WorkflowID.ToString)
                End If
            Else
                Dim objModules As New DotNetNuke.Entities.Modules.ModuleController
                objModules.UpdateModuleSetting(ModuleID, "WorkflowID", WorkflowID.ToString)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetMaximumVersionHistory retrieves the maximum number of versions to store for a module
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="PortalID">The ID of the Portal</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetMaximumVersionHistory(ByVal PortalID As Integer) As Integer

            Dim intMaximumVersionHistory As Integer = -1

            ' get from portal settings
            intMaximumVersionHistory = Integer.Parse(PortalController.GetPortalSetting("MaximumVersionHistory", PortalID, "-1"))

            ' if undefined at portal level, set portal default
            If intMaximumVersionHistory = -1 Then
                intMaximumVersionHistory = 5 ' default
                PortalController.UpdatePortalSetting(PortalID, "MaximumVersionHistory", intMaximumVersionHistory)
            End If

            Return intMaximumVersionHistory

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateWorkFlowID updates the currently active WorkflowID for the Portal
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="PortalID">The ID of the Portal</param>
        ''' <param name="MaximumVersionHistory">The MaximumVersionHistory</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub UpdateMaximumVersionHistory(ByVal PortalID As Integer, ByVal MaximumVersionHistory As Integer)

            ' data integrity check
            If MaximumVersionHistory < 0 Then
                MaximumVersionHistory = 5 ' default
            End If

            ' save portal setting
            Dim objPortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings
            If PortalSecurity.IsInRole(objPortalSettings.AdministratorRoleName) Then
                PortalController.UpdatePortalSetting(PortalID, "MaximumVersionHistory", MaximumVersionHistory.ToString)
            End If

        End Sub

#End Region

#Region "Optional Interfaces"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSearchItems implements the ISearchable Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetSearchItems(ByVal ModInfo As Entities.Modules.ModuleInfo) As Services.Search.SearchItemInfoCollection Implements Entities.Modules.ISearchable.GetSearchItems

            Dim objWorkflow As New WorkflowStateController
            Dim WorkflowID As Integer = GetWorkflowID(ModInfo.ModuleID, ModInfo.PortalID)
            Dim SearchItemCollection As New SearchItemInfoCollection
            Dim objContent As HtmlTextInfo = GetTopHtmlText(ModInfo.ModuleID, True, WorkflowID)

            If Not objContent Is Nothing Then
                'content is encoded in the Database so Decode before Indexing
                Dim strContent As String = HttpUtility.HtmlDecode(objContent.Content)

                'Get the description string
                Dim strDescription As String = HtmlUtils.Shorten(HtmlUtils.Clean(strContent, False), MAX_DESCRIPTION_LENGTH, "...")

                Dim SearchItem As SearchItemInfo = New SearchItemInfo(ModInfo.ModuleTitle, strDescription, objContent.LastModifiedByUserID, objContent.LastModifiedOnDate, ModInfo.ModuleID, "", strContent, "", Null.NullInteger)
                SearchItemCollection.Add(SearchItem)
            End If

            Return SearchItemCollection

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ExportModule implements the IPortable ExportModule Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The Id of the module to be exported</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ExportModule(ByVal ModuleID As Integer) As String Implements Entities.Modules.IPortable.ExportModule

            Dim strXML As String = ""

            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModule(ModuleID)
            Dim objWorkflow As New WorkflowStateController
            Dim WorkflowID As Integer = GetWorkflowID(ModuleID, objModule.PortalID)

            Dim objContent As HtmlTextInfo = GetTopHtmlText(ModuleID, True, WorkflowID)
            If Not objContent Is Nothing Then
                strXML += "<htmltext>"
                strXML += "<content>" & Common.Utilities.XmlUtils.XMLEncode(objContent.Content) & "</content>"
                strXML += "</htmltext>"
            End If

            Return strXML

        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ImportModule implements the IPortable ImportModule Interface
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="ModuleID">The ID of the Module being imported</param>
        ''' <param name="Content">The Content being imported</param>
        ''' <param name="Version">The Version of the Module Content being imported</param>
        ''' <param name="UserID">The UserID of the User importing the Content</param>
        ''' <history>
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements Entities.Modules.IPortable.ImportModule

            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = objModules.GetModule(ModuleID)
            Dim objWorkflow As New WorkflowStateController
            Dim intWorkflowID As Integer = GetWorkflowID(ModuleID, objModule.PortalID)
            Dim xmlContent As XmlNode = GetContent(Content, "htmltext")

            Dim objContent As HtmlTextInfo = New HtmlTextInfo
            objContent.ModuleID = ModuleID
            ' convert Version to System.Version
            Dim objVersion As New System.Version(Version)
            Select Case objVersion
                Case Is >= (New System.Version(5, 1, 0))
                    ' current module content
                    objContent.Content = xmlContent.SelectSingleNode("content").InnerText
                Case Else
                    ' legacy module content
                    objContent.Content = xmlContent.SelectSingleNode("desktophtml").InnerText
            End Select
            objContent.WorkflowID = intWorkflowID
            objContent.StateID = objWorkflow.GetFirstWorkflowStateID(intWorkflowID)
            ' import
            UpdateHtmlText(objContent, GetMaximumVersionHistory(objModule.PortalID))

        End Sub

        Public Function UpgradeModule(ByVal Version As String) As String Implements IUpgradeable.UpgradeModule
            Select Case Version
                Case "05.01.02"
                    'remove the Code SubDirectory
                    Config.RemoveCodeSubDirectory("HTML")

                    'Once the web.config entry is done we can safely remove the HTML folder
                    Dim arrPaths(0) As String
                    arrPaths(0) = "App_Code\HTML\"
                    FileSystemUtils.DeleteFiles(arrPaths)
            End Select

            Return String.Empty
        End Function

#End Region

    End Class

End Namespace
