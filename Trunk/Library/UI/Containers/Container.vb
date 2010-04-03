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
Imports System.Web.UI
Imports DotNetNuke.Application

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports System.IO
Imports DotNetNuke.UI.Modules
Imports DotNetNuke.UI.Containers
Imports DotNetNuke.Entities.Host
Imports DotNetNuke.UI.Containers.EventListeners
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.Security.Permissions

'Legacy Support
Namespace DotNetNuke

    <Obsolete("This class is obsolete.  Please use DotNetNuke.UI.Containers.Container.")> _
    Public Class Container
        Inherits DotNetNuke.UI.Containers.Container
    End Class

End Namespace

Namespace DotNetNuke.UI.Containers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Containers
    ''' Class	 : Container
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Container is the base for the Containers 
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	07/04/2005	Documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class Container
        Inherits System.Web.UI.UserControl
        Private Const c_ContainerAdminBorder As String = "containerAdminBorder"

#Region "Private Members"

        Private _ContentPane As HtmlContainerControl
        Private _ContainerSrc As String
        Private _ModuleConfiguration As ModuleInfo
        Private _ModuleHost As ModuleHost

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Content Pane Control (Id="ContentPane")
        ''' </summary>
        ''' <returns>An HtmlContainerControl</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ContentPane() As HtmlContainerControl
            Get
                If _ContentPane Is Nothing Then
                    _ContentPane = TryCast(FindControl(glbDefaultPane), HtmlContainerControl)
                End If
                Return _ContentPane
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Portal Settings for the current Portal
        ''' </summary>
        ''' <returns>A PortalSettings object</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PortalSettings() As PortalSettings
            Get
                PortalSettings = PortalController.GetCurrentPortalSettings
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleControl object that this container is displaying
        ''' </summary>
        ''' <returns>A ModuleHost object</returns>
        ''' <history>
        ''' 	[cnurse]	01/12/2009  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleControl() As IModuleControl
            Get
                Dim _ModuleControl As IModuleControl = Nothing
                If ModuleHost IsNot Nothing Then
                    _ModuleControl = ModuleHost.ModuleControl
                End If
                Return _ModuleControl
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ModuleInfo object that this container is displaying
        ''' </summary>
        ''' <returns>A ModuleInfo object</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleConfiguration() As ModuleInfo
            Get
                Return _ModuleConfiguration
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleHost object that this container is displaying
        ''' </summary>
        ''' <returns>A ModuleHost object</returns>
        ''' <history>
        ''' 	[cnurse]	01/12/2009  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ModuleHost() As ModuleHost
            Get
                Return _ModuleHost
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Parent Container for this container
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ParentSkin() As DotNetNuke.UI.Skins.Skin
            Get
                'This finds a reference to the containing skin
                Return DotNetNuke.UI.Skins.Skin.GetParentSkin(Me)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Path for this container
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/05/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ContainerPath() As String
            Get
                Return Me.TemplateSourceDirectory & "/"
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Source for this container
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	06/10/2009  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ContainerSrc() As String
            Get
                Return _ContainerSrc
            End Get
            Set(ByVal value As String)
                _ContainerSrc = value
            End Set
        End Property

#End Region

#Region "Private Helper Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessChildControls parses all the controls in the container, and if the
        ''' control is an action (IActionControl) it attaches the ModuleControl (IModuleControl)
        ''' and an EventHandler to respond to the Actions Action event.  If the control is a
        ''' Container Object (IContainerControl) it attaches the ModuleControl.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessChildControls(ByVal control As Control)
            Dim objActions As IActionControl
            Dim objSkinControl As ISkinControl

            For Each objChildControl As Control In control.Controls
                ' check if control is an action control
                objActions = TryCast(objChildControl, IActionControl)
                If objActions IsNot Nothing Then
                    objActions.ModuleControl = ModuleControl
                    AddHandler objActions.Action, AddressOf ModuleAction_Click
                End If

                ' check if control is a skin control
                objSkinControl = TryCast(objChildControl, ISkinControl)
                If objSkinControl IsNot Nothing Then
                    objSkinControl.ModuleControl = ModuleControl
                End If

                If objChildControl.HasControls Then
                    ' recursive call for child controls
                    ProcessChildControls(objChildControl)
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessContentPane processes the ContentPane, setting its style and other 
        ''' attributes.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessContentPane()
            If ModuleConfiguration.Alignment <> "" Then
                If Not ContentPane.Attributes.Item("class") Is Nothing Then
                    ContentPane.Attributes.Item("class") = String.Format("{0} DNNAlign{1}", ContentPane.Attributes.Item("class"), ModuleConfiguration.Alignment.ToLower())
                Else
                    ContentPane.Attributes.Item("class") = String.Format("DNNAlign{0}", ModuleConfiguration.Alignment.ToLower())
                End If
            End If
            If ModuleConfiguration.Color <> "" Then
                ContentPane.Style("background-color") = ModuleConfiguration.Color
            End If
            If ModuleConfiguration.Border <> "" Then
                ContentPane.Style("border-top") = String.Format("{0}px #000000 solid", ModuleConfiguration.Border)
                ContentPane.Style("border-bottom") = String.Format("{0}px #000000 solid", ModuleConfiguration.Border)
                ContentPane.Style("border-right") = String.Format("{0}px #000000 solid", ModuleConfiguration.Border)
                ContentPane.Style("border-left") = String.Format("{0}px #000000 solid", ModuleConfiguration.Border)
            End If

            ' display visual indicator if module is only visible to administrators
            Dim adminMessage As String = Null.NullString
            Dim viewRoles As String = Null.NullString
            If ModuleConfiguration.InheritViewPermissions Then
                viewRoles = TabPermissionController.GetTabPermissions(ModuleConfiguration.TabID, ModuleConfiguration.PortalID).ToString("VIEW")
            Else
                viewRoles = ModuleConfiguration.ModulePermissions.ToString("VIEW")
            End If
            viewRoles = viewRoles.Replace(";"c, String.Empty).Trim().ToLowerInvariant

            Dim isAdminTab As Boolean = False
            If PortalSettings.ActiveTab.IsSuperTab OrElse _
                    PortalSettings.ActiveTab.TabID = PortalSettings.AdminTabId OrElse _
                    PortalSettings.ActiveTab.ParentId = PortalSettings.AdminTabId Then
                isAdminTab = True
            End If
            'get settings from hash table
            Dim showMessage As Boolean = False
            If viewRoles = PortalSettings.AdministratorRoleName.ToLowerInvariant AndAlso Not isAdminTab Then
                adminMessage = Localization.GetString("ModuleVisibleAdministrator.Text")

                Dim objModules As New ModuleController
                Dim borderHash As New Hashtable
                borderHash = objModules.GetTabModuleSettings(ModuleConfiguration.TabModuleID)
                If (Not Null.IsNull(borderHash("hideadminborder"))) Or (Not borderHash("hideadminborder") Is Nothing) Then
                    showMessage = DirectCast(borderHash("hideadminborder"), Boolean)
                Else
                    showMessage = False
                End If
            End If
            If ModuleConfiguration.StartDate >= Now Then
                adminMessage = String.Format(Localization.GetString("ModuleEffective.Text"), ModuleConfiguration.StartDate.ToShortDateString())
                showMessage = True
            End If
            If ModuleConfiguration.EndDate <= Now Then
                adminMessage = String.Format(Localization.GetString("ModuleExpired.Text"), ModuleConfiguration.EndDate.ToShortDateString())
                showMessage = True
            End If
            If showMessage Then
                AddAdministratorOnlyHighlighting(adminMessage)
            End If
        End Sub

        Private Sub AddAdministratorOnlyHighlighting(ByVal message As String)
            Dim cssclass As String = ContentPane.Attributes("class")
            If String.IsNullOrEmpty(cssclass) Then
                ContentPane.Attributes("class") = c_ContainerAdminBorder
            Else
                ContentPane.Attributes("class") = String.Format("{0} {1}", cssclass.Replace(c_ContainerAdminBorder, "").Trim().Replace("  ", " "), c_ContainerAdminBorder)
            End If

            ContentPane.Controls.Add(New LiteralControl(String.Format("<div class=""NormalRed DNNAligncenter"">{0}</div>", message)))
        End Sub
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessFooter adds an optional footer (and an End_Module comment)..
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessFooter()
            ' inject the footer
            If ModuleConfiguration.Footer <> "" Then
                Dim objLabel As New Label
                objLabel.Text = ModuleConfiguration.Footer
                objLabel.CssClass = "Normal"
                ContentPane.Controls.Add(objLabel)
            End If

            ' inject an end comment around the module content
            If Not IsAdminControl() Then
                ContentPane.Controls.Add(New LiteralControl("<!-- End_Module_" & ModuleConfiguration.ModuleID.ToString & " -->"))
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessHeader adds an optional header (and a Start_Module_ comment)..
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessHeader()
            If Not IsAdminControl() Then
                ' inject a start comment around the module content
                ContentPane.Controls.Add(New LiteralControl("<!-- Start_Module_" & ModuleConfiguration.ModuleID.ToString & " -->"))
            End If

            ' inject the header
            If ModuleConfiguration.Header <> "" Then
                Dim objLabel As New Label
                objLabel.Text = ModuleConfiguration.Header
                objLabel.CssClass = "Normal"
                ContentPane.Controls.Add(objLabel)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessModule processes the module which is attached to this container
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessModule()

            If ContentPane IsNot Nothing Then
                'Process Content Pane Attributes
                ProcessContentPane()

                'Process Module Header
                ProcessHeader()

                'Try to load the module control
                _ModuleHost = New ModuleHost(ModuleConfiguration, ParentSkin)
                ContentPane.Controls.Add(ModuleHost)

                'Process Module Footer
                ProcessFooter()

                'Process the Action Controls
                If ModuleHost IsNot Nothing AndAlso ModuleControl IsNot Nothing Then
                    ProcessChildControls(Me)
                End If

                'Add Module Stylesheets
                ProcessStylesheets(ModuleHost IsNot Nothing)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessStylesheets processes the Module and Container stylesheets and adds
        ''' them to the Page.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessStylesheets(ByVal includeModuleCss As Boolean)
            Dim blnUpdateCache As Boolean = False

            'Get a reference to the Page
            Dim DefaultPage As CDefault = DirectCast(Page, CDefault)

            Dim ID As String
            Dim objCSSCache As Hashtable = Nothing
            If Host.PerformanceSetting <> Common.Globals.PerformanceSettings.NoCaching Then
                objCSSCache = CType(DataCache.GetCache("CSS"), Hashtable)
            End If
            If objCSSCache Is Nothing Then
                objCSSCache = New Hashtable
            End If

            ' container package style sheet
            ID = CreateValidID(ContainerPath)
            If objCSSCache.ContainsKey(ID) = False Then
                If File.Exists(Server.MapPath(ContainerPath) & "container.css") Then
                    objCSSCache(ID) = ContainerPath & "container.css"
                Else
                    objCSSCache(ID) = ""
                End If
                blnUpdateCache = True
            End If
            If objCSSCache(ID).ToString <> "" Then
                DefaultPage.AddStyleSheet(ID, objCSSCache(ID).ToString)
            End If

            ' container file style sheet
            ID = CreateValidID(ContainerSrc.Replace(".ascx", ".css"))
            If objCSSCache.ContainsKey(ID) = False Then
                If File.Exists(Server.MapPath(Replace(ContainerSrc, ".ascx", ".css"))) Then
                    objCSSCache(ID) = Replace(ContainerSrc, ".ascx", ".css")
                Else
                    objCSSCache(ID) = ""
                End If
                blnUpdateCache = True
            End If
            If objCSSCache(ID).ToString <> "" Then
                DefaultPage.AddStyleSheet(ID, objCSSCache(ID).ToString)
            End If

            ' process the base class module properties 
            If includeModuleCss Then
                Dim controlSrc As String = ModuleConfiguration.ModuleControl.ControlSrc
                Dim folderName As String = ModuleConfiguration.DesktopModule.FolderName

                ' module stylesheet
                ID = CreateValidID(Common.Globals.ApplicationPath & "/DesktopModules/" & folderName)
                If objCSSCache.ContainsKey(ID) = False Then
                    ' default to nothing
                    objCSSCache(ID) = ""

                    ' 1.try to load module.css from module folder
                    If File.Exists(Server.MapPath(Common.Globals.ApplicationPath & "/DesktopModules/" & folderName & "/module.css")) Then
                        objCSSCache(ID) = Common.Globals.ApplicationPath & "/DesktopModules/" & folderName & "/module.css"
                    Else
                        ' 2.otherwise try to load from Path to control
                        If controlSrc.ToLower.EndsWith(".ascx") Then
                            If File.Exists(Server.MapPath(Common.Globals.ApplicationPath & "/" & controlSrc.Substring(0, controlSrc.LastIndexOf("/") + 1)) & "module.css") Then
                                objCSSCache(ID) = Common.Globals.ApplicationPath & "/" & controlSrc.Substring(0, controlSrc.LastIndexOf("/") + 1) & "module.css"
                            End If
                        End If
                    End If
                    blnUpdateCache = True
                End If

                If objCSSCache.ContainsKey(ID) AndAlso Not String.IsNullOrEmpty(objCSSCache(ID).ToString()) Then
                    'Add it to beginning of style list
                    DefaultPage.AddStyleSheet(ID, objCSSCache(ID).ToString(), True)
                End If
            End If

            If Host.PerformanceSetting <> Common.Globals.PerformanceSettings.NoCaching Then
                If blnUpdateCache Then
                    DataCache.SetCache("CSS", objCSSCache)
                End If
            End If
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnInit runs when the Container is initialised.
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	07/04/2005	Documented
        '''     [cnurse]    12/05/2007  Refactored
        '''     [cnurse]    04/17/2009  Refactored to use ContainerAdapter
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            'Call base classes method
            MyBase.OnInit(e)

            For Each listener As ContainerEventListener In DotNetNukeContext.Current.ContainerEventListeners
                If listener.EventType = ContainerEventType.OnContainerInit Then
                    listener.ContainerEvent.Invoke(Me, New ContainerEventArgs(Me))
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnLoad runs when the Container is loaded.
        ''' </summary>
        ''' <history>
        '''     [cnurse]    04/17/2009  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            For Each listener As ContainerEventListener In DotNetNukeContext.Current.ContainerEventListeners
                If listener.EventType = ContainerEventType.OnContainerLoad Then
                    listener.ContainerEvent.Invoke(Me, New ContainerEventArgs(Me))
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnLoad runs just before the Container is rendered.
        ''' </summary>
        ''' <history>
        '''     [cnurse]    04/17/2009  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            For Each listener As ContainerEventListener In DotNetNukeContext.Current.ContainerEventListeners
                If listener.EventType = ContainerEventType.OnContainerPreRender Then
                    listener.ContainerEvent.Invoke(Me, New ContainerEventArgs(Me))
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnUnLoad runs when the Container is unloaded.
        ''' </summary>
        ''' <history>
        '''     [cnurse]    04/17/2009  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnUnLoad(ByVal e As System.EventArgs)
            MyBase.OnUnload(e)

            For Each listener As ContainerEventListener In DotNetNukeContext.Current.ContainerEventListeners
                If listener.EventType = ContainerEventType.OnContainerUnLoad Then
                    listener.ContainerEvent.Invoke(Me, New ContainerEventArgs(Me))
                End If
            Next
        End Sub

#End Region

#Region "Public Methods"

        Public Sub SetModuleConfiguration(ByVal configuration As ModuleInfo)
            _ModuleConfiguration = configuration
            ProcessModule()
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ModuleAction_Click runs when a ModuleAction is clicked.
        ''' </summary>
        ''' <remarks>The Module Action must be configured to fire an event (it may be configured 
        ''' to redirect to a new url).  The event handler finds the Parent Container and invokes each
        ''' registered ModuleActionEventListener delegate.
        ''' 
        ''' Note: with the refactoring of this to the Container, this could be handled at the container level.
        ''' However, for legacy purposes this is left this way, as many moodules would have registered their
        ''' listeners on the Container directly, rather than through the helper method in PortalModuleBase.</remarks>
        ''' <history>
        ''' 	[cnurse]	07/04/2005	Documented
        '''     [cnurse]    12/05/2007  Moved from Container.vb
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ModuleAction_Click(ByVal sender As Object, ByVal e As ActionEventArgs)
            'Search through the listeners
            Dim Listener As ModuleActionEventListener
            For Each Listener In ParentSkin.ActionEventListeners

                'If the associated module has registered a listener
                If e.ModuleConfiguration.ModuleID = Listener.ModuleID Then

                    'Invoke the listener to handle the ModuleAction_Click event
                    Listener.ActionEvent.Invoke(sender, e)
                End If
            Next
        End Sub

