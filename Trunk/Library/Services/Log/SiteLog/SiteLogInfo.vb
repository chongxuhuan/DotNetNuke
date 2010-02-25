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
Imports System.Threading

Namespace DotNetNuke.Services.Log.SiteLog

    <Serializable()> Public Class SiteLogInfo

        Private _DateTime As Date
        Private _PortalId As Integer
        Private _UserId As Integer
        Private _Referrer As String
        Private _URL As String
        Private _UserAgent As String
        Private _UserHostAddress As String
        Private _UserHostName As String
        Private _TabId As Integer
        Private _AffiliateId As Integer

        Public Sub New()
        End Sub

        Public Property DateTime() As Date
            Get
                Return _DateTime
            End Get
            Set(ByVal Value As Date)
                _DateTime = Value
            End Set
        End Property

        Public Property PortalId() As Integer
            Get
                Return _PortalId
            End Get
            Set(ByVal Value As Integer)
                _PortalId = Value
            End Set
        End Property

        Public Property UserId() As Integer
            Get
                Return _UserId
            End Get
            Set(ByVal Value As Integer)
                _UserId = Value
            End Set
        End Property

        Public Property Referrer() As String
            Get
                Return _Referrer
            End Get
            Set(ByVal Value As String)
                _Referrer = Value
            End Set
        End Property

        Public Property URL() As String
            Get
                Return _URL
            End Get
            Set(ByVal Value As String)
                _URL = Value
            End Set
        End Property

        Public Property UserAgent() As String
            Get
                Return _UserAgent
            End Get
            Set(ByVal Value As String)
                _UserAgent = Value
            End Set
        End Property

        Public Property UserHostAddress() As String
            Get
                Return _UserHostAddress
            End Get
            Set(ByVal Value As String)
                _UserHostAddress = Value
            End Set
        End Property

        Public Property UserHostName() As String
            Get
                Return _UserHostName
            End Get
            Set(ByVal Value As String)
                _UserHostName = Value
            End Set
        End Property

        Public Property TabId() As Integer
            Get
                Return _TabId
            End Get
            Set(ByVal Value As Integer)
                _TabId = Value
            End Set
        End Property

        Public Property AffiliateId() As Integer
            Get
                Return _AffiliateId
            End Get
            Set(ByVal Value As Integer)
                _AffiliateId = Value
            End Set
        End Property

    End Class

End Namespace
