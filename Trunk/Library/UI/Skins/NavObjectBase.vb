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

Imports DotNetNuke.UI.Navigation
Imports DotNetNuke.UI
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.UI.Skins
    Public Class NavObjectBase : Inherits DotNetNuke.UI.Skins.SkinObjectBase

#Region "Private Members"
        Private m_strLevel As String
        'Private m_strRootOnly As String
        Private m_strToolTip As String
        Private m_blnPopulateNodesFromClient As Boolean = True    'JH - POD
        Private m_intExpandDepth As Integer = -1    'JH - POD
        Private m_intStartTabId As Integer = -1
        Private m_objControl As NavigationProvider.NavigationProvider
        Private m_strProviderName As String = ""
        Private m_objCustomAttributes As Generic.List(Of CustomAttribute) = New Generic.List(Of CustomAttribute)  'JH - 2/5/07 - support for custom attributes
#End Region

        'JH - 2/5/07 - support for custom attributes
        <DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)> _
        Public ReadOnly Property CustomAttributes() As Generic.List(Of CustomAttribute)
            Get
                Return m_objCustomAttributes
            End Get
        End Property

        Public Property ProviderName() As String
            Get
                Return m_strProviderName
            End Get
            Set(ByVal Value As String)
                m_strProviderName = Value
            End Set
        End Property

        Protected ReadOnly Property Control() As NavigationProvider.NavigationProvider
            Get
                Return m_objControl
            End Get
        End Property

        Public Property Level() As String
            Get
                Return m_strLevel
            End Get
            Set(ByVal Value As String)
                m_strLevel = Value
            End Set
        End Property

        'Public Property RootOnly() As String
        '	Get
        '		Return m_strRootOnly
        '	End Get
        '	Set(ByVal Value As String)
        '		m_strRootOnly = Value
        '	End Set
        'End Property

        Public Property ToolTip() As String
            Get
                Return m_strToolTip
            End Get
            Set(ByVal Value As String)
                m_strToolTip = Value
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

        Public Property ExpandDepth() As Integer    'JH - POD
            Get
                Return m_intExpandDepth
            End Get
            Set(ByVal Value As Integer)
                m_intExpandDepth = Value
            End Set
        End Property

        Public Property StartTabId() As Integer
            Get
                Return m_intStartTabId
            End Get
            Set(ByVal Value As Integer)
                m_intStartTabId = Value
            End Set
        End Property

        Private m_strPathSystemImage As String
        Private m_strPathImage As String
        Private m_strPathSystemScript As String
        Private m_strControlOrientation As String
        Private m_strControlAlignment As String
        Private m_strForceCrawlerDisplay As String
        Private m_strForceDownLevel As String
        Private m_strMouseOutHideDelay As String
        Private m_strMouseOverDisplay As String
        Private m_strMouseOverAction As String
        Private m_strIndicateChildren As String
        Private m_strIndicateChildImageRoot As String
        Private m_strIndicateChildImageSub As String
        Private m_strIndicateChildImageExpandedRoot As String
        Private m_strIndicateChildImageExpandedSub As String
        Private m_strNodeLeftHTMLRoot As String
        Private m_strNodeRightHTMLRoot As String
        Private m_strNodeLeftHTMLSub As String
        Private m_strNodeRightHTMLSub As String
        Private m_strNodeLeftHTMLBreadCrumbRoot As String
        Private m_strNodeLeftHTMLBreadCrumbSub As String
        Private m_strNodeRightHTMLBreadCrumbRoot As String
        Private m_strNodeRightHTMLBreadCrumbSub As String
        Private m_strSeparatorHTML As String
        Private m_strSeparatorLeftHTML As String
        Private m_strSeparatorRightHTML As String
        Private m_strSeparatorLeftHTMLActive As String
        Private m_strSeparatorRightHTMLActive As String
        Private m_strSeparatorLeftHTMLBreadCrumb As String
        Private m_strSeparatorRightHTMLBreadCrumb As String
        Private m_strCSSControl As String
        Private m_strCSSContainerRoot As String
        Private m_strCSSNode As String
        Private m_strCSSIcon As String
        Private m_strCSSContainerSub As String
        Private m_strCSSNodeHover As String
        Private m_strCSSBreak As String
        Private m_strCSSIndicateChildSub As String
        Private m_strCSSIndicateChildRoot As String
        Private m_strCSSBreadCrumbRoot As String
        Private m_strCSSBreadCrumbSub As String
        Private m_strCSSNodeRoot As String
        Private m_strCSSNodeSelectedRoot As String
        Private m_strCSSNodeSelectedSub As String
        Private m_strCSSNodeHoverRoot As String
        Private m_strCSSNodeHoverSub As String
        Private m_strCSSSeparator As String
        Private m_strCSSLeftSeparator As String
        Private m_strCSSRightSeparator As String
        Private m_strCSSLeftSeparatorSelection As String
        Private m_strCSSRightSeparatorSelection As String
        Private m_strCSSLeftSeparatorBreadCrumb As String
        Private m_strCSSRightSeparatorBreadCrumb As String
        Private m_strStyleBackColor As String
        Private m_strStyleForeColor As String
        Private m_strStyleHighlightColor As String
        Private m_strStyleIconBackColor As String
        Private m_strStyleSelectionBorderColor As String
        Private m_strStyleSelectionColor As String
        Private m_strStyleSelectionForeColor As String
        Private m_strStyleControlHeight As String
        Private m_strStyleBorderWidth As String
        Private m_strStyleNodeHeight As String
        Private m_strStyleIconWidth As String
        Private m_strStyleFontNames As String
        Private m_strStyleFontSize As String
        Private m_strStyleFontBold As String
        Private m_strEffectsShadowColor As String
        Private m_strEffectsStyle As String
        Private m_strEffectsShadowStrength As String
        Private m_strEffectsTransition As String
        Private m_strEffectsDuration As String
        Private m_strEffectsShadowDirection As String
        Private m_strWorkImage As String