#End Region

#Region "Obsolete"

        <Obsolete("Deprecated in 5.0. Shouldn't need to be used any more.  ContainerObjects (IContainerControl implementations) have a property ModuleControl.")> _
        Public Shared Function GetPortalModuleBase(ByVal objControl As UserControl) As PortalModuleBase
            Dim objModuleControl As PortalModuleBase = Nothing
            Dim ctlPanel As Panel

            If TypeOf objControl Is UI.Skins.SkinObjectBase Then
                ctlPanel = CType(objControl.Parent.FindControl("ModuleContent"), Panel)
            Else
                ctlPanel = CType(objControl.FindControl("ModuleContent"), Panel)
            End If

            If Not ctlPanel Is Nothing Then
                Try
                    objModuleControl = CType(ctlPanel.Controls(1), PortalModuleBase)
                Catch
                    ' check if it is nested within an UpdatePanel 
                    Try
                        objModuleControl = CType(ctlPanel.Controls(0).Controls(0).Controls(1), PortalModuleBase)
                    Catch
                    End Try
                End Try
            End If

            If objModuleControl Is Nothing Then
                objModuleControl = New PortalModuleBase
                objModuleControl.ModuleConfiguration = New ModuleInfo
            End If

            Return objModuleControl
        End Function

        <Obsolete("Deprecated in 5.1. Replaced by ContainerPath")> _
        Public ReadOnly Property SkinPath() As String
            Get
                Return ContainerPath
            End Get
        End Property

#End Region

    End Class

End Namespace