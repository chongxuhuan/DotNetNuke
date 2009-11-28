Imports System
Imports System.Xml
Imports System.Collections.Generic
Imports System.Text

Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.Modules.Console.Components

	Public Class ConsoleController
		Implements IPortable

#Region "IPortable"

		Public Shared Function GetSizeValues() As IList(Of String)
			Dim returnValue As IList(Of String) = New List(Of String)
			returnValue.Add("IconFile")
			returnValue.Add("IconFileLarge")
			Return returnValue
		End Function

		Public Shared Function GetViewValues() As IList(Of String)
			Dim returnValue As IList(Of String) = New List(Of String)
			returnValue.Add("Hide")
			returnValue.Add("Show")
			Return returnValue
		End Function

		Dim _SettingKeys As String() = New String() { _
		 "ParentTabID", "DefaultSize", "AllowSizeChange", "DefaultView", "AllowViewChange", "ShowTooltip", "ConsoleWidth"}

		Public Function ExportModule(ByVal moduleID As Integer) As String Implements IPortable.ExportModule
			Dim moduleCtrl As New ModuleController
			Dim xmlStr As StringBuilder = New StringBuilder()
			xmlStr.Append("<ConsoleSettings>")

			Dim settings As Hashtable = moduleCtrl.GetModuleSettings(moduleID)

			If (Not settings Is Nothing) Then
				For Each key As String In _SettingKeys
					AddToXmlStr(xmlStr, settings, key)
				Next
			End If

			xmlStr.Append("</ConsoleSettings>")
			Return xmlStr.ToString()
		End Function

		Public Sub ImportModule(ByVal ModuleID As Integer, ByVal Content As String, ByVal Version As String, ByVal UserId As Integer) Implements IPortable.ImportModule
			Dim xmlSettings As XmlNode = GetContent(Content, "ConsoleSettings")

			Dim moduleCtrl As New ModuleController

			For Each key As String In _SettingKeys
				Dim node As XmlNode = xmlSettings.SelectSingleNode(key)
				Dim doUpdate As Boolean = True
				Dim value As String = String.Empty

				Try
					If (node Is Nothing) Then
						doUpdate = False
					Else
						value = node.InnerText

						Select Case key
							Case "ParentTabID"
								'does tab exist?
								Dim parentTabID As Integer = Integer.Parse(value)
								Dim tabInfo As TabInfo = New TabController().GetTab(parentTabID, PortalController.GetCurrentPortalSettings().PortalId, False)
								Exit Select
							Case "DefaultSize"
								doUpdate = GetSizeValues().Contains(value)
								Exit Select
							Case "DefaultView"
								doUpdate = GetViewValues().Contains(value)
								Exit Select
							Case "AllowSizeChange"
							Case "AllowViewChange"
							Case "ShowTooltip"
								Boolean.Parse(value)
								Exit Select
						End Select
					End If
				Catch ex As Exception
					LogException(New Exception("Unable to import value [" + key + "] for Console Module moduleid [" + ModuleID.ToString() + "]"))
					doUpdate = False
				End Try

				If (doUpdate) Then
					moduleCtrl.UpdateModuleSetting(ModuleID, key, value)
				End If
			Next
		End Sub

		Private Sub AddToXmlStr(ByRef xmlStr As StringBuilder, ByRef settings As Hashtable, ByVal key As String)
			If (settings.ContainsKey(key)) Then
				xmlStr.AppendFormat("<{0}>{1}</{0}>", key, settings(key).ToString())
			End If
		End Sub
#End Region

	End Class

End Namespace