#Region "Public Members"

#Region "Paths"
        Public Property PathSystemImage() As String
            Get
                If Control Is Nothing Then Return m_strPathSystemImage Else Return Control.PathSystemImage
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strPathSystemImage = Value Else Control.PathSystemImage = Value
            End Set
        End Property

        Public Property PathImage() As String
            Get
                If Control Is Nothing Then Return m_strPathImage Else Return Control.PathImage
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strPathImage = Value Else Control.PathImage = Value
            End Set
        End Property

        Public Property WorkImage() As String
            Get
                If Control Is Nothing Then Return m_strWorkImage Else Return Control.WorkImage
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strWorkImage = Value Else Control.WorkImage = Value
            End Set
        End Property

        Public Property PathSystemScript() As String
            Get
                If Control Is Nothing Then Return m_strPathSystemScript Else Return Control.PathSystemScript
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strPathSystemScript = Value Else Control.PathSystemScript = Value
            End Set
        End Property

#End Region
#Region "Rendering"
        Public Property ControlOrientation() As String
            Get
                Dim retValue As String = ""
                If Control Is Nothing Then
                    retValue = m_strControlOrientation
                Else
                    Select Case Control.ControlOrientation
                        Case NavigationProvider.NavigationProvider.Orientation.Horizontal
                            retValue = "Horizontal"
                        Case NavigationProvider.NavigationProvider.Orientation.Vertical
                            retValue = "Vertical"
                    End Select
                End If
                Return retValue
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then
                    m_strControlOrientation = Value
                Else
                    Select Case Value.ToLower
                        Case "horizontal"
                            Control.ControlOrientation = NavigationProvider.NavigationProvider.Orientation.Horizontal
                        Case "vertical"
                            Control.ControlOrientation = NavigationProvider.NavigationProvider.Orientation.Vertical
                    End Select
                End If
            End Set
        End Property

        Public Property ControlAlignment() As String
            Get
                Dim retValue As String = ""
                If Control Is Nothing Then
                    retValue = m_strControlAlignment
                Else
                    Select Case Control.ControlAlignment
                        Case NavigationProvider.NavigationProvider.Alignment.Left
                            retValue = "Left"
                        Case NavigationProvider.NavigationProvider.Alignment.Right
                            retValue = "Right"
                        Case NavigationProvider.NavigationProvider.Alignment.Center
                            retValue = "Center"
                        Case NavigationProvider.NavigationProvider.Alignment.Justify
                            retValue = "Justify"
                    End Select
                End If
                Return retValue
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then
                    m_strControlAlignment = Value
                Else
                    Select Case Value.ToLower
                        Case "left"
                            Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Left
                        Case "right"
                            Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Right
                        Case "center"
                            Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Center
                        Case "justify"
                            Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Justify
                    End Select
                End If
            End Set
        End Property

        Public Property ForceCrawlerDisplay() As String
            Get
                If Control Is Nothing Then Return m_strForceCrawlerDisplay Else Return Control.ForceCrawlerDisplay
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strForceCrawlerDisplay = Value Else Control.ForceCrawlerDisplay = Value
            End Set
        End Property

        Public Property ForceDownLevel() As String
            Get
                If Control Is Nothing Then Return m_strForceDownLevel Else Return Control.ForceDownLevel
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strForceDownLevel = Value Else Control.ForceDownLevel = Value
            End Set
        End Property

#End Region
#Region "Mouse Properties"
        Public Property MouseOutHideDelay() As String
            Get
                If Control Is Nothing Then Return m_strMouseOutHideDelay Else Return Control.MouseOutHideDelay.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strMouseOutHideDelay = Value Else Control.MouseOutHideDelay = CDec(Value)
            End Set
        End Property

        Public Property MouseOverDisplay() As String
            Get
                Dim retValue As String = ""
                If Control Is Nothing Then
                    retValue = m_strMouseOverDisplay
                Else
                    Select Case Control.MouseOverDisplay
                        Case NavigationProvider.NavigationProvider.HoverDisplay.Highlight
                            retValue = "Highlight"
                        Case NavigationProvider.NavigationProvider.HoverDisplay.None
                            retValue = "None"
                        Case NavigationProvider.NavigationProvider.HoverDisplay.Outset
                            retValue = "Outset"
                    End Select
                End If
                Return retValue
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then
                    m_strMouseOverDisplay = Value
                Else
                    Select Case Value.ToLower
                        Case "highlight"
                            Control.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.Highlight
                        Case "outset"
                            Control.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.Outset
                        Case "none"
                            Control.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.None
                    End Select
                End If
            End Set
        End Property

        Public Property MouseOverAction() As String
            Get
                Dim retValue As String = ""
                If Control Is Nothing Then
                    retValue = m_strMouseOverAction
                Else
                    Select Case Control.MouseOverAction
                        Case NavigationProvider.NavigationProvider.HoverAction.Expand
                            retValue = "True"
                        Case NavigationProvider.NavigationProvider.HoverAction.None
                            retValue = "False"
                    End Select
                End If
                Return retValue
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then
                    m_strMouseOverAction = Value
                Else
                    If CBool(GetValue(Value, "True")) Then
                        Control.MouseOverAction = NavigationProvider.NavigationProvider.HoverAction.Expand
                    Else
                        Control.MouseOverAction = NavigationProvider.NavigationProvider.HoverAction.None
                    End If
                End If
            End Set
        End Property

