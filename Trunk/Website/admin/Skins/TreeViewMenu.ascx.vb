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

Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.UI.Skins.Controls

	''' -----------------------------------------------------------------------------
	''' Project	 : DotNetNuke
	''' Class	 : TreeViewMenu
	''' -----------------------------------------------------------------------------
	''' <summary>
	''' TreeViewMenu is a Skin Object that creates a Menu using the DNN Treeview Control
	''' to provide a Windows Explore like Menu.
	''' </summary>
	''' <remarks></remarks>
	''' <history>
	''' 	[cnurse]	12/8/2004	created
	''' </history>
	''' -----------------------------------------------------------------------------
    Partial Class TreeViewMenu

        Inherits UI.Skins.NavObjectBase

#Region " Web Form Designer Generated Code "


        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

#Region "Enums"

        Private Enum eImageType
            FolderClosed = 0
            FolderOpen = 1
            Page = 2
            GotoParent = 3
        End Enum

#End Region

#Region "Private Members"
        Private _bodyCssClass As String = ""
        Private _cssClass As String = ""
        Private _headerCssClass As String = ""
        Private _headerTextCssClass As String = "Head"
        Private _headerText As String = ""
        Private _resourceKey As String = ""
        Private _includeHeader As Boolean = True
        Private _nodeChildCssClass As String = "Normal"
        Private _nodeClosedImage As String = "~/images/folderclosed.gif"
        Private _nodeCollapseImage As String = "~/images/min.gif"
        Private _nodeCssClass As String = "Normal"
        Private _nodeExpandImage As String = "~/images/max.gif"
        Private _nodeLeafImage As String = "~/images/file.gif"
        Private _nodeOpenImage As String = "~/images/folderopen.gif"
        Private _nodeOverCssClass As String = "Normal"
        Private _nodeSelectedCssClass As String = "Normal"
        Private _nowrap As Boolean = False
        Private _treeCssClass As String = ""
        Private _treeGoUpImage As String = "~/images/folderup.gif"
        Private _treeIndentWidth As Integer = 10
        Private _width As String = "100%"

        Const MyFileName As String = "TreeViewMenu.ascx"

#End Region

#Region "Public Properties"

        Public Property BodyCssClass() As String
            Get
                Return _bodyCssClass
            End Get
            Set(ByVal Value As String)
                _bodyCssClass = Value
            End Set
        End Property

        Public Property CssClass() As String
            Get
                Return _cssClass
            End Get
            Set(ByVal Value As String)
                _cssClass = Value
            End Set
        End Property

        Public Property HeaderCssClass() As String
            Get
                Return _headerCssClass
            End Get
            Set(ByVal Value As String)
                _headerCssClass = Value
            End Set
        End Property

        Public Property HeaderTextCssClass() As String
            Get
                Return _headerTextCssClass
            End Get
            Set(ByVal Value As String)
                _headerTextCssClass = Value
            End Set
        End Property

        Public Property HeaderText() As String
            Get
                Return _headerText
            End Get
            Set(ByVal Value As String)
                _headerText = Value
            End Set
        End Property

        Public Property IncludeHeader() As Boolean
            Get
                Return _includeHeader
            End Get
            Set(ByVal Value As Boolean)
                _includeHeader = Value
            End Set
        End Property

        Public Property NodeChildCssClass() As String
            Get
                Return _nodeChildCssClass
            End Get
            Set(ByVal Value As String)
                _nodeChildCssClass = Value
            End Set
        End Property

        Public Property NodeClosedImage() As String
            Get
                Return _nodeClosedImage
            End Get
            Set(ByVal Value As String)
                _nodeClosedImage = Value
            End Set
        End Property

        Public Property NodeCollapseImage() As String
            Get
                Return _nodeCollapseImage
            End Get
            Set(ByVal Value As String)
                _nodeCollapseImage = Value
            End Set
        End Property

        Public Property NodeCssClass() As String
            Get
                Return _nodeCssClass
            End Get
            Set(ByVal Value As String)
                _nodeCssClass = Value
            End Set
        End Property

        Public Property NodeExpandImage() As String
            Get
                Return _nodeExpandImage
            End Get
            Set(ByVal Value As String)
                _nodeExpandImage = Value
            End Set
        End Property

        Public Property NodeLeafImage() As String
            Get
                Return _nodeLeafImage
            End Get
            Set(ByVal Value As String)
                _nodeLeafImage = Value
            End Set
        End Property

        Public Property NodeOpenImage() As String
            Get
                Return _nodeOpenImage
            End Get
            Set(ByVal Value As String)
                _nodeOpenImage = Value
            End Set
        End Property

        Public Property NodeOverCssClass() As String
            Get
                Return _nodeOverCssClass
            End Get
            Set(ByVal Value As String)
                _nodeOverCssClass = Value
            End Set
        End Property

        Public Property NodeSelectedCssClass() As String
            Get
                Return _nodeSelectedCssClass
            End Get
            Set(ByVal Value As String)
                _nodeSelectedCssClass = Value
            End Set
        End Property

        Public Property NoWrap() As Boolean
            Get
                Return _nowrap
            End Get
            Set(ByVal Value As Boolean)
                _nowrap = Value
            End Set
        End Property

        Public Property ResourceKey() As String
            Get
                Return _resourceKey
            End Get
            Set(ByVal Value As String)
                _resourceKey = Value
            End Set
        End Property

        Public Property TreeCssClass() As String
            Get
                Return _treeCssClass
            End Get
            Set(ByVal Value As String)
                _treeCssClass = Value
            End Set
        End Property

        Public Property TreeGoUpImage() As String
            Get
                Return _treeGoUpImage
            End Get
            Set(ByVal Value As String)
                _treeGoUpImage = Value
            End Set
        End Property

        Public Property TreeIndentWidth() As Integer
            Get
                Return _treeIndentWidth
            End Get
            Set(ByVal Value As Integer)
                _treeIndentWidth = Value
            End Set
        End Property

        Public Property Width() As String
            Get
                Return _width
            End Get
            Set(ByVal Value As String)
                _width = Value
            End Set
        End Property

#End Region

#Region "Private Methods"
        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The BuildTree helper method is used to build the tree
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        12/8/2004   Created
        '''		[Jon Henning]	3/21/06		Updated to handle Auto-expand and AddUpNode	
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BuildTree(ByVal objNode As DNNNode, ByVal blnPODRequest As Boolean)      'JH - POD

            Dim blnAddUpNode As Boolean = False
            Dim objNodes As DNNNodeCollection
            objNodes = GetNavigationNodes(objNode)

            If blnPODRequest = False Then
                If Not String.IsNullOrEmpty(Level) Then
                    Select Case Level.ToLower
                        Case "root"
                        Case "child"
                            blnAddUpNode = True
                        Case Else
                            If Level.ToLower <> "root" AndAlso PortalSettings.ActiveTab.BreadCrumbs.Count > 1 Then
                                blnAddUpNode = True
                            End If
                    End Select
                End If
            End If
            'add goto Parent node
            If blnAddUpNode Then
                Dim objParentNode As DNNNode = New DNNNode
                objParentNode.ID = PortalSettings.ActiveTab.ParentId.ToString
                objParentNode.Key = objParentNode.ID
                objParentNode.Text = Services.Localization.Localization.GetString("Parent", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                objParentNode.ToolTip = Services.Localization.Localization.GetString("GoUp", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                objParentNode.CSSClass = NodeCssClass
                objParentNode.Image = ResolveUrl(TreeGoUpImage)
                objParentNode.ClickAction = eClickAction.PostBack
                objNodes.InsertBefore(0, objParentNode)
            End If

            Dim objPNode As DNNNode
            For Each objPNode In objNodes            'clean up to do in processnodes???
                ProcessNodes(objPNode)
            Next

            Me.Bind(objNodes)

            'technically this should always be a dnntree.  If using dynamic controls Nav.ascx should be used.  just being safe.
            If TypeOf Me.Control.NavigationControl Is DnnTree Then
                Dim objTree As DnnTree = CType(Me.Control.NavigationControl, DnnTree)
                If objTree.SelectedTreeNodes.Count > 0 Then
                    Dim objTNode As TreeNode = CType(objTree.SelectedTreeNodes(1), TreeNode)
                    If objTNode.DNNNodes.Count > 0 Then                'only expand it if nodes are not pending
                        objTNode.Expand()
                    End If
                End If
            End If


        End Sub

        Private Sub ProcessNodes(ByVal objParent As DNNNode)

            'If Boolean.Parse(GetValue(RootOnly, "False")) AndAlso objParent.HasNodes Then
            '	objParent.HasNodes = False
            'End If

            If Len(objParent.Image) > 0 Then
                'imagepath applied in provider...
            ElseIf objParent.HasNodes Then
                objParent.Image = ResolveUrl(NodeClosedImage)
            Else
                objParent.Image = ResolveUrl(Me.NodeLeafImage)
            End If

            Dim objNode As DNNNode
            For Each objNode In objParent.DNNNodes
                ProcessNodes(objNode)
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Sets common properties on DNNTree control
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]        12/8/2004   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InitializeTree()
            If Len(Me.PathImage) = 0 Then Me.PathImage = PortalSettings.HomeDirectory
            If Len(Me.PathSystemImage) = 0 Then Me.PathSystemImage = ResolveUrl("~/images/")
            'DNNTree.IndentWidth = TreeIndentWidth	'FIX!
            If Len(Me.IndicateChildImageRoot) = 0 Then Me.IndicateChildImageRoot = ResolveUrl(NodeExpandImage)
            If Len(Me.IndicateChildImageSub) = 0 Then Me.IndicateChildImageSub = ResolveUrl(NodeExpandImage)
            If Len(Me.IndicateChildImageExpandedRoot) = 0 Then Me.IndicateChildImageExpandedRoot = ResolveUrl(NodeCollapseImage)
            If Len(Me.IndicateChildImageExpandedSub) = 0 Then Me.IndicateChildImageExpandedSub = ResolveUrl(NodeCollapseImage)
            If Len(Me.CSSNode) = 0 Then Me.CSSNode = NodeChildCssClass '.DefaultChildNodeCssClass
            If Len(Me.CSSNodeRoot) = 0 Then Me.CSSNodeRoot = NodeCssClass 'DefaultNodeCssClass	???
            If Len(Me.CSSNodeHover) = 0 Then Me.CSSNodeHover = NodeOverCssClass 'DefaultNodeCssClassOver
            If Len(Me.CSSNodeSelectedRoot) = 0 Then Me.CSSNodeSelectedRoot = NodeSelectedCssClass 'DefaultNodeCssClassSelected
            If Len(Me.CSSNodeSelectedSub) = 0 Then Me.CSSNodeSelectedSub = NodeSelectedCssClass 'DefaultNodeCssClassSelected
            If Len(Me.CSSControl) = 0 Then Me.CSSControl = TreeCssClass 'CssClass

        End Sub


#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The Page_Load server event handler on this user control is used
        ''' to populate the tree with the Pages.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/9/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try

                If Page.IsPostBack = False Then
                    BuildTree(Nothing, False)

                    'Main Table Properties
                    If Me.Width <> "" Then
                        tblMain.Width = Me.Width
                    End If
                    If Me.CssClass <> "" Then
                        tblMain.Attributes.Add("class", Me.CssClass)
                    End If

                    'Header Properties
                    If Me.HeaderCssClass <> "" Then
                        cellHeader.Attributes.Add("class", Me.HeaderCssClass)
                    End If
                    If Me.HeaderTextCssClass <> "" Then
                        lblHeader.CssClass = Me.HeaderTextCssClass
                    End If

                    'Header Text (if set)
                    If Me.HeaderText <> "" Then
                        lblHeader.Text = Me.HeaderText
                    End If

                    'ResourceKey overrides if found
                    If Me.ResourceKey <> "" Then
                        Dim strHeader As String = Services.Localization.Localization.GetString(Me.ResourceKey, Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                        If strHeader <> "" Then
                            lblHeader.Text = Services.Localization.Localization.GetString(Me.ResourceKey, Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                        End If
                    End If

                    'If still not set get default key
                    If lblHeader.Text = "" Then
                        Dim strHeader As String = Services.Localization.Localization.GetString("Title", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                        If strHeader <> "" Then
                            lblHeader.Text = Services.Localization.Localization.GetString("Title", Services.Localization.Localization.GetResourceFile(Me, MyFileName))
                        Else
                            lblHeader.Text = "Site Navigation"
                        End If
                    End If
                    tblHeader.Visible = Me.IncludeHeader

                    'Main Panel Properties
                    If Me.BodyCssClass <> "" Then
                        cellBody.Attributes.Add("class", Me.BodyCssClass)
                    End If
                    cellBody.NoWrap = Me.NoWrap

                End If

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' The DNNTree_NodeClick server event handler on this user control runs when a
        ''' Node (Page) in the TreeView is clicked
        ''' </summary>
        ''' <remarks>The event only fires when the Node contains child nodes, as leaf nodes
        ''' have their NavigateUrl Property set.
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/9/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DNNTree_NodeClick(ByVal args As NavigationProvider.NavigationEventArgs)        'Handles DNNTree.NodeClick
            If args.Node Is Nothing Then
                args.Node = Navigation.GetNavigationNode(args.ID, Control.ID)
            End If
            Response.Redirect(ApplicationURL(Integer.Parse(args.Node.Key)), True)

        End Sub

        Private Sub DNNTree_PopulateOnDemand(ByVal args As NavigationProvider.NavigationEventArgs)      'Handles DnnTree.PopulateOnDemand
            If args.Node Is Nothing Then
                args.Node = Navigation.GetNavigationNode(args.ID, Control.ID)
            End If
            BuildTree(args.Node, True)
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            InitializeTree()
            InitializeNavControl(Me.cellBody, "DNNTreeNavigationProvider")
            AddHandler Control.NodeClick, AddressOf DNNTree_NodeClick
            AddHandler Control.PopulateOnDemand, AddressOf DNNTree_PopulateOnDemand

            MyBase.OnInit(e)
        End Sub

#End Region

    End Class

End Namespace
