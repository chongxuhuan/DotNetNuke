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

Imports System.Web.UI

Imports DotNetNuke
Imports System.Collections.Generic
Imports DotNetNuke.Services.Exceptions
Imports DotNetNuke.Modules.Console.Components
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.Modules.Admin.Console

	''' -----------------------------------------------------------------------------
	''' <summary>
	''' The Settings class manages Module Settings
	''' </summary>
	''' <remarks>
	''' </remarks>
	''' <history>
	''' </history>
	''' -----------------------------------------------------------------------------
	Partial Class Settings
		Inherits Entities.Modules.ModuleSettingsBase

#Region "Base Method Implementations"

		Public Overrides Sub LoadSettings()
			Try
				If Page.IsPostBack = False Then

					Dim portalTabs As List(Of TabInfo) = TabController.GetPortalTabs(PortalId, DotNetNuke.Common.Utilities.Null.NullInteger, False, True)

					' Add host tabs
					If UserInfo IsNot Nothing AndAlso UserInfo.IsSuperUser Then
						Dim hostTabs As TabCollection = New TabController().GetTabsByPortal(Null.NullInteger)
						portalTabs.AddRange(hostTabs.Values)
					End If

					ParentTab.Items.Clear()
					For Each t As TabInfo In portalTabs
						If (Security.Permissions.TabPermissionController.CanViewPage(t)) Then
							ParentTab.Items.Add(New ListItem(t.IndentedTabName, t.TabID))
						End If
					Next
					ParentTab.Items.Insert(0, "")
					SelectDropDownListItem(ParentTab, "ParentTabID")

					For Each val As String In ConsoleController.GetSizeValues()
						DefaultSize.Items.Add(New ListItem(Localization.GetString(val, MyBase.LocalResourceFile), val))
					Next
					SelectDropDownListItem(DefaultSize, "DefaultSize")

					If Settings.ContainsKey("AllowSizeChange") Then
						AllowResize.Checked = CType(Settings("AllowSizeChange"), Boolean)
					End If

					For Each val As String In ConsoleController.GetViewValues()
						DefaultView.Items.Add(New ListItem(Localization.GetString(val, MyBase.LocalResourceFile), val))
					Next
					SelectDropDownListItem(DefaultView, "DefaultView")

					If Settings.ContainsKey("AllowViewChange") Then
						AllowViewChange.Checked = CType(Settings("AllowViewChange"), Boolean)
					End If

					If Settings.ContainsKey("ShowTooltip") Then
						ShowTooltip.Checked = CType(Settings("ShowTooltip"), Boolean)
					End If

					If Settings.ContainsKey("ConsoleWidth") Then
						ConsoleWidth.Text = CType(Settings("ConsoleWidth"), String)
					End If

				End If
			Catch exc As Exception			 'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Public Overrides Sub UpdateSettings()
			Try
				Dim objModules As New Entities.Modules.ModuleController

				'validate console width value
				Dim wdth As String = String.Empty
				If (ConsoleWidth.Text.Trim().Length > 0) Then
					Try
						wdth = Unit.Parse(ConsoleWidth.Text.Trim()).ToString()
					Catch ex As Exception
						Throw New Exception("ConsoleWidth value is invalid. Value must be numeric.")
					End Try
				End If

				If (ParentTab.SelectedValue = String.Empty) Then
					objModules.DeleteModuleSetting(ModuleId, "ParentTabID")
				Else
					objModules.UpdateModuleSetting(ModuleId, "ParentTabID", ParentTab.SelectedValue)
				End If

				objModules.UpdateModuleSetting(ModuleId, "DefaultSize", DefaultSize.SelectedValue)
				objModules.UpdateModuleSetting(ModuleId, "AllowSizeChange", AllowResize.Checked.ToString())
				objModules.UpdateModuleSetting(ModuleId, "DefaultView", DefaultView.SelectedValue)
				objModules.UpdateModuleSetting(ModuleId, "AllowViewChange", AllowViewChange.Checked.ToString())
				objModules.UpdateModuleSetting(ModuleId, "ShowTooltip", ShowTooltip.Checked.ToString())
				objModules.UpdateModuleSetting(ModuleId, "ConsoleWidth", wdth)

			Catch exc As Exception			 'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Private Sub SelectDropDownListItem(ByRef ddl As DropDownList, ByVal key As String)

			If Settings.ContainsKey(key) Then
				ddl.ClearSelection()

				Dim selItem As ListItem
				selItem = ddl.Items.FindByValue(CType(Settings(key), String))

				If Not selItem Is Nothing Then
					selItem.Selected = True
				End If
			End If

		End Sub

#End Region

	End Class

End Namespace

