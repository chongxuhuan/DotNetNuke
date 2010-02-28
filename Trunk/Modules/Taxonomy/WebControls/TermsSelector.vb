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

Imports DotNetNuke.Entities.Content
Imports DotNetNuke.Web.UI.WebControls
Imports DotNetNuke.Entities.Content.Taxonomy
Imports DotNetNuke.Entities.Content.Common
Imports Telerik.Web.UI

Namespace DotNetNuke.Modules.Taxonomy.WebControls

    Public Class TermsSelector
        Inherits DnnComboBox

#Region "Public Properties"

        Public Property PortalId() As Integer
            Get
                Return Convert.ToInt32(ViewState("PortalId"))
            End Get
            Set(ByVal value As Integer)
                ViewState("PortalId") = value
            End Set
        End Property

        Public Property Terms() As List(Of Term)
            Get
                Return TryCast(ViewState("Terms"), List(Of Term))
            End Get
            Set(ByVal value As List(Of Term))
                ViewState("Terms") = value
            End Set
        End Property

#End Region

#Region "Protected Methods"

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Me.ItemTemplate = New TreeViewTemplate()
            Me.Items.Add(New RadComboBoxItem())
            MyBase.OnInit(e)
        End Sub

        Protected Overrides Sub OnPreRender(ByVal e As System.EventArgs)
            MyBase.OnPreRender(e)
            Me.Text = Terms.ToDelimittedString(", ")
            Me.ToolTip = Terms.ToDelimittedString(", ")
        End Sub

#End Region

#Region "Private Template Class"

        Class TreeViewTemplate
            Implements ITemplate

            Private _Container As RadComboBoxItem
            Private _TermsSelector As TermsSelector
            Private _Tree As DnnTreeView

            Private _Terms As List(Of Term)

            Private ReadOnly Property SelectedTerms() As List(Of Term)
                Get
                    Return _TermsSelector.Terms
                End Get
            End Property

            Private ReadOnly Property Terms() As List(Of Term)
                Get
                    If _Terms Is Nothing Then
                        Dim termRep As ITermController = DotNetNuke.Entities.Content.Common.Util.GetTermController()
                        Dim vocabRep As IVocabularyController = DotNetNuke.Entities.Content.Common.Util.GetVocabularyController()
                        _Terms = New List(Of Term)
                        Dim vocabularies As IQueryable(Of Vocabulary) = From v In vocabRep.GetVocabularies() _
                                                                        Where v.ScopeType.ScopeType = "Application" _
                                                                        OrElse (v.ScopeType.ScopeType = "Portal" _
                                                                                AndAlso v.ScopeId = PortalId)

                        For Each v As Vocabulary In vocabularies
                            'Add a dummy parent term if simple vocabulary
                            If v.Type = VocabularyType.Simple Then
                                Dim dummyTerm As New Term(v.VocabularyId)
                                dummyTerm.ParentTermId = Nothing
                                dummyTerm.Name = v.Name
                                dummyTerm.TermId = -v.VocabularyId
                                _Terms.Add(dummyTerm)
                            End If
                            For Each t As Term In termRep.GetTermsByVocabulary(v.VocabularyId)
                                If v.Type = VocabularyType.Simple Then
                                    t.ParentTermId = -v.VocabularyId
                                End If
                                _Terms.Add(t)
                            Next
                        Next
                    End If
                    Return _Terms
                End Get
            End Property

            Private ReadOnly Property PortalId() As Integer
                Get
                    Return _TermsSelector.PortalId
                End Get
            End Property

            Public Sub InstantiateIn(ByVal container As Control) Implements ITemplate.InstantiateIn
                _Container = CType(container, RadComboBoxItem)
                _TermsSelector = CType(container.Parent, TermsSelector)

                _Tree = New DnnTreeView
                _Tree.DataTextField = "Name"
                _Tree.DataValueField = "TermId"
                _Tree.DataFieldID = "TermId"
                _Tree.DataFieldParentID = "ParentTermId"
                _Tree.CheckBoxes = True
                _Tree.ExpandAllNodes()

                _Tree.DataSource = Terms

                AddHandler _Tree.NodeDataBound, AddressOf Me.TreeNodeDataBound
                AddHandler _Tree.NodeCheck, AddressOf Me.TreeNodeChecked
                AddHandler _Tree.DataBound, AddressOf Me.TreeDataBound

                _Container.Controls.Add(_Tree)
            End Sub

            Private Sub TreeDataBound(ByVal sender As Object, ByVal e As EventArgs)
                _Tree.ExpandAllNodes()
            End Sub

            Private Sub TreeNodeChecked(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)
                Dim node As RadTreeNode = e.Node
                Dim termId As Integer = Integer.Parse(node.Value)

                If node.Checked Then
                    'Add Term
                    For Each term In Terms
                        If term.TermId = termId Then
                            SelectedTerms.Add(term)
                            Exit For
                        End If
                    Next
                Else
                    'Remove Term
                    For Each term In SelectedTerms
                        If term.TermId = termId Then
                            SelectedTerms.Remove(term)
                            Exit For
                        End If
                    Next
                End If

                'Rebind
                _Tree.DataBind()
            End Sub

            Private Sub TreeNodeDataBound(ByVal sender As Object, ByVal e As RadTreeNodeEventArgs)

                Dim node As RadTreeNode = e.Node
                Dim term As Term = TryCast(node.DataItem, Term)

                If term.TermId < 0 Then
                    node.Checkable = False
                End If
                For Each tag In SelectedTerms
                    If tag.TermId = term.TermId Then
                        node.Checked = True
                        Exit For
                    End If
                Next

            End Sub

        End Class

#End Region

    End Class

End Namespace

