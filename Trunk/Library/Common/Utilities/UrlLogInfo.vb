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

	Public Class UrlLogInfo

		' local property declarations
		Private _UrlLogID As Integer
		Private _UrlTrackingID As Integer
		Private _ClickDate As Date
		Private _UserID As Integer
		Private _FullName As String

		' initialization
		Public Sub New()
		End Sub

		' public properties
		Public Property UrlLogID() As Integer
			Get
				Return _UrlLogID
			End Get
			Set(ByVal Value As Integer)
				_UrlLogID = Value
			End Set
		End Property

		Public Property UrlTrackingID() As Integer
			Get
				Return _UrlTrackingID
			End Get
			Set(ByVal Value As Integer)
				_UrlTrackingID = Value
			End Set
		End Property

		Public Property ClickDate() As Date
			Get
				Return _ClickDate
			End Get
			Set(ByVal Value As Date)
				_ClickDate = Value
			End Set
		End Property

		Public Property UserID() As Integer
			Get
				Return _UserID
			End Get
			Set(ByVal Value As Integer)
				_UserID = Value
			End Set
		End Property

		Public Property FullName() As String
			Get
				Return _FullName
			End Get
			Set(ByVal Value As String)
				_FullName = Value
			End Set
		End Property

	End Class

End Namespace