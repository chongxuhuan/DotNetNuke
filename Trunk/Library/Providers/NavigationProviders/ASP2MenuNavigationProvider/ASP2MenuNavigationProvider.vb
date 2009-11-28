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
Imports System.Web.UI.WebControls

Namespace DotNetNuke.NavigationControl
    Public Class ASP2MenuNavigationProvider
        Inherits DotNetNuke.Modules.NavigationProvider.NavigationProvider

#Region "Member Variables"
        Private m_objMenu As Menu
        Private m_strControlID As String
        Private m_strNodeLeftHTMLSub As String = ""
        Private m_strNodeLeftHTMLBreadCrumbSub As String = ""
        Private m_strNodeLeftHTMLBreadCrumbRoot As String = ""
        Private m_strNodeLeftHTMLRoot As String = ""
        Private m_strNodeRightHTMLSub As String = ""
        Private m_strNodeRightHTMLBreadCrumbSub As String = ""
        Private m_strNodeRightHTMLBreadCrumbRoot As String = ""
        Private m_strNodeRightHTMLRoot As String = ""
        Private m_strPathImage As String = ""
        Private m_strSystemPathImage As String = ""
#End Region

#Region "Properties"
#Region "General"

        Public ReadOnly Property Menu() As Menu
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

        Public Overrides Property ControlID() As String
            Get
                Return m_strControlID
            End Get
            Set(ByVal Value As String)
                m_strControlID = Value
            End Set
        End Property
#End Region
#Region "Paths"
        Public Overrides Property PathImage() As String
            Get
                Return m_strPathImage
            End Get
            Set(ByVal Value As String)
                m_strPathImage = Value
            End Set
        End Property

        Public Overrides Property PathSystemImage() As String
            Get
                Return m_strSystemPathImage
            End Get
            Set(ByVal Value As String)
                m_strSystemPathImage = Value
            End Set
        End Property

#End Region
#Region "Rendering"

        Public Overrides Property ForceDownLevel() As String
            Get
                Return Menu.StaticDisplayLevels > 1
            End Get
            Set(ByVal Value As String)
                If CBool(Value) Then
                    Menu.StaticDisplayLevels = 99
                Else
                    Menu.StaticDisplayLevels = 1
                End If
            End Set
        End Property

        Public Overrides Property ControlOrientation() As Modules.NavigationProvider.NavigationProvider.Orientation
            Get
                Select Case Menu.Orientation
                    Case Web.UI.WebControls.Orientation.Horizontal
                        Return Modules.NavigationProvider.NavigationProvider.Orientation.Horizontal
                    Case Web.UI.WebControls.Orientation.Vertical
                        Return Modules.NavigationProvider.NavigationProvider.Orientation.Vertical
                End Select
            End Get
            Set(ByVal Value As Modules.NavigationProvider.NavigationProvider.Orientation)
                Select Case Value
                    Case Modules.NavigationProvider.NavigationProvider.Orientation.Horizontal
                        Menu.Orientation = Web.UI.WebControls.Orientation.Horizontal
                    Case Modules.NavigationProvider.NavigationProvider.Orientation.Vertical
                        Menu.Orientation = Web.UI.WebControls.Orientation.Vertical
                End Select
            End Set
        End Property
#End Region
#Region "Mouse Properties"

        Public Overrides Property MouseOutHideDelay() As Decimal
            Get
                Return Menu.DisappearAfter
            End Get
            Set(ByVal Value As Decimal)
                Menu.DisappearAfter = Value
            End Set
        End Property
#End Region
#Region "Indicate Children"

        Public Overrides Property IndicateChildren() As Boolean
            Get
                Return Menu.DynamicEnableDefaultPopOutImage
            End Get
            Set(ByVal Value As Boolean)
                Menu.DynamicEnableDefaultPopOutImage = Value
            End Set
        End Property

        Public Overrides Property IndicateChildImageSub() As String
            Get
                Return Menu.DynamicPopOutImageUrl
            End Get
            Set(ByVal Value As String)
                Menu.DynamicPopOutImageUrl = Value
            End Set
        End Property

        Public Overrides Property IndicateChildImageRoot() As String
            Get
                Return Menu.StaticPopOutImageUrl
            End Get
            Set(ByVal Value As String)
                Menu.StaticPopOutImageUrl = Value

            End Set
        End Property
