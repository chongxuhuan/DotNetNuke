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

Imports System
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Services.Vendors

    <Serializable()> Public Class VendorInfo
        Private _VendorId As Integer
        Private _VendorName As String
        Private _Street As String
        Private _City As String
        Private _Region As String
        Private _Country As String
        Private _PostalCode As String
        Private _Telephone As String
        Private _PortalId As Integer
        Private _Fax As String
        Private _Cell As String
        Private _Email As String
        Private _Website As String
        Private _ClickThroughs As Integer
        Private _Views As Integer
        Private _CreatedByUser As String
        Private _CreatedDate As Date
        Private _LogoFile As String
        Private _KeyWords As String
        Private _Unit As String
        Private _Authorized As Boolean
        Private _FirstName As String
        Private _LastName As String
        Private _Banners As Integer
        Private _UserName As String

        Public Sub New()
        End Sub

        Public Property VendorId() As Integer
            Get
                Return _VendorId
            End Get
            Set(ByVal Value As Integer)
                _VendorId = Value
            End Set
        End Property
        Public Property VendorName() As String
            Get
                Return _VendorName
            End Get
            Set(ByVal Value As String)
                _VendorName = Value
            End Set
        End Property
        Public Property Street() As String
            Get
                Return _Street
            End Get
            Set(ByVal Value As String)
                _Street = Value
            End Set
        End Property
        Public Property City() As String
            Get
                Return _City
            End Get
            Set(ByVal Value As String)
                _City = Value
            End Set
        End Property
        Public Property Region() As String
            Get
                Return _Region
            End Get
            Set(ByVal Value As String)
                _Region = Value
            End Set
        End Property
        Public Property Country() As String
            Get
                Return _Country
            End Get
            Set(ByVal Value As String)
                _Country = Value
            End Set
        End Property
        Public Property PostalCode() As String
            Get
                Return _PostalCode
            End Get
            Set(ByVal Value As String)
                _PostalCode = Value
            End Set
        End Property
        Public Property Telephone() As String
            Get
                Return _Telephone
            End Get
            Set(ByVal Value As String)
                _Telephone = Value
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
        Public Property Fax() As String
            Get
                Return _Fax
            End Get
            Set(ByVal Value As String)
                _Fax = Value
            End Set
        End Property
        Public Property Cell() As String
            Get
                Return _Cell
            End Get
            Set(ByVal Value As String)
                _Cell = Value
            End Set
        End Property
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal Value As String)
                _Email = Value
            End Set
        End Property
        Public Property Website() As String
            Get
                Return _Website
            End Get
            Set(ByVal Value As String)
                _Website = Value
            End Set
        End Property
        Public Property ClickThroughs() As Integer
            Get
                Return _ClickThroughs
            End Get
            Set(ByVal Value As Integer)
                _ClickThroughs = Value
            End Set
        End Property
        Public Property Views() As Integer
            Get
                Return _Views
            End Get
            Set(ByVal Value As Integer)
                _Views = Value
            End Set
        End Property
        Public Property CreatedByUser() As String
            Get
                Return _CreatedByUser
            End Get
            Set(ByVal Value As String)
                _CreatedByUser = Value
            End Set
        End Property
        Public Property CreatedDate() As Date
            Get
                Return _CreatedDate
            End Get
            Set(ByVal Value As Date)
                _CreatedDate = Value
            End Set
        End Property
        Public Property LogoFile() As String
            Get
                Return _LogoFile
            End Get
            Set(ByVal Value As String)
                _LogoFile = Value
            End Set
        End Property
        Public Property KeyWords() As String
            Get
                Return _KeyWords
            End Get
            Set(ByVal Value As String)
                _KeyWords = Value
            End Set
        End Property
        Public Property Unit() As String
            Get
                Return _Unit
            End Get
            Set(ByVal Value As String)
                _Unit = Value
            End Set
        End Property
        Public Property Authorized() As Boolean
            Get
                Return _Authorized
            End Get
            Set(ByVal Value As Boolean)
                _Authorized = Value
            End Set
        End Property
        Public Property FirstName() As String
            Get
                Return _FirstName
            End Get
            Set(ByVal Value As String)
                _FirstName = Value
            End Set
        End Property
        Public Property LastName() As String
            Get
                Return _LastName
            End Get
            Set(ByVal Value As String)
                _LastName = Value
            End Set
        End Property
        Public Property Banners() As Integer
            Get
                Return _Banners
            End Get
            Set(ByVal Value As Integer)
                _Banners = Value
            End Set
        End Property
        Public Property UserName() As String
            Get
                Return _UserName
            End Get
            Set(ByVal Value As String)
                _UserName = Value
            End Set
        End Property
    End Class

End Namespace