#End Region
#Region "Indicate Child"
        Public Property IndicateChildren() As String
            Get
                If Control Is Nothing Then Return m_strIndicateChildren Else Return Control.IndicateChildren.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strIndicateChildren = Value Else Control.IndicateChildren = CBool(Value)
            End Set
        End Property

        Public Property IndicateChildImageRoot() As String
            Get
                If Control Is Nothing Then Return m_strIndicateChildImageRoot Else Return Control.IndicateChildImageRoot
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strIndicateChildImageRoot = Value Else Control.IndicateChildImageRoot = Value
            End Set
        End Property

        Public Property IndicateChildImageSub() As String
            Get
                If Control Is Nothing Then Return m_strIndicateChildImageSub Else Return Control.IndicateChildImageSub
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strIndicateChildImageSub = Value Else Control.IndicateChildImageSub = Value
            End Set
        End Property

        Public Property IndicateChildImageExpandedRoot() As String
            Get
                If Control Is Nothing Then Return m_strIndicateChildImageExpandedRoot Else Return Control.IndicateChildImageExpandedRoot
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strIndicateChildImageExpandedRoot = Value Else Control.IndicateChildImageExpandedRoot = Value
            End Set
        End Property

        Public Property IndicateChildImageExpandedSub() As String
            Get
                If Control Is Nothing Then Return m_strIndicateChildImageExpandedSub Else Return Control.IndicateChildImageExpandedSub
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strIndicateChildImageExpandedSub = Value Else Control.IndicateChildImageExpandedSub = Value
            End Set
        End Property
#End Region

#Region "Custom HTML"
#Region "Node HTML"
        Public Property NodeLeftHTMLRoot() As String
            Get
                If Control Is Nothing Then Return m_strNodeLeftHTMLRoot Else Return Control.NodeLeftHTMLRoot
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeLeftHTMLRoot = Value Else Control.NodeLeftHTMLRoot = Value
            End Set
        End Property

        Public Property NodeRightHTMLRoot() As String
            Get
                If Control Is Nothing Then Return m_strNodeRightHTMLRoot Else Return Control.NodeRightHTMLRoot
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeRightHTMLRoot = Value Else Control.NodeRightHTMLRoot = Value
            End Set
        End Property

        Public Property NodeLeftHTMLSub() As String
            Get
                If Control Is Nothing Then Return m_strNodeLeftHTMLSub Else Return Control.NodeLeftHTMLSub
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeLeftHTMLSub = Value Else Control.NodeLeftHTMLSub = Value
            End Set
        End Property

        Public Property NodeRightHTMLSub() As String
            Get
                If Control Is Nothing Then Return m_strNodeRightHTMLSub Else Return Control.NodeRightHTMLSub
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeRightHTMLSub = Value Else Control.NodeRightHTMLSub = Value
            End Set
        End Property

        Public Property NodeLeftHTMLBreadCrumbRoot() As String
            Get
                If Control Is Nothing Then Return m_strNodeLeftHTMLBreadCrumbRoot Else Return Control.NodeLeftHTMLBreadCrumbRoot
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeLeftHTMLBreadCrumbRoot = Value Else Control.NodeLeftHTMLBreadCrumbRoot = Value
            End Set
        End Property

        Public Property NodeLeftHTMLBreadCrumbSub() As String
            Get
                If Control Is Nothing Then Return m_strNodeLeftHTMLBreadCrumbSub Else Return Control.NodeLeftHTMLBreadCrumbSub
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeLeftHTMLBreadCrumbSub = Value Else Control.NodeLeftHTMLBreadCrumbSub = Value
            End Set
        End Property

        Public Property NodeRightHTMLBreadCrumbRoot() As String
            Get
                If Control Is Nothing Then Return m_strNodeRightHTMLBreadCrumbRoot Else Return Control.NodeRightHTMLBreadCrumbRoot
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeRightHTMLBreadCrumbRoot = Value Else Control.NodeRightHTMLBreadCrumbRoot = Value
            End Set
        End Property

        Public Property NodeRightHTMLBreadCrumbSub() As String
            Get
                If Control Is Nothing Then Return m_strNodeRightHTMLBreadCrumbSub Else Return Control.NodeRightHTMLBreadCrumbSub
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strNodeRightHTMLBreadCrumbSub = Value Else Control.NodeRightHTMLBreadCrumbSub = Value
            End Set
        End Property

#End Region

#Region "Separator HTML"
        Public Property SeparatorHTML() As String
            Get
                If Control Is Nothing Then Return m_strSeparatorHTML Else Return Control.SeparatorHTML
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strSeparatorHTML = Value Else Control.SeparatorHTML = Value
            End Set
        End Property

        Public Property SeparatorLeftHTML() As String
            Get
                If Control Is Nothing Then Return m_strSeparatorLeftHTML Else Return Control.SeparatorLeftHTML
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strSeparatorLeftHTML = Value Else Control.SeparatorLeftHTML = Value
            End Set
        End Property

        Public Property SeparatorRightHTML() As String
            Get
                If Control Is Nothing Then Return m_strSeparatorRightHTML Else Return Control.SeparatorRightHTML
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strSeparatorRightHTML = Value Else Control.SeparatorRightHTML = Value
            End Set
        End Property

        Public Property SeparatorLeftHTMLActive() As String
            Get
                If Control Is Nothing Then Return m_strSeparatorLeftHTMLActive Else Return Control.SeparatorLeftHTMLActive
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strSeparatorLeftHTMLActive = Value Else Control.SeparatorLeftHTMLActive = Value
            End Set
        End Property

        Public Property SeparatorRightHTMLActive() As String
            Get
                If Control Is Nothing Then Return m_strSeparatorRightHTMLActive Else Return Control.SeparatorRightHTMLActive
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strSeparatorRightHTMLActive = Value Else Control.SeparatorRightHTMLActive = Value
            End Set
        End Property

        Public Property SeparatorLeftHTMLBreadCrumb() As String
            Get
                If Control Is Nothing Then Return m_strSeparatorLeftHTMLBreadCrumb Else Return Control.SeparatorLeftHTMLBreadCrumb
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strSeparatorLeftHTMLBreadCrumb = Value Else Control.SeparatorLeftHTMLBreadCrumb = Value
            End Set
        End Property

        Public Property SeparatorRightHTMLBreadCrumb() As String
            Get
                If Control Is Nothing Then Return m_strSeparatorRightHTMLBreadCrumb Else Return Control.SeparatorRightHTMLBreadCrumb
            End Get
            Set(ByVal Value As String)
                Value = GetPath(Value)
                If Control Is Nothing Then m_strSeparatorRightHTMLBreadCrumb = Value Else Control.SeparatorRightHTMLBreadCrumb = Value
            End Set
        End Property

