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
Imports System.Web
Imports System.Xml
Imports System.Collections
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Web.UI.WebControls

Imports DotNetNuke.Security
Imports DotNetNuke.Entities.Portals
Imports DotNetNuke.Entities.Tabs

Imports Telerik.Web.UI


Namespace DotNetNuke.UI.Skins.Controls

    ''' <summary>
	''' LeftMenu skinobject
    ''' </summary>
    ''' <remarks></remarks>
	Partial Class LeftMenu
		Inherits UI.Skins.SkinObjectBase

#Region "Private Variables"
		'variables and structures
		Private PagesQueue As Queue
		Private AuthPages As ArrayList
		Private arrayShowPath As ArrayList
		Private dnnSkinSrc As String = PortalSettings.ActiveTab.SkinSrc.Replace("\"c, "/"c).Replace("//", "/")
		Private dnnSkinPath As String = dnnSkinSrc.Substring(0, dnnSkinSrc.LastIndexOf("/"c))
		Private Structure qElement
			Dim radPanelItem As RadPanelItem
			Dim page As TabInfo
			Dim item As Integer
		End Structure

		Private _Style As System.String = String.Empty

		'panel item properties
		Private _ItemCssClass As System.String = String.Empty
		Private _ItemDisabledCssClass As System.String = String.Empty
		Private _ItemExpandedCssClass As System.String = String.Empty
		Private _ItemFocusedCssClass As System.String = String.Empty
		Private _ItemClickedCssClass As System.String = String.Empty
		Private _ItemImageUrl As System.String = String.Empty
		Private _ItemHoveredImageUrl As System.String = String.Empty
		Private _ItemHeight As System.Web.UI.WebControls.Unit = Unit.Empty
		Private _ItemWidth As System.Web.UI.WebControls.Unit = Unit.Empty

		'panel ROOT item properties
		Private _RootItemCssClass As System.String = String.Empty
		Private _RootItemDisabledCssClass As System.String = String.Empty
		Private _RootItemExpandedCssClass As System.String = String.Empty
		Private _RootItemFocusedCssClass As System.String = String.Empty
		Private _RootItemClickedCssClass As System.String = String.Empty
		Private _RootItemImageUrl As System.String = String.Empty
		Private _RootItemHoveredImageUrl As System.String = String.Empty
		Private _RootItemHeight As System.Web.UI.WebControls.Unit = Unit.Empty
		Private _RootItemWidth As System.Web.UI.WebControls.Unit = Unit.Empty

		Private _SelectedPathHeaderItemCss As String
		Private _SelectedPathItemCss As String
		Private _SelectedPathItemImage As String
		Private _SelectedPathHeaderItemImage As String

		'other properties
		Private _ShowPath As Boolean = True
		Private _EnableToolTips As Boolean = True
		Private _ImagesOnlyPanel As Boolean = False
		Private _EnableLevelCss As Boolean = False
		Private _EnableItemCss As Boolean = False
		Private _EnableRootItemCss As Boolean = False
		Private _MaxLevelNumber As Integer = 10
		Private _MaxItemNumber As Integer = 20
		Private _MaxLevel As Integer = -1
		Private _ShowOnlyCurrent As String = String.Empty
		Private _EnableItemId As Boolean = False
		Private _EnablePageIcons As Boolean = True
		Private _EnableUserMenus As Boolean = True
		Private _EnableAdminMenus As Boolean = True
		Private _CopyChildItemLink As Boolean = False
		Private _PagesToExclude As String = String.Empty

		Private _RadPanel1 As Telerik.Web.UI.RadPanelBar = Nothing
		Protected ReadOnly Property RadPanel1() As Telerik.Web.UI.RadPanelBar
			Get
				If (IsNothing(_RadPanel1)) Then
					_RadPanel1 = New Telerik.Web.UI.RadPanelBar()
					_RadPanel1.ID = "RadPanel1"
					LeftMenu1.Controls.Add(_RadPanel1)
				End If
				Return _RadPanel1
			End Get
		End Property

#End Region

#Region "Public Properties"
		Public Property AllowCollapseAllItems() As Boolean
			Get
				Return RadPanel1.AllowCollapseAllItems
			End Get
			Set(ByVal Value As Boolean)
				RadPanel1.AllowCollapseAllItems = Value
			End Set
		End Property
		Public Property ExpandAnimationType() As AnimationType
			Get
				Return RadPanel1.ExpandAnimation.Type
			End Get
			Set(ByVal Value As AnimationType)
				RadPanel1.ExpandAnimation.Type = Value
			End Set
		End Property
		Public Property ExpandAnimationDuration() As Integer
			Get
				Return RadPanel1.ExpandAnimation.Duration
			End Get
			Set(ByVal Value As Integer)
				RadPanel1.ExpandAnimation.Duration = Value
			End Set
		End Property
		Public Property ExpandDelay() As Integer
			Get
				Return RadPanel1.ExpandDelay
			End Get
			Set(ByVal Value As Integer)
				RadPanel1.ExpandDelay = Value
			End Set
		End Property
		Public Property CollapseAnimationType() As AnimationType
			Get
				Return RadPanel1.CollapseAnimation.Type
			End Get
			Set(ByVal Value As AnimationType)
				RadPanel1.CollapseAnimation.Type = Value
			End Set
		End Property
		Public Property CollapseAnimationDuration() As Integer
			Get
				Return RadPanel1.CollapseAnimation.Duration
			End Get
			Set(ByVal Value As Integer)
				RadPanel1.CollapseAnimation.Duration = Value
			End Set
		End Property
		Public Property CollapseDelay() As Integer
			Get
				Return RadPanel1.CollapseDelay
			End Get
			Set(ByVal Value As Integer)
				RadPanel1.CollapseDelay = Value
			End Set
		End Property
		Public Property ExpandMode() As PanelBarExpandMode
			Get
				Return RadPanel1.ExpandMode
			End Get
			Set(ByVal Value As PanelBarExpandMode)
				RadPanel1.ExpandMode = Value
			End Set
		End Property
		Private _Skin As String = String.Empty
		Public Property Skin() As System.String
			Get
				Return _Skin
			End Get
			Set(ByVal Value As System.String)
				_Skin = Value
			End Set
		End Property
		Public Property EnableEmbeddedSkins() As Boolean
			Get
				Return RadPanel1.EnableEmbeddedSkins
			End Get
			Set(ByVal Value As Boolean)
				RadPanel1.EnableEmbeddedSkins = Value
			End Set
		End Property
		Public Property EnableEmbeddedBaseStylesheet() As Boolean
			Get
				Return RadPanel1.EnableEmbeddedBaseStylesheet
			End Get
			Set(ByVal Value As Boolean)
				RadPanel1.EnableEmbeddedBaseStylesheet = Value
			End Set
		End Property
		Public Property EnableEmbeddedScripts() As Boolean
			Get
				Return RadPanel1.EnableEmbeddedScripts
			End Get
			Set(ByVal Value As Boolean)
				RadPanel1.EnableEmbeddedScripts = Value
			End Set
		End Property
		Public Property Dir() As String
			Get
				Return RadPanel1.Attributes("dir")
			End Get
			Set(ByVal Value As String)
				RadPanel1.Attributes("dir") = Value
			End Set
		End Property

		Public Property CausesValidation() As System.Boolean
			Get
				Return RadPanel1.CausesValidation
			End Get
			Set(ByVal Value As System.Boolean)
				RadPanel1.CausesValidation = Value
			End Set
		End Property
		Public Property OnClientContextMenu() As System.String
			Get
				Return RadPanel1.OnClientContextMenu
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientContextMenu = Value
			End Set
		End Property
		Public Property OnClientMouseOver() As System.String
			Get
				Return RadPanel1.OnClientMouseOver
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientMouseOver = Value
			End Set
		End Property
		Public Property OnClientMouseOut() As System.String
			Get
				Return RadPanel1.OnClientMouseOut
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientMouseOut = Value
			End Set
		End Property
		Public Property OnClientItemFocus() As System.String
			Get
				Return RadPanel1.OnClientItemFocus
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientItemFocus = Value
			End Set
		End Property
		Public Property OnClientItemBlur() As System.String
			Get
				Return RadPanel1.OnClientItemBlur
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientItemBlur = Value
			End Set
		End Property
		Public Property OnClientItemClicking() As System.String
			Get
				Return RadPanel1.OnClientItemClicking
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientItemClicking = Value
			End Set
		End Property
		Public Property OnClientItemClicked() As System.String
			Get
				Return RadPanel1.OnClientItemClicked
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientItemClicked = Value
			End Set
		End Property
		Public Property OnClientItemExpand() As System.String
			Get
				Return RadPanel1.OnClientItemExpand
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientItemExpand = Value
			End Set
		End Property
		Public Property OnClientItemCollapse() As System.String
			Get
				Return RadPanel1.OnClientItemCollapse
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientItemCollapse = Value
			End Set
		End Property
		Public Property OnClientLoad() As System.String
			Get
				Return RadPanel1.OnClientLoad
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.OnClientLoad = Value
			End Set
		End Property
		Public Property CssClass() As System.String
			Get
				Return RadPanel1.CssClass
			End Get
			Set(ByVal Value As System.String)
				RadPanel1.CssClass = Value
			End Set
		End Property
		Public Property Height() As System.Web.UI.WebControls.Unit
			Get
				Return RadPanel1.Height
			End Get
			Set(ByVal Value As System.Web.UI.WebControls.Unit)
				RadPanel1.Height = Value
			End Set
		End Property
		Public Property Width() As System.Web.UI.WebControls.Unit
			Get
				Return RadPanel1.Width
			End Get
			Set(ByVal Value As System.Web.UI.WebControls.Unit)
				RadPanel1.Width = Value
			End Set
		End Property
		Public Property PersistStateInCookie() As Boolean
			Get
				Return RadPanel1.PersistStateInCookie
			End Get
			Set(ByVal Value As Boolean)
				RadPanel1.PersistStateInCookie = Value
			End Set
		End Property
		Public Property CookieName() As String
			Get
				Return RadPanel1.CookieName
			End Get
			Set(ByVal Value As String)
				RadPanel1.CookieName = Value
			End Set
		End Property
		Public Property PagesToExclude() As String
			Get
				Return _PagesToExclude
			End Get
			Set(ByVal Value As String)
				_PagesToExclude = Value
			End Set
		End Property
		Public Property ItemCssClass() As System.String
			Get
				Return _ItemCssClass
			End Get
			Set(ByVal Value As System.String)
				_ItemCssClass = Value
			End Set
		End Property
		Public Property ItemDisabledCssClass() As System.String
			Get
				Return _ItemDisabledCssClass
			End Get
			Set(ByVal Value As System.String)
				_ItemDisabledCssClass = Value
			End Set
		End Property
		Public Property ItemExpandedCssClass() As System.String
			Get
				Return _ItemExpandedCssClass
			End Get
			Set(ByVal Value As System.String)
				_ItemExpandedCssClass = Value
			End Set
		End Property
		Public Property ItemFocusedCssClass() As System.String
			Get
				Return _ItemFocusedCssClass
			End Get
			Set(ByVal Value As System.String)
				_ItemFocusedCssClass = Value
			End Set
		End Property
		Public Property ItemClickedCssClass() As System.String
			Get
				Return _ItemClickedCssClass
			End Get
			Set(ByVal Value As System.String)
				_ItemClickedCssClass = Value
			End Set
		End Property
		Public Property ItemImageUrl() As System.String
			Get
				Return _ItemImageUrl
			End Get
			Set(ByVal Value As System.String)
				_ItemImageUrl = Value
			End Set
		End Property
		Public Property ItemHoveredImageUrl() As System.String
			Get
				Return _ItemHoveredImageUrl
			End Get
			Set(ByVal Value As System.String)
				_ItemHoveredImageUrl = Value
			End Set
		End Property
		Public Property ItemHeight() As System.Web.UI.WebControls.Unit
			Get
				Return _ItemHeight
			End Get
			Set(ByVal Value As System.Web.UI.WebControls.Unit)
				_ItemHeight = Value
			End Set
		End Property
		Public Property ItemWidth() As System.Web.UI.WebControls.Unit
			Get
				Return _ItemWidth
			End Get
			Set(ByVal Value As System.Web.UI.WebControls.Unit)
				_ItemWidth = Value
			End Set
		End Property
		Public Property RootItemCssClass() As System.String
			Get
				Return _RootItemCssClass
			End Get
			Set(ByVal Value As System.String)
				_RootItemCssClass = Value
			End Set
		End Property
		Public Property RootItemDisabledCssClass() As System.String
			Get
				Return _RootItemDisabledCssClass
			End Get
			Set(ByVal Value As System.String)
				_RootItemDisabledCssClass = Value
			End Set
		End Property
		Public Property RootItemExpandedCssClass() As System.String
			Get
				Return _RootItemExpandedCssClass
			End Get
			Set(ByVal Value As System.String)
				_RootItemExpandedCssClass = Value
			End Set
		End Property
		Public Property RootItemFocusedCssClass() As System.String
			Get
				Return _RootItemFocusedCssClass
			End Get
			Set(ByVal Value As System.String)
				_RootItemFocusedCssClass = Value
			End Set
		End Property
		Public Property RootItemClickedCssClass() As System.String
			Get
				Return _RootItemClickedCssClass
			End Get
			Set(ByVal Value As System.String)
				_RootItemClickedCssClass = Value
			End Set
		End Property
		Public Property RootItemImageUrl() As System.String
			Get
				Return _RootItemImageUrl
			End Get
			Set(ByVal Value As System.String)
				_RootItemImageUrl = Value
			End Set
		End Property
		Public Property RootItemHoveredImageUrl() As System.String
			Get
				Return _RootItemHoveredImageUrl
			End Get
			Set(ByVal Value As System.String)
				_RootItemHoveredImageUrl = Value
			End Set
		End Property
		Public Property RootItemHeight() As System.Web.UI.WebControls.Unit
			Get
				Return _RootItemHeight
			End Get
			Set(ByVal Value As System.Web.UI.WebControls.Unit)
				_RootItemHeight = Value
			End Set
		End Property
		Public Property RootItemWidth() As System.Web.UI.WebControls.Unit
			Get
				Return _RootItemWidth
			End Get
			Set(ByVal Value As System.Web.UI.WebControls.Unit)
				_RootItemWidth = Value
			End Set
		End Property
		Public Property SelectedPathHeaderItemCss() As String
			Get
				Return _SelectedPathHeaderItemCss
			End Get
			Set(ByVal Value As String)
				_SelectedPathHeaderItemCss = Value
			End Set
		End Property
		Public Property SelectedPathItemCss() As String
			Get
				Return _SelectedPathItemCss
			End Get
			Set(ByVal Value As String)
				_SelectedPathItemCss = Value
			End Set
		End Property
		Public Property SelectedPathItemImage() As String
			Get
				Return _SelectedPathItemImage
			End Get
			Set(ByVal Value As String)
				_SelectedPathItemImage = Value
			End Set
		End Property
		Public Property SelectedPathHeaderItemImage() As String
			Get
				Return _SelectedPathHeaderItemImage
			End Get
			Set(ByVal Value As String)
				_SelectedPathHeaderItemImage = Value
			End Set
		End Property
		Public Property EnableToolTips() As Boolean
			Get
				Return _EnableToolTips
			End Get
			Set(ByVal Value As Boolean)
				_EnableToolTips = Value
			End Set
		End Property
		Public Property ImagesOnlyPanel() As Boolean
			Get
				Return _ImagesOnlyPanel
			End Get
			Set(ByVal Value As Boolean)
				_ImagesOnlyPanel = Value
			End Set
		End Property
		Public Property EnableLevelCss() As Boolean
			Get
				Return _EnableLevelCss
			End Get
			Set(ByVal Value As Boolean)
				_EnableLevelCss = Value
			End Set
		End Property
		Public Property EnableItemCss() As Boolean
			Get
				Return _EnableItemCss
			End Get
			Set(ByVal Value As Boolean)
				_EnableItemCss = Value
			End Set
		End Property
		Public Property EnableRootItemCss() As Boolean
			Get
				Return _EnableRootItemCss
			End Get
			Set(ByVal Value As Boolean)
				_EnableRootItemCss = Value
			End Set
		End Property
		Public Property MaxLevelNumber() As Integer
			Get
				Return _MaxLevelNumber
			End Get
			Set(ByVal Value As Integer)
				_MaxLevelNumber = Value
			End Set
		End Property
		Public Property MaxItemNumber() As Integer
			Get
				Return _MaxItemNumber
			End Get
			Set(ByVal Value As Integer)
				_MaxItemNumber = Value
			End Set
		End Property
		Public Property MaxLevel() As Integer
			Get
				Return _MaxLevel
			End Get
			Set(ByVal Value As Integer)
				_MaxLevel = Value
			End Set
		End Property
		Public Property ShowOnlyCurrent() As String
			Get
				Return _ShowOnlyCurrent
			End Get
			Set(ByVal Value As String)
				_ShowOnlyCurrent = Value
			End Set
		End Property
		Public Property EnableItemId() As Boolean
			Get
				Return _EnableItemId
			End Get
			Set(ByVal Value As Boolean)
				_EnableItemId = Value
			End Set
		End Property
		Public Property EnablePageIcons() As Boolean
			Get
				Return _EnablePageIcons
			End Get
			Set(ByVal Value As Boolean)
				_EnablePageIcons = Value
			End Set
		End Property
		Public Property EnableUserMenus() As Boolean
			Get
				Return _EnableUserMenus
			End Get
			Set(ByVal Value As Boolean)
				_EnableUserMenus = Value
			End Set
		End Property
		Public Property EnableAdminMenus() As Boolean
			Get
				Return _EnableAdminMenus
			End Get
			Set(ByVal Value As Boolean)
				_EnableAdminMenus = Value
			End Set
		End Property
		Public Property CopyChildItemLink() As Boolean
			Get
				Return _CopyChildItemLink
			End Get
			Set(ByVal Value As Boolean)
				_CopyChildItemLink = Value
			End Set
		End Property
		Public Property ShowPath() As System.Boolean
			Get
				Return _ShowPath
			End Get
			Set(ByVal Value As System.Boolean)
				_ShowPath = Value
			End Set
		End Property
		Public Property Style() As System.String
			Get
				Return _Style
			End Get
			Set(ByVal value As System.String)
				_Style = value
			End Set
		End Property

#End Region

		Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
			ApplySkin()
		End Sub

		Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			Dim objTabController As New TabController
			Dim i, iItemIndex, iRootGroupId As Integer
			Dim temp As qElement
			Dim StartingItemId As Integer

			AuthPages = New ArrayList
			PagesQueue = New Queue
			arrayShowPath = New ArrayList
			iItemIndex = 0
			'---------------------------------------------------

            SetPanelbarProperties()

            If Not Page.IsPostBack Then
                'optional code to support displaying a specific branch of the page tree
                GetShowOnlyCurrent(objTabController, StartingItemId, iRootGroupId)
                'Fixed: For i = 0 To Me.PortalSettings.DesktopTabs.Count - 1
                Dim portalID As Integer = IIf(PortalSettings.ActiveTab.IsSuperTab, -1, PortalSettings.PortalId)
                Dim desktopTabs As IList(Of TabInfo) = TabController.GetTabsBySortOrder(portalID, PortalController.GetActivePortalLanguage(portalID), True)
                For i = 0 To desktopTabs.Count - 1
                    With CType(desktopTabs(i), TabInfo)
                        If (.TabID = Me.PortalSettings.ActiveTab.TabID) Then
                            FillShowPathArray(arrayShowPath, .TabID, objTabController)
                        End If
                        If (.IsVisible And Not .IsDeleted) And _
                         ((.StartDate = DateTime.MinValue And .EndDate = DateTime.MinValue) Or (.StartDate < Now And .EndDate > Now) Or AdminMode) And _
                         (Permissions.TabPermissionController.CanViewPage(desktopTabs(i)) And Not CheckToExclude(.TabName, .TabID)) Then
                            '(PortalSecurity.IsInRoles(.AuthorizedRoles) And Not CheckToExclude(.TabName, .TabID)) Then
                            temp = New qElement
                            temp.page = CType(desktopTabs(i), TabInfo)
                            temp.radPanelItem = New RadPanelItem
                            If CheckShowOnlyCurrent(.TabID, .ParentId, StartingItemId, iRootGroupId) AndAlso _
                               CheckPanelVisibility(CType(desktopTabs(i), TabInfo)) Then
                                iItemIndex = iItemIndex + 1
                                temp.item = iItemIndex
                                PagesQueue.Enqueue(AuthPages.Count)
                                RadPanel1.Items.Add(temp.radPanelItem)
                            End If
                            AuthPages.Add(temp)
                        End If
                    End With
                Next i
                BuildPanelbar(RadPanel1.Items)
                If (0 = RadPanel1.Items.Count) Then
                    RadPanel1.Visible = False
                End If
            End If



        End Sub


#Region "Private Helper Functions"
		Private Sub ApplySkin()
			If (EnableEmbeddedSkins = False AndAlso Not String.IsNullOrEmpty(Skin)) Then
				Dim cssLink As String = "<link href=""{0}/WebControlSkin/{1}/PanelBar.{1}.css"" rel=""stylesheet"" type=""text/css"" />"
				cssLink = String.Format(cssLink, dnnSkinPath, Skin)
				Page.Header.Controls.Add(New LiteralControl(cssLink))
				RadPanel1.Skin = Skin
			End If
		End Sub

		Private Function CheckToExclude(ByVal tabName As String, ByVal tabId As Int32) As Boolean
			If (PagesToExclude = String.Empty) Then
				Return False
			End If
			Dim temp As String() = PagesToExclude.Split(New Char() {","c})
			If (temp.Length = 0) Then
				Return False
			End If
			Dim item As String
			For Each item In temp
				Try
					If tabId = Int32.Parse(item.Trim()) Then
						Return True
					End If
				Catch
					If tabName = item.Trim() Then
						Return True
					End If
				End Try
			Next
			Return False
		End Function

		Private Sub GetShowOnlyCurrent(ByVal objTabController As TabController, ByRef StartingItemId As Integer, ByRef iRootGroupId As Integer)
			StartingItemId = 0
			iRootGroupId = 0
			'check if we have a value to work with
			If (ShowOnlyCurrent = String.Empty) Then
				Exit Sub
			End If
			'check if user specified an ID
			If (Char.IsDigit(ShowOnlyCurrent.Chars(0))) Then
				Try
					StartingItemId = Integer.Parse(ShowOnlyCurrent)
				Catch ex As Exception
				End Try
			End If
			'check if user specified a page name
			If (ShowOnlyCurrent.StartsWith("PageItem:")) Then
				Dim temptab As TabInfo = objTabController.GetTabByName(ShowOnlyCurrent.Substring(("PageItem:").Length), PortalSettings.PortalId)
				If (Not temptab Is Nothing) Then
					StartingItemId = temptab.TabID
				End If
			End If
			'RootItem
			If ("RootItem" = ShowOnlyCurrent) Then
				iRootGroupId = Me.PortalSettings.ActiveTab.TabID()
				While (CType(objTabController.GetTab(iRootGroupId, PortalSettings.PortalId, True), TabInfo).ParentId <> -1)
					iRootGroupId = CType(objTabController.GetTab(iRootGroupId, PortalSettings.PortalId, True), TabInfo).ParentId
				End While
			End If
		End Sub
		Private Function CheckShowOnlyCurrent(ByVal tabId As Integer, ByVal parentId As Integer, ByVal StartingItemId As Integer, ByVal iRootGroupId As Integer) As Boolean
			If _
			 (String.Empty = ShowOnlyCurrent AndAlso parentId = -1) OrElse _
			 ("ChildItems" = ShowOnlyCurrent AndAlso parentId = Me.PortalSettings.ActiveTab.TabID) OrElse _
			 ("CurrentItem" = ShowOnlyCurrent AndAlso tabId = Me.PortalSettings.ActiveTab.TabID) OrElse _
			 ("RootItem" = ShowOnlyCurrent AndAlso iRootGroupId = parentId) OrElse _
			 (StartingItemId > 0 AndAlso parentId = StartingItemId) _
			Then
				Return True
			Else
				Return False
			End If
		End Function
		Private Function CheckPanelVisibility(ByVal tab As TabInfo) As Boolean
			'Fixed: If (Not EnableAdminMenus AndAlso (tab.IsAdminTab Or tab.IsSuperTab)) Then
			If (Not EnableAdminMenus AndAlso (tab.IsSuperTab)) Then
				Return False
			End If
			'Fixed: If (Not EnableUserMenus AndAlso Not (tab.IsAdminTab Or tab.IsSuperTab)) Then
			If (Not EnableUserMenus AndAlso Not (tab.IsSuperTab)) Then
				Return False
			End If
			Return True
		End Function
		Private Sub FillShowPathArray(ByRef arrayShowPath As ArrayList, ByVal selectedTabID As Integer, ByVal objTabController As TabController)
			Dim tempTab As TabInfo = objTabController.GetTab(selectedTabID, PortalSettings.PortalId, True)
			While (tempTab.ParentId <> -1)
				arrayShowPath.Add(tempTab.TabID)
				tempTab = objTabController.GetTab(tempTab.ParentId, PortalSettings.PortalId, True)
			End While
			arrayShowPath.Add(tempTab.TabID)
		End Sub
		Private Sub CheckShowPath(ByVal tabId As Integer, ByVal panelItemToCheck As RadPanelItem, ByVal pageName As String)
			If (arrayShowPath(0) = tabId) Then
				panelItemToCheck.Expanded = True
			End If
			If (arrayShowPath.Contains(tabId)) Then
				If (panelItemToCheck.Level > 0) Then
					panelItemToCheck.Selected = True
					Dim parent As RadPanelItem = CType(panelItemToCheck.Owner, RadPanelItem)
					While (Not parent Is Nothing AndAlso parent.Items.Count > 0)
						Try
							parent.Expanded = True
							parent = CType(parent.Owner, RadPanelItem)
						Catch
							parent = Nothing
						End Try
					End While
					If SelectedPathItemCss <> String.Empty Then
						panelItemToCheck.CssClass = SelectedPathItemCss
					End If
					If (SelectedPathItemImage <> String.Empty) Then
						panelItemToCheck.ImageUrl = SelectedPathItemImage.Replace("*SkinPath*", dnnSkinPath).Replace("*PageName*", pageName)
					End If
				Else
					If (SelectedPathHeaderItemCss <> String.Empty) Then
						panelItemToCheck.CssClass = SelectedPathHeaderItemCss
					End If
					If (SelectedPathHeaderItemImage <> String.Empty) Then
						panelItemToCheck.ImageUrl = SelectedPathHeaderItemImage.Replace("*SkinPath*", dnnSkinPath).Replace("*PageName*", pageName)
					End If
				End If
			End If
		End Sub
		Private Sub SetPanelbarProperties()
			If (Style <> String.Empty) Then
				Style += "; "
				Try
					For Each cStyle As String In Style.Split(";"c)
						If (cStyle.Trim.Length > 0) Then
							RadPanel1.Style.Add(cStyle.Split(":"c)(0), cStyle.Split(":"c)(1))
						End If
					Next
				Catch
				End Try
			End If
		End Sub
		Private Sub SetItemProperties(ByVal currentPanelItem As RadPanelItem, ByVal iLevel As Integer, ByVal iItem As Integer, ByVal pageName As String)
			Dim sLevel As String = CType(IIf(EnableLevelCss And iLevel < MaxLevelNumber, "Level" + iLevel.ToString, String.Empty), String)
			Dim sItem As String = CType(IIf(iItem <= MaxItemNumber And ((EnableItemCss And iLevel > 0) Or (EnableRootItemCss And iLevel = 0)), iItem.ToString, String.Empty), String)

			If (ItemCssClass <> String.Empty) Then
				currentPanelItem.CssClass = sLevel & ItemCssClass & sItem
			End If

			If (ItemDisabledCssClass <> String.Empty) Then
				currentPanelItem.DisabledCssClass = sLevel & ItemDisabledCssClass & sItem
			End If

			If (ItemExpandedCssClass <> String.Empty) Then
				currentPanelItem.ExpandedCssClass = sLevel & ItemExpandedCssClass & sItem
			End If

			If (ItemFocusedCssClass <> String.Empty) Then
				currentPanelItem.FocusedCssClass = sLevel & ItemFocusedCssClass & sItem
			End If

			If (ItemClickedCssClass <> String.Empty) Then
				currentPanelItem.ClickedCssClass = sLevel & ItemClickedCssClass & sItem
			End If

			If (ItemImageUrl <> String.Empty) Then
				currentPanelItem.ImageUrl = ItemImageUrl.Replace("*SkinPath*", dnnSkinPath).Replace("*PageName*", pageName)
			End If

			If (ItemHoveredImageUrl <> String.Empty) Then
				currentPanelItem.HoveredImageUrl = ItemHoveredImageUrl.Replace("*SkinPath*", dnnSkinPath).Replace("*PageName*", pageName)
			End If

			If (Not ItemHeight.IsEmpty) Then
				currentPanelItem.Height = ItemHeight
			End If

			If (Not ItemWidth.IsEmpty) Then
				currentPanelItem.Width = ItemWidth
			End If
		End Sub
		Private Sub SetRootItemProperties(ByVal currentPanelItem As RadPanelItem, ByVal iLevel As Integer, ByVal iItem As Integer, ByVal pageName As String)
			Dim sLevel As String = CType(IIf(EnableLevelCss And iLevel < MaxLevelNumber, "Level" + iLevel.ToString, String.Empty), String)
			Dim sItem As String = CType(IIf(iItem <= MaxItemNumber And ((EnableItemCss And iLevel > 0) Or (EnableRootItemCss And iLevel = 0)), iItem.ToString, String.Empty), String)

			If (RootItemCssClass <> String.Empty) Then
				currentPanelItem.CssClass = sLevel & RootItemCssClass & sItem
			End If

			If (RootItemDisabledCssClass <> String.Empty) Then
				currentPanelItem.DisabledCssClass = sLevel & RootItemDisabledCssClass & sItem
			End If

			If (RootItemExpandedCssClass <> String.Empty) Then
				currentPanelItem.ExpandedCssClass = sLevel & RootItemExpandedCssClass & sItem
			End If

			If (RootItemFocusedCssClass <> String.Empty) Then
				currentPanelItem.FocusedCssClass = sLevel & RootItemFocusedCssClass & sItem
			End If

			If (RootItemClickedCssClass <> String.Empty) Then
				currentPanelItem.ClickedCssClass = sLevel & RootItemClickedCssClass & sItem
			End If

			If (RootItemImageUrl <> String.Empty) Then
				currentPanelItem.ImageUrl = RootItemImageUrl.Replace("*SkinPath*", dnnSkinPath).Replace("*PageName*", pageName)
			End If

			If (RootItemHoveredImageUrl <> String.Empty) Then
				currentPanelItem.HoveredImageUrl = RootItemHoveredImageUrl.Replace("*SkinPath*", dnnSkinPath).Replace("*PageName*", pageName)
			End If

			If (Not RootItemHeight.IsEmpty) Then
				currentPanelItem.Height = RootItemHeight
			End If

			If (Not RootItemWidth.IsEmpty) Then
				currentPanelItem.Width = RootItemWidth
			End If

		End Sub
		Private Sub BuildPanelbar(ByVal rootCollection As RadPanelItemCollection)
			Dim temp, temp2 As qElement
			Dim page As TabInfo
			Dim pageID, j, iItemIndex As Integer

			While Not (PagesQueue.Count = 0)
				pageID = CType(PagesQueue.Dequeue(), Integer)
				temp = CType(AuthPages(pageID), qElement)
				page = CType(temp.page, TabInfo)

				temp.radPanelItem.Text = page.TabName

				If (page.IconFile <> "" And EnablePageIcons) Then
					'Fixed: If (DotNetNuke.Common.glbAppVersion.StartsWith("05")) Then
					'If (DotNetNuke.Application.DotNetNukeContext.Current.Application.Version.ToString().StartsWith("05")) Then
					If (page.IconFile.StartsWith("~")) Then
						temp.radPanelItem.ImageUrl = Me.Page.ResolveUrl(page.IconFile)
					Else
						temp.radPanelItem.ImageUrl = PortalSettings.HomeDirectory & page.IconFile
					End If
					'Else
					'Fixed: If (page.IsAdminTab Or page.IsSuperTab) Then
					'If (page.IsSuperTab) Then
					'temp.radPanelItem.ImageUrl = CType(IIf(Me.Request.ApplicationPath <> "/", Me.Request.ApplicationPath.ToUpper, String.Empty), String) & "/images/" & page.IconFile
					'Else
					'temp.radPanelItem.ImageUrl = PortalSettings.HomeDirectory & page.IconFile
					'End If
					'End If
				End If

				If (Not page.DisableLink) Then
					If (page.FullUrl.StartsWith("*") And page.FullUrl.IndexOf("*", 1) <> -1) Then
						temp.radPanelItem.NavigateUrl = page.FullUrl.Substring(page.FullUrl.IndexOf("*", 1) + 1)
						temp.radPanelItem.Target = page.FullUrl.Substring(1, page.FullUrl.IndexOf("*", 1) - 1)
					Else
						temp.radPanelItem.NavigateUrl = page.FullUrl
					End If
				ElseIf (CopyChildItemLink AndAlso page.Level >= MaxLevel) Then
					j = 0
					'check if there are any child items and use a href from the first one
					While (j < AuthPages.Count AndAlso _
					  (CType(AuthPages(j), qElement).page.ParentId <> page.TabID OrElse _
					  CType(AuthPages(j), qElement).page.DisableLink))
						j = j + 1
					End While
					If (j < AuthPages.Count) Then
						' child item found. use its link
						temp.radPanelItem.NavigateUrl = CType(AuthPages(j), qElement).page.FullUrl
					End If
				End If
				If (EnableToolTips And page.Description <> "") Then
					temp.radPanelItem.ToolTip = page.Description
				End If

				'set all other item properties
				If (temp.radPanelItem.Level = 0) Then
					SetRootItemProperties(temp.radPanelItem, page.Level, temp.item, page.TabName)
				Else
					SetItemProperties(temp.radPanelItem, page.Level, temp.item, page.TabName)
				End If

				'check showpath
				If (ShowPath) Then
					CheckShowPath(page.TabID, temp.radPanelItem, page.TabName)
				End If

				'image-only panel check
				If (ImagesOnlyPanel And temp.radPanelItem.ImageUrl <> String.Empty) Then
					temp.radPanelItem.Text = String.Empty
				End If

				'attach child items the current one
				If (page.Level < MaxLevel Or MaxLevel < 0) Then
					iItemIndex = 0
					For j = 0 To AuthPages.Count - 1
						temp2 = CType(AuthPages(j), qElement)
						If (temp2.page.ParentId = page.TabID) Then
							temp.radPanelItem.Items.Add(temp2.radPanelItem)
							PagesQueue.Enqueue(j)
							iItemIndex = iItemIndex + 1
							temp2.item = iItemIndex
							AuthPages(j) = temp2
						End If
					Next j
				End If
			End While
		End Sub

#End Region

	End Class

End Namespace
