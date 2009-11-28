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

Imports System
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.IO
Imports System.Xml

Imports DotNetNuke
Imports DNNControls = DotNetNuke.UI.WebControls
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Common.Lists

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manages Entry List
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[tamttt] 20/10/2004	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial  Class ListEditor
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private Enum eImageType
            Folder = 0
            Page = 1
        End Enum

#End Region

#Region "Protected Properties"

        Protected Property Mode() As String
            Get
                Return lstEntries.Mode
            End Get
            Set(ByVal value As String)
                lstEntries.Mode = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        Private Sub BindList(ByVal key As String)

            lstEntries.SelectedKey = key
            If PortalSettings.ActiveTab.IsSuperTab Then
                lstEntries.ListPortalID = Null.NullInteger
            Else
                lstEntries.ListPortalID = Me.PortalId
            End If
            lstEntries.ShowDelete = True
            lstEntries.DataBind()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Loads top level entry list into DNNTree
        ''' </summary>        
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt] 20/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindTree()
            Dim ctlLists As New ListController
            Dim colLists As ListInfoCollection = ctlLists.GetListInfoCollection()
            Dim indexLookup As New Hashtable

            DNNtree.TreeNodes.Clear()

            For Each list As ListInfo In colLists
                Dim node As New DNNControls.TreeNode(list.DisplayName)
                With node
                    .Key = list.Key
                    .ToolTip = list.EntryCount.ToString & " entries"
                    .ImageIndex = eImageType.Folder
                End With

                If list.Level = 0 Then
                    DNNtree.TreeNodes.Add(node)
                Else
                    If Not indexLookup.Item(list.ParentList) Is Nothing Then
                        Dim parentNode As DNNControls.TreeNode = CType(indexLookup.Item(list.ParentList), DNNControls.TreeNode)
                        parentNode.TreeNodes.Add(node)
                    End If
                End If

                ' Add index key here to find it later, should suggest with Joe to add it to DNNTree
                If indexLookup.Item(list.Key) Is Nothing Then
                    indexLookup.Add(list.Key, node)
                End If

            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Loads top level entry list
        ''' </summary>
        ''' <param name="ParentKey"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt] 20/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Function GetParentNode(ByVal [ParentKey] As String) As DNNControls.TreeNode
            Dim i As Integer
            For i = 0 To DNNtree.TreeNodes.Count - 1
                If DNNtree.TreeNodes(i).Key = ParentKey Then
                    Return DNNtree.TreeNodes(i)
                End If
            Next
            Return Nothing
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	02/05/2007  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'Set the List Entries Control Properties
            lstEntries.ID = "ListEntries"

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Page load, bind tree and enable controls
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt] 20/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal [sender] As System.Object, ByVal [e] As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    ' configure tree
                    DNNtree.ImageList.Add(ResolveUrl("~/images/folder.gif"))
                    DNNtree.ImageList.Add(ResolveUrl("~/images/file.gif"))
                    DNNtree.IndentWidth = 10
                    DNNtree.CollapsedNodeImage = ResolveUrl("~/images/max.gif")
                    DNNtree.ExpandedNodeImage = ResolveUrl("~/images/min.gif")

                    If Not Request.QueryString("Key") Is Nothing Then
                        Mode = "ListEntries"
                        BindList(Request.QueryString("Key"))
                    Else
                        Mode = "NoList"
                    End If
                    BindTree()
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_PreRender runs just prior to the control being rendered
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse] 01/29/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Mode = "NoList" Then
                dshDetails.Visible = False
                divDetails.Visible = False
            Else
                dshDetails.Visible = True
                divDetails.Visible = True
                dshDetails.Text = Localization.GetString(Mode, Me.LocalResourceFile)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Populate list entries based on value selected in DNNTree
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt] 20/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DNNTree_NodeClick(ByVal [source] As Object, ByVal [e] As DotNetNuke.UI.WebControls.DNNTreeNodeClickEventArgs) Handles DNNtree.NodeClick

            Mode = "ListEntries"
            BindList(e.Node.Key)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Handles Add New List command
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        '''     Using "CommandName" property of cmdSaveEntry to determine this is a new list
        ''' </remarks>
        ''' <history>
        '''     [tamttt] 20/10/2004	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdAddList_Click(ByVal [sender] As Object, ByVal [e] As System.EventArgs) Handles cmdAddList.Click

            Mode = "AddList"
            BindList("")

        End Sub

#End Region

    End Class

End Namespace