#End Region

#End Region

#Region "CSS"
        Public Property CSSControl() As String
            Get
                If Control Is Nothing Then Return m_strCSSControl Else Return Control.CSSControl
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSControl = Value Else Control.CSSControl = Value
            End Set
        End Property

        Public Property CSSContainerRoot() As String
            Get
                If Control Is Nothing Then Return m_strCSSContainerRoot Else Return Control.CSSContainerRoot
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSContainerRoot = Value Else Control.CSSContainerRoot = Value
            End Set
        End Property

        Public Property CSSNode() As String
            Get
                If Control Is Nothing Then Return m_strCSSNode Else Return Control.CSSNode
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSNode = Value Else Control.CSSNode = Value
            End Set
        End Property

        Public Property CSSIcon() As String
            Get
                If Control Is Nothing Then Return m_strCSSIcon Else Return Control.CSSIcon
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSIcon = Value Else Control.CSSIcon = Value
            End Set
        End Property

        Public Property CSSContainerSub() As String
            Get
                If Control Is Nothing Then Return m_strCSSContainerSub Else Return Control.CSSContainerSub
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSContainerSub = Value Else Control.CSSContainerSub = Value
            End Set
        End Property

        Public Property CSSNodeHover() As String
            Get
                If Control Is Nothing Then Return m_strCSSNodeHover Else Return Control.CSSNodeHover
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSNodeHover = Value Else Control.CSSNodeHover = Value
            End Set
        End Property

        Public Property CSSBreak() As String
            Get
                If Control Is Nothing Then Return m_strCSSBreak Else Return Control.CSSBreak
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSBreak = Value Else Control.CSSBreak = Value
            End Set
        End Property

        Public Property CSSIndicateChildSub() As String
            Get
                If Control Is Nothing Then Return m_strCSSIndicateChildSub Else Return Control.CSSIndicateChildSub
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSIndicateChildSub = Value Else Control.CSSIndicateChildSub = Value
            End Set
        End Property

        Public Property CSSIndicateChildRoot() As String
            Get
                If Control Is Nothing Then Return m_strCSSIndicateChildRoot Else Return Control.CSSIndicateChildRoot
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSIndicateChildRoot = Value Else Control.CSSIndicateChildRoot = Value
            End Set
        End Property

        Public Property CSSBreadCrumbRoot() As String
            Get
                If Control Is Nothing Then Return m_strCSSBreadCrumbRoot Else Return Control.CSSBreadCrumbRoot
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSBreadCrumbRoot = Value Else Control.CSSBreadCrumbRoot = Value
            End Set
        End Property

        Public Property CSSBreadCrumbSub() As String
            Get
                If Control Is Nothing Then Return m_strCSSBreadCrumbSub Else Return Control.CSSBreadCrumbSub
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSBreadCrumbSub = Value Else Control.CSSBreadCrumbSub = Value
            End Set
        End Property

        Public Property CSSNodeRoot() As String
            Get
                If Control Is Nothing Then Return m_strCSSNodeRoot Else Return Control.CSSNodeRoot
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSNodeRoot = Value Else Control.CSSNodeRoot = Value
            End Set
        End Property

        Public Property CSSNodeSelectedRoot() As String
            Get
                If Control Is Nothing Then Return m_strCSSNodeSelectedRoot Else Return Control.CSSNodeSelectedRoot
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSNodeSelectedRoot = Value Else Control.CSSNodeSelectedRoot = Value
            End Set
        End Property

        Public Property CSSNodeSelectedSub() As String
            Get
                If Control Is Nothing Then Return m_strCSSNodeSelectedSub Else Return Control.CSSNodeSelectedSub
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSNodeSelectedSub = Value Else Control.CSSNodeSelectedSub = Value
            End Set
        End Property

        Public Property CSSNodeHoverRoot() As String
            Get
                If Control Is Nothing Then Return m_strCSSNodeHoverRoot Else Return Control.CSSNodeHoverRoot
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSNodeHoverRoot = Value Else Control.CSSNodeHoverRoot = Value
            End Set
        End Property

        Public Property CSSNodeHoverSub() As String
            Get
                If Control Is Nothing Then Return m_strCSSNodeHoverSub Else Return Control.CSSNodeHoverSub
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSNodeHoverSub = Value Else Control.CSSNodeHoverSub = Value
            End Set
        End Property

        Public Property CSSSeparator() As String
            Get
                If Control Is Nothing Then Return m_strCSSSeparator Else Return Control.CSSSeparator
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSSeparator = Value Else Control.CSSSeparator = Value
            End Set
        End Property

        Public Property CSSLeftSeparator() As String
            Get
                If Control Is Nothing Then Return m_strCSSLeftSeparator Else Return Control.CSSLeftSeparator
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSLeftSeparator = Value Else Control.CSSLeftSeparator = Value
            End Set
        End Property

        Public Property CSSRightSeparator() As String
            Get
                If Control Is Nothing Then Return m_strCSSRightSeparator Else Return Control.CSSRightSeparator
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSRightSeparator = Value Else Control.CSSRightSeparator = Value
            End Set
        End Property

        Public Property CSSLeftSeparatorSelection() As String
            Get
                If Control Is Nothing Then Return m_strCSSLeftSeparatorSelection Else Return Control.CSSLeftSeparatorSelection
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSLeftSeparatorSelection = Value Else Control.CSSLeftSeparatorSelection = Value
            End Set
        End Property

        Public Property CSSRightSeparatorSelection() As String
            Get
                If Control Is Nothing Then Return m_strCSSRightSeparatorSelection Else Return Control.CSSRightSeparatorSelection
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSRightSeparatorSelection = Value Else Control.CSSRightSeparatorSelection = Value
            End Set
        End Property

        Public Property CSSLeftSeparatorBreadCrumb() As String
            Get
                If Control Is Nothing Then Return m_strCSSLeftSeparatorBreadCrumb Else Return Control.CSSLeftSeparatorBreadCrumb
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSLeftSeparatorBreadCrumb = Value Else Control.CSSLeftSeparatorBreadCrumb = Value
            End Set
        End Property

        Public Property CSSRightSeparatorBreadCrumb() As String
            Get
                If Control Is Nothing Then Return m_strCSSRightSeparatorBreadCrumb Else Return Control.CSSRightSeparatorBreadCrumb
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strCSSRightSeparatorBreadCrumb = Value Else Control.CSSRightSeparatorBreadCrumb = Value
            End Set
        End Property

