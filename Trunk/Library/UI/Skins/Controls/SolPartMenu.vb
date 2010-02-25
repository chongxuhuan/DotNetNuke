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
Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.UI.Skins.Controls
	''' -----------------------------------------------------------------------------
	''' <summary></summary>
    ''' <remarks></remarks>
	''' <history>
	''' 	[cniknet]	10/15/2004	Replaced public members with properties and removed
	'''                             brackets from property names
	''' </history>
	''' -----------------------------------------------------------------------------
    Public Class SolPartMenu

        Inherits UI.Skins.NavObjectBase

#Region "Private Members"
        Private _separateCss As String
        Private _useSkinPathArrowImages As String
        Private _useRootBreadCrumbArrow As String
        Private _useSubMenuBreadCrumbArrow As String
        Private _downArrow As String
        Private _rightArrow As String
        Private _level As String
        Private _rootOnly As String
        Private _toolTip As String
        Private _clearDefaults As String

        Private m_strRootBreadCrumbArrow As String
        Private m_strSubMenuBreadCrumbArrow As String
        Private m_strRootOnly As String

#End Region

        ' protected controls
#Region "Public Members"

        Public Property SeparateCss() As String
            Get
                Return _separateCss
            End Get
            Set(ByVal Value As String)
                _separateCss = Value
            End Set
        End Property

        Public Property MenuBarCssClass() As String
            Get
                Return Me.CSSControl
            End Get
            Set(ByVal Value As String)
                Me.CSSControl = Value
            End Set
        End Property

        Public Property MenuContainerCssClass() As String
            Get
                Return Me.CSSContainerRoot
            End Get
            Set(ByVal Value As String)
                Me.CSSContainerRoot = Value
            End Set
        End Property

        Public Property MenuItemCssClass() As String
            Get
                Return Me.CSSNode
            End Get
            Set(ByVal Value As String)
                Me.CSSNode = Value
            End Set
        End Property

        Public Property MenuIconCssClass() As String
            Get
                Return Me.CSSIcon
            End Get
            Set(ByVal Value As String)
                Me.CSSIcon = Value
            End Set
        End Property

        Public Property SubMenuCssClass() As String
            Get
                Return Me.CSSContainerSub
            End Get
            Set(ByVal Value As String)
                Me.CSSContainerSub = Value
            End Set
        End Property

        Public Property MenuItemSelCssClass() As String
            Get
                Return Me.CSSNodeHover
            End Get
            Set(ByVal Value As String)
                Me.CSSNodeHover = Value
            End Set
        End Property

        Public Property MenuBreakCssClass() As String
            Get
                Return Me.CSSBreak
            End Get
            Set(ByVal Value As String)
                Me.CSSBreak = Value
            End Set
        End Property

        Public Property MenuArrowCssClass() As String
            Get
                Return Me.CSSIndicateChildSub
            End Get
            Set(ByVal Value As String)
                Me.CSSIndicateChildSub = Value
            End Set
        End Property

        Public Property MenuRootArrowCssClass() As String
            Get
                Return Me.CSSIndicateChildRoot
            End Get
            Set(ByVal Value As String)
                Me.CSSIndicateChildRoot = Value
            End Set
        End Property

        Public Property BackColor() As String
            Get
                Return Me.StyleBackColor
            End Get
            Set(ByVal Value As String)
                Me.StyleBackColor = Value
            End Set
        End Property

        Public Property ForeColor() As String
            Get
                Return Me.StyleForeColor
            End Get
            Set(ByVal Value As String)
                Me.StyleForeColor = Value
            End Set
        End Property

        Public Property HighlightColor() As String
            Get
                Return Me.StyleHighlightColor
            End Get
            Set(ByVal Value As String)
                Me.StyleHighlightColor = Value
            End Set
        End Property

        Public Property IconBackgroundColor() As String
            Get
                Return Me.StyleIconBackColor
            End Get
            Set(ByVal Value As String)
                Me.StyleIconBackColor = Value
            End Set
        End Property

        Public Property SelectedBorderColor() As String
            Get
                Return Me.StyleSelectionBorderColor
            End Get
            Set(ByVal Value As String)
                Me.StyleSelectionBorderColor = Value
            End Set
        End Property

        Public Property SelectedColor() As String
            Get
                Return Me.StyleSelectionColor
            End Get
            Set(ByVal Value As String)
                Me.StyleSelectionColor = Value
            End Set
        End Property

        Public Property SelectedForeColor() As String
            Get
                Return Me.StyleSelectionForeColor
            End Get
            Set(ByVal Value As String)
                Me.StyleSelectionForeColor = Value
            End Set
        End Property

        Public Property Display() As String
            Get
                Return Me.ControlOrientation
            End Get
            Set(ByVal Value As String)
                Me.ControlOrientation = Value
            End Set
        End Property

        Public Property MenuBarHeight() As String
            Get
                Return Me.StyleControlHeight
            End Get
            Set(ByVal Value As String)
                Me.StyleControlHeight = Value
            End Set
        End Property

        Public Property MenuBorderWidth() As String
            Get
                Return Me.StyleBorderWidth
            End Get
            Set(ByVal Value As String)
                Me.StyleBorderWidth = Value
            End Set
        End Property

        Public Property MenuItemHeight() As String
            Get
                Return Me.StyleNodeHeight
            End Get
            Set(ByVal Value As String)
                Me.StyleNodeHeight = Value
            End Set
        End Property

        Public Property Moveable() As String
            Get
                Return String.Empty
            End Get
            Set(ByVal Value As String)
            End Set
        End Property

        Public Property IconWidth() As String
            Get
                Return Me.StyleIconWidth
            End Get
            Set(ByVal Value As String)
                Me.StyleIconWidth = Value
            End Set
        End Property

        Public Property MenuEffectsShadowColor() As String
            Get
                Return Me.EffectsShadowColor
            End Get
            Set(ByVal Value As String)
                Me.EffectsShadowColor = Value
            End Set
        End Property

        Public Property MenuEffectsMouseOutHideDelay() As String
            Get
                Return Me.MouseOutHideDelay
            End Get
            Set(ByVal Value As String)
                Me.MouseOutHideDelay = Value
            End Set
        End Property

        Public Property MenuEffectsMouseOverDisplay() As String
            Get
                Return Me.MouseOverDisplay
            End Get
            Set(ByVal Value As String)
                Me.MouseOverDisplay = Value
            End Set
        End Property

        Public Property MenuEffectsMouseOverExpand() As String
            Get
                Return Me.MouseOverAction
            End Get
            Set(ByVal Value As String)
                Me.MouseOverAction = Value
            End Set
        End Property

        Public Property MenuEffectsStyle() As String
            Get
                Return Me.EffectsStyle
            End Get
            Set(ByVal Value As String)
                Me.EffectsStyle = Value
            End Set
        End Property

        Public Property FontNames() As String
            Get
                Return Me.StyleFontNames
            End Get
            Set(ByVal Value As String)
                Me.StyleFontNames = Value
            End Set
        End Property

        Public Property FontSize() As String
            Get
                Return Me.StyleFontSize
            End Get
            Set(ByVal Value As String)
                Me.StyleFontSize = Value
            End Set
        End Property

        Public Property FontBold() As String
            Get
                Return Me.StyleFontBold
            End Get
            Set(ByVal Value As String)
                Me.StyleFontBold = Value
            End Set
        End Property

        Public Property MenuEffectsShadowStrength() As String
            Get
                Return Me.EffectsShadowStrength
            End Get
            Set(ByVal Value As String)
                Me.EffectsShadowStrength = Value
            End Set
        End Property

        Public Property MenuEffectsMenuTransition() As String
            Get
                Return Me.EffectsTransition
            End Get
            Set(ByVal Value As String)
                Me.EffectsTransition = Value
            End Set
        End Property

        Public Property MenuEffectsMenuTransitionLength() As String
            Get
                Return Me.EffectsDuration
            End Get
            Set(ByVal Value As String)
                Me.EffectsDuration = Value
            End Set
        End Property

        Public Property MenuEffectsShadowDirection() As String
            Get
                Return Me.EffectsShadowDirection
            End Get
            Set(ByVal Value As String)
                Me.EffectsShadowDirection = Value
            End Set
        End Property

        Public Property ForceFullMenuList() As String
            Get
                Return Me.ForceCrawlerDisplay
            End Get
            Set(ByVal Value As String)
                Me.ForceCrawlerDisplay = Value
            End Set
        End Property

        Public Property UseSkinPathArrowImages() As String
            Get
                Return _useSkinPathArrowImages
            End Get
            Set(ByVal Value As String)
                _useSkinPathArrowImages = Value
            End Set
        End Property

        Public Property UseRootBreadCrumbArrow() As String
            Get
                Return _useRootBreadCrumbArrow
            End Get
            Set(ByVal Value As String)
                _useRootBreadCrumbArrow = Value
            End Set
        End Property

        Public Property UseSubMenuBreadCrumbArrow() As String
            Get
                Return _useSubMenuBreadCrumbArrow
            End Get
            Set(ByVal Value As String)
                _useSubMenuBreadCrumbArrow = Value
            End Set
        End Property

        Public Property RootMenuItemBreadCrumbCssClass() As String
            Get
                Return Me.CSSBreadCrumbRoot
            End Get
            Set(ByVal Value As String)
                Me.CSSBreadCrumbRoot = Value
            End Set
        End Property

        Public Property SubMenuItemBreadCrumbCssClass() As String
            Get
                Return Me.CSSBreadCrumbSub
            End Get
            Set(ByVal Value As String)
                Me.CSSBreadCrumbSub = Value
            End Set
        End Property

        Public Property RootMenuItemCssClass() As String
            Get
                Return Me.CSSNodeRoot
            End Get
            Set(ByVal Value As String)
                Me.CSSNodeRoot = Value
            End Set
        End Property

        Public Property RootBreadCrumbArrow() As String
            Get
                Return m_strRootBreadCrumbArrow             'Me.IndicateChildImageBreadCrumbRoot
            End Get
            Set(ByVal Value As String)
                m_strRootBreadCrumbArrow = Value
            End Set
        End Property

        Public Property SubMenuBreadCrumbArrow() As String
            Get
                Return m_strSubMenuBreadCrumbArrow              'Me.IndicateChildImageBreadCrumbSub
            End Get
            Set(ByVal Value As String)
                m_strSubMenuBreadCrumbArrow = Value
            End Set
        End Property

        Public Property UseArrows() As String
            Get
                Return Me.IndicateChildren
            End Get
            Set(ByVal Value As String)
                Me.IndicateChildren = Value
            End Set
        End Property

        Public Property DownArrow() As String
            Get
                Return _downArrow
            End Get
            Set(ByVal Value As String)
                _downArrow = Value
            End Set
        End Property

        Public Property RightArrow() As String
            Get
                Return _rightArrow
            End Get
            Set(ByVal Value As String)
                _rightArrow = Value
            End Set
        End Property

        Public Property RootMenuItemActiveCssClass() As String
            Get
                Return Me.CSSNodeSelectedRoot
            End Get
            Set(ByVal Value As String)
                Me.CSSNodeSelectedRoot = Value
            End Set
        End Property

        Public Property SubMenuItemActiveCssClass() As String
            Get
                Return Me.CSSNodeSelectedSub
            End Get
            Set(ByVal Value As String)
                Me.CSSNodeSelectedSub = Value
            End Set
        End Property

        Public Property RootMenuItemSelectedCssClass() As String
            Get
                Return Me.CSSNodeHoverRoot
            End Get
            Set(ByVal Value As String)
                Me.CSSNodeHoverRoot = Value
            End Set
        End Property

        Public Property SubMenuItemSelectedCssClass() As String
            Get
                Return Me.CSSNodeHoverSub
            End Get
            Set(ByVal Value As String)
                Me.CSSNodeHoverSub = Value
            End Set
        End Property

        Public Property Separator() As String
            Get
                Return Me.SeparatorHTML
            End Get
            Set(ByVal Value As String)
                Me.SeparatorHTML = Value
            End Set
        End Property

        Public Property SeparatorCssClass() As String
            Get
                Return Me.CSSSeparator
            End Get
            Set(ByVal Value As String)
                Me.CSSSeparator = Value
            End Set
        End Property

        Public Property RootMenuItemLeftHtml() As String
            Get
                Return Me.NodeLeftHTMLRoot
            End Get
            Set(ByVal Value As String)
                Me.NodeLeftHTMLRoot = Value
            End Set
        End Property

        Public Property RootMenuItemRightHtml() As String
            Get
                Return Me.NodeRightHTMLRoot
            End Get
            Set(ByVal Value As String)
                Me.NodeRightHTMLRoot = Value
            End Set
        End Property

        Public Property SubMenuItemLeftHtml() As String
            Get
                Return Me.NodeLeftHTMLSub
            End Get
            Set(ByVal Value As String)
                Me.NodeLeftHTMLSub = Value
            End Set
        End Property

        Public Property SubMenuItemRightHtml() As String
            Get
                Return Me.NodeRightHTMLSub
            End Get
            Set(ByVal Value As String)
                Me.NodeRightHTMLSub = Value
            End Set
        End Property

        Public Property LeftSeparator() As String
            Get
                Return Me.SeparatorLeftHTML
            End Get
            Set(ByVal Value As String)
                Me.SeparatorLeftHTML = Value
            End Set
        End Property

        Public Property RightSeparator() As String
            Get
                Return Me.SeparatorRightHTML
            End Get
            Set(ByVal Value As String)
                Me.SeparatorRightHTML = Value
            End Set
        End Property

        Public Property LeftSeparatorActive() As String
            Get
                Return Me.SeparatorLeftHTMLActive
            End Get
            Set(ByVal Value As String)
                Me.SeparatorLeftHTMLActive = Value
            End Set
        End Property

        Public Property RightSeparatorActive() As String
            Get
                Return Me.SeparatorRightHTMLActive
            End Get
            Set(ByVal Value As String)
                Me.SeparatorRightHTMLActive = Value
            End Set
        End Property

        Public Property LeftSeparatorBreadCrumb() As String
            Get
                Return Me.SeparatorLeftHTMLBreadCrumb
            End Get
            Set(ByVal Value As String)
                Me.SeparatorLeftHTMLBreadCrumb = Value
            End Set
        End Property

        Public Property RightSeparatorBreadCrumb() As String
            Get
                Return Me.SeparatorRightHTMLBreadCrumb
            End Get
            Set(ByVal Value As String)
                Me.SeparatorRightHTMLBreadCrumb = Value
            End Set
        End Property

        Public Property LeftSeparatorCssClass() As String
            Get
                Return Me.CSSLeftSeparator
            End Get
            Set(ByVal Value As String)
                Me.CSSLeftSeparator = Value
            End Set
        End Property

        Public Property RightSeparatorCssClass() As String
            Get
                Return Me.CSSRightSeparator
            End Get
            Set(ByVal Value As String)
                Me.CSSRightSeparator = Value
            End Set
        End Property

        Public Property LeftSeparatorActiveCssClass() As String
            Get
                Return Me.CSSLeftSeparatorSelection
            End Get
            Set(ByVal Value As String)
                Me.CSSLeftSeparatorSelection = Value
            End Set
        End Property

        Public Property RightSeparatorActiveCssClass() As String
            Get
                Return Me.CSSRightSeparatorSelection
            End Get
            Set(ByVal Value As String)
                Me.CSSRightSeparatorSelection = Value
            End Set
        End Property

        Public Property LeftSeparatorBreadCrumbCssClass() As String
            Get
                Return Me.CSSLeftSeparatorBreadCrumb
            End Get
            Set(ByVal Value As String)
                Me.CSSLeftSeparatorBreadCrumb = Value
            End Set
        End Property

        Public Property RightSeparatorBreadCrumbCssClass() As String
            Get
                Return Me.CSSRightSeparatorBreadCrumb
            End Get
            Set(ByVal Value As String)
                Me.CSSRightSeparatorBreadCrumb = Value
            End Set
        End Property

        Public Property MenuAlignment() As String
            Get
                Return Me.ControlAlignment
            End Get
            Set(ByVal Value As String)
                Me.ControlAlignment = Value
            End Set
        End Property

        Public Property ClearDefaults() As String
            Get
                Return _clearDefaults
            End Get
            Set(ByVal Value As String)
                _clearDefaults = Value
            End Set
        End Property

        Public Property DelaySubmenuLoad() As String
            Get
                Return String.Empty
            End Get
            Set(ByVal Value As String)
            End Set
        End Property

        Public Property RootOnly() As String
            Get
                Return m_strRootOnly
            End Get
            Set(ByVal Value As String)
                m_strRootOnly = Value
            End Set
        End Property

