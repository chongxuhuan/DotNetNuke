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

Namespace DotNetNuke.Modules.NavigationProvider

	Public MustInherit Class NavigationProvider
		Inherits Framework.UserControlBase

		Public Event NodeClick(ByVal args As NavigationEventArgs)
		Public Event PopulateOnDemand(ByVal args As NavigationEventArgs)

		Public Enum Orientation
			Horizontal
			Vertical
		End Enum
		Public Enum Alignment
			Left
			Right
			Center
			Justify
		End Enum
		Public Enum HoverAction
			Expand
			None
		End Enum
		Public Enum HoverDisplay
			Highlight
			Outset
			None
		End Enum

#Region "Shared/Static Methods"
		Public Shared Shadows Function Instance(ByVal FriendlyName As String) As NavigationProvider
            Return CType(Framework.Reflection.CreateObject("navigationControl", FriendlyName, "", ""), NavigationProvider)
            'Return ComponentModel.ComponentFactory.GetComponent(Of NavigationProvider)(FriendlyName)
		End Function
#End Region

#Region "Properties"

		' Properties
		Public MustOverride ReadOnly Property NavigationControl() As System.Web.UI.Control
		Public MustOverride Property ControlID() As String
		Public MustOverride ReadOnly Property SupportsPopulateOnDemand() As Boolean

		Public Overridable Property PathImage() As String
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property PathSystemImage() As String
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property PathSystemScript() As String
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property WorkImage() As String
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property IndicateChildImageSub() As String
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property IndicateChildImageRoot() As String
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property IndicateChildImageExpandedSub() As String		  'for tree
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property IndicateChildImageExpandedRoot() As String		  'for tree
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		'NodeLeftHTMLBreadCrumbSub
		'Public Overridable Property IndicateChildImageBreadCrumbSub() As String		  'SubMenuBreadCrumbArrow
		'	Get
		'		Return ""
		'	End Get
		'	Set(ByVal Value As String)

		'	End Set
		'End Property
		'NodeLeftHTMLBreadCrumbRoot
		'Public Overridable Property IndicateChildImageBreadCrumbRoot() As String		  'RootBreadCrumbArrow
		'	Get
		'		Return ""
		'	End Get
		'	Set(ByVal Value As String)

		'	End Set
		'End Property

		Public MustOverride Property CSSControl() As String		  'MenuBarCSSClass
		Public Overridable Property CSSContainerRoot() As String		  'MenuContainerCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSContainerSub() As String		  'SubMenuCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property CSSNode() As String		'MenuItemCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSNodeRoot() As String		'RootMenuItemCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSIcon() As String		  'MenuIconCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSNodeHover() As String		  'MenuItemSelCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSNodeHoverSub() As String		  'SubMenuItemSelectedCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSNodeHoverRoot() As String		  'RootMenuItemSelectedCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSBreak() As String		  'MenuBreakCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSIndicateChildSub() As String		  'MenuArrowCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSIndicateChildRoot() As String		  'MenuRootArrowCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSBreadCrumbSub() As String		  'SubMenuItemBreadCrumbCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSBreadCrumbRoot() As String		  'RootMenuItemBreadCrumbCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSNodeSelectedSub() As String		  'SubMenuItemActiveCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSNodeSelectedRoot() As String		  'RootMenuItemActiveCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property CSSSeparator() As String		  'SeparatorCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSLeftSeparator() As String		  'LeftSeparatorCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSRightSeparator() As String		  'RightSeparatorCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property CSSLeftSeparatorSelection() As String		  'LeftSeparatorActiveCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSRightSeparatorSelection() As String		  'RightSeparatorActiveCssClass
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSLeftSeparatorBreadCrumb() As String		  'LeftSeparatorCssClassBreadCrumb
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property CSSRightSeparatorBreadCrumb() As String		  'RightSeparatorCssClassBreadCrumb
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property StyleBackColor() As String		  'BackColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleForeColor() As String		  'ForeColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleHighlightColor() As String		  'HighlightColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleIconBackColor() As String		  'IconBackgroundColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleSelectionBorderColor() As String		  'SelectedBorderColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleSelectionColor() As String		  'SelectedColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleSelectionForeColor() As String		  'SelectedForeColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleControlHeight() As Decimal		  'MenuBarHeight
			Get
				Return 25
			End Get
			Set(ByVal Value As Decimal)

			End Set
		End Property
		Public Overridable Property StyleBorderWidth() As Decimal		  'MenuBorderWidth
			Get
				Return 0
			End Get
			Set(ByVal Value As Decimal)

			End Set
		End Property
		Public Overridable Property StyleNodeHeight() As Decimal		  'MenuItemHeight
			Get
				Return 25
			End Get
			Set(ByVal Value As Decimal)

			End Set
		End Property
		Public Overridable Property StyleIconWidth() As Decimal		  'IconWidth
			Get
				Return 0
			End Get
			Set(ByVal Value As Decimal)

			End Set
		End Property
		Public Overridable Property StyleFontNames() As String		  'FontNames
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property StyleFontSize() As Decimal		  'FontSize
			Get
				Return 0
			End Get
			Set(ByVal Value As Decimal)

			End Set
		End Property
		Public Overridable Property StyleFontBold() As String		  'FontBold
			Get
				Return "False"
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property StyleRoot() As String		  'For action menu backwards compatibility
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property StyleSub() As String		  'For action menu backwards compatibility (actually this is new, but since we needed the root...)
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property EffectsStyle() As String		  'MenuEffectsStyle
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property EffectsTransition() As String		  'MenuEffectsTransition
			Get
				Return "'"
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property EffectsDuration() As Double		   'MenuEffectsMenuTransitionLength
			Get
				Return -1
			End Get
			Set(ByVal Value As Double)

			End Set
		End Property
		Public Overridable Property EffectsShadowColor() As String		  'MenuEffectsShadowColor
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property EffectsShadowDirection() As String		  'MenuEffectsShadowDirection
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property EffectsShadowStrength() As Integer		  'MenuEffectsShadowStrength
			Get
				Return -1
			End Get
			Set(ByVal Value As Integer)

			End Set
		End Property

		Public Overridable Property ControlOrientation() As Orientation		  'Display
			Get
				Return Orientation.Horizontal
			End Get
			Set(ByVal Value As Orientation)

			End Set
		End Property
		Public Overridable Property ControlAlignment() As Alignment		  'MenuAlignment
			Get
				Return Alignment.Left
			End Get
			Set(ByVal Value As Alignment)

			End Set
		End Property

		Public Overridable Property ForceDownLevel() As String		  'ForceDownLevel
			Get
				Return False.ToString
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property MouseOutHideDelay() As Decimal		  'MenuEffectsMouseOutHideDelay, MouseOutHideDelay
			Get
				Return -1
			End Get
			Set(ByVal Value As Decimal)

			End Set
		End Property
		Public Overridable Property MouseOverDisplay() As HoverDisplay		  'MenuEffectsMouseOverDisplay
			Get
				Return HoverDisplay.Highlight
			End Get
			Set(ByVal Value As HoverDisplay)

			End Set
		End Property
		Public Overridable Property MouseOverAction() As HoverAction		  'MenuEffectsMouseOverExpand
			Get
				Return HoverAction.Expand
			End Get
			Set(ByVal Value As HoverAction)

			End Set
		End Property
		Public Overridable Property ForceCrawlerDisplay() As String		  'ForceFullMenuList
			Get
				Return "False"
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property IndicateChildren() As Boolean		  'UseIndicateChilds
			Get
				Return True
			End Get
			Set(ByVal Value As Boolean)

			End Set
		End Property
		'Public MustOverride Property Moveable() As String		  'Moveable

		Public Overridable Property SeparatorHTML() As String		  'Separator
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property SeparatorLeftHTML() As String		  'LeftSeparator
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property SeparatorRightHTML() As String		  'RightSeparator
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property SeparatorLeftHTMLActive() As String		  'LeftSeparatorActive
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property SeparatorRightHTMLActive() As String		  'RightSeparatorActive
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property SeparatorLeftHTMLBreadCrumb() As String		  'LeftSeparatorBreadCrumb
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property SeparatorRightHTMLBreadCrumb() As String		  'RightSeparatorBreadCrumb
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property NodeLeftHTMLSub() As String		  'SubMenuItemLeftHtml
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property NodeLeftHTMLRoot() As String		  'RootMenuItemLeftHtml
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property NodeRightHTMLSub() As String		  'SubMenuItemRightHtml
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property NodeRightHTMLRoot() As String		  'RootMenuItemRightHtml
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property NodeLeftHTMLBreadCrumbSub() As String		  'New
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property NodeRightHTMLBreadCrumbSub() As String		  'New
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property NodeLeftHTMLBreadCrumbRoot() As String		  'New
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property
		Public Overridable Property NodeRightHTMLBreadCrumbRoot() As String		  'New
			Get
				Return ""
			End Get
			Set(ByVal Value As String)

			End Set
		End Property

		Public Overridable Property PopulateNodesFromClient() As Boolean
			Get
				Return False
			End Get
			Set(ByVal Value As Boolean)

			End Set
		End Property

    'JH - 2/5/07 - support for custom attributes
    Public Overridable Property CustomAttributes() As Generic.List(Of DotNetNuke.UI.Skins.CustomAttribute)
      Get
        Return Nothing
      End Get
      Set(ByVal value As Generic.List(Of DotNetNuke.UI.Skins.CustomAttribute))

      End Set
    End Property


		'UseSkinPathArrowImages		'skin object will toggle ImageDirectory based on value
		'UseRootBreadCrumbArrow		'skin object will assign NodeLeftHTMLBreadCrumbRoot
		'UseSubMenuBreadCrumbArrow	'skin object will assign NodeLeftHTMLBreadCrumb
		'DownArrow					'skin object will assign IndicateChildImage/IndicateChildImageRoot
		'RightArrow					'skin object will assign IndicateChildImage/IndicateChildImageRoot
		'Tooltip					'skin object decides whether to populate tooltips...  maybe need to fix in navigation.vb class...
		'ClearDefaults				'skin object decides if defaults should be populated
		'DelaySubmenuLoad			'should no longer be necessary (was for Operation Aborted error)
