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
#Region "Imports"
Imports System.IO
Imports System.Web
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common
Imports DotNetNuke.Framework.Providers
Imports DotNetNuke.UI.WebControls
#End Region

Namespace DotNetNuke.NavigationControl
    Public Class DNNTreeNavigationProvider
        Inherits DotNetNuke.Modules.NavigationProvider.NavigationProvider

#Region "Private Members"
        Private m_objTree As DnnTree
        Private m_strControlID As String
        Private m_strCSSBreadCrumbSub As String
        Private m_strCSSBreadCrumbRoot As String

        Private m_strNodeSelectedSub As String
        Private m_strNodeSelectedRoot As String
        Private m_strCSSNodeRoot As String
        Private m_strCSSNodeHoverSub As String
        Private m_strCSSNodeHoverRoot As String
        Private m_strNodeLeftHTML As String
        Private m_strNodeLeftHTMLBreadCrumb As String
        Private m_strNodeLeftHTMLBreadCrumbRoot As String
        Private m_strNodeLeftHTMLRoot As String
        Private m_strNodeRightHTML As String
        Private m_strNodeRightHTMLBreadCrumb As String
        Private m_strNodeRightHTMLBreadCrumbRoot As String
        Private m_strNodeRightHTMLRoot As String
        Private m_blnIndicateChildren As Boolean
        Private m_strPathImage As String
#End Region

#Region "Public Properties"

        Public ReadOnly Property Tree() As DnnTree
            Get
                Return m_objTree
            End Get
        End Property

        Public Overrides ReadOnly Property NavigationControl() As System.Web.UI.Control
            Get
                Return Tree
            End Get
        End Property

        Public Overrides ReadOnly Property SupportsPopulateOnDemand() As Boolean
            Get
                Return True
            End Get
        End Property

        Public Overrides Property IndicateChildImageSub() As String
            Get
                Return Tree.CollapsedNodeImage
            End Get
            Set(ByVal Value As String)
                Tree.CollapsedNodeImage = Value
            End Set
        End Property

        Public Overrides Property IndicateChildImageRoot() As String
            Get
                Return Tree.CollapsedNodeImage
            End Get
            Set(ByVal Value As String)
                Tree.CollapsedNodeImage = Value
            End Set
        End Property

		Public Overrides Property WorkImage() As String
			Get
				Return Tree.WorkImage
			End Get
			Set(ByVal Value As String)
				Tree.WorkImage = Value
			End Set
		End Property

		Public Overrides Property IndicateChildImageExpandedRoot() As String
			Get
				Return Tree.ExpandedNodeImage
			End Get
			Set(ByVal Value As String)
				Tree.ExpandedNodeImage = Value
			End Set
		End Property

		Public Overrides Property IndicateChildImageExpandedSub() As String
			Get
				Return Tree.ExpandedNodeImage
			End Get
			Set(ByVal Value As String)
				Tree.ExpandedNodeImage = Value
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


		Public Overrides Property CSSControl() As String
			Get
				Return Tree.CssClass				   '???
			End Get
			Set(ByVal Value As String)
				Tree.CssClass = Value
			End Set
		End Property

		Public Overrides Property CSSIcon() As String
			Get
				Return Tree.DefaultIconCssClass
			End Get
			Set(ByVal Value As String)
				Tree.DefaultIconCssClass = Value
			End Set
		End Property


		Public Overrides Property CSSNode() As String
			Get
				Return Tree.DefaultNodeCssClass
			End Get
			Set(ByVal Value As String)
				Tree.DefaultNodeCssClass = Value
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
				Return Tree.DefaultNodeCssClassOver
			End Get
			Set(ByVal Value As String)
				Tree.DefaultNodeCssClassOver = Value
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

		Public Overrides Property ForceDownLevel() As String
			Get
				Return Tree.ForceDownLevel.ToString
			End Get
			Set(ByVal Value As String)
				Tree.ForceDownLevel = CBool(Value)
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
				Return Tree.PopulateNodesFromClient
			End Get
			Set(ByVal Value As Boolean)
				Tree.PopulateNodesFromClient = Value
			End Set
		End Property

		Public Overrides Property PathSystemImage() As String
			Get
				Return Tree.SystemImagesPath
			End Get
			Set(ByVal Value As String)
				Tree.SystemImagesPath = Value
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
				Return Tree.TreeScriptPath
			End Get
			Set(ByVal Value As String)
				'Tree.TreeScriptPath = Value	'Take out, use default CAPI
			End Set
		End Property
#End Region

