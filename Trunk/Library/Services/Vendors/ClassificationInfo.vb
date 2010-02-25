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
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Services.Vendors

    <Serializable()> Public Class ClassificationInfo
        Private _ClassificationId As Integer
        Private _ClassificationName As String
        Private _ParentId As Integer
        Private _IsAssociated As Boolean

        Public Sub New()
        End Sub

        Public Property ClassificationId() As Integer
            Get
                Return _ClassificationId
            End Get
            Set(ByVal Value As Integer)
                _ClassificationId = Value
            End Set
        End Property
        Public Property ClassificationName() As String
            Get
                Return _ClassificationName
            End Get
            Set(ByVal Value As String)
                _ClassificationName = Value
            End Set
        End Property
        Public Property ParentId() As Integer
            Get
                Return _ParentId
            End Get
            Set(ByVal Value As Integer)
                _ParentId = Value
            End Set
        End Property
        Public Property IsAssociated() As Boolean
            Get
                Return _IsAssociated
            End Get
            Set(ByVal Value As Boolean)
                _IsAssociated = Value
            End Set
        End Property
    End Class

End Namespace

