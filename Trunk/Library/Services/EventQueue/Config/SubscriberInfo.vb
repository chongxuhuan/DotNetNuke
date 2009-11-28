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
Imports DotNetNuke.Security
Namespace DotNetNuke.Services.EventQueue.Config

    Public Class SubscriberInfo

        Private _id As String
        Private _name As String
        Private _description As String
        Private _address As String
        Private _privateKey As String
        Public Sub New()
            _id = System.Guid.NewGuid.ToString()
            _name = ""
            _description = ""
            _address = ""
            Dim oPortalSecurity As New PortalSecurity
            _privateKey = oPortalSecurity.CreateKey(16)
        End Sub
        Public Sub New(ByVal subscriberName As String)
            Me.New()
            _name = subscriberName
        End Sub
        Public Property ID() As String
            Get
                Return _id
            End Get
            Set(ByVal Value As String)
                _id = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal Value As String)
                _name = Value
            End Set
        End Property
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal Value As String)
                _description = Value
            End Set
        End Property
        Public Property Address() As String
            Get
                Return _address
            End Get
            Set(ByVal Value As String)
                _address = Value
            End Set
        End Property

        Public Property PrivateKey() As String
            Get
                Return _privateKey
            End Get
            Set(ByVal Value As String)
                _privateKey = Value
            End Set
        End Property

    End Class

End Namespace

