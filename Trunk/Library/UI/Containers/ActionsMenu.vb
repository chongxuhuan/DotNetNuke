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
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Modules

Namespace DotNetNuke.UI.Containers

    ''' -----------------------------------------------------------------------------
    ''' Project	 : DotNetNuke
    ''' Namespace: DotNetNuke.UI.Containers
    ''' Class	 : ActionsMenu
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' ActionsMenu provides a menu for a collection of actions.
    ''' </summary>
    ''' <remarks>
    ''' ActionsMenu inherits from CompositeControl, and implements the IActionControl 
    ''' Interface. It uses the Navigation Providers to implement the Menu.
    ''' </remarks>
    ''' <history>
    ''' 	[cnurse]	12/24/2007  created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class ActionsMenu
        Inherits Control
        Implements IActionControl

#Region "Private Members"

        Private _ActionManager As ActionManager
        Private _ActionRoot As ModuleAction
        Private _ModuleControl As IModuleControl

        Private _ExpandDepth As Integer = -1
        Private _PathSystemScript As String
        Private _PopulateNodesFromClient As Boolean = False
        Private _ProviderControl As NavigationProvider.NavigationProvider
        Private _ProviderName As String = "DNNMenuNavigationProvider"

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the ActionRoot
        ''' </summary>
        ''' <returns>A ModuleActionCollection</returns>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ActionRoot() As ModuleAction
            Get
                If _ActionRoot Is Nothing Then
                    _ActionRoot = New ModuleAction(ModuleControl.ModuleContext.GetNextActionID(), " ", "", "", "action.gif")
                End If
                Return _ActionRoot
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Provider Control
        ''' </summary>
        ''' <returns>A NavigationProvider</returns>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property ProviderControl() As NavigationProvider.NavigationProvider
            Get
                Return _ProviderControl
            End Get
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Expansion Depth for the Control
        ''' </summary>
        ''' <returns>An Integer</returns>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ExpandDepth() As Integer
            Get
                If PopulateNodesFromClient = False OrElse ProviderControl.SupportsPopulateOnDemand = False Then
                    Return -1
                End If
                Return _ExpandDepth
            End Get
            Set(ByVal Value As Integer)
                _ExpandDepth = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Path to the Script Library for the provider
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PathSystemScript() As String
            Get
                Return _PathSystemScript
            End Get
            Set(ByVal Value As String)
                _PathSystemScript = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets whether the Menu should be populated from the client
        ''' </summary>
        ''' <returns>A Boolean</returns>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property PopulateNodesFromClient() As Boolean
            Get
                Return _PopulateNodesFromClient
            End Get
            Set(ByVal Value As Boolean)
                _PopulateNodesFromClient = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and Sets the Name of the provider to use
        ''' </summary>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ProviderName() As String
            Get
                Return _ProviderName
            End Get
            Set(ByVal Value As String)
                _ProviderName = Value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindMenu binds the Navigation Provider to the Node Collection
        ''' </summary>
        ''' <param name="objNodes">The Nodes collection to bind</param>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindMenu(ByVal objNodes As DNNNodeCollection)
            Me.Visible = ActionManager.DisplayControl(objNodes)

            If Me.Visible Then
                'since we always bind we need to clear the nodes for providers that maintain their state
                Me.ProviderControl.ClearNodes()
                For Each objNode As DNNNode In objNodes
                    ProcessNodes(objNode)
                Next
                ProviderControl.Bind(objNodes)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProcessNodes proceses a single node and its children
        ''' </summary>
        ''' <param name="objParent">The Node to process</param>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProcessNodes(ByVal objParent As DNNNode)
            If Len(objParent.JSFunction) > 0 Then
                objParent.JSFunction = String.Format("if({0}){{{1}}};", objParent.JSFunction, Page.ClientScript.GetPostBackEventReference(ProviderControl.NavigationControl, objParent.ID))
            End If

            Dim objNode As DNNNode
            For Each objNode In objParent.DNNNodes
                ProcessNodes(objNode)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' SetMenuDefaults sets up the default values
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub SetMenuDefaults()
            Try
                '--- original page set attributes ---'
                ProviderControl.StyleIconWidth = 15
                ProviderControl.MouseOutHideDelay = 500
                ProviderControl.MouseOverAction = NavigationProvider.NavigationProvider.HoverAction.Expand
                ProviderControl.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.None

                ' style sheet settings
                ProviderControl.CSSControl = "ModuleTitle_MenuBar"
                ProviderControl.CSSContainerRoot = "ModuleTitle_MenuContainer"
                ProviderControl.CSSNode = "ModuleTitle_MenuItem"
                ProviderControl.CSSIcon = "ModuleTitle_MenuIcon"
                ProviderControl.CSSContainerSub = "ModuleTitle_SubMenu"
                ProviderControl.CSSBreak = "ModuleTitle_MenuBreak"
                ProviderControl.CSSNodeHover = "ModuleTitle_MenuItemSel"
                ProviderControl.CSSIndicateChildSub = "ModuleTitle_MenuArrow"
                ProviderControl.CSSIndicateChildRoot = "ModuleTitle_RootMenuArrow"

                ' generate dynamic menu
                If Len(ProviderControl.PathSystemScript) = 0 Then ProviderControl.PathSystemScript = Common.Globals.ApplicationPath & "/Controls/SolpartMenu/"
                ProviderControl.PathImage = Common.Globals.ApplicationPath & "/Images/"
                ProviderControl.PathSystemImage = Common.Globals.ApplicationPath & "/Images/"
                ProviderControl.IndicateChildImageSub = "action_right.gif"
                ProviderControl.IndicateChildren = True

                ProviderControl.StyleRoot = "background-color: Transparent; font-size: 1pt;"                'backwards compatibility HACK

                AddHandler ProviderControl.NodeClick, AddressOf MenuItem_Click

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindMenu binds the Navigation Provider to the Node Collection
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub BindMenu()
            BindMenu(Navigation.GetActionNodes(Me.ActionRoot, Me, Me.ExpandDepth))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnAction raises the Action Event
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
        ''' OnInit runs during the controls initialisation phase
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/02/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            _ProviderControl = NavigationProvider.NavigationProvider.Instance(Me.ProviderName)
            AddHandler ProviderControl.PopulateOnDemand, AddressOf ProviderControl_PopulateOnDemand

            MyBase.OnInit(e)
            ProviderControl.ControlID = "ctl" & Me.ID
            ProviderControl.Initialize()
            Me.Controls.Add(ProviderControl.NavigationControl)
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnLoad runs during the controls load phase
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/02/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
            MyBase.OnLoad(e)

            'Add the Actions to the Action Root
            ActionRoot.Actions.AddRange(ModuleControl.ModuleContext.Actions)

            'Set Menu Defaults
            SetMenuDefaults()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' OnPreRender runs during the controls pre-render phase
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	01/02/2008  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)

            BindMenu()
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' MenuItem_Click handles the Menu Click event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub MenuItem_Click(ByVal args As NavigationProvider.NavigationEventArgs)
            If IsNumeric(args.ID) Then
                Dim action As ModuleAction = ModuleControl.ModuleContext.Actions.GetActionByID(Convert.ToInt32(args.ID))
                If Not ActionManager.ProcessAction(action) Then
                    OnAction(New ActionEventArgs(action, ModuleControl.ModuleContext.Configuration))
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ProviderControl_PopulateOnDemand handles the Populate On Demand Event
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/24/2007  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ProviderControl_PopulateOnDemand(ByVal args As NavigationProvider.NavigationEventArgs)
            SetMenuDefaults()
            ActionRoot.Actions.AddRange(ModuleControl.ModuleContext.Actions)             'Modules how add custom actions in control lifecycle will not have those actions populated...  

            Dim objAction As Entities.Modules.Actions.ModuleAction = ActionRoot
            If ActionRoot.ID <> CInt(args.ID) Then
                objAction = ModuleControl.ModuleContext.Actions.GetActionByID(CInt(args.ID))
            End If
            If args.Node Is Nothing Then
                args.Node = Navigation.GetActionNode(args.ID, ProviderControl.ID, objAction, Me)
            End If
            Me.ProviderControl.ClearNodes()          'since we always bind we need to clear the nodes for providers that maintain their state
            Me.BindMenu(Navigation.GetActionNodes(objAction, args.Node, Me, Me.ExpandDepth))
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
        ''' 	[cnurse]	12/24/2007  created
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
        ''' 	[cnurse]	12/24/2007  created
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

    End Class
End Namespace
