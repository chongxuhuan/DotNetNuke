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

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.Security
Imports DotNetNuke.UI.Skins
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Modules
Imports System.Diagnostics.CodeAnalysis

Namespace DotNetNuke.UI.Containers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Containers
    ''' Class	 : ActionBase
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ActionBase is an abstract base control for Action objects that inherit from UserControl.
    ''' </summary>
    ''' <remarks>
    ''' ActionBase inherits from UserControl, and implements the IActionControl Interface
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	10/07/2004	Documented
    '''     [cnurse]    12/15/2007  Refactored 
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public MustInherit Class ActionBase
        Inherits UserControl
        Implements IActionControl

#Region "Private Members"

        Private _ActionManager As ActionManager
        Private _ActionRoot As ModuleAction
        Private _ModuleControl As IModuleControl

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Actions Collection
        ''' </summary>
        ''' <returns>A ModuleActionCollection</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property Actions() As ModuleActionCollection
            Get
                Return ModuleContext.Actions
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ActionRoot
        ''' </summary>
        ''' <returns>A ModuleActionCollection</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ActionRoot() As ModuleAction
            Get
                If _ActionRoot Is Nothing Then
                    _ActionRoot = New ModuleAction(ModuleContext.GetNextActionID(), " ", "", "", "action.gif")
                End If
                Return _ActionRoot
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ModuleContext
        ''' </summary>
        ''' <returns>A ModuleInstanceContext</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ModuleContext() As ModuleInstanceContext
            Get
                Return ModuleControl.ModuleContext
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the PortalSettings
        ''' </summary>
        ''' <returns>A PortalSettings object</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PortalSettings() As PortalSettings
            Get
                Dim _settings As PortalSettings = ModuleControl.ModuleContext.PortalSettings
                'following If clase left to preserve backwards compatibility
                'liable to be removed if related obsolete variable gets removed
                If Not _settings.ActiveTab.IsSuperTab Then
                    m_tabPreview = (_settings.UserMode = PortalSettings.Mode.View)
                End If
                Return _settings
            End Get
        End Property

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DisplayControl determines whether the control should be displayed
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function DisplayControl(ByVal objNodes As DNNNodeCollection) As Boolean
            Return ActionManager.DisplayControl(objNodes)
        End Function

        Public ReadOnly Property EditMode() As Boolean
            Get
                Return Not (ModuleContext.PortalSettings.UserMode = DotNetNuke.Entities.Portals.PortalSettings.Mode.View)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnAction raises the Action Event for this control
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overridable Sub OnAction(ByVal e As ActionEventArgs)
            RaiseEvent Action(Me, e)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessAction processes the action event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/23/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub ProcessAction(ByVal ActionID As String)
            If IsNumeric(ActionID) Then
                Dim action As ModuleAction = Actions.GetActionByID(Convert.ToInt32(ActionID))
                If action IsNot Nothing Then
                    If Not ActionManager.ProcessAction(action) Then
                        OnAction(New ActionEventArgs(action, ModuleContext.Configuration))
                    End If
                End If
            End If
        End Sub

        Protected m_supportsIcons As Boolean = True
        Public ReadOnly Property SupportsIcons() As Boolean
            Get
                Return m_supportsIcons
            End Get
        End Property

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the class is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	05/12/2005	Documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                ActionRoot.Actions.AddRange(Actions)
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "IActionControl Members"

        Public Event Action As ActionEventHandler Implements IActionControl.Action

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ActionManager instance for this Action control
        ''' </summary>
        ''' <returns>An ActionManager object</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public ReadOnly Property ActionManager() As ActionManager Implements IActionControl.ActionManager
            Get
                If _ActionManager Is Nothing Then
                    _ActionManager = New ActionManager(Me)
                End If
                Return _ActionManager
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the ModuleControl instance for this Action control
        ''' </summary>
        ''' <returns>An IModuleControl object</returns>
        ''' <history>
        ''' 	[cnurse]	12/15/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ModuleControl() As IModuleControl Implements IActionControl.ModuleControl
            Get
                Return _ModuleControl
            End Get
            Set(ByVal Value As IModuleControl)
                _ModuleControl = Value
            End Set
        End Property

#End Region

#Region "Obsolete Members"

        <Obsolete("Obsoleted in DotNetNuke 5.0. Use ModuleContext.Configuration")> _
        Public ReadOnly Property ModuleConfiguration() As ModuleInfo
            Get
                Return ModuleContext.Configuration
            End Get
        End Property

        <Obsolete("Obsoleted in DotNetNuke 5.0. Replaced by ModuleControl")> _
        Public Property PortalModule() As PortalModuleBase
            Get
                Return New PortalModuleBase
            End Get
            Set(ByVal Value As PortalModuleBase)
                ModuleControl = Value
            End Set
        End Property

        <Obsolete("Obsoleted in DotNetNuke 5.1.2 Replaced by ActionRoot Property")> _
        Protected m_menuActionRoot As ModuleAction

        <Obsolete("Obsoleted in DotNetNuke 5.1.2. The concept of an adminControl no longer exists.")> _
        Protected m_adminControl As Boolean = False

        <Obsolete("Obsoleted in DotNetNuke 5.1.2. The concept of an adminModule no longer exists.")> _
        Protected m_adminModule As Boolean = False

        <Obsolete("Obsoleted in DotNetNuke 5.1.2. No longer neccessary as there is no concept of an Admin Page")> _
        Protected m_tabPreview As Boolean = False

        <Obsolete("Obsoleted in DotNetNuke 5.1.2. Replaced by Actions Property")> _
        Protected m_menuActions As ModuleActionCollection

        <Obsolete("Obsoleted in DotNetNuke 5.1.2. Replaced by Actions Property")> _
        Public ReadOnly Property MenuActions() As ModuleActionCollection
            Get
                Return Actions
            End Get
        End Property

#End Region

    End Class

End Namespace
