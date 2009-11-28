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
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Utilities.Animation

Namespace DotNetNuke.NavigationControl
    Public Class DNNMenuNavigationProvider
        Inherits DotNetNuke.Modules.NavigationProvider.NavigationProvider
        Private m_objMenu As DNNMenu
        Private m_strControlID As String
        Private m_strCSSBreak As String
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
        Private m_strPathImage As String
        Private m_objCustomAttributes As Generic.List(Of UI.Skins.CustomAttribute) = New Generic.List(Of UI.Skins.CustomAttribute) 'JH - 2/5/07 - support for custom attributes


        'Private PreviousRootBreadcrumbFlag As Boolean
        'Private PreviousRootActiveFlag As Boolean
        'Private NextRootBreadcrumbFlag As Boolean
        'Private NextRootActiveFlag As Boolean


        Public ReadOnly Property Menu() As DNNMenu
            Get
                Return m_objMenu
            End Get
        End Property

        'JH - 2/5/07 - support for custom attributes
        Public Overrides Property CustomAttributes() As System.Collections.Generic.List(Of UI.Skins.CustomAttribute)
            Get
                Return m_objCustomAttributes
            End Get
            Set(ByVal value As System.Collections.Generic.List(Of UI.Skins.CustomAttribute))
                m_objCustomAttributes = value
            End Set
        End Property

        Public Overrides ReadOnly Property NavigationControl() As System.Web.UI.Control
            Get
                Return Menu
            End Get
        End Property

        Public Overrides ReadOnly Property SupportsPopulateOnDemand() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Property WorkImage() As String
            Get
                Return Menu.WorkImage
            End Get
            Set(ByVal Value As String)
                Menu.WorkImage = Value
            End Set
        End Property

        Public Overrides Property IndicateChildImageSub() As String
            Get
                Return Menu.ChildArrowImage
            End Get
            Set(ByVal Value As String)
                Menu.ChildArrowImage = Value
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
                Select Case Menu.Orientation
                    Case DotNetNuke.UI.WebControls.Orientation.Horizontal
                        Return Modules.NavigationProvider.NavigationProvider.Orientation.Horizontal
                    Case DotNetNuke.UI.WebControls.Orientation.Vertical
                        Return Modules.NavigationProvider.NavigationProvider.Orientation.Vertical
                End Select
            End Get
            Set(ByVal Value As Modules.NavigationProvider.NavigationProvider.Orientation)
                Select Case Value
                    Case Modules.NavigationProvider.NavigationProvider.Orientation.Horizontal
                        Menu.Orientation = DotNetNuke.UI.WebControls.Orientation.Horizontal
                    Case Modules.NavigationProvider.NavigationProvider.Orientation.Vertical
                        Menu.Orientation = DotNetNuke.UI.WebControls.Orientation.Vertical
                End Select
            End Set
        End Property

        Public Overrides Property CSSIndicateChildSub() As String
            Get
                Return ""
            End Get
            Set(ByVal Value As String)
            End Set
        End Property

        Public Overrides Property CSSIndicateChildRoot() As String
            Get
                Return ""
            End Get
            Set(ByVal Value As String)
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
                Return m_strCSSBreak
            End Get
            Set(ByVal Value As String)
                m_strCSSBreak = Value
            End Set
        End Property

        Public Overrides Property CSSControl() As String
            Get
                Return Menu.MenuBarCssClass
            End Get
            Set(ByVal Value As String)
                Menu.MenuBarCssClass = Value
            End Set
        End Property

        Public Overrides Property CSSIcon() As String
            Get
                Return Menu.DefaultIconCssClass
            End Get
            Set(ByVal Value As String)
                Menu.DefaultIconCssClass = Value
            End Set
        End Property

        Public Overrides Property CSSNode() As String
            Get
                Return Menu.DefaultNodeCssClass
            End Get
            Set(ByVal Value As String)
                Menu.DefaultNodeCssClass = Value
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
                Return Menu.DefaultNodeCssClassOver
            End Get
            Set(ByVal Value As String)
                Menu.DefaultNodeCssClassOver = Value
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

        Public Overrides Property CSSContainerSub() As String
            Get
                Return Menu.MenuCssClass
            End Get
            Set(ByVal Value As String)
                Menu.MenuCssClass = Value
            End Set
        End Property

        Public Overrides Property ForceDownLevel() As String
            Get
                Return Menu.ForceDownLevel.ToString
            End Get
            Set(ByVal Value As String)
                Menu.ForceDownLevel = CBool(Value)
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

        Public Overrides Property PopulateNodesFromClient() As Boolean
            Get
                Return Menu.PopulateNodesFromClient
            End Get
            Set(ByVal Value As Boolean)
                Menu.PopulateNodesFromClient = Value
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

        Public Overrides Property PathSystemImage() As String
            Get
                Return Menu.SystemImagesPath
            End Get
            Set(ByVal Value As String)
                Menu.SystemImagesPath = Value
            End Set
        End Property

        Public Overrides Property PathImage() As String
            Get
                Return m_strPathImage
            End Get
            Set(ByVal Value As String)
                m_strPathImage = Value
            End Set
        End Property

        Public Overrides Property PathSystemScript() As String
            Get
                Return Menu.MenuScriptPath
            End Get
            Set(ByVal Value As String)
                'Menu.MenuScriptPath = Value	'Take out, use default CAPI
            End Set
        End Property

        Public Overrides Sub Initialize()
            m_objMenu = New DNNMenu
            Menu.ID = m_strControlID
            Menu.EnableViewState = False
            AddHandler Menu.NodeClick, AddressOf DNNMenu_NodeClick
            AddHandler Menu.PopulateOnDemand, AddressOf DNNMenu_PopulateOnDemand
        End Sub

        Public Overrides Sub Bind(ByVal objNodes As DotNetNuke.UI.WebControls.DNNNodeCollection)
            Dim objNode As DotNetNuke.UI.WebControls.DNNNode = Nothing
            Dim objMenuItem As MenuNode
            Dim objPrevNode As DotNetNuke.UI.WebControls.DNNNode = Nothing
            Dim RootFlag As Boolean
            Dim intIndex As Integer

            If IndicateChildren = False Then
                IndicateChildImageSub = ""
                IndicateChildImageRoot = ""
            End If

            If Len(Me.CSSNodeSelectedRoot) > 0 AndAlso Me.CSSNodeSelectedRoot = Me.CSSNodeSelectedSub Then
                Menu.DefaultNodeCssClassSelected = Me.CSSNodeSelectedRoot                 'set on parent, thus decreasing overall payload
            End If

            'JH - 2/5/07 - support for custom attributes
            For Each objAttr As DotNetNuke.UI.Skins.CustomAttribute In Me.CustomAttributes
                Select Case objAttr.Name.ToLower
                    Case "submenuorientation"
                        Me.Menu.SubMenuOrientation = DirectCast(System.Enum.Parse(Me.Menu.SubMenuOrientation.GetType, objAttr.Value), DotNetNuke.UI.WebControls.Orientation)
                    Case "usetables"
                        Me.Menu.RenderMode = DNNMenu.MenuRenderMode.Normal
                    Case "rendermode"
                        Me.Menu.RenderMode = DirectCast(System.Enum.Parse(GetType(DNNMenu.MenuRenderMode), objAttr.Value), DNNMenu.MenuRenderMode)
                    Case "animationtype"
                        Me.Menu.Animation.AnimationType = DirectCast(System.Enum.Parse(GetType(AnimationType), objAttr.Value), AnimationType)
                    Case "easingdirection"
                        Me.Menu.Animation.EasingDirection = DirectCast(System.Enum.Parse(GetType(EasingDirection), objAttr.Value), EasingDirection)
                    Case "easingtype"
                        Me.Menu.Animation.EasingType = DirectCast(System.Enum.Parse(GetType(EasingType), objAttr.Value), EasingType)
                    Case "animationinterval"
                        Me.Menu.Animation.Interval = Integer.Parse(objAttr.Value)
                    Case "animationlength"
                        Me.Menu.Animation.Length = Integer.Parse(objAttr.Value)
                End Select
            Next

            For Each objNode In objNodes
                If objNode.Level = 0 Then          ' root menu
                    intIndex = Menu.MenuNodes.Import(objNode, False)
                    objMenuItem = Menu.MenuNodes(intIndex)

                    If objNode.BreadCrumb AndAlso String.IsNullOrEmpty(NodeRightHTMLBreadCrumbRoot) = False Then
                        objMenuItem.RightHTML &= NodeRightHTMLBreadCrumbRoot
                    ElseIf String.IsNullOrEmpty(Me.NodeRightHTMLRoot) = False Then
                        objMenuItem.RightHTML = NodeRightHTMLRoot
                    End If

                    If RootFlag = True Then           'first root item has already been entered
                        AddSeparator("All", objPrevNode, objNode, objMenuItem)
                    Else
                        If String.IsNullOrEmpty(SeparatorLeftHTML) = False OrElse String.IsNullOrEmpty(SeparatorLeftHTMLBreadCrumb) = False OrElse String.IsNullOrEmpty(Me.SeparatorLeftHTMLActive) = False Then
                            AddSeparator("Left", objPrevNode, objNode, objMenuItem)
                        End If
                        RootFlag = True
                    End If

                    If objNode.BreadCrumb AndAlso String.IsNullOrEmpty(NodeLeftHTMLBreadCrumbRoot) = False Then
                        objMenuItem.LeftHTML &= NodeLeftHTMLBreadCrumbRoot
                    ElseIf String.IsNullOrEmpty(Me.NodeLeftHTMLRoot) = False Then
                        objMenuItem.LeftHTML &= Me.NodeLeftHTMLRoot
                    End If

                    If Me.CSSNodeRoot <> "" Then
                        objMenuItem.CSSClass = Me.CSSNodeRoot
                    End If
                    If Me.CSSNodeHoverRoot <> "" AndAlso Me.CSSNodeHoverRoot <> Me.CSSNodeHoverSub Then
                        objMenuItem.CSSClassHover = Me.CSSNodeHoverRoot
                    End If

                    objMenuItem.CSSIcon = " "          '< ignore for root...???
                    If objNode.BreadCrumb Then
                        If CSSBreadCrumbRoot <> "" Then objMenuItem.CSSClass = CSSBreadCrumbRoot
                        If objNode.Selected AndAlso Len(Menu.DefaultNodeCssClassSelected) = 0 Then '<--- not necessary when both are the same
                            objMenuItem.CSSClassSelected = Me.CSSNodeSelectedRoot
                        End If
                    End If


                Else          'If Not blnRootOnly Then
                    Try
                        Dim objParent As MenuNode = Menu.MenuNodes.FindNode(objNode.ParentNode.ID)

                        If objParent Is Nothing Then             'POD
                            objParent = Menu.MenuNodes(Menu.MenuNodes.Import(objNode.ParentNode.Clone, True))
                        End If
                        objMenuItem = objParent.MenuNodes.FindNode(objNode.ID)
                        If objMenuItem Is Nothing Then              'POD
                            objMenuItem = objParent.MenuNodes(objParent.MenuNodes.Import(objNode.Clone, True))
                        End If

                        If NodeLeftHTMLSub <> "" Then objMenuItem.LeftHTML = NodeLeftHTMLSub
                        If NodeRightHTMLSub <> "" Then objMenuItem.RightHTML = Me.NodeRightHTMLSub


                        If CSSNodeHoverSub <> "" AndAlso Me.CSSNodeHoverRoot <> Me.CSSNodeHoverSub Then
                            objMenuItem.CSSClassHover = CSSNodeHoverSub
                        End If

                        If objNode.BreadCrumb Then
                            If CSSBreadCrumbSub <> "" Then objMenuItem.CSSClass = Me.CSSBreadCrumbSub
                            If NodeLeftHTMLBreadCrumbSub <> "" Then objMenuItem.LeftHTML = NodeLeftHTMLBreadCrumbSub
                            If NodeRightHTMLBreadCrumbSub <> "" Then objMenuItem.RightHTML = NodeRightHTMLBreadCrumbSub
                            If objNode.Selected AndAlso Len(Menu.DefaultNodeCssClassSelected) = 0 Then
                                objMenuItem.CSSClass = Me.CSSNodeSelectedSub
                            End If
                        End If

                    Catch
                        ' throws exception if the parent tab has not been loaded ( may be related to user role security not allowing access to a parent tab )
                        objMenuItem = Nothing
                    End Try
                End If

                If Len(objNode.Image) > 0 Then
                    If objNode.Image.StartsWith("~/images/") Then
                        objNode.Image = objNode.Image.Replace("~/images/", Me.PathSystemImage)
                    ElseIf objNode.Image.StartsWith("~/") Then
                        objNode.Image = Globals.ResolveUrl(objNode.Image)
                    ElseIf objNode.Image.StartsWith("/") = False AndAlso Len(Me.PathImage) > 0 Then
                        objNode.Image = Me.PathImage & objNode.Image
                    End If
                    objMenuItem.Image = objNode.Image
                End If

                If objMenuItem.IsBreak Then
                    objMenuItem.CSSClass = Me.CSSBreak
                End If

                objMenuItem.ToolTip = objNode.ToolTip

                Bind(objNode.DNNNodes)
                objPrevNode = objNode
            Next

            If Not objNode Is Nothing AndAlso objNode.Level = 0 Then       ' root menu
                'solpartactions has a hardcoded image with no path information.  Assume if value is present and no path we need to add one.
                If Len(IndicateChildImageSub) > 0 AndAlso IndicateChildImageSub.IndexOf("/") = -1 Then IndicateChildImageSub = Me.PathSystemImage & IndicateChildImageSub
                If Len(IndicateChildImageRoot) > 0 AndAlso IndicateChildImageRoot.IndexOf("/") = -1 Then IndicateChildImageRoot = Me.PathSystemImage & IndicateChildImageRoot

            End If

        End Sub

        Private Sub AddSeparator(ByVal strType As String, ByVal objPrevNode As DotNetNuke.UI.WebControls.DNNNode, ByVal objNextNode As DotNetNuke.UI.WebControls.DNNNode, ByVal objMenuItem As MenuNode)
            Dim strLeftHTML As String = SeparatorLeftHTML & SeparatorLeftHTMLBreadCrumb & SeparatorLeftHTMLActive
            Dim strRightHTML As String = SeparatorRightHTML & SeparatorRightHTMLBreadCrumb & SeparatorRightHTMLActive
            Dim strHTML As String = Me.SeparatorHTML & strLeftHTML & strRightHTML
            If Len(strHTML) > 0 Then
                Dim strSeparator As String = ""
                Dim strSeparatorLeftHTML As String = ""
                Dim strSeparatorRightHTML As String = ""
                Dim strSeparatorClass As String = ""
                Dim strLeftSeparatorClass As String = ""
                Dim strRightSeparatorClass As String = ""

                If String.IsNullOrEmpty(strLeftHTML) = False Then
                    strLeftSeparatorClass = Me.GetSeparatorText(CSSLeftSeparator, CSSLeftSeparatorBreadCrumb, CSSLeftSeparatorSelection, objNextNode)
                    strSeparatorLeftHTML = Me.GetSeparatorText(SeparatorLeftHTML, SeparatorLeftHTMLBreadCrumb, SeparatorLeftHTMLActive, objNextNode)
                End If
                If String.IsNullOrEmpty(SeparatorHTML) = False Then
                    If CSSSeparator <> "" Then strSeparatorClass = CSSSeparator
                    strSeparator = SeparatorHTML
                End If
                If String.IsNullOrEmpty(strRightHTML) = False Then
                    strRightSeparatorClass = Me.GetSeparatorText(CSSRightSeparator, CSSRightSeparatorBreadCrumb, CSSRightSeparatorSelection, objNextNode)
                    strSeparatorRightHTML = Me.GetSeparatorText(SeparatorRightHTML, SeparatorRightHTMLBreadCrumb, SeparatorRightHTMLActive, objNextNode)
                End If

                If String.IsNullOrEmpty(strSeparatorRightHTML) = False Then 'AndAlso strType <> "Left" Then
                    objMenuItem.RightHTML &= GetSeparatorMarkup(strRightSeparatorClass, strSeparatorRightHTML)
                End If
                If String.IsNullOrEmpty(strSeparator) = False AndAlso strType = "All" Then 'strType <> "Left" AndAlso strType <> "Right" Then
                    objMenuItem.LeftHTML &= GetSeparatorMarkup(strSeparatorClass, strSeparator)
                End If
                If String.IsNullOrEmpty(strSeparatorLeftHTML) = False Then 'AndAlso strType <> "Right" Then
                    objMenuItem.LeftHTML &= GetSeparatorMarkup(strLeftSeparatorClass, strSeparatorLeftHTML)
                End If
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

        Private Function GetSeparatorMarkup(ByVal strClass As String, ByVal strHTML As String) As String
            Dim strRet As String = ""
            If String.IsNullOrEmpty(strClass) = False Then
                strRet &= "<span class=""" & strClass & """>" & strHTML & "</span>"
            Else
                strRet &= strHTML
            End If
            Return strRet
        End Function


        Private Sub DNNMenu_NodeClick(ByVal source As Object, ByVal e As UI.WebControls.DNNMenuNodeClickEventArgs)
            MyBase.RaiseEvent_NodeClick(e.Node)
        End Sub

        Private Sub DNNMenu_PopulateOnDemand(ByVal source As Object, ByVal e As UI.WebControls.DNNMenuEventArgs)
            MyBase.RaiseEvent_PopulateOnDemand(e.Node)
        End Sub

        Public Overrides Sub ClearNodes()
            Menu.MenuNodes.Clear()
        End Sub

    End Class

End Namespace