#End Region
#Region "Styles"
        Public Property StyleBackColor() As String
            Get
                If Control Is Nothing Then Return m_strStyleBackColor Else Return Control.StyleBackColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleBackColor = Value Else Control.StyleBackColor = Value
            End Set
        End Property

        Public Property StyleForeColor() As String
            Get
                If Control Is Nothing Then Return m_strStyleForeColor Else Return Control.StyleForeColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleForeColor = Value Else Control.StyleForeColor = Value
            End Set
        End Property

        Public Property StyleHighlightColor() As String
            Get
                If Control Is Nothing Then Return m_strStyleHighlightColor Else Return Control.StyleHighlightColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleHighlightColor = Value Else Control.StyleHighlightColor = Value
            End Set
        End Property

        Public Property StyleIconBackColor() As String
            Get
                If Control Is Nothing Then Return m_strStyleIconBackColor Else Return Control.StyleIconBackColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleIconBackColor = Value Else Control.StyleIconBackColor = Value
            End Set
        End Property

        Public Property StyleSelectionBorderColor() As String
            Get
                If Control Is Nothing Then Return m_strStyleSelectionBorderColor Else Return Control.StyleSelectionBorderColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleSelectionBorderColor = Value Else Control.StyleSelectionBorderColor = Value
            End Set
        End Property

        Public Property StyleSelectionColor() As String
            Get
                If Control Is Nothing Then Return m_strStyleSelectionColor Else Return Control.StyleSelectionColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleSelectionColor = Value Else Control.StyleSelectionColor = Value
            End Set
        End Property

        Public Property StyleSelectionForeColor() As String
            Get
                If Control Is Nothing Then Return m_strStyleSelectionForeColor Else Return Control.StyleSelectionForeColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleSelectionForeColor = Value Else Control.StyleSelectionForeColor = Value
            End Set
        End Property

        Public Property StyleControlHeight() As String
            Get
                If Control Is Nothing Then Return m_strStyleControlHeight Else Return Control.StyleControlHeight.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleControlHeight = Value Else Control.StyleControlHeight = CDec(Value)
            End Set
        End Property

        Public Property StyleBorderWidth() As String
            Get
                If Control Is Nothing Then Return m_strStyleBorderWidth Else Return Control.StyleBorderWidth.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleBorderWidth = Value Else Control.StyleBorderWidth = CDec(Value)
            End Set
        End Property

        Public Property StyleNodeHeight() As String
            Get
                If Control Is Nothing Then Return m_strStyleNodeHeight Else Return Control.StyleNodeHeight.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleNodeHeight = Value Else Control.StyleNodeHeight = CDec(Value)
            End Set
        End Property

        Public Property StyleIconWidth() As String
            Get
                If Control Is Nothing Then Return m_strStyleIconWidth Else Return Control.StyleIconWidth.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleIconWidth = Value Else Control.StyleIconWidth = CDec(Value)
            End Set
        End Property

        Public Property StyleFontNames() As String
            Get
                If Control Is Nothing Then Return m_strStyleFontNames Else Return Control.StyleFontNames
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleFontNames = Value Else Control.StyleFontNames = Value
            End Set
        End Property

        Public Property StyleFontSize() As String
            Get
                If Control Is Nothing Then Return m_strStyleFontSize Else Return Control.StyleFontSize.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleFontSize = Value Else Control.StyleFontSize = CDec(Value)
            End Set
        End Property

        Public Property StyleFontBold() As String
            Get
                If Control Is Nothing Then Return m_strStyleFontBold Else Return Control.StyleFontBold
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strStyleFontBold = Value Else Control.StyleFontBold = Value
            End Set
        End Property


