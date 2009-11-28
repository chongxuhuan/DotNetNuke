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

Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.Security.Permissions


Namespace DotNetNuke.UI.Containers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Containers
    ''' Class	 : ActionManager
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ActionManager is a helper class that provides common Action Behaviours that can 
    ''' be used by any IActionControl implementation
    ''' </summary>
    ''' <history>
    ''' 	[cnurse]	12/23/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ActionManager

#Region "Private Members"

        Private _ActionControl As IActionControl
        Private PortalSettings As PortalSettings = PortalController.GetCurrentPortalSettings()
        Private Request As HttpRequest = HttpContext.Current.Request
        Private Response As HttpResponse = HttpContext.Current.Response

#End Region

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a new ActionManager
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal actionControl As IActionControl)
            _ActionControl = actionControl
        End Sub

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the Action Control that is connected to this ActionManager instance
        ''' </summary>
        ''' <returns>An IActionControl object</returns>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ActionControl() As IActionControl
            Get
                Return _ActionControl
            End Get
            Set(ByVal value As IActionControl)
                _ActionControl = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleInstanceContext instance that is connected to this ActionManager 
        ''' instance
        ''' </summary>
        ''' <returns>A ModuleInstanceContext object</returns>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ModuleContext() As ModuleInstanceContext
            Get
                Return ActionControl.ModuleControl.ModuleContext
            End Get
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ClearCache clears the Module cache
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ClearCache(ByVal Command As ModuleAction)
            ' synchronize cache
            ModuleController.SynchronizeModule(ModuleContext.ModuleId)

            ' Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Delete deletes the associated Module
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Delete(ByVal Command As ModuleAction)
            Dim objModules As New ModuleController

            Dim objModule As ModuleInfo = objModules.GetModule(Integer.Parse(Command.CommandArgument), ModuleContext.TabId, True)
            If Not objModule Is Nothing Then
                objModules.DeleteTabModule(ModuleContext.TabId, Integer.Parse(Command.CommandArgument), True)

                Dim m_UserInfo As UserInfo = UserController.GetCurrentUserInfo
                Dim objEventLog As New Services.Log.EventLog.EventLogController
                objEventLog.AddLog(objModule, PortalSettings, m_UserInfo.UserID, "", Services.Log.EventLog.EventLogController.EventLogType.MODULE_SENT_TO_RECYCLE_BIN)
            End If

            ' Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DoAction redirects to the Url associated with the Action
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DoAction(ByVal Command As ModuleAction)
            If Command.NewWindow Then
                UrlUtils.OpenNewWindow(ActionControl.ModuleControl.Control.Page, Me.GetType(), Command.Url)
            Else
                Response.Redirect(Command.Url, True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MoveToPane moves the Module to the relevant Pane
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MoveToPane(ByVal Command As ModuleAction)
            Dim objModules As New ModuleController

            objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, -1, Command.CommandArgument)
            objModules.UpdateTabModuleOrder(ModuleContext.TabId)

            ' Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, True)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MoveUpDown moves the Module within its Pane.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MoveUpDown(ByVal Command As ModuleAction)
            Dim objModules As New ModuleController
            Select Case Command.CommandName
                Case ModuleActionType.MoveTop
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, 0, Command.CommandArgument)
                Case ModuleActionType.MoveUp
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, ModuleContext.Configuration.ModuleOrder - 3, Command.CommandArgument)
                Case ModuleActionType.MoveDown
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, ModuleContext.Configuration.ModuleOrder + 3, Command.CommandArgument)
                Case ModuleActionType.MoveBottom
                    objModules.UpdateModuleOrder(ModuleContext.TabId, ModuleContext.ModuleId, (ModuleContext.Configuration.PaneModuleCount * 2) + 1, Command.CommandArgument)
            End Select

            objModules.UpdateTabModuleOrder(ModuleContext.TabId)

            ' Redirect to the same page to pick up changes
            Response.Redirect(Request.RawUrl, True)
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayControl determines whether the associated Action control should be 
        ''' displayed
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function DisplayControl(ByVal objNodes As DNNNodeCollection) As Boolean
            If Not objNodes Is Nothing AndAlso objNodes.Count > 0 AndAlso PortalSettings.UserMode <> PortalSettings.Mode.View Then
                Dim objRootNode As DNNNode = objNodes(0)
                If objRootNode.HasNodes AndAlso objRootNode.DNNNodes.Count = 0 Then
                    'if has pending node then display control
                    Return True
                ElseIf objRootNode.DNNNodes.Count > 0 Then
                    'verify that at least one child is not a break
                    For Each childNode As DNNNode In objRootNode.DNNNodes
                        If Not childNode.IsBreak Then
                            'Found a child so make Visible
                            Return True
                        End If
                    Next
                End If
            End If
            Return False
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAction gets the action associated with the commandName
        ''' </summary>
        ''' <param name="commandName">The command name</param>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetAction(ByVal commandName As String) As ModuleAction
            Return ActionControl.ModuleControl.ModuleContext.Actions.GetActionByCommandName(commandName)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetAction gets the action associated with the id
        ''' </summary>
        ''' <param name="id">The Id</param>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function GetAction(ByVal id As Integer) As ModuleAction
            Return ActionControl.ModuleControl.ModuleContext.Actions.GetActionByID(id)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetClientScriptURL gets the client script to attach to the control's client 
        ''' side onclick event
        ''' </summary>
        ''' <param name="action">The Action</param>
        ''' <param name="control">The Control</param>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub GetClientScriptURL(ByVal action As ModuleAction, ByVal control As WebControl)
            If Len(action.ClientScript) > 0 Then
                Dim Script As String = action.ClientScript

                Dim JSPos As Integer = Script.ToLower.IndexOf("javascript:")
                If JSPos > -1 Then
                    Script = Script.Substring(JSPos + 11)
                End If

                Dim FormatScript As String = "javascript: return {0};"

                control.Attributes.Add("onClick", String.Format(FormatScript, Script))
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IsVisible determines whether the action control is Visible
        ''' </summary>
        ''' <param name="action">The Action</param>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function IsVisible(ByVal action As ModuleAction) As Boolean
            Dim _IsVisible As Boolean = False
            If action.Visible = True And ModulePermissionController.HasModuleAccess(action.Secure, Null.NullString, ModuleContext.Configuration) = True Then
                If (ModuleContext.PortalSettings.UserMode = PortalSettings.Mode.Edit) OrElse (action.Secure = SecurityAccessLevel.Anonymous OrElse action.Secure = SecurityAccessLevel.View) Then
                    _IsVisible = True
                Else
                    _IsVisible = False
                End If
            Else
                _IsVisible = False
            End If
            Return _IsVisible
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessAction processes the action
        ''' </summary>
        ''' <param name="id">The Id of the Action</param>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ProcessAction(ByVal id As String) As Boolean
            Dim bProcessed As Boolean = True
            If IsNumeric(id) Then
                bProcessed = ProcessAction(ActionControl.ModuleControl.ModuleContext.Actions.GetActionByID(Convert.ToInt32(id)))
            End If
            Return bProcessed
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessAction processes the action
        ''' </summary>
        ''' <param name="action">The Action</param>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function ProcessAction(ByVal action As ModuleAction) As Boolean
            Dim bProcessed As Boolean = True
            Select Case action.CommandName
                Case ModuleActionType.ModuleHelp
                    DoAction(action)
                Case ModuleActionType.OnlineHelp
                    DoAction(action)
                Case ModuleActionType.ModuleSettings
                    DoAction(action)
                Case ModuleActionType.DeleteModule
                    Delete(action)
                Case ModuleActionType.PrintModule, ModuleActionType.SyndicateModule
                    DoAction(action)
                Case ModuleActionType.ClearCache
                    ClearCache(action)
                Case ModuleActionType.MovePane
                    MoveToPane(action)
                Case ModuleActionType.MoveTop, ModuleActionType.MoveUp, ModuleActionType.MoveDown, ModuleActionType.MoveBottom
                    MoveUpDown(action)
                Case Else       ' custom action
                    If action.Url.Length > 0 AndAlso action.UseActionEvent = False Then
                        DoAction(action)
                    Else
                        bProcessed = False
                    End If
            End Select
            Return bProcessed
        End Function

#End Region

    End Class

End Namespace