#End Region
#Region "Custom HTML"

        Public Overrides Property NodeLeftHTMLRoot() As String
            Get
                Return m_strNodeLeftHTMLRoot
            End Get
            Set(ByVal Value As String)
                m_strNodeLeftHTMLRoot = Value
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

        Public Overrides Property NodeLeftHTMLSub() As String
            Get
                Return m_strNodeLeftHTMLSub
            End Get
            Set(ByVal Value As String)
                m_strNodeLeftHTMLSub = Value
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

        Public Overrides Property NodeLeftHTMLBreadCrumbSub() As String
            Get
                Return m_strNodeLeftHTMLBreadCrumbSub
            End Get
            Set(ByVal Value As String)
                m_strNodeLeftHTMLBreadCrumbSub = Value
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

        Public Overrides Property NodeLeftHTMLBreadCrumbRoot() As String
            Get
                Return m_strNodeLeftHTMLBreadCrumbRoot
            End Get
            Set(ByVal Value As String)
                m_strNodeLeftHTMLBreadCrumbRoot = Value
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
#End Region
#Region "CSS"
        Public Overrides Property CSSControl() As String
            Get
                Return Menu.DynamicMenuStyle.CssClass
            End Get
            Set(ByVal Value As String)
                Menu.DynamicMenuStyle.CssClass = Value
            End Set
        End Property

        Public Overrides Property CSSNode() As String
            Get
                Return Menu.DynamicMenuItemStyle.CssClass
            End Get
            Set(ByVal Value As String)
                Menu.DynamicMenuItemStyle.CssClass = Value
                For i As Integer = 0 To Menu.LevelMenuItemStyles.Count - 1
                    Menu.LevelMenuItemStyles(i).CssClass = Value
                Next
            End Set
        End Property

        Public Overrides Property CSSNodeRoot() As String
            Get
                Return Menu.LevelMenuItemStyles(0).CssClass
            End Get
            Set(ByVal Value As String)
                Menu.LevelMenuItemStyles(0).CssClass = Value
            End Set
        End Property

        Public Overrides Property CSSNodeSelectedRoot() As String
            Get
                Return Menu.LevelSelectedStyles(0).CssClass
            End Get
            Set(ByVal Value As String)
                Menu.LevelSelectedStyles(0).CssClass = Value
            End Set
        End Property

        Public Overrides Property CSSNodeSelectedSub() As String
            Get
                Return Menu.LevelSelectedStyles(1).CssClass
            End Get
            Set(ByVal Value As String)
                For i As Integer = 1 To Menu.LevelSelectedStyles.Count - 1
                    Menu.LevelSelectedStyles(i).CssClass = Value
                Next
            End Set
        End Property

        '* Same as CSSNodeHoverSub
        Public Overrides Property CSSNodeHover() As String
            Get
                Return Menu.DynamicHoverStyle.CssClass
            End Get
            Set(ByVal Value As String)
                Menu.DynamicHoverStyle.CssClass = Value
                Menu.StaticHoverStyle.CssClass = Value
            End Set
        End Property

        Public Overrides Property CSSNodeHoverSub() As String
            Get
                Return Menu.DynamicHoverStyle.CssClass
            End Get
            Set(ByVal Value As String)
                Menu.DynamicHoverStyle.CssClass = Value
            End Set
        End Property

        Public Overrides Property CSSNodeHoverRoot() As String
            Get
                Return Menu.StaticHoverStyle.CssClass
            End Get
            Set(ByVal Value As String)
                Menu.StaticHoverStyle.CssClass = Value
            End Set
        End Property

#End Region

#End Region

