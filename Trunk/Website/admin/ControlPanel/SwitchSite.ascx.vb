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

Imports System.Collections.Generic

Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.Security.Permissions
Imports DotNetNuke.Entities.Portals.PortalSettings
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Entities.Modules.Definitions
Imports DotNetNuke.Application
Imports DotNetNuke.Entities.Host
Imports Telerik.Web.UI

Namespace DotNetNuke.UI.ControlPanel

	Partial Class SwitchSite
		Inherits System.Web.UI.UserControl
		Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool

#Region "Event Handlers"

		Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

		End Sub

		Protected Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
				If (Visible AndAlso Not IsPostBack()) Then
					LoadPortalsList()
				End If
			Catch exc As Exception
				ProcessModuleLoadException(Me, exc)
			End Try
		End Sub

		Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
		End Sub

		Protected Sub cmdSwitch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSwitch.Click
			Try
				If (Not String.IsNullOrEmpty(SitesLst.SelectedValue)) Then
					Dim selectedPortalID As Integer = Integer.Parse(SitesLst.SelectedValue)
					Dim portalAliasCtrl As New PortalAliasController()
					Dim portalAliases As ArrayList = portalAliasCtrl.GetPortalAliasArrayByPortalID(selectedPortalID)
					
					If (Not IsNothing(portalAliases) AndAlso portalAliases.Count > 0 AndAlso Not IsNothing(portalAliases(0))) Then
						Response.Redirect(DotNetNuke.Common.Globals.AddHTTP(portalAliases(0).HTTPAlias))
					End If
				End If
			Catch ex As Exception
				LogException(ex)
			End Try
		End Sub

#End Region

#Region "Properties"
		
		Public Property ToolName() As String Implements DotNetNuke.Web.UI.WebControls.IDnnRibbonBarTool.ToolName
			Get
				Return "QuickSwitchSite"
			End Get
			Set(ByVal value As String)
				Throw New NotSupportedException("Set ToolName not supported")
			End Set
		End Property

		Public Overrides Property Visible() As Boolean
			Get
				If (PortalSettings.Current.UserId = Null.NullInteger) Then
					Return False
				ElseIf (Not PortalSettings.Current.UserInfo.IsSuperUser) Then
					Return False
				End If
				Return MyBase.Visible
			End Get
			Set(ByVal value As Boolean)
				MyBase.Visible = value
			End Set
		End Property

#End Region

#Region "Methods"

		Private Sub LoadPortalsList()
			Dim portalCtrl As New PortalController()
			Dim portals As ArrayList = portalCtrl.GetPortals()

			SitesLst.ClearSelection()
			SitesLst.Items.Clear()

			SitesLst.DataSource = portals
			SitesLst.DataTextField = "PortalName"
			SitesLst.DataValueField = "PortalID"
			SitesLst.DataBind()

			SitesLst.Items.Insert(0, New RadComboBoxItem(""))
		End Sub

		Private ReadOnly Property LocalResourceFile() As String
			Get
				Return String.Format("{0}/{1}/{2}.ascx.resx", Me.TemplateSourceDirectory, Localization.LocalResourceDirectory, Me.GetType().BaseType().Name)
			End Get
		End Property

		Private Function GetString(ByVal key As String) As String
			Return Localization.GetString(key, LocalResourceFile)
		End Function

#End Region

	End Class

End Namespace


