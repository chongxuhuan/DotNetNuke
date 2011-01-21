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
Imports System.Collections.Generic
Imports System.Web.UI
Imports DotNetNuke.UI.ControlPanels

Imports DotNetNuke.Application
Imports DotNetNuke.Common
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Entities.Modules.Communications
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.UI.Skins.EventListeners

'Legacy Support
Namespace DotNetNuke

    <Obsolete("This class is obsolete.  Please use DotNetNuke.UI.Skins.Skin.")> _
    Public Class Skin
        Inherits DotNetNuke.UI.Skins.Skin
    End Class

End Namespace

Namespace DotNetNuke.UI.Skins

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Skins
    ''' Class	 : Skin
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Skin is the base for the Skins 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/04/2005	Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Skin
        Inherits Framework.UserControlBase

#Region "Private Members"

        Private _actionEventListeners As ArrayList
        Private _Communicator As New ModuleCommunicate
        Private _ControlPanel As Control
        Private _panes As Dictionary(Of String, Pane)
        Private _skinSrc As String

#End Region

#Region "Public Constants"

        'Localized Strings
        Public PANE_LOAD_ERROR As String = Localization.GetString("PaneNotFound.Error")
        Public CONTRACTEXPIRED_ERROR As String = Localization.GetString("ContractExpired.Error")
        Public TABACCESS_ERROR As String = Localization.GetString("TabAccess.Error")
        Public MODULEACCESS_ERROR As String = Localization.GetString("ModuleAccess.Error")
        Public CRITICAL_ERROR As String = Localization.GetString("CriticalError.Error")
        Public MODULELOAD_WARNING As String = Localization.GetString("ModuleLoadWarning.Error")
        Public MODULELOAD_WARNINGTEXT As String = Localization.GetString("ModuleLoadWarning.Text")

        Public Shared MODULELOAD_ERROR As String = Localization.GetString("ModuleLoad.Error")
        Public Shared CONTAINERLOAD_ERROR As String = Localization.GetString("ContainerLoad.Error")
        Public Shared MODULEADD_ERROR As String = Localization.GetString("ModuleAdd.Error")

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ControlPanel container.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        '''     [cnurse]    04/17/2009  Refactored from Skin
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend ReadOnly Property ControlPanel() As Control
            Get
                If _ControlPanel Is Nothing Then
                    _ControlPanel = FindControl("ControlPanel")
                End If
                Return _ControlPanel
            End Get
        End Property

#End Region

