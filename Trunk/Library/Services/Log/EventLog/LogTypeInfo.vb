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
Imports System.Xml.Serialization

Namespace DotNetNuke.Services.Log.EventLog

    <Serializable()> Public Class LogTypeInfo
        Private _LogTypeKey As String
        Private _LogTypeFriendlyName As String
        Private _LogTypeDescription As String
        Private _LogTypeOwner As String
        Private _LogTypeCSSClass As String

        Public Property LogTypeKey() As String
            Get
                Return _LogTypeKey
            End Get
            Set(ByVal Value As String)
                _LogTypeKey = Value
            End Set
        End Property
        Public Property LogTypeFriendlyName() As String
            Get
                Return _LogTypeFriendlyName
            End Get
            Set(ByVal Value As String)
                _LogTypeFriendlyName = Value
            End Set
        End Property
        Public Property LogTypeDescription() As String
            Get
                Return _LogTypeDescription
            End Get
            Set(ByVal Value As String)
                _LogTypeDescription = Value
            End Set
        End Property
        Public Property LogTypeOwner() As String
            Get
                Return _LogTypeOwner
            End Get
            Set(ByVal Value As String)
                _LogTypeOwner = Value
            End Set
        End Property
        Public Property LogTypeCSSClass() As String
            Get
                Return _LogTypeCSSClass
            End Get
            Set(ByVal Value As String)
                _LogTypeCSSClass = Value
            End Set
        End Property
    End Class

End Namespace




