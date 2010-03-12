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
    Public Class DnnGridCheckBoxColumn
        Inherits Telerik.Web.UI.GridCheckBoxColumn
        Implements ILocalizable

#Region "Private Members"

        Private _Localize As Boolean = True
        Private _LocalResourceFile As String

#End Region

#Region "Public Methods"

        Public Overrides Sub InitializeCell(ByVal cell As System.Web.UI.WebControls.TableCell, ByVal columnIndex As Integer, ByVal inItem As Telerik.Web.UI.GridItem)
            MyBase.InitializeCell(cell, columnIndex, inItem)
            If TypeOf inItem Is GridHeaderItem Then
                cell.Text = Localization.GetString(String.Format("{0}.Header", Me.HeaderText), LocalResourceFile)
            End If
        End Sub

#End Region

#Region "ILocalizable Implementation"

        Public Property Localize() As Boolean Implements ILocalizable.Localize
            Get
                Return _Localize
            End Get
            Set(ByVal value As Boolean)
                _Localize = value
            End Set
        End Property

        Public Property LocalResourceFile() As String Implements ILocalizable.LocalResourceFile
            Get
                Return _LocalResourceFile
            End Get
            Set(ByVal value As String)
                _LocalResourceFile = value
            End Set
        End Property

        Protected Overridable Sub LocalizeStrings() Implements ILocalizable.LocalizeStrings
        End Sub

#End Region
    End Class
End Namespace
