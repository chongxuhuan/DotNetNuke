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

Imports System.web.UI.Design
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.UI.WebControls.Design
    Public Class PropertyEditorControlDesigner
        Inherits ControlDesigner

        Public Overrides Function GetDesignTimeHtml() As String
            'TODO:  There is a bug here somewhere that results in a design-time rendering error when the control is re-rendered [jmb]
            Dim DesignTimeHtml As String = Nothing
            'Dim control As PropertyEditorControl = CType(Component, PropertyEditorControl)

            'Try
            '    If control.DataSource Is Nothing Then
            '        control.DataSource = New DefaultDesignerInfo
            '        control.DataBind()
            '    End If
            '    DesignTimeHtml = MyBase.GetDesignTimeHtml()
            'Catch ex As Exception
            '    DesignTimeHtml = GetErrorDesignTimeHtml(ex)
            'Finally
            '    If TypeOf control.DataSource Is DefaultDesignerInfo Then
            '        control.DataSource = Nothing
            '        control.DataBind()
            '    End If
            'End Try

            If DesignTimeHtml = Nothing Then
                DesignTimeHtml = GetEmptyDesignTimeHtml
            End If

            Return DesignTimeHtml
        End Function

        Public Overrides Sub Initialize(ByVal component As System.ComponentModel.IComponent)
            If Not TypeOf component Is PropertyEditorControl Then
                Throw New ArgumentException("Component must be of Type PropertyEditorControl", "component")
            End If
            MyBase.Initialize(component)
        End Sub
    End Class
End Namespace