#End Region


        '*******************************************************
        '
        ' The Page_Load server event handler on this page is used
        ' to populate the role information for the page
        '
        '*******************************************************
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                Dim blnUseSkinPathArrowImages As Boolean = Boolean.Parse(GetValue(UseSkinPathArrowImages, "False"))             'This setting allows for skin independant images without breaking legacy skins
                Dim blnUseRootBreadcrumbArrow As Boolean = Boolean.Parse(GetValue(UseRootBreadCrumbArrow, "True"))              'This setting determines if the DNN root menu will use an arrow to indicate it is the active root level tab
                Dim blnUseSubMenuBreadcrumbArrow As Boolean = Boolean.Parse(GetValue(UseSubMenuBreadCrumbArrow, "False"))               'This setting determines if the DNN sub-menus will use an arrow to indicate it is a member of the Breadcrumb tabs
                Dim blnUseArrows As Boolean = Boolean.Parse(GetValue(UseArrows, "True"))                'This setting determines if the submenu arrows will be used
                Dim blnRootOnly As Boolean = Boolean.Parse(GetValue(RootOnly, "False"))             'This setting determines if the submenu will be shown
                Dim strRootBreadcrumbArrow As String
                Dim strSubMenuBreadcrumbArrow As String
                Dim strRightArrow As String
                Dim strDownArrow As String

                If blnRootOnly Then
                    blnUseArrows = False
                    Me.PopulateNodesFromClient = False
                    Me.StartTabId = -1
                    Me.ExpandDepth = 1
                End If

                Dim objSkins As New UI.Skins.SkinController
                'image for root menu breadcrumb marking
                If RootBreadCrumbArrow <> "" Then
                    strRootBreadcrumbArrow = PortalSettings.ActiveTab.SkinPath & RootBreadCrumbArrow
                Else
                    strRootBreadcrumbArrow = Common.Globals.ApplicationPath & "/images/breadcrumb.gif"
                End If

                'image for submenu breadcrumb marking
                If SubMenuBreadCrumbArrow <> "" Then
                    strSubMenuBreadcrumbArrow = PortalSettings.ActiveTab.SkinPath & SubMenuBreadCrumbArrow
                End If

                If blnUseSubMenuBreadcrumbArrow Then
                    strSubMenuBreadcrumbArrow = Common.Globals.ApplicationPath & "/images/breadcrumb.gif"
                    Me.NodeLeftHTMLBreadCrumbSub = "<img alt=""*"" BORDER=""0"" src=""" & strSubMenuBreadcrumbArrow & """>"
                End If


                If blnUseRootBreadcrumbArrow Then
                    Me.NodeLeftHTMLBreadCrumbRoot = "<img alt=""*"" BORDER=""0"" src=""" & strRootBreadcrumbArrow & """>"
                End If

                'image for right facing arrow
                If RightArrow <> "" Then
                    strRightArrow = RightArrow
                Else
                    strRightArrow = "breadcrumb.gif"
                End If
                'image for down facing arrow
                If DownArrow <> "" Then
                    strDownArrow = DownArrow
                Else
                    strDownArrow = "menu_down.gif"
                End If

                'Set correct image path for all separator images
                If Separator <> "" Then
                    If Separator.IndexOf("src=") <> -1 Then
                        Separator = Replace(Separator, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                    End If
                End If

                If LeftSeparator <> "" Then
                    If LeftSeparator.IndexOf("src=") <> -1 Then
                        LeftSeparator = Replace(LeftSeparator, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                    End If
                End If
                If RightSeparator <> "" Then
                    If RightSeparator.IndexOf("src=") <> -1 Then
                        RightSeparator = Replace(RightSeparator, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                    End If
                End If
                If LeftSeparatorBreadCrumb <> "" Then
                    If LeftSeparatorBreadCrumb.IndexOf("src=") <> -1 Then
                        LeftSeparatorBreadCrumb = Replace(LeftSeparatorBreadCrumb, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                    End If
                End If
                If RightSeparatorBreadCrumb <> "" Then
                    If RightSeparatorBreadCrumb.IndexOf("src=") <> -1 Then
                        RightSeparatorBreadCrumb = Replace(RightSeparatorBreadCrumb, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                    End If
                End If
                If LeftSeparatorActive <> "" Then
                    If LeftSeparatorActive.IndexOf("src=") <> -1 Then
                        LeftSeparatorActive = Replace(LeftSeparatorActive, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                    End If
                End If
                If RightSeparatorActive <> "" Then
                    If RightSeparatorActive.IndexOf("src=") <> -1 Then
                        RightSeparatorActive = Replace(RightSeparatorActive, "src=""", "src=""" & PortalSettings.ActiveTab.SkinPath)
                    End If
                End If

                ' generate dynamic menu
                If blnUseSkinPathArrowImages = True Then
                    Me.PathSystemImage = PortalSettings.ActiveTab.SkinPath
                Else
                    Me.PathSystemImage = Common.Globals.ApplicationPath & "/images/"
                End If
                If Len(Me.PathImage) = 0 Then
                    Me.PathImage = PortalSettings.HomeDirectory
                End If
                If blnUseArrows Then
                    Me.IndicateChildImageSub = strRightArrow
                    If Me.ControlOrientation.ToLower = "vertical" Then                     'NavigationProvider.NavigationProvider.Orientation.Vertical Then
                        Me.IndicateChildImageRoot = strRightArrow
                    Else
                        Me.IndicateChildImageRoot = strDownArrow
                    End If
                Else
                    Me.PathSystemImage = Common.Globals.ApplicationPath & "/images/"
                    Me.IndicateChildImageSub = "spacer.gif"
                End If
                If Len(Me.PathSystemScript) = 0 Then Me.PathSystemScript = Common.Globals.ApplicationPath & "/controls/SolpartMenu/"

                BuildNodes(Nothing)

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        Private Sub BuildNodes(ByVal objNode As WebControls.DNNNode)
            Dim objNodes As DNNNodeCollection
            objNodes = GetNavigationNodes(objNode)
            Me.Control.ClearNodes()          'since we always bind we need to clear the nodes for providers that maintain their state
            Me.Bind(objNodes)
        End Sub

        Private Sub SetAttributes()
            SeparateCss = "1"           ' CStr(True)
            'Me.StyleSelectionBorderColor = Nothing

            If Boolean.Parse(GetValue(ClearDefaults, "False")) Then
                'Me.StyleSelectionBorderColor = Nothing
                'Me.StyleSelectionForeColor = Nothing
                'Me.StyleHighlightColor = Nothing
                'Me.StyleIconBackColor = Nothing
                'Me.EffectsShadowColor = Nothing
                'Me.StyleSelectionColor = Nothing
                'Me.StyleBackColor = Nothing
                'Me.StyleForeColor = Nothing
            Else             'these defaults used to be on the page HTML
                If Len(Me.MouseOutHideDelay) = 0 Then Me.MouseOutHideDelay = "500"
                If Len(Me.MouseOverAction) = 0 Then Me.MouseOverAction = True.ToString 'NavigationProvider.NavigationProvider.HoverAction.Expand
                If Len(Me.StyleBorderWidth) = 0 Then Me.StyleBorderWidth = "0"
                If Len(Me.StyleControlHeight) = 0 Then Me.StyleControlHeight = "16"
                If Len(Me.StyleNodeHeight) = 0 Then Me.StyleNodeHeight = "21"
                If Len(Me.StyleIconWidth) = 0 Then Me.StyleIconWidth = "0"
                'Me.StyleSelectionBorderColor = "#333333" 'cleared above
                If Len(Me.StyleSelectionColor) = 0 Then Me.StyleSelectionColor = "#CCCCCC"
                If Len(Me.StyleSelectionForeColor) = 0 Then Me.StyleSelectionForeColor = "White"
                If Len(Me.StyleHighlightColor) = 0 Then Me.StyleHighlightColor = "#FF8080"
                If Len(Me.StyleIconBackColor) = 0 Then Me.StyleIconBackColor = "#333333"
                If Len(Me.EffectsShadowColor) = 0 Then Me.EffectsShadowColor = "#404040"
                If Len(Me.MouseOverDisplay) = 0 Then Me.MouseOverDisplay = "highlight" 'NavigationProvider.NavigationProvider.HoverDisplay.Highlight
                If Len(Me.EffectsStyle) = 0 Then Me.EffectsStyle = "filter:progid:DXImageTransform.Microsoft.Shadow(color='DimGray', Direction=135, Strength=3);"
                If Len(Me.StyleFontSize) = 0 Then Me.StyleFontSize = "9"
                If Len(Me.StyleFontBold) = 0 Then Me.StyleFontBold = "True"
                If Len(Me.StyleFontNames) = 0 Then Me.StyleFontNames = "Tahoma,Arial,Helvetica"
                If Len(Me.StyleForeColor) = 0 Then Me.StyleForeColor = "White"
                If Len(Me.StyleBackColor) = 0 Then Me.StyleBackColor = "#333333"
                If Len(Me.PathSystemImage) = 0 Then Me.PathSystemImage = "/"
            End If
            If CBool(SeparateCss) = True Then
                If MenuBarCssClass <> "" Then
                    Me.CSSControl = MenuBarCssClass
                Else
                    Me.CSSControl = "MainMenu_MenuBar"
                End If
                If MenuContainerCssClass <> "" Then
                    Me.CSSContainerRoot = MenuContainerCssClass
                Else
                    Me.CSSContainerRoot = "MainMenu_MenuContainer"
                End If
                If MenuItemCssClass <> "" Then
                    Me.CSSNode = MenuItemCssClass
                Else
                    Me.CSSNode = "MainMenu_MenuItem"
                End If
                If MenuIconCssClass <> "" Then
                    Me.CSSIcon = MenuIconCssClass
                Else
                    Me.CSSIcon = "MainMenu_MenuIcon"
                End If
                If SubMenuCssClass <> "" Then
                    Me.CSSContainerSub = SubMenuCssClass
                Else
                    Me.CSSContainerSub = "MainMenu_SubMenu"
                End If
                If MenuBreakCssClass <> "" Then
                    Me.CSSBreak = MenuBreakCssClass
                Else
                    Me.CSSBreak = "MainMenu_MenuBreak"
                End If
                If MenuItemSelCssClass <> "" Then
                    Me.CSSNodeHover = MenuItemSelCssClass
                Else
                    Me.CSSNodeHover = "MainMenu_MenuItemSel"
                End If
                If MenuArrowCssClass <> "" Then
                    Me.CSSIndicateChildSub = MenuArrowCssClass
                Else
                    Me.CSSIndicateChildSub = "MainMenu_MenuArrow"
                End If
                If MenuRootArrowCssClass <> "" Then
                    Me.CSSIndicateChildRoot = MenuRootArrowCssClass
                Else
                    Me.CSSIndicateChildRoot = "MainMenu_RootMenuArrow"
                End If
            End If
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            SetAttributes()
            InitializeNavControl(Me, "SolpartMenuNavigationProvider")
            MyBase.OnInit(e)
        End Sub


    End Class

End Namespace
