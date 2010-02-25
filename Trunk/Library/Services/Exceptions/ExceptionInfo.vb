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

Imports System.Collections.Specialized
Imports System.Text
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Security.Permissions
Imports System.Web
Imports System.Reflection
Imports System.Diagnostics



Namespace DotNetNuke.Services.Exceptions

    <Serializable()> Public Class ExceptionInfo
        Private _Method As String
        Private _FileColumnNumber As Integer
        Private _FileName As String
        Private _FileLineNumber As Integer

        Public Property Method() As String
            Get
                Return _Method
            End Get
            Set(ByVal Value As String)
                _Method = Value
            End Set
        End Property
        Public Property FileColumnNumber() As Integer
            Get
                Return _FileColumnNumber
            End Get
            Set(ByVal Value As Integer)
                _FileColumnNumber = Value
            End Set
        End Property
        Public Property FileName() As String
            Get
                Return _FileName
            End Get
            Set(ByVal Value As String)
                _FileName = Value
            End Set
        End Property
        Public Property FileLineNumber() As Integer
            Get
                Return _FileLineNumber
            End Get
            Set(ByVal Value As Integer)
                _FileLineNumber = Value
            End Set
        End Property
    End Class
End Namespace
