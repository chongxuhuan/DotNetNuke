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
Imports System

Namespace DotNetNuke.Common

    ''' <summary>
    ''' This class handles basic elements about File Items. Is is a basic Get/Set for Value and Text
    ''' </summary>
    Public Class FileItem
        Private _Value As String
        Private _Text As String

        ''' <summary>
        ''' Initializes a new instance of the <see cref="FileItem" /> class.
        ''' </summary>
        ''' <param name="Value">The value.</param>
        ''' <param name="Text">The text.</param>
        Public Sub New(ByVal Value As String, ByVal Text As String)
            _Value = Value
            _Text = Text
        End Sub

        ''' <summary>
        ''' Gets the value.
        ''' </summary>
        ''' <value>The value.</value>
        Public ReadOnly Property Value() As String
            Get
                Return _Value
            End Get
        End Property

        ''' <summary>
        ''' Gets the text.
        ''' </summary>
        ''' <value>The text.</value>
        Public ReadOnly Property Text() As String
            Get
                Return _Text
            End Get
        End Property

    End Class

End Namespace
