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

Imports System
Imports System.Web.UI.WebControls

Namespace DotNetNuke.Web.UI
    Public Class MessageWindowParameters

        Public Sub New(ByVal message As String)
            _Message = message
        End Sub

        Public Sub New(ByVal message As String, ByVal title As String)
            _Message = message
            _Title = title
        End Sub

        Public Sub New(ByVal message As String, ByVal title As String, ByVal windowWidth As String, ByVal windowHeight As String)
            _Message = message
            _Title = title
            _WindowWidth = Unit.Parse(windowWidth)
            _WindowHeight = Unit.Parse(windowHeight)
        End Sub

        Private _Message As String = ""
        Public Property Message() As String
            Get
                Return _Message
            End Get
            Set(ByVal value As String)
                'todo: javascript encode for onclick events
                _Message = value
                _Message = _Message.Replace("'", "\'")
                _Message = _Message.Replace("""", "\""")
            End Set
        End Property

        Private _Title As String = ""
        Public Property Title() As String
            Get
                Return _Title
            End Get
            Set(ByVal value As String)
                'todo: javascript encode for onclick events
                _Title = value
                _Title = _Title.Replace("'", "\'")
                _Title = _Title.Replace("""", "\""")
            End Set
        End Property

        Private _WindowWidth As Unit = Unit.Pixel(350)
        Public Property WindowWidth() As Unit
            Get
                Return _WindowWidth
            End Get
            Set(ByVal value As Unit)
                _WindowWidth = value
            End Set
        End Property

        Private _WindowHeight As Unit = Unit.Pixel(175)
        Public Property WindowHeight() As Unit
            Get
                Return _WindowHeight
            End Get
            Set(ByVal value As Unit)
                _WindowHeight = value
            End Set
        End Property

    End Class
End Namespace