#End Region
#Region "Methods"

		' Methods
		Public MustOverride Sub Initialize()
		Public MustOverride Sub Bind(ByVal objNodes As DotNetNuke.UI.WebControls.DNNNodeCollection)
		Public Overridable Sub ClearNodes()

		End Sub
#End Region
#Region "Events"
		Protected Sub RaiseEvent_NodeClick(ByVal objNode As DotNetNuke.UI.WebControls.DNNNode)
			RaiseEvent NodeClick(New NavigationEventArgs(objNode.ID, objNode))
		End Sub

		Protected Sub RaiseEvent_NodeClick(ByVal strID As String)
			RaiseEvent NodeClick(New NavigationEventArgs(strID, Nothing))			 'DotNetNuke.UI.Navigation.GetNavigationNode(strID, Me.ControlID))
		End Sub

		Protected Sub RaiseEvent_PopulateOnDemand(ByVal objNode As DotNetNuke.UI.WebControls.DNNNode)
			RaiseEvent PopulateOnDemand(New NavigationEventArgs(objNode.ID, objNode))
		End Sub

		Protected Sub RaiseEvent_PopulateOnDemand(ByVal strID As String)
			'RaiseEvent_PopulateOnDemand(DotNetNuke.UI.Navigation.GetNavigationNode(strID, Me.ControlID))
			RaiseEvent PopulateOnDemand(New NavigationEventArgs(strID, Nothing))
		End Sub

#End Region

	End Class

	Public Class NavigationEventArgs
		Public ID As String
		Public Node As DotNetNuke.UI.WebControls.DNNNode

		Public Sub New(ByVal strID As String, ByVal objNode As DotNetNuke.UI.WebControls.DNNNode)
			Me.ID = strID
			Me.Node = objNode
		End Sub

	End Class

End Namespace