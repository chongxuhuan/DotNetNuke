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

Imports System.IO
Imports System.Web
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Common
Imports DotNetNuke.Framework.Providers
Imports System.Web.UI.WebControls

Namespace DotNetNuke.NavigationControl
	Public Class DNNDropDownNavigationProvider
		Inherits DotNetNuke.Modules.NavigationProvider.NavigationProvider
		Private m_objDropDown As DropDownList
		Private m_strControlID As String

		Public ReadOnly Property DropDown() As DropDownList
			Get
				Return m_objDropDown
			End Get
		End Property

		Public Overrides ReadOnly Property NavigationControl() As System.Web.UI.Control
			Get
				Return DropDown
			End Get
		End Property

		Public Overrides Property ControlID() As String
			Get
				Return m_strControlID
			End Get
			Set(ByVal Value As String)
				m_strControlID = Value
			End Set
		End Property

		Public Overrides ReadOnly Property SupportsPopulateOnDemand() As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides Property CSSControl() As String
			Get
				Return DropDown.CssClass
			End Get
			Set(ByVal Value As String)
				DropDown.CssClass = Value
			End Set
		End Property

		Public Overrides Sub Initialize()
			m_objDropDown = New DropDownList
			DropDown.ID = m_strControlID
			AddHandler DropDown.SelectedIndexChanged, AddressOf DropDown_SelectedIndexChanged
		End Sub

		Public Overrides Sub Bind(ByVal objNodes As DotNetNuke.UI.WebControls.DNNNodeCollection)
			Dim objNode As DotNetNuke.UI.WebControls.DNNNode
            Dim strLevelPrefix As String

            For Each objNode In objNodes
                If objNode.Level = 0 Then
                    DropDown.Items.Clear()
                End If
                If objNode.ClickAction = UI.WebControls.eClickAction.PostBack Then
                    DropDown.AutoPostBack = True                   'its all or nothing...
                End If
                strLevelPrefix = Space(objNode.Level).Replace(" ", "_")
                If objNode.IsBreak Then
                    DropDown.Items.Add("-------------------")
                Else
                    DropDown.Items.Add(New ListItem(strLevelPrefix & objNode.Text, objNode.ID))
                End If
                Bind(objNode.DNNNodes)
            Next

		End Sub

		Private Sub DropDown_SelectedIndexChanged(ByVal source As Object, ByVal e As System.EventArgs)
			If DropDown.SelectedIndex > -1 Then
				MyBase.RaiseEvent_NodeClick(DropDown.SelectedItem.Value)
			End If
		End Sub

	End Class



End Namespace
