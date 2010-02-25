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

    <Serializable()> Public Class BannerInfo
        Private _BannerId As Integer
        Private _VendorId As Integer
        Private _ImageFile As String
        Private _BannerName As String
        Private _URL As String
        Private _Impressions As Integer
        Private _CPM As Double
        Private _Views As Integer
        Private _ClickThroughs As Integer
        Private _StartDate As Date
        Private _EndDate As Date
        Private _CreatedByUser As String
        Private _CreatedDate As Date
        Private _BannerTypeId As Integer
        Private _Description As String
        Private _GroupName As String
        Private _Criteria As Integer
        Private _Width As Integer
        Private _Height As Integer

        Public Sub New()
        End Sub

        Public Property BannerId() As Integer
            Get
                Return _BannerId
            End Get
            Set(ByVal Value As Integer)
                _BannerId = Value
            End Set
        End Property
        Public Property VendorId() As Integer
            Get
                Return _VendorId
            End Get
            Set(ByVal Value As Integer)
                _VendorId = Value
            End Set
        End Property
        Public Property ImageFile() As String
            Get
                Return _ImageFile
            End Get
            Set(ByVal Value As String)
                _ImageFile = Value
            End Set
        End Property
        Public Property BannerName() As String
            Get
                Return _BannerName
            End Get
            Set(ByVal Value As String)
                _BannerName = Value
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
        Public Property Impressions() As Integer
            Get
                Return _Impressions
            End Get
            Set(ByVal Value As Integer)
                _Impressions = Value
            End Set
        End Property
        Public Property CPM() As Double
            Get
                Return _CPM
            End Get
            Set(ByVal Value As Double)
                _CPM = Value
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
        Public Property ClickThroughs() As Integer
            Get
                Return _ClickThroughs
            End Get
            Set(ByVal Value As Integer)
                _ClickThroughs = Value
            End Set
        End Property
        Public Property StartDate() As Date
            Get
                Return _StartDate
            End Get
            Set(ByVal Value As Date)
                _StartDate = Value
            End Set
        End Property
        Public Property EndDate() As Date
            Get
                Return _EndDate
            End Get
            Set(ByVal Value As Date)
                _EndDate = Value
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
        Public Property BannerTypeId() As Integer
            Get
                Return _BannerTypeId
            End Get
            Set(ByVal Value As Integer)
                _BannerTypeId = Value
            End Set
        End Property
        Public Property Description() As String
            Get
                Return _Description
            End Get
            Set(ByVal Value As String)
                _Description = Value
            End Set
        End Property
        Public Property GroupName() As String
            Get
                Return _GroupName
            End Get
            Set(ByVal Value As String)
                _GroupName = Value
            End Set
        End Property
        Public Property Criteria() As Integer
            Get
                Return _Criteria
            End Get
            Set(ByVal Value As Integer)
                _Criteria = Value
            End Set
        End Property
        Public Property Width() As Integer
            Get
                Return _Width
            End Get
            Set(ByVal Value As Integer)
                _Width = Value
            End Set
        End Property
        Public Property Height() As Integer
            Get
                Return _Height
            End Get
            Set(ByVal Value As Integer)
                _Height = Value
            End Set
        End Property
    End Class

End Namespace

