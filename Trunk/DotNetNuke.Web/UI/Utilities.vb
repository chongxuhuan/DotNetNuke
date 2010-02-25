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
Imports System.Reflection
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.ControlPanels
Imports DotNetNuke.Entities.Portals

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
			_WindowWidth = windowWidth
			_WindowHeight = windowHeight
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

	Public Class Utilities

		Public Shared Sub RegisterAlertOnPageLoad(ByRef ctrl As Control, ByVal message As String)
			RegisterAlertOnPageLoad(ctrl, New MessageWindowParameters(message))
		End Sub

		Public Shared Sub RegisterAlertOnPageLoad(ByRef ctrl As Control, ByVal message As MessageWindowParameters)
			AddMessageWindow(ctrl)
			ctrl.Page.ClientScript.RegisterClientScriptBlock(ctrl.GetType(), ctrl.ID + "_AlertOnPageLoad", "function pageLoad() { " + GetClientAlert(ctrl, message) + " }", True)
		End Sub

		Public Shared Function GetClientAlert(ByRef ctrl As Control, ByVal message As String) As String
			Return GetClientAlert(ctrl, New MessageWindowParameters(message))
		End Function

		Public Shared Function GetClientAlert(ByRef ctrl As Control, ByVal message As MessageWindowParameters) As String
			AddMessageWindow(ctrl)
			'function(text, oWidth, oHeight, oTitle) 
			Return String.Format("radalert('{0}', '{1}', '{2}', '{3}');" _
			 , message.Message, message.WindowWidth, message.WindowHeight, message.Title)
		End Function

		Public Shared Function GetOnClientClickConfirm(ByRef ctrl As Control, ByVal message As String) As String
			Return GetOnClientClickConfirm(ctrl, New MessageWindowParameters(message))
		End Function

		Public Shared Function GetOnClientClickConfirm(ByRef ctrl As Control, ByVal message As MessageWindowParameters) As String
			AddMessageWindow(ctrl)
			'function(text, mozEvent, oWidth, oHeight, callerObj, oTitle) 
			Return String.Format("return postBackConfirm('{0}', event, '{1}', '{2}', '', '{3}');" _
			 , message.Message, message.WindowWidth, message.WindowHeight, message.Title)
		End Function

		Private Shared Sub AddMessageWindow(ByRef ctrl As Control)
			Dim msgCtrl As Control = ctrl.Page.FindControl("DnnWindowManager")
			If (Not IsNothing(msgCtrl)) Then
				ctrl.Page.ClientScript.RegisterClientScriptInclude("postBackConfirm", ctrl.ResolveUrl("~/js/dnn.postbackconfirm.js"))
				msgCtrl.Visible = True
			End If
		End Sub

		'todo
		'Public Function GetPromptMessage() As String
		'End Function

		Public Shared Sub ApplySkin(ByRef telerikControl As Control)
			ApplySkin(telerikControl, "", "", "")
		End Sub

		Public Shared Sub ApplySkin(ByRef telerikControl As Control, ByVal fallBackEmbeddedSkinName As String)
			ApplySkin(telerikControl, "", "", fallBackEmbeddedSkinName)
		End Sub

		Public Shared Sub ApplySkin(ByRef telerikControl As Control, ByVal fallBackEmbeddedSkinName As String, ByVal controlName As String)
			ApplySkin(telerikControl, "", controlName, fallBackEmbeddedSkinName)
		End Sub

		'Use selected skin's webcontrol skin if one exists
		'or use _default skin's webcontrol skin if one exists
		'or use embedded skin
		Public Shared Sub ApplySkin(ByRef telerikControl As Control, ByVal fallBackEmbeddedSkinName As String, ByVal controlName As String, ByVal webControlSkinSubFolderName As String)
			Dim skinProperty As PropertyInfo = Nothing
			Dim enableEmbeddedSkinsProperty As PropertyInfo = Nothing
			Dim skinApplied As Boolean = False

			Try
				skinProperty = telerikControl.GetType().GetProperty("Skin")
				enableEmbeddedSkinsProperty = telerikControl.GetType().GetProperty("EnableEmbeddedSkins")

				If (String.IsNullOrEmpty(controlName)) Then
					controlName = telerikControl.GetType().BaseType.Name
					If (controlName.StartsWith("Rad")) Then
						controlName = controlName.Substring(3)
					End If
				End If

				Dim skinVirtualFolder As String = PortalSettings.Current.ActiveTab.SkinPath.Replace("\"c, "/"c).Replace("//", "/")
				Dim skinName As String = ""

				If (skinVirtualFolder.EndsWith("/")) Then
					skinVirtualFolder = skinVirtualFolder.Substring(0, skinVirtualFolder.Length - 1)
				End If
				Dim lastIndex As Integer = skinVirtualFolder.LastIndexOf("/")
				If (lastIndex > -1 AndAlso skinVirtualFolder.Length > lastIndex) Then
					skinName = skinVirtualFolder.Substring(skinVirtualFolder.LastIndexOf("/") + 1)
				End If

				Dim systemWebControlSkin As String = String.Empty
				If (Not String.IsNullOrEmpty(skinName) AndAlso Not String.IsNullOrEmpty(skinVirtualFolder)) Then
					systemWebControlSkin = telerikControl.Page.Server.MapPath(skinVirtualFolder)
					systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, "WebControlSkin")
					systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, skinName)
					systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, webControlSkinSubFolderName)
					systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, String.Format("{0}.{1}.css", controlName, skinName))

					'Check if the selected skin has the webcontrol skin
					If (Not System.IO.File.Exists(systemWebControlSkin)) Then
						systemWebControlSkin = ""
					End If

					'No skin, try default folder
					If (systemWebControlSkin = "") Then
						skinVirtualFolder = telerikControl.ResolveUrl("~/Portals/_default/Skins/_default")
						skinName = "Default"

						If (skinVirtualFolder.EndsWith("/")) Then
							skinVirtualFolder = skinVirtualFolder.Substring(0, skinVirtualFolder.Length - 1)
						End If

						If (Not String.IsNullOrEmpty(skinName) AndAlso Not String.IsNullOrEmpty(skinVirtualFolder)) Then
							systemWebControlSkin = telerikControl.Page.Server.MapPath(skinVirtualFolder)
							systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, "WebControlSkin")
							systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, skinName)
							systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, webControlSkinSubFolderName)
							systemWebControlSkin = System.IO.Path.Combine(systemWebControlSkin, String.Format("{0}.{1}.css", controlName, skinName))

							If (Not System.IO.File.Exists(systemWebControlSkin)) Then
								systemWebControlSkin = ""
							End If
						End If
					End If
				End If

				If (systemWebControlSkin <> "") Then
					Dim cssLink As String = "<link href=""{0}"" rel=""stylesheet"" type=""text/css"" />"
					Dim cssVirtual As String = System.IO.Path.Combine(skinVirtualFolder, "WebControlSkin")
					cssVirtual = System.IO.Path.Combine(cssVirtual, skinName)
					cssVirtual = System.IO.Path.Combine(cssVirtual, webControlSkinSubFolderName)
					cssVirtual = System.IO.Path.Combine(cssVirtual, String.Format("{0}.{1}.css", controlName, skinName))
					cssLink = String.Format(cssLink, cssVirtual.Replace("\"c, "/"c).Replace("//", "/"))

					If (Not IsNothing(System.Web.HttpContext.Current)) Then
						If (Not System.Web.HttpContext.Current.Items.Contains(cssVirtual)) Then
							telerikControl.Page.Header.Controls.Add(New LiteralControl(cssLink))
							System.Web.HttpContext.Current.Items.Add(cssVirtual, "")
						End If
					End If

					If (Not IsNothing(skinProperty) AndAlso Not IsNothing(enableEmbeddedSkinsProperty)) Then
						skinApplied = True
						skinProperty.SetValue(telerikControl, skinName, Nothing)
						enableEmbeddedSkinsProperty.SetValue(telerikControl, False, Nothing)
					End If
				End If
			Catch ex As Exception
				DotNetNuke.Services.Exceptions.LogException(ex)
			End Try

			If (Not IsNothing(skinProperty) AndAlso Not IsNothing(enableEmbeddedSkinsProperty) AndAlso Not (skinApplied)) Then
				If (String.IsNullOrEmpty(fallBackEmbeddedSkinName)) Then
					fallBackEmbeddedSkinName = "Simple"
				End If

				'Set fall back skin Embedded Skin
				skinProperty.SetValue(telerikControl, fallBackEmbeddedSkinName, Nothing)
				enableEmbeddedSkinsProperty.SetValue(telerikControl, True, Nothing)
			End If
		End Sub

		Public Shared Function GetLocalizedString(ByVal key As String) As String
			Dim resourceFile As String = "/App_GlobalResources/WebControls.resx"
			Return Localization.GetString(key, resourceFile)
		End Function

		Public Shared Function GetLocalizedStringFromParent(ByVal key As String, ByVal control As Control) As String
			Dim returnValue As String = key
			Dim ctrl As Control = control.Parent

			While ctrl IsNot Nothing
				If TypeOf ctrl Is UserControl Then
					Dim resourceFileName As String = String.Format("{0}/{1}/{2}.ascx.resx", ctrl.TemplateSourceDirectory, Localization.LocalResourceDirectory, ctrl.GetType().BaseType().Name)
					If (System.IO.File.Exists(ctrl.Page.Server.MapPath(resourceFileName))) Then
						returnValue = Localization.GetString(key, resourceFileName)
						Exit While
					End If
				End If

				If TypeOf ctrl Is PortalModuleBase Then
					returnValue = Localization.GetString(key, DirectCast(ctrl, PortalModuleBase).LocalResourceFile)
					Exit While
				End If

				If TypeOf ctrl Is ControlPanelBase Then
					returnValue = Localization.GetString(key, DirectCast(ctrl, ControlPanelBase).LocalResourceFile)
					Exit While
				End If

				ctrl = ctrl.Parent
			End While

			Return returnValue
		End Function

	End Class

End Namespace

