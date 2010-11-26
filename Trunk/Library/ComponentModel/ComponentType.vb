'
' DotNetNuke® - http://www.dotnetnuke.com
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

Imports System.Collections.ObjectModel
Imports DotNetNuke.Collections

Namespace DotNetNuke.ComponentModel

    Friend Class ComponentType

        Private _BaseType As Type
        Private _ComponentBuilders As New ComponentBuilderCollection

        Public ReadOnly Property BaseType() As Type
            Get
                Return _BaseType
            End Get
        End Property

        Public ReadOnly Property ComponentBuilders() As ComponentBuilderCollection
            Get
                Return _ComponentBuilders
            End Get
        End Property

        ''' <summary>
        ''' Initializes a new instance of the ComponentType class.
        ''' </summary>
        ''' <param name="baseType">The base type of Components of this ComponentType</param>
        Public Sub New(ByVal baseType As Type)
            _BaseType = baseType
        End Sub

    End Class

End Namespace