#Region "Friend Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleCommunicate instance for the skin
        ''' </summary>
        ''' <returns>The ModuleCommunicate instance for the Skin</returns>
        ''' <history>
        ''' 	[cnurse]	01/12/2009  created
        ''' </history>
        Friend ReadOnly Property Communicator() As ModuleCommunicate
            Get
                Return _Communicator
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets a Dictionary of Panes.
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Friend ReadOnly Property Panes() As Dictionary(Of String, Pane)
            Get
                If _panes Is Nothing Then
                    _panes = New Dictionary(Of String, Pane)
                End If
                Return _panes
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets an ArrayList of ActionEventListeners
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ActionEventListeners() As ArrayList
            Get
                If _actionEventListeners Is Nothing Then
                    _actionEventListeners = New ArrayList
                End If
                Return _actionEventListeners
            End Get
            Set(ByVal Value As ArrayList)
                _actionEventListeners = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Path for this skin
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property SkinPath() As String
            Get
                Return Me.TemplateSourceDirectory & "/"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Source for this skin
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SkinSrc() As String
            Get
                Return _skinSrc
            End Get
            Set(ByVal value As String)
                _skinSrc = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CheckExpired checks whether the portal has expired
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        '''     [cnurse]    04/17/2009  Refactored from Skin
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function CheckExpired() As Boolean
            Dim blnExpired As Boolean = False
            If PortalSettings.ExpiryDate <> Null.NullDate Then
                If Convert.ToDateTime(PortalSettings.ExpiryDate) < Now() And PortalSettings.ActiveTab.ParentId <> PortalSettings.SuperTabId Then
                    blnExpired = True
                End If
            End If
            Return blnExpired
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InjectControlPanel injects the ControlPanel into the ControlPanel pane
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        '''     [cnurse]    04/17/2009  Refactored from Skin
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InjectControlPanel()
            'if querystring dnnprintmode=true, controlpanel will not be shown
            If Request.QueryString("dnnprintmode") <> "true" Then
                ' ControlPanel processing
                Dim objControlPanel As ControlPanelBase = ControlUtilities.LoadControl(Of ControlPanelBase)(Me, Host.ControlPanel)

                ' inject ControlPanel control into skin
                If ControlPanel Is Nothing Then
                    Dim objForm As HtmlForm = CType(Parent.FindControl("Form"), HtmlForm)

                    If objForm IsNot Nothing Then
                        objForm.Controls.AddAt(0, objControlPanel)
                    Else
                        Page.Controls.AddAt(0, objControlPanel)
                    End If
                Else
                    ControlPanel.Controls.Add(objControlPanel)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadPanes parses the Skin and loads the "Panes"
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        '''     [cnurse]    04/17/2009  Refactored from Skin
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub LoadPanes()
            Dim ctlControl As Control
            Dim objPaneControl As HtmlContainerControl

            ' iterate page controls
            For Each ctlControl In Controls
                objPaneControl = TryCast(ctlControl, HtmlContainerControl)

                'Panes must be runat=server controls so they have to have an ID
                If objPaneControl IsNot Nothing AndAlso Not String.IsNullOrEmpty(objPaneControl.ID) Then
                    ' load the skin panes
                    Select Case objPaneControl.TagName.ToLowerInvariant
                        Case "td", "div", "span", "p"
                            ' content pane
                            If objPaneControl.ID.ToLower() <> "controlpanel" Then
                                'Add to the PortalSettings (for use in the Control Panel)
                                PortalSettings.ActiveTab.Panes.Add(objPaneControl.ID)

                                'Add to the Panes collection
                                Panes.Add(objPaneControl.ID.ToLowerInvariant, New Pane(objPaneControl))
                            Else
                                'Control Panel pane
                                _ControlPanel = objPaneControl
                            End If
                    End Select
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessMasterModules processes all the master modules in the Active Tab's
        ''' Modules Collection.
        ''' </summary>
        ''' <returns>A flag that indicates whether the modules were successfully processed.</returns>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        '''     [cnurse]    04/17/2009  Refactored from Skin
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ProcessMasterModules() As Boolean
            Dim objModule As ModuleInfo = Nothing
            Dim bSuccess As Boolean = True

            If TabPermissionController.CanViewPage() Then
                ' check portal expiry date
                If Not CheckExpired() Then
                    If (PortalSettings.ActiveTab.StartDate < Now AndAlso PortalSettings.ActiveTab.EndDate > Now) _
                                OrElse TabPermissionController.CanAdminPage() _
                                OrElse IsLayoutMode() Then
                        ' dynamically populate the panes with modules
                        If PortalSettings.ActiveTab.Modules.Count > 0 Then
                            ' loop through each entry in the configuration system for this tab
                            For Each objModule In PortalSettings.ActiveTab.Modules
                                ' if user is allowed to view module and module is not deleted
                                If ModulePermissionController.CanViewModule(objModule) AndAlso objModule.IsDeleted = False Then
                                    ' if current date is within module display schedule or user is admin
                                    If (objModule.StartDate < Now AndAlso objModule.EndDate > Now) _
                                                OrElse IsLayoutMode() OrElse IsEditMode() Then
                                        Dim pane As Pane = Nothing
                                        Dim bFound As Boolean = Panes.TryGetValue(objModule.PaneName.ToLowerInvariant, pane)

                                        If Not bFound Then
                                            ' the pane specified in the database does not exist for this skin
                                            ' insert the module into the default pane instead
                                            bFound = Panes.TryGetValue(glbDefaultPane.ToLowerInvariant, pane)
                                        End If

                                        If bFound Then
                                            'try to inject the module into the pane
                                            bSuccess = InjectModule(pane, objModule)
                                        Else             ' no ContentPane in skin
                                            Dim lex As ModuleLoadException
                                            lex = New ModuleLoadException(PANE_LOAD_ERROR)
                                            Controls.Add(New ErrorContainer(PortalSettings, MODULELOAD_ERROR, lex).Container)
                                            LogException(lex)
                                            Err.Clear()
                                        End If
                                    End If
                                End If
                            Next objModule
                        End If
                    Else
                        AddPageMessage(Me, "", TABACCESS_ERROR, UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                    End If
                Else
                    AddPageMessage(Me, "", String.Format(CONTRACTEXPIRED_ERROR, PortalSettings.PortalName, GetMediumDate(PortalSettings.ExpiryDate.ToString), PortalSettings.Email), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                End If
            Else
                Response.Redirect(AccessDeniedURL(TABACCESS_ERROR), True)
            End If

            Return bSuccess
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessPanes processes the Attributes for the Panes
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        '''     [cnurse]    04/17/2009  Refactored from Skin
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessPanes()
            For Each kvp As KeyValuePair(Of String, Pane) In Panes
                kvp.Value.ProcessPane()
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessSlaveModule processes the slave module specifeid by the "ctl=xxx" ControlKey.
        ''' </summary>
        ''' <returns>A flag that indicates whether the modules were successfully processed.</returns>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        '''     [cnurse]    04/17/2009  Refactored from Skin
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function ProcessSlaveModule() As Boolean
            Dim objModules As New ModuleController
            Dim objModule As ModuleInfo = Nothing
            Dim slaveModule As ModuleInfo = Nothing
            Dim moduleId As Integer = -1
            Dim key As String = ""
            Dim bSuccess As Boolean = True

            ' get ModuleId
            If Not IsNothing(Request.QueryString("mid")) Then
                Integer.TryParse(Request.QueryString("mid"), moduleId)
            End If

            ' get ControlKey
            If Not IsNothing(Request.QueryString("ctl")) Then
                key = Request.QueryString("ctl")
            End If

            ' initialize moduleid for modulesettings
            If Not IsNothing(Request.QueryString("moduleid")) And (key.ToLower = "module" Or key.ToLower = "help") Then
                Integer.TryParse(Request.QueryString("moduleid"), moduleId)
            End If

            If moduleId <> -1 Then
                ' get master module security settings
                objModule = objModules.GetModule(moduleId, PortalSettings.ActiveTab.TabID, False)
                If Not objModule Is Nothing Then
                    'Clone the Master Module as we do not want to modify the cached module
                    slaveModule = objModule.Clone()
                End If
            End If

            If slaveModule Is Nothing Then
                ' initialize object not related to a module
                slaveModule = New ModuleInfo
                slaveModule.ModuleID = moduleId
                slaveModule.ModuleDefID = -1
                slaveModule.TabID = PortalSettings.ActiveTab.TabID
            End If

            ' initialize moduledefid for modulesettings
            If Not IsNothing(Request.QueryString("moduleid")) And (key.ToLower = "module" Or key.ToLower = "help") Then
                slaveModule.ModuleDefID = -1
            End If

            ' override slave module settings
            If Request.QueryString("dnnprintmode") <> "true" Then
                slaveModule.ModuleTitle = ""
            End If
            slaveModule.Header = ""
            slaveModule.Footer = ""
            slaveModule.StartDate = DateTime.MinValue
            slaveModule.EndDate = DateTime.MaxValue
            slaveModule.PaneName = glbDefaultPane
            slaveModule.Visibility = VisibilityState.None
            slaveModule.Color = ""
            slaveModule.Border = ""
            slaveModule.DisplayTitle = True
            slaveModule.DisplayPrint = False
            slaveModule.DisplaySyndicate = False

            ' get container from Active Tab for slave module
            slaveModule.ContainerSrc = PortalSettings.ActiveTab.ContainerSrc
            If String.IsNullOrEmpty(slaveModule.ContainerSrc) Then
                'Next try default container for portal
                slaveModule.ContainerSrc = PortalSettings.DefaultPortalContainer
            End If
            slaveModule.ContainerSrc = SkinController.FormatSkinSrc(slaveModule.ContainerSrc, PortalSettings)
            slaveModule.ContainerPath = SkinController.FormatSkinPath(slaveModule.ContainerSrc)

            ' get the pane
            Dim pane As Pane = Nothing
            Dim bFound As Boolean = Panes.TryGetValue(glbDefaultPane.ToLowerInvariant, pane)

            ' load the control
            Dim objModuleControl As ModuleControlInfo = ModuleControlController.GetModuleControlByControlKey(key, slaveModule.ModuleDefID)

            If objModuleControl IsNot Nothing Then
                ' initialize control values
                slaveModule.ModuleControlId = objModuleControl.ModuleControlID
                slaveModule.IconFile = objModuleControl.IconFile

                ' verify that the current user has access to this control
                If ModulePermissionController.HasModuleAccess(slaveModule.ModuleControl.ControlType, Null.NullString, slaveModule) Then
                    'try to inject the module into the pane
                    bSuccess = InjectModule(pane, slaveModule)
                Else
                    Response.Redirect(AccessDeniedURL(MODULEACCESS_ERROR), True)
                End If

            End If
            Return bSuccess
        End Function

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnInit runs when the Skin is initialised.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/04/2005	Documented
        '''     [cnurse]    12/05/2007  Refactored
        '''     [cnurse]    04/17/2009  Refactored to use SkinAdapter
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            'Call base classes method
            MyBase.OnInit(e)

            Dim bSuccess As Boolean = True

            'Load the Panes
            LoadPanes()

            'Load the Module Control(s)
            If Not IsAdminControl() Then
                ' master module
                bSuccess = ProcessMasterModules()
            Else
                ' slave module
                bSuccess = ProcessSlaveModule()
            End If

            'Load the Control Panel
            InjectControlPanel()

            'Process the Panes attributes
            ProcessPanes()

            'Register any error messages on the Skin
            If Not Request.QueryString("error") Is Nothing Then
                AddPageMessage(Me, CRITICAL_ERROR, Server.HtmlEncode(Request.QueryString("error")), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
            End If

            If Not TabPermissionController.CanAdminPage() Then
                ' only display the warning to non-administrators (adminsitrators will see the errors)
                If Not bSuccess Then
                    AddPageMessage(Me, MODULELOAD_WARNING, String.Format(MODULELOAD_WARNINGTEXT, PortalSettings.Email), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
                End If
            End If

            For Each listener As SkinEventListener In DotNetNukeContext.Current.SkinEventListeners
                If listener.EventType = SkinEventType.OnSkinInit Then
                    listener.SkinEvent.Invoke(Me, New SkinEventArgs(Me))
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnLoad runs when the Skin is loaded.
        ''' </summary>
        ''' <history>
        '''     [cnurse]    04/17/2009  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            For Each listener As SkinEventListener In DotNetNukeContext.Current.SkinEventListeners
                If listener.EventType = SkinEventType.OnSkinLoad Then
                    listener.SkinEvent.Invoke(Me, New SkinEventArgs(Me))
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnLoad runs just before the Skin is rendered.
        ''' </summary>
        ''' <history>
        '''     [cnurse]    04/17/2009  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            For Each listener As SkinEventListener In DotNetNukeContext.Current.SkinEventListeners
                If listener.EventType = SkinEventType.OnSkinPreRender Then
                    listener.SkinEvent.Invoke(Me, New SkinEventArgs(Me))
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnUnLoad runs when the Skin is unloaded.
        ''' </summary>
        ''' <history>
        '''     [cnurse]    04/17/2009  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnUnLoad(ByVal e As System.EventArgs)
            MyBase.OnUnload(e)

            For Each listener As SkinEventListener In DotNetNukeContext.Current.SkinEventListeners
                If listener.EventType = SkinEventType.OnSkinUnLoad Then
                    listener.SkinEvent.Invoke(Me, New SkinEventArgs(Me))
                End If
            Next
        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' InjectModule injects the module into the Pane
        ''' </summary>
        ''' <param name="objModule">The module to inject</param>
        ''' <param name="objPane">The pane</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  created
        '''     [cnurse]    04/17/2009  Refactored to use SkinAdapter
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Function InjectModule(ByVal objPane As Pane, ByVal objModule As ModuleInfo) As Boolean
            Dim bSuccess As Boolean = True

            'try to inject the module into the pane
            Try
                objPane.InjectModule(objModule)
            Catch ex As Exception
                bSuccess = False
            End Try

            Return bSuccess
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' RegisterModuleActionEvent registers a Module Action Event
        ''' </summary>
        ''' <param name="ModuleID">The ID of the module</param>
        ''' <param name="e">An Action Event Handler</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub RegisterModuleActionEvent(ByVal ModuleID As Integer, ByVal e As ActionEventHandler)
            ActionEventListeners.Add(New ModuleActionEventListener(ModuleID, e))
        End Sub

#End Region

#Region "Private Shared/Static Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LoadSkin loads the Skin
        ''' </summary>
        ''' <param name="Page">The Page that will contain the Skin</param>
        ''' <param name="SkinPath">The path to the Skin file</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function LoadSkin(ByVal Page As PageBase, ByVal SkinPath As String) As DotNetNuke.UI.Skins.Skin
            Dim ctlSkin As DotNetNuke.UI.Skins.Skin = Nothing

            Try
                Dim SkinSrc As String = SkinPath
                If SkinPath.ToLower.IndexOf(Common.Globals.ApplicationPath) <> -1 Then
                    SkinPath = SkinPath.Remove(0, Len(Common.Globals.ApplicationPath))
                End If
                ctlSkin = ControlUtilities.LoadControl(Of DotNetNuke.UI.Skins.Skin)(Page, SkinPath)
                ctlSkin.SkinSrc = SkinSrc
                ' call databind so that any server logic in the skin is executed
                ctlSkin.DataBind()
            Catch exc As Exception
                ' could not load user control
                Dim lex As New PageLoadException("Unhandled error loading page.", exc)
                If TabPermissionController.CanAdminPage() Then
                    ' only display the error to administrators
                    Dim SkinError As Label = CType(Page.FindControl("SkinError"), Label)
                    SkinError.Text = String.Format(Localization.GetString("SkinLoadError", Localization.GlobalResourceFile), SkinPath, Page.Server.HtmlEncode(exc.Message))
                    SkinError.Visible = True
                End If
                LogException(lex)
                Err.Clear()
            End Try

            Return ctlSkin
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleMessage adds a Moduel Message control to the Skin
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="IconSrc">The Icon to diplay</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objControl">The current control</param>
        ''' <param name="objModuleMessageType">The type of the message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddModuleMessage(ByVal objControl As Control, ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType, ByVal IconSrc As String)
            If Not objControl Is Nothing Then
                If Message <> "" Then
                    Dim MessagePlaceHolder As PlaceHolder = CType(objControl.Parent.FindControl("MessagePlaceHolder"), PlaceHolder)
                    If Not MessagePlaceHolder Is Nothing Then
                        MessagePlaceHolder.Visible = True
                        Dim objModuleMessage As UI.Skins.Controls.ModuleMessage
                        objModuleMessage = GetModuleMessageControl(Heading, Message, objModuleMessageType, IconSrc)
                        MessagePlaceHolder.Controls.Add(objModuleMessage)
                    End If
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPageMessage adds a Page Message control to the Skin
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="IconSrc">The Icon to diplay</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objControl">The current control</param>
        ''' <param name="objModuleMessageType">The type of the message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub AddPageMessage(ByVal objControl As Control, ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType, ByVal IconSrc As String)
            If Message <> "" Then
                Dim ContentPane As Control = CType(objControl.FindControl(glbDefaultPane), Control)
                If Not ContentPane Is Nothing Then
                    Dim objModuleMessage As UI.Skins.Controls.ModuleMessage
                    objModuleMessage = GetModuleMessageControl(Heading, Message, objModuleMessageType, IconSrc)
                    ContentPane.Controls.AddAt(0, objModuleMessage)
                End If
            End If
        End Sub

#End Region

#Region "Public Shared/Static Methods"

        Public Shared Sub AddModuleMessage(ByVal objControl As PortalModuleBase, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType)
            AddModuleMessage(objControl, "", Message, objModuleMessageType, Null.NullString)
        End Sub

        Public Shared Sub AddModuleMessage(ByVal objControl As PortalModuleBase, ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType)
            AddModuleMessage(objControl, Heading, Message, objModuleMessageType, Null.NullString)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleMessage adds a Moduel Message control to the Skin
        ''' </summary>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objControl">The current control</param>
        ''' <param name="objModuleMessageType">The type of the message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddModuleMessage(ByVal objControl As Control, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType)
            AddModuleMessage(objControl, "", Message, objModuleMessageType, Null.NullString)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddModuleMessage adds a Moduel Message control to the Skin
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objControl">The current control</param>
        ''' <param name="objModuleMessageType">The type of the message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddModuleMessage(ByVal objControl As Control, ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType)
            AddModuleMessage(objControl, Heading, Message, objModuleMessageType, Null.NullString)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPageMessage adds a Page Message control to the Skin
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="IconSrc">The Icon to diplay</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objPage">The Page</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddPageMessage(ByVal objPage As Page, ByVal Heading As String, ByVal Message As String, ByVal IconSrc As String)
            AddPageMessage(objPage, Heading, Message, Nothing, IconSrc)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPageMessage adds a Page Message control to the Skin
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="IconSrc">The Icon to diplay</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objSkin">The skin</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddPageMessage(ByVal objSkin As UI.Skins.Skin, ByVal Heading As String, ByVal Message As String, ByVal IconSrc As String)
            AddPageMessage(objSkin, Heading, Message, Nothing, IconSrc)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPageMessage adds a Page Message control to the Skin
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objSkin">The skin</param>
        ''' <param name="objModuleMessageType">The type of the message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddPageMessage(ByVal objSkin As UI.Skins.Skin, ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType)
            AddPageMessage(objSkin, Heading, Message, objModuleMessageType, Null.NullString)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddPageMessage adds a Page Message control to the Skin
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objPage">The Page</param>
        ''' <param name="objModuleMessageType">The type of the message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub AddPageMessage(ByVal objPage As Page, ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType)
            AddPageMessage(objPage, Heading, Message, objModuleMessageType, Null.NullString)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleMessageControl gets an existing Message Control and sets its properties
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="IconImage">The Message Icon</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleMessageControl(ByVal Heading As String, ByVal Message As String, ByVal IconImage As String) As UI.Skins.Controls.ModuleMessage
            Return GetModuleMessageControl(Heading, Message, Nothing, IconImage)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleMessageControl gets an existing Message Control and sets its properties
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="objModuleMessageType">The type of message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleMessageControl(ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType) As UI.Skins.Controls.ModuleMessage
            Return GetModuleMessageControl(Heading, Message, objModuleMessageType, Null.NullString)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetModuleMessageControl gets an existing Message Control and sets its properties
        ''' </summary>
        ''' <param name="Heading">The Message Heading</param>
        ''' <param name="Message">The Message Text</param>
        ''' <param name="IconImage">The Message Icon</param>
        ''' <param name="objModuleMessageType">The type of message</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetModuleMessageControl(ByVal Heading As String, ByVal Message As String, ByVal objModuleMessageType As UI.Skins.Controls.ModuleMessage.ModuleMessageType, ByVal IconImage As String) As UI.Skins.Controls.ModuleMessage
            'Use this to get a module message control
            'with a standard DotNetNuke icon
            Dim objModuleMessage As UI.Skins.Controls.ModuleMessage
            Dim s As New UI.Skins.Skin
            objModuleMessage = CType(s.LoadControl("~/admin/skins/ModuleMessage.ascx"), UI.Skins.Controls.ModuleMessage)
            objModuleMessage.Heading = Heading
            objModuleMessage.Text = Message
            objModuleMessage.IconImage = IconImage
            objModuleMessage.IconType = objModuleMessageType
            Return objModuleMessage
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetParentSkin gets the Parent Skin for a control
        ''' </summary>
        ''' <param name="objModule">The control whose Parent Skin is requested</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetParentSkin(ByVal objModule As DotNetNuke.Entities.Modules.PortalModuleBase) As UI.Skins.Skin
            Return GetParentSkin(TryCast(objModule, System.Web.UI.Control))
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetParentSkin gets the Parent Skin for a control
        ''' </summary>
        ''' <param name="objControl">The control whose Parent Skin is requested</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetParentSkin(ByVal objControl As System.Web.UI.Control) As UI.Skins.Skin
            If objControl IsNot Nothing Then
                Dim MyParent As System.Web.UI.Control = objControl.Parent
                Dim FoundSkin As Boolean = False
                While Not MyParent Is Nothing
                    If TypeOf MyParent Is Skin Then
                        FoundSkin = True
                        Exit While
                    End If
                    MyParent = MyParent.Parent
                End While
                If FoundSkin Then
                    Return DirectCast(MyParent, Skin)
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetSkin gets the Skin
        ''' </summary>
        ''' <param name="Page">The Page</param>
        ''' <history>
        ''' 	[cnurse]	12/04/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Function GetSkin(ByVal Page As PageBase) As DotNetNuke.UI.Skins.Skin
            ' load skin control
            Dim ctlSkin As DotNetNuke.UI.Skins.Skin = Nothing
            Dim skinSource As String = Null.NullString

            ' skin preview
            If (Not Page.Request.QueryString("SkinSrc") Is Nothing) Then
                skinSource = SkinController.FormatSkinSrc(QueryStringDecode(Page.Request.QueryString("SkinSrc")) & ".ascx", Page.PortalSettings)
                ctlSkin = LoadSkin(Page, skinSource)
            End If

            ' load user skin ( based on cookie )
            If ctlSkin Is Nothing Then
                If Not Page.Request.Cookies("_SkinSrc" & Page.PortalSettings.PortalId.ToString) Is Nothing Then
                    If Page.Request.Cookies("_SkinSrc" & Page.PortalSettings.PortalId.ToString).Value <> "" Then
                        skinSource = SkinController.FormatSkinSrc(Page.Request.Cookies("_SkinSrc" & Page.PortalSettings.PortalId.ToString).Value & ".ascx", Page.PortalSettings)
                        ctlSkin = LoadSkin(Page, skinSource)
                    End If
                End If
            End If

            ' load assigned skin
            If ctlSkin Is Nothing Then
                If IsAdminSkin() Then
                    skinSource = SkinController.FormatSkinSrc(Page.PortalSettings.DefaultAdminSkin, Page.PortalSettings)
                Else
                    skinSource = Page.PortalSettings.ActiveTab.SkinSrc
                End If

                If skinSource <> "" Then
                    skinSource = SkinController.FormatSkinSrc(skinSource, Page.PortalSettings)
                    ctlSkin = LoadSkin(Page, skinSource)
                End If
            End If

            ' error loading skin - load default
            If ctlSkin Is Nothing Then
                skinSource = SkinController.FormatSkinSrc(SkinController.GetDefaultPortalSkin, Page.PortalSettings)
                ctlSkin = LoadSkin(Page, skinSource)
            End If

            ' set skin path
            Page.PortalSettings.ActiveTab.SkinPath = SkinController.FormatSkinPath(skinSource)

            ' set skin id to an explicit short name to reduce page payload and make it standards compliant
            ctlSkin.ID = "dnn"

            Return ctlSkin
        End Function

#End Region

    End Class

End Namespace

