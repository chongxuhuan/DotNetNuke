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

Imports DotNetNuke.Entities.Content.Taxonomy

Namespace DotNetNuke.Modules.Taxonomy.Views.Controls

    Partial Public Class EditTermControl
        Inherits UserControl

#Region "Private Members"

        Private _LocalResourceFile As String

#End Region

#Region "Public Properties"

        Public Property LocalResourceFile() As String
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal value As String)
                _LocalResourceFile = value
            End Set
        End Property

#End Region

#Region "Public Methods"

        Public Sub BindTerm(ByVal term As Term, ByVal terms As IEnumerable(Of Term), ByVal isHeirarchical As Boolean, ByVal loadFromControl As Boolean, ByVal editEnabled As Boolean)
            If loadFromControl Then
                term.Name = nameTextBox.Text
                term.Description = descriptionTextBox.Text
                If isHeirarchical AndAlso Not String.IsNullOrEmpty(parentTermCombo.SelectedValue) Then
                    term.ParentTermId = Int32.Parse(parentTermCombo.SelectedValue)
                End If
            Else
                nameTextBox.Text = term.Name
                descriptionTextBox.Text = term.Description

                'Remove this term (and its descendants) from the collection, so we don't get wierd heirarchies
                Dim termsList As List(Of Term) = (From t In terms _
                                                 Where Not (t.Left >= term.Left AndAlso t.Right <= term.Right) _
                                                 Select t) _
                                                 .ToList()

                parentTermCombo.DataSource = termsList
                parentTermCombo.DataBind()

                If term.ParentTermId.HasValue AndAlso parentTermCombo.FindItemByValue(term.ParentTermId.ToString()) IsNot Nothing Then
                    parentTermCombo.FindItemByValue(term.ParentTermId.ToString()).Selected = True
                End If

                parentTermRow.Visible = isHeirarchical AndAlso termsList.Count > 0
                nameTextBox.Enabled = editEnabled
                descriptionTextBox.Enabled = editEnabled
                parentTermCombo.Enabled = editEnabled
            End If
        End Sub

#End Region

    End Class

End Namespace