#Region "Overridden Methods"
        Public Overrides Sub Initialize()
            m_objTree = New DnnTree
            Tree.ID = m_strControlID
            AddHandler Tree.NodeClick, AddressOf DNNTree_NodeClick
            AddHandler Tree.PopulateOnDemand, AddressOf DNNTree_PopulateOnDemand
        End Sub

        Public Overrides Sub Bind(ByVal objNodes As DotNetNuke.UI.WebControls.DNNNodeCollection)
            Dim objNode As DotNetNuke.UI.WebControls.DNNNode
            Dim objTreeItem As TreeNode
            Dim intIndex As Integer

            If IndicateChildren = False Then
                IndicateChildImageSub = ""
                IndicateChildImageRoot = ""
                Me.IndicateChildImageExpandedSub = ""
                Me.IndicateChildImageExpandedRoot = ""
            End If

            If Len(Me.CSSNodeSelectedRoot) > 0 AndAlso Me.CSSNodeSelectedRoot = Me.CSSNodeSelectedSub Then
                Tree.DefaultNodeCssClassSelected = Me.CSSNodeSelectedRoot               'set on parent, thus decreasing overall payload
            End If

            For Each objNode In objNodes
                If objNode.Level = 0 Then               ' root Tree
                    intIndex = Tree.TreeNodes.Import(objNode, True)
                    objTreeItem = Tree.TreeNodes(intIndex)
                    If objNode.Enabled = False Then
                        objTreeItem.ClickAction = eClickAction.Expand
                    End If

                    If Me.CSSNodeRoot <> "" Then
                        objTreeItem.CssClass = Me.CSSNodeRoot
                    End If
                    If Me.CSSNodeHoverRoot <> "" Then
                        objTreeItem.CSSClassHover = Me.CSSNodeHoverRoot
                    End If


                    If Len(Tree.DefaultNodeCssClassSelected) = 0 AndAlso Len(Me.CSSNodeSelectedRoot) > 0 Then
                        objTreeItem.CSSClassSelected = Me.CSSNodeSelectedRoot
                    End If

                    objTreeItem.CSSIcon = " "                     '< ignore for root...???
                    If objNode.BreadCrumb Then
                        objTreeItem.CssClass = Me.CSSBreadCrumbRoot
                    End If
                Else
                    Try
                        Dim objParent As TreeNode = Tree.TreeNodes.FindNode(objNode.ParentNode.ID)

                        If objParent Is Nothing Then                         'POD
                            objParent = Tree.TreeNodes(Tree.TreeNodes.Import(objNode.ParentNode.Clone, True))
                        End If
                        objTreeItem = objParent.TreeNodes.FindNode(objNode.ID)
                        If objTreeItem Is Nothing Then                       'POD
                            objTreeItem = objParent.TreeNodes(objParent.TreeNodes.Import(objNode.Clone, True))
                        End If

                        If objNode.Enabled = False Then
                            objTreeItem.ClickAction = eClickAction.Expand
                        End If

                        If CSSNodeHover <> "" Then
                            objTreeItem.CSSClassHover = CSSNodeHover
                        End If

                        If Len(Tree.DefaultNodeCssClassSelected) = 0 AndAlso Len(Me.CSSNodeSelectedSub) > 0 Then
                            objTreeItem.CSSClassSelected = Me.CSSNodeSelectedSub
                        End If

                        If objNode.BreadCrumb Then
                            objTreeItem.CssClass = Me.CSSBreadCrumbSub
                        End If

                    Catch
                        ' throws exception if the parent tab has not been loaded ( may be related to user role security not allowing access to a parent tab )
                        objTreeItem = Nothing
                    End Try
                End If

                If Len(objNode.Image) > 0 Then
                    If objNode.Image.StartsWith("~/images/") Then
                        objNode.Image = objNode.Image.Replace("~/images/", Me.PathSystemImage)
                    ElseIf objNode.Image.StartsWith("/") = False AndAlso Len(Me.PathImage) > 0 Then
                        objNode.Image = Me.PathImage & objNode.Image
                    End If
                    objTreeItem.Image = objNode.Image
                End If
                objTreeItem.ToolTip = objNode.ToolTip

                'End Select
                If objNode.Selected Then
                    Tree.SelectNode(objNode.ID)
                End If
                Bind(objNode.DNNNodes)
            Next

        End Sub

#End Region

#Region "Event Handlers"
        Private Sub DNNTree_NodeClick(ByVal source As Object, ByVal e As UI.WebControls.DNNTreeNodeClickEventArgs)
            MyBase.RaiseEvent_NodeClick(e.Node)
        End Sub

        Private Sub DNNTree_PopulateOnDemand(ByVal source As Object, ByVal e As UI.WebControls.DNNTreeEventArgs)
            MyBase.RaiseEvent_PopulateOnDemand(e.Node)
        End Sub
#End Region

        Public Overrides Sub ClearNodes()
            Tree.TreeNodes.Clear()
        End Sub
    End Class

End Namespace
