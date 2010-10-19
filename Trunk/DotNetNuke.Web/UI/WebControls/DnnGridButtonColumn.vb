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

Imports Telerik.Web.UI
Imports DotNetNuke.Services.Localization

Namespace DotNetNuke.Web.UI.WebControls

    Public Class DnnGridButtonColumn
        Inherits Telerik.Web.UI.GridButtonColumn

#Region "Public Properties"

        Public ReadOnly Property LocalResourceFile As String
            Get
                Return Utilities.GetLocalResourceFile(Me.Owner.OwnerGrid.Parent)
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Overloads Overrides Function Clone() As GridColumn
            Dim dnnGridColumn As New DnnGridButtonColumn()

            'you should override CopyBaseProperties if you have some column specific properties
            dnnGridColumn.CopyBaseProperties(Me)

            Return dnnGridColumn
        End Function

        Public Overrides Sub InitializeCell(ByVal cell As System.Web.UI.WebControls.TableCell, ByVal columnIndex As Integer, ByVal inItem As Telerik.Web.UI.GridItem)
            MyBase.InitializeCell(cell, columnIndex, inItem)
            If TypeOf inItem Is GridHeaderItem Then
                cell.Text = Localization.GetString(String.Format("{0}.Header", Me.HeaderText), LocalResourceFile)
            End If
        End Sub

#End Region

    End Class

End Namespace