#End Region
#Region "Animation"
        Public Property EffectsShadowColor() As String
            Get
                If Control Is Nothing Then Return m_strEffectsShadowColor Else Return Control.EffectsShadowColor
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strEffectsShadowColor = Value Else Control.EffectsShadowColor = Value
            End Set
        End Property

        Public Property EffectsStyle() As String
            Get
                If Control Is Nothing Then Return m_strEffectsStyle Else Return Control.EffectsStyle
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strEffectsStyle = Value Else Control.EffectsStyle = Value
            End Set
        End Property

        Public Property EffectsShadowStrength() As String
            Get
                If Control Is Nothing Then Return m_strEffectsShadowStrength Else Return Control.EffectsShadowStrength.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strEffectsShadowStrength = Value Else Control.EffectsShadowStrength = CInt(Value)
            End Set
        End Property

        Public Property EffectsTransition() As String
            Get
                If Control Is Nothing Then Return m_strEffectsTransition Else Return Control.EffectsTransition
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strEffectsTransition = Value Else Control.EffectsTransition = Value
            End Set
        End Property

        Public Property EffectsDuration() As String
            Get
                If Control Is Nothing Then Return m_strEffectsDuration Else Return Control.EffectsDuration.ToString
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strEffectsDuration = Value Else Control.EffectsDuration = CDbl(Value)
            End Set
        End Property

        Public Property EffectsShadowDirection() As String
            Get
                If Control Is Nothing Then Return m_strEffectsShadowDirection Else Return Control.EffectsShadowDirection
            End Get
            Set(ByVal Value As String)
                If Control Is Nothing Then m_strEffectsShadowDirection = Value Else Control.EffectsShadowDirection = Value
            End Set
        End Property

#End Region


