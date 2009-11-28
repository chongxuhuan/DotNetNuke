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

Imports DotNetNuke.Entities.Tabs
Imports DotNetNuke.UI.WebControls
Imports DotNetNuke.UI.Navigation

Namespace DotNetNuke.UI.Skins.Controls

	Partial  Class Nav
		Inherits UI.Skins.NavObjectBase

		' protected controls

#Region "Public Members"



#End Region


#Region " Web Form Designer Generated Code "


		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

		End Sub

		Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
			'CODEGEN: This method call is required by the Web Form Designer
			'Do not modify it using the code editor.
			InitializeComponent()
		End Sub

#End Region

		'*******************************************************
		'
		' The Page_Load server event handler on this page is used
		' to populate the role information for the page
		'
		'*******************************************************

		Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			Try
				Dim blnIndicateChildren As Boolean = Boolean.Parse(GetValue(Me.IndicateChildren, "True"))				 'This setting determines if the submenu arrows will be used
				'Dim blnRootOnly As Boolean = Boolean.Parse(GetValue(RootOnly, "False"))				'This setting determines if the submenu will be shown
				Dim strRightArrow As String
				Dim strDownArrow As String

				'If blnRootOnly Then blnIndicateChildren = False

				Dim objSkins As New UI.Skins.SkinController

				'image for right facing arrow
				If IndicateChildImageSub <> "" Then
					strRightArrow = IndicateChildImageSub
				Else
					'strRightArrow = "[APPIMAGEPATH]breadcrumb.gif"
					strRightArrow = "breadcrumb.gif"	'removed APPIMAGEPATH token - http://www.dotnetnuke.com/Community/ForumsDotNetNuke/tabid/795/forumid/76/threadid/85554/scope/posts/Default.aspx
				End If
				'image for down facing arrow
				If IndicateChildImageRoot <> "" Then
					strDownArrow = IndicateChildImageRoot
				Else
					'strDownArrow = "[APPIMAGEPATH]menu_down.gif"
					strDownArrow = "menu_down.gif"	'removed APPIMAGEPATH token - http://www.dotnetnuke.com/Community/ForumsDotNetNuke/tabid/795/forumid/76/threadid/85554/scope/posts/Default.aspx
				End If

				'Set correct image path for all separator images
				If SeparatorHTML <> "" Then
					SeparatorHTML = FixImagePath(SeparatorHTML)
				End If

				If SeparatorLeftHTML <> "" Then
					SeparatorLeftHTML = FixImagePath(SeparatorLeftHTML)
				End If
				If SeparatorRightHTML <> "" Then
					SeparatorRightHTML = FixImagePath(SeparatorRightHTML)
				End If
				If SeparatorLeftHTMLBreadCrumb <> "" Then
					SeparatorLeftHTMLBreadCrumb = FixImagePath(SeparatorLeftHTMLBreadCrumb)
				End If
				If SeparatorRightHTMLBreadCrumb <> "" Then
					SeparatorRightHTMLBreadCrumb = FixImagePath(SeparatorRightHTMLBreadCrumb)
				End If
				If SeparatorLeftHTMLActive <> "" Then
					SeparatorLeftHTMLActive = FixImagePath(SeparatorLeftHTMLActive)
				End If
				If SeparatorRightHTMLActive <> "" Then
					SeparatorRightHTMLActive = FixImagePath(SeparatorRightHTMLActive)
				End If

				If NodeLeftHTMLBreadCrumbRoot <> "" Then
					NodeLeftHTMLBreadCrumbRoot = FixImagePath(NodeLeftHTMLBreadCrumbRoot)
				End If
				If NodeRightHTMLBreadCrumbRoot <> "" Then
					NodeRightHTMLBreadCrumbRoot = FixImagePath(NodeRightHTMLBreadCrumbRoot)
				End If
				If NodeLeftHTMLBreadCrumbSub <> "" Then
					NodeLeftHTMLBreadCrumbSub = FixImagePath(NodeLeftHTMLBreadCrumbSub)
				End If
				If NodeRightHTMLBreadCrumbSub <> "" Then
					NodeRightHTMLBreadCrumbSub = FixImagePath(NodeRightHTMLBreadCrumbSub)
				End If
				If NodeLeftHTMLRoot <> "" Then
					NodeLeftHTMLRoot = FixImagePath(NodeLeftHTMLRoot)
				End If
				If NodeRightHTMLRoot <> "" Then
					NodeRightHTMLRoot = FixImagePath(NodeRightHTMLRoot)
				End If
				If NodeLeftHTMLSub <> "" Then
					NodeLeftHTMLSub = FixImagePath(NodeLeftHTMLSub)
				End If
				If NodeRightHTMLSub <> "" Then
					NodeRightHTMLSub = FixImagePath(NodeRightHTMLSub)
				End If

				If Len(Me.PathImage) = 0 Then
					Me.PathImage = PortalSettings.HomeDirectory
				End If
				If blnIndicateChildren Then
					Me.IndicateChildImageSub = strRightArrow
					'Me.IndicateChildren = True.ToString
					If Me.ControlOrientation.ToLower = "vertical" Then					 'NavigationProvider.NavigationProvider.Orientation.Vertical Then
						Me.IndicateChildImageRoot = strRightArrow
					Else
						Me.IndicateChildImageRoot = strDownArrow
					End If
				Else
					Me.IndicateChildImageSub = "[APPIMAGEPATH]spacer.gif"
				End If
				Me.PathSystemScript = Common.Globals.ApplicationPath & "/controls/SolpartMenu/"
				Me.PathSystemImage = "[APPIMAGEPATH]"

				BuildNodes(Nothing)

			Catch exc As Exception			 'Module failed to load
				ProcessModuleLoadException(Me, exc)
			End Try

		End Sub

		Private Function FixImagePath(ByVal strPath As String) As String
            If strPath.IndexOf("src=") <> -1 AndAlso strPath.IndexOf("src=""/") < 0 Then
                Return Replace(strPath, "src=""", "src=""[SKINPATH]")
            Else
                Return strPath
            End If
        End Function

		Private Sub BuildNodes(ByVal objNode As WebControls.DNNNode)
			Dim objNodes As DNNNodeCollection
			objNodes = GetNavigationNodes(objNode)
			Me.Control.ClearNodes()			 'since we always bind we need to clear the nodes for providers that maintain their state
			Me.Bind(objNodes)
		End Sub

		Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
			InitializeNavControl(Me, "SolpartMenuNavigationProvider")
			AddHandler Control.NodeClick, AddressOf Control_NodeClick
			AddHandler Control.PopulateOnDemand, AddressOf Control_PopulateOnDemand

			MyBase.OnInit(e)
		End Sub

		Private Sub Control_NodeClick(ByVal args As NavigationProvider.NavigationEventArgs)
			If args.Node Is Nothing Then
				args.Node = Navigation.GetNavigationNode(args.ID, Control.ID)
			End If
			Response.Redirect(ApplicationURL(Integer.Parse(args.Node.Key)), True)

		End Sub

		Private Sub Control_PopulateOnDemand(ByVal args As NavigationProvider.NavigationEventArgs)
			If args.Node Is Nothing Then
				args.Node = Navigation.GetNavigationNode(args.ID, Control.ID)
			End If
			BuildNodes(args.Node)
		End Sub

	End Class

End Namespace