#Region "Event Handlers"
        ''' <summary>
        ''' This method is called by the provider to allow for the control to default values and set up
        ''' event handlers
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Initialize()
            m_objMenu = New Menu
            Menu.ID = m_strControlID
            Menu.EnableViewState = False   'Not sure why, but when we disable viewstate the menuitemclick does not fire...
            'Menu.Items.Clear()

            'default properties to match DNN defaults 
            Menu.DynamicPopOutImageUrl = ""
            Menu.StaticPopOutImageUrl = ""
            Me.ControlOrientation = Orientation.Horizontal

            'add event handlers
            AddHandler Menu.MenuItemClick, AddressOf Menu_NodeClick
            AddHandler Menu.PreRender, AddressOf Menu_PreRender

            'add how many levels worth of styles???
            For i As Integer = 0 To 6
                Menu.LevelMenuItemStyles.Add(New MenuItemStyle)
                Menu.LevelSelectedStyles.Add(New MenuItemStyle)
            Next
        End Sub

        ''' <summary>
        ''' Responsible for the populating of the underlying navigation control 
        ''' </summary>
        ''' <param name="objNodes">Node hierarchy used in control population</param>
        ''' <remarks></remarks>
        Public Overrides Sub Bind(ByVal objNodes As DotNetNuke.UI.WebControls.DNNNodeCollection)
            Dim objNode As DotNetNuke.UI.WebControls.DNNNode
            Dim objMenuItem As MenuItem = Nothing
            Dim objPrevNode As DotNetNuke.UI.WebControls.DNNNode

            If IndicateChildren = False Then
                IndicateChildImageSub = ""
                IndicateChildImageRoot = ""
            End If

            Dim strLeftHTML As String = ""
            Dim strRightHTML As String = ""

            For Each objNode In objNodes
                If objNode.IsBreak Then
                    'Not sure how to make breaks work...
                    'If Not objMenuItem Is Nothing Then
                    '    objMenuItem.SeparatorImageUrl = "~/images/spacer.gif"
                    'End If
                Else
                    strLeftHTML = ""
                    strRightHTML = ""

                    If objNode.Level = 0 Then               ' root menu
                        Menu.Items.Add(GetMenuItem(objNode))
                        objMenuItem = Menu.Items(Menu.Items.Count - 1)

                        If Me.NodeLeftHTMLRoot <> "" Then
                            strLeftHTML = Me.NodeLeftHTMLRoot
                        End If

                        If objNode.BreadCrumb Then
                            If NodeLeftHTMLBreadCrumbRoot <> "" Then strLeftHTML = NodeLeftHTMLBreadCrumbRoot
                            If NodeRightHTMLBreadCrumbRoot <> "" Then strRightHTML = NodeRightHTMLBreadCrumbRoot
                        End If

                        If Me.NodeRightHTMLRoot <> "" Then
                            strRightHTML = NodeRightHTMLRoot
                        End If

                    Else
                        Try
                            Dim objParent As MenuItem = Menu.FindItem(GetValuePath(objNode.ParentNode))
                            objParent.ChildItems.Add(GetMenuItem(objNode))
                            objMenuItem = objParent.ChildItems(objParent.ChildItems.Count - 1)

                            If NodeLeftHTMLSub <> "" Then
                                strLeftHTML = NodeLeftHTMLSub
                            End If

                            If objNode.BreadCrumb Then
                                If NodeLeftHTMLBreadCrumbSub <> "" Then strLeftHTML = NodeLeftHTMLBreadCrumbSub
                                If NodeRightHTMLBreadCrumbSub <> "" Then strRightHTML = NodeRightHTMLBreadCrumbSub
                            End If

                            If Me.NodeRightHTMLSub <> "" Then
                                strRightHTML = Me.NodeRightHTMLSub
                            End If
                        Catch
                            ' throws exception if the parent tab has not been loaded ( may be related to user role security not allowing access to a parent tab )
                            objMenuItem = Nothing
                        End Try
                    End If

                    'Append LeftHTML/RightHTML to menu's text property
                    If Not objMenuItem Is Nothing Then
                        If Len(strLeftHTML) > 0 Then objMenuItem.Text = strLeftHTML & objMenuItem.Text
                        If Len(strRightHTML) > 0 Then objMenuItem.Text = objMenuItem.Text & strRightHTML
                    End If

                    'Figure out image paths
                    If Len(objNode.Image) > 0 Then
                        If objNode.Image.StartsWith("~/images/") Then
                            objNode.Image = objNode.Image.Replace("~/images/", Me.PathSystemImage)
                        ElseIf objNode.Image.StartsWith("/") = False AndAlso Len(Me.PathImage) > 0 Then
                            objNode.Image = Me.PathImage & objNode.Image
                        End If
                        objMenuItem.ImageUrl = objNode.Image
                    End If

                    Bind(objNode.DNNNodes)
                    objPrevNode = objNode
                End If
            Next
        End Sub

        Private Sub Menu_NodeClick(ByVal source As Object, ByVal e As System.Web.UI.WebControls.MenuEventArgs)
            MyBase.RaiseEvent_NodeClick(e.Item.Value)
        End Sub

        Private Sub Menu_PreRender(ByVal sender As Object, ByVal e As System.EventArgs)
            If Len(Menu.StaticPopOutImageUrl) > 0 Then
                Menu.StaticPopOutImageUrl = Me.PathSystemImage & Menu.StaticPopOutImageUrl
            End If
            If Len(Menu.DynamicPopOutImageUrl) > 0 Then
                Menu.DynamicPopOutImageUrl = Me.PathSystemImage & Menu.DynamicPopOutImageUrl
            End If
        End Sub

