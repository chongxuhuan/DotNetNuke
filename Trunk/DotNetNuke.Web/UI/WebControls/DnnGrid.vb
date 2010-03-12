Imports Telerik.Web.UI

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

Namespace DotNetNuke.Web.UI.WebControls

    Public Class DnnGrid
        Inherits Telerik.Web.UI.RadGrid

        Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
            Utilities.ApplySkin(Me)
            PagerStyle.Mode = GridPagerMode.NumericPages
            MyBase.OnInit(e)
        End Sub

        Protected Overrides Sub OnDataBinding(ByVal e As System.EventArgs)
            MyBase.OnDataBinding(e)
            For Each col As GridColumn In Me.Columns
                Dim localizableColumn As ILocalizable = TryCast(col, ILocalizable)
                If localizableColumn IsNot Nothing Then
                    localizableColumn.LocalResourceFile = Utilities.GetLocalResourceFile(Me.Parent)
                    If Not Page.IsPostBack Then
                        localizableColumn.LocalizeStrings()
                    End If
                End If
            Next
        End Sub
    End Class

End Namespace
