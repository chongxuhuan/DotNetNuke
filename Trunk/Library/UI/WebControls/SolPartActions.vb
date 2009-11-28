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

Namespace DotNetNuke.UI.WebControls

	Public  Class SolPartActions
        Inherits UI.Containers.ActionBase

        Private m_objControl As NavigationProvider.NavigationProvider
        Private m_strProviderName As String = "SolpartMenuNavigationProvider"
        Private m_strPathSystemScript As String
        Private m_blnPopulateNodesFromClient As Boolean = False       'JH - POD
        Private m_intExpandDepth As Integer = -1          'JH - POD

        Public Property ProviderName() As String
            Get
                Return m_strProviderName
            End Get
            Set(ByVal Value As String)
                m_strProviderName = Value
            End Set
        End Property

        Public Property PopulateNodesFromClient() As Boolean
            Get
                Return m_blnPopulateNodesFromClient
            End Get
            Set(ByVal Value As Boolean)
                m_blnPopulateNodesFromClient = Value
            End Set
        End Property

        Public Property ExpandDepth() As Integer          'JH - POD
            Get
                If PopulateNodesFromClient = False OrElse Control.SupportsPopulateOnDemand = False Then
                    Return -1
                End If
                Return m_intExpandDepth
            End Get
            Set(ByVal Value As Integer)
                m_intExpandDepth = Value
            End Set
        End Property

        Public ReadOnly Property Control() As NavigationProvider.NavigationProvider     'Modules.ActionProvider.ActionProvider
            Get
                Return m_objControl
            End Get
        End Property

        Public Property PathSystemScript() As String
            Get
                Return m_strPathSystemScript
            End Get
            Set(ByVal Value As String)
                m_strPathSystemScript = Value
            End Set
        End Property

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            SetMenuDefaults()
        End Sub

        Private Sub SetMenuDefaults()
            Try
                '--- original page set attributes ---'
                Control.StyleIconWidth = 15
                Control.MouseOutHideDelay = 500
                Control.MouseOverAction = NavigationProvider.NavigationProvider.HoverAction.Expand
                Control.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.None
                'Control.MenuEffectsStyle = " "
                'Menu.ShadowColor = System.Drawing.Color.Gray
                'Menu.MenuEffects.Style  "filter:progid:DXImageTransform.Microsoft.Shadow(color='DimGray', Direction=135, Strength=3) ;"
                'Control.MouseOverDisplay = MenuEffectsMouseOverDisplay.None
                'Control.MouseOverAction = ActionProvider.ActionProvider.HoverAction.Expand

                ' style sheet settings
                Control.CSSControl = "ModuleTitle_MenuBar"                'ctlActions.MenuCSS.MenuBar
                Control.CSSContainerRoot = "ModuleTitle_MenuContainer"              'ctlActions.MenuCSS.MenuContainer
                Control.CSSNode = "ModuleTitle_MenuItem"                 'ctlActions.MenuCSS.MenuItem
                Control.CSSIcon = "ModuleTitle_MenuIcon"                 ' ctlActions.MenuCSS.MenuIcon
                Control.CSSContainerSub = "ModuleTitle_SubMenu"              ' ctlActions.MenuCSS.SubMenu
                Control.CSSBreak = "ModuleTitle_MenuBreak"              'ctlActions.MenuCSS.MenuBreak
                Control.CSSNodeHover = "ModuleTitle_MenuItemSel"                'ctlActions.MenuCSS.MenuItemSel
                Control.CSSIndicateChildSub = "ModuleTitle_MenuArrow"                'ctlActions.MenuCSS.MenuArrow
                Control.CSSIndicateChildRoot = "ModuleTitle_RootMenuArrow"               'ctlActions.MenuCSS.RootMenuArrow

                ' generate dynamic menu
                If Len(Control.PathSystemScript) = 0 Then Control.PathSystemScript = Common.Globals.ApplicationPath & "/Controls/SolpartMenu/"
                Control.PathImage = Common.Globals.ApplicationPath & "/Images/"
                Control.PathSystemImage = Common.Globals.ApplicationPath & "/Images/"
                Control.IndicateChildImageSub = "action_right.gif"
                Control.IndicateChildren = True

                Control.StyleRoot = "background-color: Transparent; font-size: 1pt;"                'backwards compatibility HACK

                AddHandler Control.NodeClick, AddressOf ctlActions_MenuClick

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub ctlActions_MenuClick(ByVal args As NavigationProvider.NavigationEventArgs)         'Handles ctlActions.MenuClick
            Try
                ProcessAction(args.ID)
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Public Sub BindMenu()
            BindMenu(Navigation.GetActionNodes(Me.ActionRoot, Me, Me.ExpandDepth))
        End Sub

        Private Sub BindMenu(ByVal objNodes As DNNNodeCollection)
            Me.Visible = DisplayControl(objNodes)

            If Me.Visible Then
                Me.Control.ClearNodes()                'since we always bind we need to clear the nodes for providers that maintain their state
                For Each objNode As DNNNode In objNodes
                    ProcessNodes(objNode)
                Next
                Control.Bind(objNodes)
            End If
        End Sub

        Private Sub ProcessNodes(ByVal objParent As DNNNode)
            If Len(objParent.JSFunction) > 0 Then
                objParent.JSFunction = String.Format("if({0}){{{1}}};", objParent.JSFunction, Page.ClientScript.GetPostBackEventReference(Control.NavigationControl, objParent.ID))
            End If

            Dim objNode As DNNNode
            For Each objNode In objParent.DNNNodes
                ProcessNodes(objNode)
            Next
        End Sub

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.PreRender
            Try
                BindMenu()

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            m_objControl = NavigationProvider.NavigationProvider.Instance(Me.ProviderName)
            AddHandler Control.PopulateOnDemand, AddressOf Control_PopulateOnDemand

            MyBase.OnInit(e)
            Control.ControlID = "ctl" & Me.ID
            Control.Initialize()
            Me.Controls.Add(Control.NavigationControl)
        End Sub

        Private Sub Control_PopulateOnDemand(ByVal args As NavigationProvider.NavigationEventArgs)
            SetMenuDefaults()
            ActionRoot.Actions.AddRange(Actions)             'Modules how add custom actions in control lifecycle will not have those actions populated...  

            Dim objAction As Entities.Modules.Actions.ModuleAction = ActionRoot
            If ActionRoot.ID <> CInt(args.ID) Then
                objAction = Me.ActionManager.GetAction(CInt(args.ID))
            End If
            If args.Node Is Nothing Then
                args.Node = Navigation.GetActionNode(args.ID, Control.ID, objAction, Me)
            End If
            Me.Control.ClearNodes()          'since we always bind we need to clear the nodes for providers that maintain their state
            Me.BindMenu(Navigation.GetActionNodes(objAction, args.Node, Me, Me.ExpandDepth))
        End Sub

    End Class
End Namespace