#End Region

        Public Function GetNavigationNodes(ByVal objNode As WebControls.DNNNode) As WebControls.DNNNodeCollection
            Dim intRootParent As Integer = PortalSettings.ActiveTab.TabID
            Dim objNodes As WebControls.DNNNodeCollection
            Dim eToolTips As ToolTipSource
            Dim intNavNodeOptions As Integer
            'Dim blnRootOnly As Boolean = Boolean.Parse(GetValue(RootOnly, "False"))
            Dim intDepth As Integer = ExpandDepth

            'This setting indicates the root level for the menu
            Select Case LCase(Level)
                Case "child"
                Case "parent"
                    intNavNodeOptions = NavNodeOptions.IncludeParent + NavNodeOptions.IncludeSelf
                Case "same"
                    intNavNodeOptions = NavNodeOptions.IncludeSiblings + NavNodeOptions.IncludeSelf
                Case Else    'root
                    intRootParent = -1
                    intNavNodeOptions = NavNodeOptions.IncludeSiblings + NavNodeOptions.IncludeSelf
            End Select

            Select Case LCase(ToolTip)
                Case "name"
                    eToolTips = ToolTipSource.TabName
                Case "title"
                    eToolTips = ToolTipSource.Title
                Case "description"
                    eToolTips = ToolTipSource.Description
                Case Else
                    eToolTips = ToolTipSource.None
            End Select

            'If blnRootOnly Then
            '	intDepth = 1
            '	intRootParent = -1
            '	'intNavNodeOptions = NavNodeOptions.IncludeRootOnly
            'Else
            If Me.PopulateNodesFromClient AndAlso Control.SupportsPopulateOnDemand Then
                intNavNodeOptions += NavNodeOptions.MarkPendingNodes
            End If
            If Me.PopulateNodesFromClient AndAlso Control.SupportsPopulateOnDemand = False Then
                ExpandDepth = -1
            End If

            'End If

            If StartTabId <> -1 Then intRootParent = StartTabId

            If Not objNode Is Nothing Then
                'we are in a POD request
                intRootParent = CInt(objNode.ID)
                intNavNodeOptions = NavNodeOptions.MarkPendingNodes    'other options for this don't apply, but we do know we want to mark pending nodes
                objNodes = Navigation.GetNavigationNodes(objNode, eToolTips, intRootParent, intDepth, intNavNodeOptions)
            Else
                objNodes = Navigation.GetNavigationNodes(Control.ClientID, eToolTips, intRootParent, intDepth, intNavNodeOptions)
            End If

            'If NeedsProcessNodes() Then
            '	ProcessNodes(objNodes)
            'End If

            Return objNodes

        End Function

        'Private Function NeedsProcessNodes() As Boolean
        '	If Boolean.Parse(GetValue(RootOnly, "False")) Then
        '		Return True
        '	End If
        'End Function

        'Private Sub ProcessNodes(ByVal objNodes As DNNNodeCollection)
        '	Dim objNode As DNNNode
        '	For Each objNode In objNodes
        '		If Boolean.Parse(GetValue(RootOnly, "False")) AndAlso objNode.HasNodes Then
        '			objNode.HasNodes = False
        '		End If
        '		ProcessNodes(objNode.DNNNodes)
        '	Next
        'End Sub

        Protected Function GetValue(ByVal strVal As String, ByVal strDefault As String) As String
            If Len(strVal) = 0 Then
                Return strDefault
            Else
                Return strVal
            End If
        End Function

        Protected Sub InitializeNavControl(ByVal objParent As Control, ByVal strDefaultProvider As String)
            If Len(Me.ProviderName) = 0 Then ProviderName = strDefaultProvider
            m_objControl = NavigationProvider.NavigationProvider.Instance(Me.ProviderName)
            Control.ControlID = "ctl" & Me.ID
            Control.Initialize()
            AssignControlProperties()
            objParent.Controls.Add(Control.NavigationControl)
        End Sub

        'since page needs to assign attributes before the init method we need this class to act as place holders.  Then once we are 
        'ready to bind the control (thus it is instantiated) we will assign the placeholder properties to the control
        Private Sub AssignControlProperties()
            If Len(m_strPathSystemImage) > 0 Then Control.PathSystemImage = m_strPathSystemImage
            If Len(m_strPathImage) > 0 Then Control.PathImage = m_strPathImage
            If Len(m_strPathSystemScript) > 0 Then Control.PathSystemScript = m_strPathSystemScript
            If Len(m_strWorkImage) > 0 Then Control.WorkImage = m_strWorkImage
            If Len(m_strControlOrientation) > 0 Then
                Select Case m_strControlOrientation.ToLower
                    Case "horizontal"
                        Control.ControlOrientation = NavigationProvider.NavigationProvider.Orientation.Horizontal
                    Case "vertical"
                        Control.ControlOrientation = NavigationProvider.NavigationProvider.Orientation.Vertical
                End Select
            End If
            If Len(m_strControlAlignment) > 0 Then
                Select Case m_strControlAlignment.ToLower
                    Case "left"
                        Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Left
                    Case "right"
                        Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Right
                    Case "center"
                        Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Center
                    Case "justify"
                        Control.ControlAlignment = NavigationProvider.NavigationProvider.Alignment.Justify
                End Select
            End If
            Control.ForceCrawlerDisplay = GetValue(m_strForceCrawlerDisplay, "False")
            Control.ForceDownLevel = GetValue(m_strForceDownLevel, "False")
            If Len(m_strMouseOutHideDelay) > 0 Then Control.MouseOutHideDelay = CDec(m_strMouseOutHideDelay)
            If Len(m_strMouseOverDisplay) > 0 Then
                Select Case m_strMouseOverDisplay.ToLower
                    Case "highlight"
                        Control.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.Highlight
                    Case "outset"
                        Control.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.Outset
                    Case "none"
                        Control.MouseOverDisplay = NavigationProvider.NavigationProvider.HoverDisplay.None
                End Select
            End If
            If CBool(GetValue(m_strMouseOverAction, "True")) Then
                Control.MouseOverAction = NavigationProvider.NavigationProvider.HoverAction.Expand
            Else
                Control.MouseOverAction = NavigationProvider.NavigationProvider.HoverAction.None
            End If
            Control.IndicateChildren = CBool(GetValue(m_strIndicateChildren, "True"))
            If Len(m_strIndicateChildImageRoot) > 0 Then Control.IndicateChildImageRoot = m_strIndicateChildImageRoot
            If Len(m_strIndicateChildImageSub) > 0 Then Control.IndicateChildImageSub = m_strIndicateChildImageSub
            If Len(m_strIndicateChildImageExpandedRoot) > 0 Then Control.IndicateChildImageExpandedRoot = m_strIndicateChildImageExpandedRoot
            If Len(m_strIndicateChildImageExpandedSub) > 0 Then Control.IndicateChildImageExpandedSub = m_strIndicateChildImageExpandedSub
            If Len(m_strNodeLeftHTMLRoot) > 0 Then Control.NodeLeftHTMLRoot = m_strNodeLeftHTMLRoot
            If Len(m_strNodeRightHTMLRoot) > 0 Then Control.NodeRightHTMLRoot = m_strNodeRightHTMLRoot
            If Len(m_strNodeLeftHTMLSub) > 0 Then Control.NodeLeftHTMLSub = m_strNodeLeftHTMLSub
            If Len(m_strNodeRightHTMLSub) > 0 Then Control.NodeRightHTMLSub = m_strNodeRightHTMLSub
            If Len(m_strNodeLeftHTMLBreadCrumbRoot) > 0 Then Control.NodeLeftHTMLBreadCrumbRoot = m_strNodeLeftHTMLBreadCrumbRoot
            If Len(m_strNodeLeftHTMLBreadCrumbSub) > 0 Then Control.NodeLeftHTMLBreadCrumbSub = m_strNodeLeftHTMLBreadCrumbSub
            If Len(m_strNodeRightHTMLBreadCrumbRoot) > 0 Then Control.NodeRightHTMLBreadCrumbRoot = m_strNodeRightHTMLBreadCrumbRoot
            If Len(m_strNodeRightHTMLBreadCrumbSub) > 0 Then Control.NodeRightHTMLBreadCrumbSub = m_strNodeRightHTMLBreadCrumbSub
            If Len(m_strSeparatorHTML) > 0 Then Control.SeparatorHTML = m_strSeparatorHTML
            If Len(m_strSeparatorLeftHTML) > 0 Then Control.SeparatorLeftHTML = m_strSeparatorLeftHTML
            If Len(m_strSeparatorRightHTML) > 0 Then Control.SeparatorRightHTML = m_strSeparatorRightHTML
            If Len(m_strSeparatorLeftHTMLActive) > 0 Then Control.SeparatorLeftHTMLActive = m_strSeparatorLeftHTMLActive
            If Len(m_strSeparatorRightHTMLActive) > 0 Then Control.SeparatorRightHTMLActive = m_strSeparatorRightHTMLActive
            If Len(m_strSeparatorLeftHTMLBreadCrumb) > 0 Then Control.SeparatorLeftHTMLBreadCrumb = m_strSeparatorLeftHTMLBreadCrumb
            If Len(m_strSeparatorRightHTMLBreadCrumb) > 0 Then Control.SeparatorRightHTMLBreadCrumb = m_strSeparatorRightHTMLBreadCrumb
            If Len(m_strCSSControl) > 0 Then Control.CSSControl = m_strCSSControl
            If Len(m_strCSSContainerRoot) > 0 Then Control.CSSContainerRoot = m_strCSSContainerRoot
            If Len(m_strCSSNode) > 0 Then Control.CSSNode = m_strCSSNode
            If Len(m_strCSSIcon) > 0 Then Control.CSSIcon = m_strCSSIcon
            If Len(m_strCSSContainerSub) > 0 Then Control.CSSContainerSub = m_strCSSContainerSub
            If Len(m_strCSSNodeHover) > 0 Then Control.CSSNodeHover = m_strCSSNodeHover
            If Len(m_strCSSBreak) > 0 Then Control.CSSBreak = m_strCSSBreak
            If Len(m_strCSSIndicateChildSub) > 0 Then Control.CSSIndicateChildSub = m_strCSSIndicateChildSub
            If Len(m_strCSSIndicateChildRoot) > 0 Then Control.CSSIndicateChildRoot = m_strCSSIndicateChildRoot
            If Len(m_strCSSBreadCrumbRoot) > 0 Then Control.CSSBreadCrumbRoot = m_strCSSBreadCrumbRoot
            If Len(m_strCSSBreadCrumbSub) > 0 Then Control.CSSBreadCrumbSub = m_strCSSBreadCrumbSub
            If Len(m_strCSSNodeRoot) > 0 Then Control.CSSNodeRoot = m_strCSSNodeRoot
            If Len(m_strCSSNodeSelectedRoot) > 0 Then Control.CSSNodeSelectedRoot = m_strCSSNodeSelectedRoot
            If Len(m_strCSSNodeSelectedSub) > 0 Then Control.CSSNodeSelectedSub = m_strCSSNodeSelectedSub
            If Len(m_strCSSNodeHoverRoot) > 0 Then Control.CSSNodeHoverRoot = m_strCSSNodeHoverRoot
            If Len(m_strCSSNodeHoverSub) > 0 Then Control.CSSNodeHoverSub = m_strCSSNodeHoverSub
            If Len(m_strCSSSeparator) > 0 Then Control.CSSSeparator = m_strCSSSeparator
            If Len(m_strCSSLeftSeparator) > 0 Then Control.CSSLeftSeparator = m_strCSSLeftSeparator
            If Len(m_strCSSRightSeparator) > 0 Then Control.CSSRightSeparator = m_strCSSRightSeparator
            If Len(m_strCSSLeftSeparatorSelection) > 0 Then Control.CSSLeftSeparatorSelection = m_strCSSLeftSeparatorSelection
            If Len(m_strCSSRightSeparatorSelection) > 0 Then Control.CSSRightSeparatorSelection = m_strCSSRightSeparatorSelection
            If Len(m_strCSSLeftSeparatorBreadCrumb) > 0 Then Control.CSSLeftSeparatorBreadCrumb = m_strCSSLeftSeparatorBreadCrumb
            If Len(m_strCSSRightSeparatorBreadCrumb) > 0 Then Control.CSSRightSeparatorBreadCrumb = m_strCSSRightSeparatorBreadCrumb
            If Len(m_strStyleBackColor) > 0 Then Control.StyleBackColor = m_strStyleBackColor
            If Len(m_strStyleForeColor) > 0 Then Control.StyleForeColor = m_strStyleForeColor
            If Len(m_strStyleHighlightColor) > 0 Then Control.StyleHighlightColor = m_strStyleHighlightColor
            If Len(m_strStyleIconBackColor) > 0 Then Control.StyleIconBackColor = m_strStyleIconBackColor
            If Len(m_strStyleSelectionBorderColor) > 0 Then Control.StyleSelectionBorderColor = m_strStyleSelectionBorderColor
            If Len(m_strStyleSelectionColor) > 0 Then Control.StyleSelectionColor = m_strStyleSelectionColor
            If Len(m_strStyleSelectionForeColor) > 0 Then Control.StyleSelectionForeColor = m_strStyleSelectionForeColor
            If Len(m_strStyleControlHeight) > 0 Then Control.StyleControlHeight = CDec(m_strStyleControlHeight)
            If Len(m_strStyleBorderWidth) > 0 Then Control.StyleBorderWidth = CDec(m_strStyleBorderWidth)
            If Len(m_strStyleNodeHeight) > 0 Then Control.StyleNodeHeight = CDec(m_strStyleNodeHeight)
            If Len(m_strStyleIconWidth) > 0 Then Control.StyleIconWidth = CDec(m_strStyleIconWidth)
            If Len(m_strStyleFontNames) > 0 Then Control.StyleFontNames = m_strStyleFontNames
            If Len(m_strStyleFontSize) > 0 Then Control.StyleFontSize = CDec(m_strStyleFontSize)
            If Len(m_strStyleFontBold) > 0 Then Control.StyleFontBold = m_strStyleFontBold
            If Len(m_strEffectsShadowColor) > 0 Then Control.EffectsShadowColor = m_strEffectsShadowColor
            If Len(m_strEffectsStyle) > 0 Then Control.EffectsStyle = m_strEffectsStyle
            If Len(m_strEffectsShadowStrength) > 0 Then Control.EffectsShadowStrength = CInt(m_strEffectsShadowStrength)
            If Len(m_strEffectsTransition) > 0 Then Control.EffectsTransition = m_strEffectsTransition
            If Len(m_strEffectsDuration) > 0 Then Control.EffectsDuration = CDbl(m_strEffectsDuration)
            If Len(m_strEffectsShadowDirection) > 0 Then Control.EffectsShadowDirection = m_strEffectsShadowDirection

            Control.CustomAttributes = Me.CustomAttributes  'JH - 2/5/07 - support for custom attributes

        End Sub

        Protected Sub Bind(ByVal objNodes As DNNNodeCollection)
            Control.Bind(objNodes)
        End Sub

        Private Function GetPath(ByVal strPath As String) As String
            Select Case True
                Case strPath.IndexOf("[SKINPATH]") > -1
                    Return Replace(strPath, "[SKINPATH]", PortalSettings.ActiveTab.SkinPath)
                Case strPath.IndexOf("[APPIMAGEPATH]") > -1
                    Return Replace(strPath, "[APPIMAGEPATH]", Common.Globals.ApplicationPath & "/images/")
                Case strPath.IndexOf("[HOMEDIRECTORY]") > -1
                    Return Replace(strPath, "[HOMEDIRECTORY]", PortalSettings.HomeDirectory)
                Case Else
                    If strPath.StartsWith("~") Then
                        Return ResolveUrl(strPath)
                    End If
            End Select
            Return strPath
        End Function

    End Class

    'JH - 2/5/07 - support for custom attributes
    Public Class CustomAttribute
        Public Name As String
        Public Value As String
    End Class
End Namespace