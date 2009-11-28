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

Namespace DotNetNuke.ComponentModel

    Public Class TransientComponentBuilder
        Implements IComponentBuilder

        Private _Name As String
        Private _Type As Type

        ''' <summary>
        ''' Initializes a new instance of the TransientComponentBuilder class.
        ''' </summary>
        ''' <param name="name">The name of the component</param>
        ''' <param name="type">The type of the component</param>
        Public Sub New(ByVal name As String, ByVal type As Type)
            _Name = name
            _Type = type
        End Sub

        Public Function BuildComponent() As Object Implements IComponentBuilder.BuildComponent
            Return Reflection.CreateInstance(_Type)
        End Function

        Public ReadOnly Property Name() As String Implements IComponentBuilder.Name
            Get
                Return _Name
            End Get
        End Property

    End Class

End Namespace
