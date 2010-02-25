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

	Public Class ErrorContainer
		Inherits Control

		Private _Container As UI.Skins.Controls.ModuleMessage
		Public Property Container() As UI.Skins.Controls.ModuleMessage
			Get
				Return _Container
			End Get
			Set(ByVal Value As UI.Skins.Controls.ModuleMessage)
				_Container = Value
			End Set
		End Property

		Public Sub New(ByVal strError As String)
			Container = FormatException(strError)
		End Sub
		Public Sub New(ByVal strError As String, ByVal exc As Exception)
			Container = FormatException(strError, exc)
		End Sub
		Public Sub New(ByVal _PortalSettings As PortalSettings, ByVal strError As String, ByVal exc As Exception)
			Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
			If objUserInfo.IsSuperUser Then
				Container = FormatException(strError, exc)
			Else
				Container = FormatException(strError)
			End If
		End Sub
		Private Function FormatException(ByVal strError As String) As UI.Skins.Controls.ModuleMessage
			Dim m As UI.Skins.Controls.ModuleMessage
			m = UI.Skins.Skin.GetModuleMessageControl(Localization.Localization.GetString("ErrorOccurred"), strError, Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
			Return m
		End Function
		Private Function FormatException(ByVal strError As String, ByVal exc As Exception) As UI.Skins.Controls.ModuleMessage
			Dim m As UI.Skins.Controls.ModuleMessage
			If Not exc Is Nothing Then
				m = UI.Skins.Skin.GetModuleMessageControl(strError, exc.ToString, Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
			Else
				m = UI.Skins.Skin.GetModuleMessageControl(Localization.Localization.GetString("ErrorOccurred"), strError, Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
			End If
			Return m
		End Function
	End Class
End Namespace
