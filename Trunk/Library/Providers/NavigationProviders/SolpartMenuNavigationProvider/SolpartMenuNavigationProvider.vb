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
Imports System.Web
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common
Imports DotNetNuke.Framework.Providers
Imports Solpart.WebControls

Namespace DotNetNuke.NavigationControl
	Public Class SolpartMenuNavigationProvider
		Inherits DotNetNuke.Modules.NavigationProvider.NavigationProvider
		Private m_objMenu As SolpartMenu
		Private m_strControlID As String
		Private m_strCSSBreadCrumbSub As String
		Private m_strIndicateChildImageBreadCrumbSub As String
		Private m_strIndicateChildImageBreadCrumbRoot As String
		Private m_strCSSBreadCrumbRoot As String

		Private m_strCSSLeftSeparator As String
		Private m_strCSSLeftSeparatorBreadCrumb As String
		Private m_strCSSLeftSeparatorSelection As String
		Private m_strCSSRightSeparator As String
		Private m_strCSSRightSeparatorBreadCrumb As String
		Private m_strCSSRightSeparatorSelection As String
		Private m_strNodeSelectedSub As String
		Private m_strNodeSelectedRoot As String
		Private m_strCSSNodeRoot As String
		Private m_strCSSNodeHoverSub As String
		Private m_strCSSNodeHoverRoot As String
		Private m_strCSSSeparator As String
		Private m_strNodeLeftHTMLSub As String = ""
		Private m_strNodeLeftHTMLBreadCrumbSub As String = ""
		Private m_strNodeLeftHTMLBreadCrumbRoot As String = ""
		Private m_strNodeLeftHTMLRoot As String = ""
		Private m_strNodeRightHTMLSub As String = ""
		Private m_strNodeRightHTMLBreadCrumbSub As String = ""
		Private m_strNodeRightHTMLBreadCrumbRoot As String = ""
		Private m_strNodeRightHTMLRoot As String = ""
		Private m_strSeparatorHTML As String = ""
		Private m_strSeparatorLeftHTML As String = ""
		Private m_strSeparatorLeftHTMLBreadCrumb As String = ""
		Private m_strSeparatorLeftHTMLActive As String = ""
		Private m_strSeparatorRightHTML As String = ""
		Private m_strSeparatorRightHTMLBreadCrumb As String = ""
		Private m_strSeparatorRightHTMLActive As String = ""
		Private m_blnIndicateChildren As Boolean
		Private m_strStyleRoot As String

		Public ReadOnly Property Menu() As SolpartMenu
			Get
				Return m_objMenu
			End Get
		End Property

		Public Overrides ReadOnly Property NavigationControl() As System.Web.UI.Control
			Get
				Return Menu
			End Get
		End Property

		Public Overrides ReadOnly Property SupportsPopulateOnDemand() As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Property IndicateChildImageSub() As String
			Get
				Return Menu.ArrowImage
			End Get
			Set(ByVal Value As String)
				Menu.ArrowImage = Value
			End Set
		End Property

		Public Overrides Property IndicateChildImageRoot() As String
			Get
				Return Menu.RootArrowImage
			End Get
			Set(ByVal Value As String)
				Menu.RootArrowImage = Value
			End Set
		End Property

		Public Overrides Property ControlAlignment() As Modules.NavigationProvider.NavigationProvider.Alignment
			Get
				Select Case Menu.MenuAlignment.ToLower
					Case "left"
						Return Modules.NavigationProvider.NavigationProvider.Alignment.Left
					Case "right"
						Return Modules.NavigationProvider.NavigationProvider.Alignment.Right
					Case "center"
						Return Modules.NavigationProvider.NavigationProvider.Alignment.Center
					Case "justify"
						Return Modules.NavigationProvider.NavigationProvider.Alignment.Justify
				End Select
			End Get
			Set(ByVal Value As Modules.NavigationProvider.NavigationProvider.Alignment)
				Select Case Value
					Case Modules.NavigationProvider.NavigationProvider.Alignment.Left
						Menu.MenuAlignment = "Left"
					Case Modules.NavigationProvider.NavigationProvider.Alignment.Right
						Menu.MenuAlignment = "Right"
					Case Modules.NavigationProvider.NavigationProvider.Alignment.Center
						Menu.MenuAlignment = "Center"
					Case Modules.NavigationProvider.NavigationProvider.Alignment.Justify
						Menu.MenuAlignment = "Justify"
				End Select
			End Set
		End Property

		Public Overrides Property ControlID() As String
			Get
				Return m_strControlID
			End Get
			Set(ByVal Value As String)
				m_strControlID = Value
			End Set
		End Property

		Public Overrides Property ControlOrientation() As Modules.NavigationProvider.NavigationProvider.Orientation
			Get
				Select Case Menu.Display.ToLower
					Case "horizontal"
						Return Modules.NavigationProvider.NavigationProvider.Orientation.Horizontal
					Case "vertical"
						Return Modules.NavigationProvider.NavigationProvider.Orientation.Vertical
				End Select
			End Get
			Set(ByVal Value As Modules.NavigationProvider.NavigationProvider.Orientation)
				Select Case Value
					Case Modules.NavigationProvider.NavigationProvider.Orientation.Horizontal
						Menu.Display = "Horizontal"
					Case Modules.NavigationProvider.NavigationProvider.Orientation.Vertical
						Menu.Display = "Vertical"
				End Select
			End Set
		End Property

		Public Overrides Property CSSIndicateChildSub() As String
			Get
				Return Menu.MenuCSS.MenuArrow
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.MenuArrow = Value
			End Set
		End Property

		Public Overrides Property CSSIndicateChildRoot() As String
			Get
				Return Menu.MenuCSS.RootMenuArrow
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.RootMenuArrow = Value
			End Set
		End Property

		Public Overrides Property CSSBreadCrumbSub() As String
			Get
				Return m_strCSSBreadCrumbSub
			End Get
			Set(ByVal Value As String)
				m_strCSSBreadCrumbSub = Value
			End Set
		End Property

        Public Overrides Property CSSBreadCrumbRoot() As String
            Get
                Return m_strCSSBreadCrumbRoot
            End Get
            Set(ByVal Value As String)
                m_strCSSBreadCrumbRoot = Value
            End Set
        End Property

		Public Overrides Property CSSBreak() As String
			Get
				Return Menu.MenuCSS.MenuBreak
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.MenuBreak = Value
			End Set
		End Property

		Public Overrides Property CSSContainerRoot() As String
			Get
				Return Menu.MenuCSS.MenuContainer
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.MenuContainer = Value
			End Set
		End Property

		Public Overrides Property CSSControl() As String
			Get
				Return Menu.MenuCSS.MenuBar
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.MenuBar = Value
			End Set
		End Property

		Public Overrides Property CSSIcon() As String
			Get
				Return Menu.MenuCSS.MenuIcon
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.MenuIcon = Value
			End Set
		End Property

		Public Overrides Property CSSLeftSeparator() As String
			Get
				Return m_strCSSLeftSeparator
			End Get
			Set(ByVal Value As String)
				m_strCSSLeftSeparator = Value
			End Set
		End Property

		Public Overrides Property CSSLeftSeparatorBreadCrumb() As String
			Get
				Return m_strCSSLeftSeparatorBreadCrumb
			End Get
			Set(ByVal Value As String)
				m_strCSSLeftSeparatorBreadCrumb = Value
			End Set
		End Property

		Public Overrides Property CSSLeftSeparatorSelection() As String
			Get
				Return m_strCSSLeftSeparatorSelection
			End Get
			Set(ByVal Value As String)
				m_strCSSLeftSeparatorSelection = Value
			End Set
		End Property

		Public Overrides Property CSSNode() As String
			Get
				Return Menu.MenuCSS.MenuItem
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.MenuItem = Value
			End Set
		End Property

		Public Overrides Property CSSNodeSelectedSub() As String
			Get
				Return m_strNodeSelectedSub
			End Get
			Set(ByVal Value As String)
				m_strNodeSelectedSub = Value
			End Set
		End Property

		Public Overrides Property CSSNodeSelectedRoot() As String
			Get
				Return m_strNodeSelectedRoot
			End Get
			Set(ByVal Value As String)
				m_strNodeSelectedRoot = Value
			End Set
		End Property

		Public Overrides Property CSSNodeHover() As String
			Get
				Return Menu.MenuCSS.MenuItemSel
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.MenuItemSel = Value
			End Set
		End Property

		Public Overrides Property CSSNodeRoot() As String
			Get
				Return m_strCSSNodeRoot
			End Get
			Set(ByVal Value As String)
				m_strCSSNodeRoot = Value
			End Set
		End Property

		Public Overrides Property CSSNodeHoverSub() As String
			Get
				Return m_strCSSNodeHoverSub
			End Get
			Set(ByVal Value As String)
				m_strCSSNodeHoverSub = Value
			End Set
		End Property

		Public Overrides Property CSSNodeHoverRoot() As String
			Get
				Return m_strCSSNodeHoverRoot
			End Get
			Set(ByVal Value As String)
				m_strCSSNodeHoverRoot = Value
			End Set
		End Property

		Public Overrides Property CSSRightSeparator() As String
			Get
				Return m_strCSSRightSeparator
			End Get
			Set(ByVal Value As String)
				m_strCSSRightSeparator = Value
			End Set
		End Property

		Public Overrides Property CSSRightSeparatorBreadCrumb() As String
			Get
				Return m_strCSSRightSeparatorBreadCrumb
			End Get
			Set(ByVal Value As String)
				m_strCSSRightSeparatorBreadCrumb = Value
			End Set
		End Property

		Public Overrides Property CSSRightSeparatorSelection() As String
			Get
				Return m_strCSSRightSeparatorSelection
			End Get
			Set(ByVal Value As String)
				m_strCSSRightSeparatorSelection = Value
			End Set
		End Property

		Public Overrides Property CSSSeparator() As String
			Get
				Return m_strCSSSeparator
			End Get
			Set(ByVal Value As String)
				m_strCSSSeparator = Value
			End Set
		End Property

		Public Overrides Property CSSContainerSub() As String
			Get
				Return Menu.MenuCSS.SubMenu
			End Get
			Set(ByVal Value As String)
				Menu.MenuCSS.SubMenu = Value
			End Set
		End Property

		Public Overrides Property ForceCrawlerDisplay() As String
			Get
				Return Menu.ForceFullMenuList.ToString
			End Get
			Set(ByVal Value As String)
				Menu.ForceFullMenuList = CBool(Value)
			End Set
		End Property

		Public Overrides Property ForceDownLevel() As String
			Get
				Return Menu.ForceDownlevel.ToString
			End Get
			Set(ByVal Value As String)
				Menu.ForceDownlevel = CBool(Value)
			End Set
		End Property

		Public Overrides Property PathImage() As String
			Get
				Return Menu.IconImagesPath
			End Get
			Set(ByVal Value As String)
				Menu.IconImagesPath = Value
			End Set
		End Property

		Public Overrides Property IndicateChildren() As Boolean
			Get
				Return m_blnIndicateChildren
			End Get
			Set(ByVal Value As Boolean)
				m_blnIndicateChildren = Value
			End Set
		End Property

		Public Overrides Property EffectsStyle() As String
			Get
				Return Menu.MenuEffects.Style.ToString
			End Get
			Set(ByVal Value As String)
				String.Concat(Menu.MenuEffects.Style.ToString, Value)
			End Set
		End Property

		Public Overrides Property EffectsDuration() As Double
			Get
				Return Menu.MenuEffects.MenuTransitionLength
			End Get
			Set(ByVal Value As Double)
				Menu.MenuEffects.MenuTransitionLength = Value
			End Set
		End Property

		Public Overrides Property EffectsShadowColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.ShadowColor)
			End Get
			Set(ByVal Value As String)
				Menu.ShadowColor = System.Drawing.Color.FromName(Value)
			End Set
		End Property

		Public Overrides Property EffectsShadowDirection() As String
			Get
				Return Menu.MenuEffects.ShadowDirection
			End Get
			Set(ByVal Value As String)
				Menu.MenuEffects.ShadowDirection = Value
			End Set
		End Property

		Public Overrides Property EffectsShadowStrength() As Integer
			Get
				Return Menu.MenuEffects.ShadowStrength
			End Get
			Set(ByVal Value As Integer)
				Menu.MenuEffects.ShadowStrength = Value
			End Set
		End Property

		Public Overrides Property EffectsTransition() As String
			Get
				Return Menu.MenuEffects.MenuTransition
			End Get
			Set(ByVal Value As String)
				Menu.MenuEffects.MenuTransition = Value
			End Set
		End Property

		Public Overrides Property MouseOutHideDelay() As Decimal
			Get
				Return Menu.MenuEffects.MouseOutHideDelay
			End Get
			Set(ByVal Value As Decimal)
				Menu.MenuEffects.MouseOutHideDelay = CInt(Value)
			End Set
		End Property

		Public Overrides Property MouseOverAction() As Modules.NavigationProvider.NavigationProvider.HoverAction
			Get
				If Menu.MenuEffects.MouseOverExpand Then
					Return Modules.NavigationProvider.NavigationProvider.HoverAction.Expand
				Else
					Return Modules.NavigationProvider.NavigationProvider.HoverAction.None
				End If
			End Get
			Set(ByVal Value As Modules.NavigationProvider.NavigationProvider.HoverAction)
				If Value = Modules.NavigationProvider.NavigationProvider.HoverAction.Expand Then
					Menu.MenuEffects.MouseOverExpand = True
				Else
					Menu.MenuEffects.MouseOverExpand = False
				End If
			End Set
		End Property

		Public Overrides Property MouseOverDisplay() As Modules.NavigationProvider.NavigationProvider.HoverDisplay
			Get
				Select Case Menu.MenuEffects.MouseOverDisplay
					Case MenuEffectsMouseOverDisplay.Highlight
						Return Modules.NavigationProvider.NavigationProvider.HoverDisplay.Highlight
					Case MenuEffectsMouseOverDisplay.Outset
						Return Modules.NavigationProvider.NavigationProvider.HoverDisplay.Outset
					Case MenuEffectsMouseOverDisplay.None
						Return Modules.NavigationProvider.NavigationProvider.HoverDisplay.None
				End Select
			End Get
			Set(ByVal Value As Modules.NavigationProvider.NavigationProvider.HoverDisplay)
				Select Case Value
					Case Modules.NavigationProvider.NavigationProvider.HoverDisplay.Highlight
						Menu.MenuEffects.MouseOverDisplay = MenuEffectsMouseOverDisplay.Highlight
					Case Modules.NavigationProvider.NavigationProvider.HoverDisplay.Outset
						Menu.MenuEffects.MouseOverDisplay = MenuEffectsMouseOverDisplay.Outset
					Case Modules.NavigationProvider.NavigationProvider.HoverDisplay.None
						Menu.MenuEffects.MouseOverDisplay = MenuEffectsMouseOverDisplay.None
				End Select
			End Set
		End Property

		Public Overrides Property NodeLeftHTMLSub() As String
			Get
				Return m_strNodeLeftHTMLSub
			End Get
			Set(ByVal Value As String)
				m_strNodeLeftHTMLSub = Value
			End Set
		End Property

		Public Overrides Property NodeLeftHTMLBreadCrumbSub() As String
			Get
				Return m_strNodeLeftHTMLBreadCrumbSub
			End Get
			Set(ByVal Value As String)
				m_strNodeLeftHTMLBreadCrumbSub = Value
			End Set
		End Property

		Public Overrides Property NodeLeftHTMLBreadCrumbRoot() As String
			Get
				Return m_strNodeLeftHTMLBreadCrumbRoot
			End Get
			Set(ByVal Value As String)
				m_strNodeLeftHTMLBreadCrumbRoot = Value
			End Set
		End Property

		Public Overrides Property NodeLeftHTMLRoot() As String
			Get
				Return m_strNodeLeftHTMLRoot
			End Get
			Set(ByVal Value As String)
				m_strNodeLeftHTMLRoot = Value
			End Set
		End Property

		Public Overrides Property NodeRightHTMLSub() As String
			Get
				Return m_strNodeRightHTMLSub
			End Get
			Set(ByVal Value As String)
				m_strNodeRightHTMLSub = Value
			End Set
		End Property

		Public Overrides Property NodeRightHTMLBreadCrumbSub() As String
			Get
				Return m_strNodeRightHTMLBreadCrumbSub
			End Get
			Set(ByVal Value As String)
				m_strNodeRightHTMLBreadCrumbSub = Value
			End Set
		End Property

		Public Overrides Property NodeRightHTMLBreadCrumbRoot() As String
			Get
				Return m_strNodeRightHTMLBreadCrumbRoot
			End Get
			Set(ByVal Value As String)
				m_strNodeRightHTMLBreadCrumbRoot = Value
			End Set
		End Property

		Public Overrides Property NodeRightHTMLRoot() As String
			Get
				Return m_strNodeRightHTMLRoot
			End Get
			Set(ByVal Value As String)
				m_strNodeRightHTMLRoot = Value
			End Set
		End Property

		Public Overrides Property SeparatorHTML() As String
			Get
				Return m_strSeparatorHTML
			End Get
			Set(ByVal Value As String)
				m_strSeparatorHTML = Value
			End Set
		End Property

		Public Overrides Property SeparatorLeftHTML() As String
			Get
				Return m_strSeparatorLeftHTML
			End Get
			Set(ByVal Value As String)
				m_strSeparatorLeftHTML = Value
			End Set
		End Property

		Public Overrides Property SeparatorLeftHTMLActive() As String
			Get
				Return m_strSeparatorLeftHTMLActive
			End Get
			Set(ByVal Value As String)
				m_strSeparatorLeftHTMLActive = Value
			End Set
		End Property

		Public Overrides Property SeparatorLeftHTMLBreadCrumb() As String
			Get
				Return m_strSeparatorLeftHTMLBreadCrumb
			End Get
			Set(ByVal Value As String)
				m_strSeparatorLeftHTMLBreadCrumb = Value
			End Set
		End Property

		Public Overrides Property SeparatorRightHTML() As String
			Get
				Return m_strSeparatorRightHTML
			End Get
			Set(ByVal Value As String)
				m_strSeparatorRightHTML = Value
			End Set
		End Property

		Public Overrides Property SeparatorRightHTMLActive() As String
			Get
				Return m_strSeparatorRightHTMLActive
			End Get
			Set(ByVal Value As String)
				m_strSeparatorRightHTMLActive = Value
			End Set
		End Property

		Public Overrides Property SeparatorRightHTMLBreadCrumb() As String
			Get
				Return m_strSeparatorRightHTMLBreadCrumb
			End Get
			Set(ByVal Value As String)
				m_strSeparatorRightHTMLBreadCrumb = Value
			End Set
		End Property

		Public Overrides Property StyleBackColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.BackColor)
			End Get
			Set(ByVal Value As String)
				Menu.BackColor = System.Drawing.Color.FromName(Value)
			End Set
		End Property

		Public Overrides Property StyleBorderWidth() As Decimal
			Get
				Return Menu.MenuBorderWidth
			End Get
			Set(ByVal Value As Decimal)
				Menu.MenuBorderWidth = CInt(Value)
			End Set
		End Property

		Public Overrides Property StyleControlHeight() As Decimal
			Get
				Return Menu.MenuBarHeight
			End Get
			Set(ByVal Value As Decimal)
				Menu.MenuBarHeight = CInt(Value)
			End Set
		End Property

		Public Overrides Property StyleFontBold() As String
			Get
				Return Menu.Font.Bold.ToString
			End Get
			Set(ByVal Value As String)
				Menu.Font.Bold = CBool(Value)
			End Set
		End Property

		Public Overrides Property StyleFontNames() As String
			Get
				Return Join(Menu.Font.Names, ";")
			End Get
			Set(ByVal Value As String)
				Menu.Font.Names = Value.Split(CChar(";"))
			End Set
		End Property

		Public Overrides Property StyleFontSize() As Decimal
			Get
				Return CDec(Menu.Font.Size.Unit.Value)
			End Get
			Set(ByVal Value As Decimal)
				Menu.Font.Size = Web.UI.WebControls.FontUnit.Parse(Value.ToString)
			End Set
		End Property

		Public Overrides Property StyleForeColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.ForeColor)
			End Get
			Set(ByVal Value As String)
				Menu.ForeColor = System.Drawing.Color.FromName(Value)
			End Set
		End Property

		Public Overrides Property StyleHighlightColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.HighlightColor)
			End Get
			Set(ByVal Value As String)
				Menu.HighlightColor = System.Drawing.Color.FromName(Value)
			End Set
		End Property

		Public Overrides Property StyleIconBackColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.IconBackgroundColor)
			End Get
			Set(ByVal Value As String)
				Menu.IconBackgroundColor = System.Drawing.Color.FromName(Value)
			End Set
		End Property

		Public Overrides Property StyleIconWidth() As Decimal
			Get
				Return Menu.IconWidth
			End Get
			Set(ByVal Value As Decimal)
				Menu.IconWidth = CInt(Value)
			End Set
		End Property

		Public Overrides Property StyleNodeHeight() As Decimal
			Get
				Return Menu.MenuItemHeight
			End Get
			Set(ByVal Value As Decimal)
				Menu.MenuItemHeight = CInt(Value)
			End Set
		End Property

		Public Overrides Property StyleSelectionBorderColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.SelectedBorderColor)
			End Get
			Set(ByVal Value As String)
				If Not Value Is Nothing Then
					Menu.SelectedBorderColor = System.Drawing.Color.FromName(Value)
				Else
					Menu.SelectedBorderColor = System.Drawing.Color.Empty
				End If
			End Set
		End Property

		Public Overrides Property StyleSelectionColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.SelectedColor)
			End Get
			Set(ByVal Value As String)
				Menu.SelectedColor = System.Drawing.Color.FromName(Value)
			End Set
		End Property

		Public Overrides Property StyleSelectionForeColor() As String
			Get
				Return System.Drawing.ColorTranslator.ToHtml(Menu.SelectedForeColor)
			End Get
			Set(ByVal Value As String)
				Menu.SelectedForeColor = System.Drawing.Color.FromName(Value)
			End Set
		End Property

		Public Overrides Property StyleRoot() As String
			Get
				Return m_strStyleRoot
			End Get
			Set(ByVal Value As String)
				m_strStyleRoot = Value
			End Set
		End Property

		Public Overrides Property PathSystemImage() As String
			Get
				Return Menu.SystemImagesPath
			End Get
			Set(ByVal Value As String)
				Menu.SystemImagesPath = Value
			End Set
		End Property

		Public Overrides Property PathSystemScript() As String
			Get
				Return Menu.SystemScriptPath
			End Get
			Set(ByVal Value As String)
				Menu.SystemScriptPath = Value
			End Set
		End Property

		Public Overrides Sub Initialize()
			m_objMenu = New SolpartMenu
			Menu.ID = m_strControlID
			Menu.SeparateCSS = True
			StyleSelectionBorderColor = Nothing		  'used to be done in skin object...

			AddHandler m_objMenu.MenuClick, AddressOf ctlActions_MenuClick

		End Sub

		Private Sub ctlActions_MenuClick(ByVal ID As String)
			RaiseEvent_NodeClick(ID)
		End Sub

		Public Overrides Sub Bind(ByVal objNodes As DotNetNuke.UI.WebControls.DNNNodeCollection)
			Dim objNode As DotNetNuke.UI.WebControls.DNNNode = Nothing
			Dim objMenuItem As Solpart.WebControls.SPMenuItemNode = Nothing
			Dim objPrevNode As DotNetNuke.UI.WebControls.DNNNode = Nothing
			Dim RootFlag As Boolean
			If IndicateChildren = False Then
				'should this be spacer.gif???
				'IndicateChildImageSub = ""
				'IndicateChildImageRoot = ""
			Else
				If Len(Me.IndicateChildImageRoot) > 0 Then Menu.RootArrow = True
			End If

			For Each objNode In objNodes
				Try
					If objNode.Level = 0 Then					  ' root menu
						If RootFlag = True Then						 'first root item has already been entered
							AddSeparator("All", objPrevNode, objNode)
						Else
							If SeparatorLeftHTML <> "" OrElse SeparatorLeftHTMLBreadCrumb <> "" OrElse Me.SeparatorLeftHTMLActive <> "" Then
								AddSeparator("Left", objPrevNode, objNode)
							End If
							RootFlag = True
						End If

						If objNode.Enabled = False Then
							objMenuItem = New Solpart.WebControls.SPMenuItemNode(Menu.AddMenuItem(objNode.ID.ToString, objNode.Text, ""))
						Else
							If Len(objNode.JSFunction) > 0 Then
								objMenuItem = New Solpart.WebControls.SPMenuItemNode(Menu.AddMenuItem(objNode.ID.ToString, objNode.Text, GetClientScriptURL(objNode.JSFunction, objNode.ID)))
							Else
								objMenuItem = New Solpart.WebControls.SPMenuItemNode(Menu.AddMenuItem(objNode.ID.ToString, objNode.Text, objNode.NavigateURL))
							End If
						End If
						If Len(Me.StyleRoot) > 0 Then
							objMenuItem.ItemStyle = Me.StyleRoot
						End If
						If Me.CSSNodeRoot <> "" Then
							objMenuItem.ItemCss = Me.CSSNodeRoot
						End If
						If Me.CSSNodeHoverRoot <> "" Then
							objMenuItem.ItemSelectedCss = Me.CSSNodeHoverRoot
						End If

						If Me.NodeLeftHTMLRoot <> "" Then
							objMenuItem.LeftHTML = Me.NodeLeftHTMLRoot
						End If

						If objNode.BreadCrumb Then
							objMenuItem.ItemCss = objMenuItem.ItemCss & " " & Me.CSSBreadCrumbRoot
							If NodeLeftHTMLBreadCrumbRoot <> "" Then objMenuItem.LeftHTML = NodeLeftHTMLBreadCrumbRoot
							If NodeRightHTMLBreadCrumbRoot <> "" Then objMenuItem.RightHTML = NodeRightHTMLBreadCrumbRoot
							If objNode.Selected Then
								objMenuItem.ItemCss = objMenuItem.ItemCss & " " & Me.CSSNodeSelectedRoot
							End If
						End If

						If Me.NodeRightHTMLRoot <> "" Then
							objMenuItem.RightHTML = NodeRightHTMLRoot
						End If

					ElseIf objNode.IsBreak Then
						Menu.AddBreak(objNode.ParentNode.ID.ToString)
					Else					 'If Not blnRootOnly Then
						Try
							If objNode.Enabled = False Then
								objMenuItem = New Solpart.WebControls.SPMenuItemNode(Menu.AddMenuItem(objNode.ParentNode.ID.ToString, objNode.ID.ToString, "&nbsp;" & objNode.Text, ""))
							Else
								If Len(objNode.JSFunction) > 0 Then
									objMenuItem = New Solpart.WebControls.SPMenuItemNode(Menu.AddMenuItem(objNode.ParentNode.ID.ToString, objNode.ID.ToString, "&nbsp;" & objNode.Text, GetClientScriptURL(objNode.JSFunction, objNode.ID)))
								Else
									objMenuItem = New Solpart.WebControls.SPMenuItemNode(Menu.AddMenuItem(objNode.ParentNode.ID.ToString, objNode.ID.ToString, "&nbsp;" & objNode.Text, objNode.NavigateURL))
								End If
							End If

							If objNode.ClickAction = UI.WebControls.eClickAction.PostBack Then
								objMenuItem.RunAtServer = True
							End If
							If CSSNodeHoverSub <> "" Then
								objMenuItem.ItemSelectedCss = CSSNodeHoverSub
							End If
							If NodeLeftHTMLSub <> "" Then
								objMenuItem.LeftHTML = NodeLeftHTMLSub
							End If

							If objNode.BreadCrumb Then
								objMenuItem.ItemCss = Me.CSSBreadCrumbSub
								If NodeLeftHTMLBreadCrumbSub <> "" Then objMenuItem.LeftHTML = NodeLeftHTMLBreadCrumbSub
								If NodeRightHTMLBreadCrumbSub <> "" Then objMenuItem.RightHTML = NodeRightHTMLBreadCrumbSub
								If objNode.Selected Then
                                    objMenuItem.ItemCss = Me.CSSNodeSelectedSub

                                    Dim objParentNode As DotNetNuke.UI.WebControls.DNNNode = objNode
                                    Do
                                        objParentNode = objParentNode.ParentNode
                                        Menu.FindMenuItem(objParentNode.ID.ToString()).ItemCss = Me.CSSNodeSelectedSub
                                    Loop Until objParentNode.Level = 0
                                    Menu.FindMenuItem(objParentNode.ID.ToString()).ItemCss = Me.CSSBreadCrumbRoot & " " & Me.CSSNodeSelectedRoot
								End If
							End If

							If Me.NodeRightHTMLSub <> "" Then
								objMenuItem.RightHTML = Me.NodeRightHTMLSub
							End If
						Catch
							' throws exception if the parent tab has not been loaded ( may be related to user role security not allowing access to a parent tab )
							objMenuItem = Nothing
						End Try
                    End If

					If Len(objNode.Image) > 0 Then
                        If objNode.Image.StartsWith("~/images/") Then
                            objMenuItem.Image = objNode.Image.Replace("~/images/", "")
                        ElseIf objNode.Image.IndexOf("/") > -1 Then                      'if image contains a path
                            Dim strImage As String = objNode.Image
                            If strImage.StartsWith(Menu.IconImagesPath) Then                            'if path (or portion) is already set in header of menu truncate it off
                                strImage = strImage.Substring(Menu.IconImagesPath.Length)
                            End If
                            If strImage.IndexOf("/") > -1 Then                          'if the image still contains path info assign it
                                objMenuItem.Image = strImage.Substring(strImage.LastIndexOf("/") + 1)
                                If strImage.StartsWith("/") Then                                  'is absolute path?
                                    objMenuItem.ImagePath = strImage.Substring(0, strImage.LastIndexOf("/") + 1)
                                ElseIf strImage.StartsWith("~/") Then
                                    objMenuItem.ImagePath = Globals.ResolveUrl(strImage.Substring(0, strImage.LastIndexOf("/") + 1))
                                Else
                                    objMenuItem.ImagePath = Menu.IconImagesPath & strImage.Substring(0, strImage.LastIndexOf("/") + 1)
                                End If
                            Else
                                objMenuItem.Image = strImage
                            End If
                        Else
                            objMenuItem.Image = objNode.Image
                        End If
					End If
					If Len(objNode.ToolTip) > 0 Then objMenuItem.ToolTip = objNode.ToolTip

					Bind(objNode.DNNNodes)

				Catch ex As Exception
					Throw ex
				End Try
				objPrevNode = objNode
			Next

			If Not objNode Is Nothing AndAlso objNode.Level = 0 Then			 ' root menu
				If SeparatorRightHTML <> "" OrElse SeparatorRightHTMLBreadCrumb <> "" OrElse Me.SeparatorRightHTMLActive <> "" Then
					AddSeparator("Right", objPrevNode, Nothing)
				End If
			End If
		End Sub

		Private Sub AddSeparator(ByVal strType As String, ByVal objPrevNode As DotNetNuke.UI.WebControls.DNNNode, ByVal objNextNode As DotNetNuke.UI.WebControls.DNNNode)
			Dim strLeftHTML As String = SeparatorLeftHTML & SeparatorLeftHTMLBreadCrumb & SeparatorLeftHTMLActive
			Dim strRightHTML As String = SeparatorRightHTML & SeparatorRightHTMLBreadCrumb & SeparatorRightHTMLActive
			Dim strHTML As String = Me.SeparatorHTML & strLeftHTML & strRightHTML
			Dim objBreak As System.Xml.XmlNode
			If Len(strHTML) > 0 Then
				Dim strSeparatorTable As String = ""
				Dim strSeparator As String = ""
				Dim strSeparatorLeftHTML As String = ""
				Dim strSeparatorRightHTML As String = ""
				Dim strSeparatorClass As String = ""
				Dim strLeftSeparatorClass As String = ""
				Dim strRightSeparatorClass As String = ""

				If Len(strLeftHTML) > 0 Then
					strLeftSeparatorClass = Me.GetSeparatorText(CSSLeftSeparator, CSSLeftSeparatorBreadCrumb, CSSLeftSeparatorSelection, objNextNode)
					strSeparatorLeftHTML = Me.GetSeparatorText(SeparatorLeftHTML, SeparatorLeftHTMLBreadCrumb, SeparatorLeftHTMLActive, objNextNode)
				End If
				If SeparatorHTML <> "" Then
					If CSSSeparator <> "" Then
						strSeparatorClass = CSSSeparator
					End If
					strSeparator = SeparatorHTML
				End If
				If Len(strRightHTML) > 0 Then
					strRightSeparatorClass = Me.GetSeparatorText(CSSRightSeparator, CSSRightSeparatorBreadCrumb, CSSRightSeparatorSelection, objPrevNode)
					strSeparatorRightHTML = Me.GetSeparatorText(SeparatorRightHTML, SeparatorRightHTMLBreadCrumb, SeparatorRightHTMLActive, objPrevNode)
				End If
				strSeparatorTable = "<table summary=""Table for menu separator design"" border=""0"" cellpadding=""0"" cellspacing=""0""><tr>"

				If strSeparatorRightHTML <> "" AndAlso strType <> "Left" Then
					strSeparatorTable &= GetSeparatorTD(strRightSeparatorClass, strSeparatorRightHTML)
				End If
				If strSeparator <> "" AndAlso strType <> "Left" AndAlso strType <> "Right" Then
					strSeparatorTable &= GetSeparatorTD(strSeparatorClass, strSeparator)
				End If
				If strSeparatorLeftHTML <> "" AndAlso strType <> "Right" Then
					strSeparatorTable &= GetSeparatorTD(strLeftSeparatorClass, strSeparatorLeftHTML)
				End If
				strSeparatorTable &= "</tr></table>"
				objBreak = Menu.AddBreak("", strSeparatorTable)
			End If
		End Sub

		Private Function GetSeparatorText(ByVal strNormal As String, ByVal strBreadCrumb As String, ByVal strSelection As String, ByVal objNode As DotNetNuke.UI.WebControls.DNNNode) As String
			Dim strRet As String = ""
			If strNormal <> "" Then
				strRet = strNormal
			End If
			If strBreadCrumb <> "" AndAlso Not objNode Is Nothing AndAlso objNode.BreadCrumb Then
				strRet = strBreadCrumb
			End If
			If strSelection <> "" AndAlso Not objNode Is Nothing AndAlso objNode.Selected Then
				strRet = strSelection
			End If
			Return strRet
		End Function

		Private Function GetSeparatorTD(ByVal strClass As String, ByVal strHTML As String) As String
			Dim strRet As String = ""
			If strClass <> "" Then
				strRet += "<td class = """ & strClass & """>"
			Else
				strRet += "<td>"
			End If
			strRet += strHTML & "</td>"

			Return strRet
		End Function

		Private Function GetClientScriptURL(ByVal strScript As String, ByVal strID As String) As String
			If strScript.ToLower.StartsWith("javascript:") = False Then
				strScript = "javascript: " & strScript
			End If
			Return strScript
		End Function

	End Class

End Namespace
