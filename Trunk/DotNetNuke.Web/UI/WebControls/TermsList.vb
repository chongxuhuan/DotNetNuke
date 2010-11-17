'
' DotNetNuke - http://www.dotnetnuke.com
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

Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Common.Utilities
Imports DotNetNuke.Entities.Content.Taxonomy
Imports System.Web.UI.WebControls

Namespace DotNetNuke.Web.UI.WebControls

    Public Class TermsList
        Inherits WebControl

#Region "Private Members"

        Private _IsHeirarchical As Boolean

        Private _ListBox As DnnListBox
        Private _TreeView As DnnTreeView

#End Region

#Region "Events"

        Public Event SelectedTermChanged As EventHandler(Of TermsEventArgs)

#End Region

#Region "Public Properties"

        Public ReadOnly Property IsHeirarchical() As Boolean
            Get
                Return _IsHeirarchical
            End Get
        End Property

        Public ReadOnly Property SelectedTerm() As Term
            Get
                Dim _SelectedTerm As Term = Nothing
                If Not String.IsNullOrEmpty(Me.SelectedValue) Then
                    Dim _TermId As Integer = Integer.Parse(Me.SelectedValue)
                    For Each term As Term In Terms
                        If term.TermId = _TermId Then
                            _SelectedTerm = term
                            Exit For
                        End If
                    Next
                End If
                Return _SelectedTerm
            End Get
        End Property

        Public ReadOnly Property SelectedValue() As String
            Get
                Dim _SelectedValue As String = Null.NullString
                If IsHeirarchical Then
                    _SelectedValue = _TreeView.SelectedValue
                Else
                    _SelectedValue = _ListBox.SelectedValue
                End If
                Return _SelectedValue
            End Get
        End Property

        Public ReadOnly Property Terms() As List(Of Term)
            Get
                Dim _DataSource As Object = Nothing
                If IsHeirarchical Then
                    _DataSource = _TreeView.DataSource
                Else
                    _DataSource = _ListBox.DataSource
                End If
                Return TryCast(_DataSource, List(Of Term))
            End Get
        End Property

#End Region

#Region "Protected Methods"

        Protected Overrides Sub CreateChildControls()
            Controls.Clear()

            _ListBox = New DnnListBox
            _ListBox.ID = String.Concat(ID, "_List")
            _ListBox.DataTextField = "Name"
            _ListBox.DataValueField = "TermId"
            _ListBox.AutoPostBack = True
            AddHandler _ListBox.SelectedIndexChanged, AddressOf ListBoxSelectedIndexChanged

            _TreeView = New DnnTreeView
            _TreeView.ID = String.Concat(ID, "_Tree")
            _TreeView.DataTextField = "Name"
            _TreeView.DataValueField = "TermId"
            _TreeView.DataFieldID = "TermId"
            _TreeView.DataFieldParentID = "ParentTermId"
            AddHandler _TreeView.NodeClick, AddressOf TreeViewNodeClick

            Controls.Add(_ListBox)
            Controls.Add(_TreeView)
        End Sub

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            EnsureChildControls()
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            _ListBox.Visible = Not IsHeirarchical
            _TreeView.Visible = IsHeirarchical

            _ListBox.Height = Me.Height
            _ListBox.Width = Me.Width
            _TreeView.Height = Me.Height
            _TreeView.Width = Me.Width

            _TreeView.ExpandAllNodes()

            MyBase.OnPreRender(e)
        End Sub

        Protected Overridable Sub OnSelectedTermChanged(ByVal e As TermsEventArgs)
            'Raise the SelectedTermChanged Event
            RaiseEvent SelectedTermChanged(Me, e)
        End Sub

#End Region

#Region "Event Handlers"

        Private Sub ListBoxSelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            'Raise the SelectedTermChanged Event
            OnSelectedTermChanged(New TermsEventArgs(SelectedTerm))
        End Sub

        Private Sub TreeViewNodeClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadTreeNodeEventArgs)
            'Raise the SelectedTermChanged Event
            OnSelectedTermChanged(New TermsEventArgs(SelectedTerm))
        End Sub

#End Region

#Region "Public Methods"

        Public Sub BindTerms(ByVal terms As List(Of Term), ByVal isHeirarchical As Boolean, ByVal dataBind As Boolean)
            _IsHeirarchical = isHeirarchical

            _ListBox.DataSource = terms
            _TreeView.DataSource = terms

            If dataBind Then
                _ListBox.DataBind()
                _TreeView.DataBind()
            End If
        End Sub

        Public Sub ClearSelectedTerm()
            _ListBox.SelectedIndex = Null.NullInteger
            _TreeView.UnselectAllNodes()
        End Sub

#End Region

    End Class

End Namespace

