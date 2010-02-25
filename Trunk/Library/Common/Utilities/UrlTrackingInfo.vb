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

Namespace DotNetNuke.Common.Utilities

	Public Class UrlTrackingInfo

		' local property declarations
		Private _UrlTrackingID As Integer
		Private _PortalID As Integer
		Private _Url As String
		Private _UrlType As String
		Private _Clicks As Integer
		Private _LastClick As Date
		Private _CreatedDate As Date
		Private _LogActivity As Boolean
		Private _TrackClicks As Boolean
		Private _ModuleID As Integer
        Private _NewWindow As Boolean

		' initialization
		Public Sub New()
		End Sub

		' public properties
		Public Property UrlTrackingID() As Integer
			Get
				Return _UrlTrackingID
			End Get
			Set(ByVal Value As Integer)
				_UrlTrackingID = Value
			End Set
		End Property

		Public Property PortalID() As Integer
			Get
				Return _PortalID
			End Get
			Set(ByVal Value As Integer)
				_PortalID = Value
			End Set
		End Property

		Public Property Url() As String
			Get
				Return _Url
			End Get
			Set(ByVal Value As String)
				_Url = Value
			End Set
		End Property

		Public Property UrlType() As String
			Get
				Return _UrlType
			End Get
			Set(ByVal Value As String)
				_UrlType = Value
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

		Public Property LastClick() As Date
			Get
				Return _LastClick
			End Get
			Set(ByVal Value As Date)
				_LastClick = Value
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

		Public Property LogActivity() As Boolean
			Get
				Return _LogActivity
			End Get
			Set(ByVal Value As Boolean)
				_LogActivity = Value
			End Set
		End Property

		Public Property TrackClicks() As Boolean
			Get
				Return _TrackClicks
			End Get
			Set(ByVal Value As Boolean)
				_TrackClicks = Value
			End Set
		End Property

		Public Property ModuleID() As Integer
			Get
				Return _ModuleID
			End Get
			Set(ByVal Value As Integer)
				_ModuleID = Value
			End Set
		End Property

        Public Property NewWindow() As Boolean
            Get
                Return _NewWindow
            End Get
            Set(ByVal Value As Boolean)
                _NewWindow = Value
            End Set
        End Property

    End Class

End Namespace