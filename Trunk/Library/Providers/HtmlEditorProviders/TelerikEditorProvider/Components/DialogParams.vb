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
Imports System

Namespace DotNetNuke.HtmlEditor.TelerikEditorProvider
    Public Class DialogParams

        Private _moduleId As Integer
        Public Property ModuleId() As Integer
            Get
                Return _moduleId
            End Get
            Set(ByVal value As Integer)
                _moduleId = value
            End Set
        End Property

        Private _linkUrl As String
        Public Property LinkUrl() As String
            Get
                Return _linkUrl
            End Get
            Set(ByVal value As String)
                _linkUrl = value
            End Set
        End Property

        Private _linkClickUrl As String
        Public Property LinkClickUrl() As String
            Get
                Return _linkClickUrl
            End Get
            Set(ByVal value As String)
                _linkClickUrl = value
            End Set
        End Property

        Private _track As Boolean
        Public Property Track() As Boolean
            Get
                Return _track
            End Get
            Set(ByVal value As Boolean)
                _track = value
            End Set
        End Property

        Private _trackUser As Boolean
        Public Property TrackUser() As Boolean
            Get
                Return _trackUser
            End Get
            Set(ByVal value As Boolean)
                _trackUser = value
            End Set
        End Property

    End Class
End Namespace