#End Region

#Region "Helper Functions"

        ''' <summary>
        ''' Loops through each of the nodes parents and concatenates the keys to derive its valuepath
        ''' </summary>
        ''' <param name="objNode">DNNNode object to obtain valuepath from</param>
        ''' <returns>ValuePath of node</returns>
        ''' <remarks>
        ''' the ASP.NET Menu creates a unique key based off of all the menuitem's parents, delimited by a string.
        ''' I wish there was a way around this, for we are already guaranteeing the uniqueness of the key since is it pulled from the
        ''' database.  
        ''' </remarks>
        Private Function GetValuePath(ByVal objNode As DNNNode) As String
            Dim objParent As DNNNode = objNode.ParentNode
            Dim strPath As String = objNode.Key
            Do
                If objParent Is Nothing OrElse objParent.Level = -1 Then Exit Do
                strPath = objParent.Key & Menu.PathSeparator & strPath
                objParent = objParent.ParentNode
            Loop
            Return strPath
        End Function

        ''' <summary>
        ''' Create a ASP.NET Menu item for a given DNNNode
        ''' </summary>
        ''' <param name="objNode">Node to create item off of</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Due to ValuePath needed for postback routine, there is a HACK to replace out the 
        ''' id with the valuepath if a JSFunciton is specified
        ''' </remarks>
        Private Function GetMenuItem(ByVal objNode As DNNNode) As MenuItem
            Dim objItem As MenuItem = New MenuItem()
            objItem.Text = objNode.Text
            objItem.Value = objNode.Key

            If Len(objNode.JSFunction) > 0 Then
                'HACK...  The postback event needs to have the entire ValuePath to the menu item, not just the unique id
                '__doPostBack('dnn:ctr365:dnnACTIONS:ctldnnACTIONS','6') -> __doPostBack('dnn:ctr365:dnnACTIONS:ctldnnACTIONS','0\\6')}
                objItem.NavigateUrl = "javascript:" & objNode.JSFunction.Replace(Menu.ID & "','" & objNode.Key & "'", Menu.ID & "','" & GetValuePath(objNode).Replace("/", "\\") & "'") & objNode.NavigateURL  'TODO!
            Else
                objItem.NavigateUrl = objNode.NavigateURL
            End If
            objItem.Target = objNode.Target
            objItem.Selectable = objNode.Enabled
            objItem.ImageUrl = objNode.Image    'possibly fix for path
            'objItem.PopOutImageUrl
            objItem.Selected = objNode.Selected
            objItem.ToolTip = objNode.ToolTip

            Return objItem
        End Function
#End Region

    End Class

End Namespace
