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

    <Serializable()> Public Class AffiliateInfo
        Private _AffiliateId As Integer
        Private _VendorId As Integer
        Private _StartDate As Date
        Private _EndDate As Date
        Private _CPC As Double
        Private _Clicks As Integer
        Private _CPCTotal As Double
        Private _CPA As Double
        Private _Acquisitions As Integer
        Private _CPATotal As Double

        Public Sub New()
        End Sub

        Public Property AffiliateId() As Integer
            Get
                Return _AffiliateId
            End Get
            Set(ByVal Value As Integer)
                _AffiliateId = Value
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
        Public Property CPC() As Double
            Get
                Return _CPC
            End Get
            Set(ByVal Value As Double)
                _CPC = Value
            End Set
        End Property
        Public Property Clicks() As Integer
            Get
                Return _Clicks
            End Get
            Set(ByVal Value As Integer)
                _Clicks = Value
            End Set
        End Property
        Public Property CPCTotal() As Double
            Get
                Return _CPCTotal
            End Get
            Set(ByVal Value As Double)
                _CPCTotal = Value
            End Set
        End Property
        Public Property CPA() As Double
            Get
                Return _CPA
            End Get
            Set(ByVal Value As Double)
                _CPA = Value
            End Set
        End Property
        Public Property Acquisitions() As Integer
            Get
                Return _Acquisitions
            End Get
            Set(ByVal Value As Integer)
                _Acquisitions = Value
            End Set
        End Property
        Public Property CPATotal() As Double
            Get
                Return _CPATotal
            End Get
            Set(ByVal Value As Double)
                _CPATotal = Value
            End Set
        End Property
    End Class

End Namespace

