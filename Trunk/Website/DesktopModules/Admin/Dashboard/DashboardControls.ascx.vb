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
Imports DotNetNuke.Modules.Dashboard.Components
Imports DotNetNuke.Entities.Modules
Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls
Imports System.Collections.Generic
Imports DotNetNuke.Services.Installer

Namespace DotNetNuke.Modules.Admin.Dashboard

    Partial Class DashboardControls
        Inherits PortalModuleBase

#Region "Constants"

        Const COLUMN_ENABLED As Integer = 5
        Const COLUMN_MOVE_DOWN As Integer = 1
        Const COLUMN_MOVE_UP As Integer = 2

#End Region

#Region "Private Members"

        Private _DashboardControls As List(Of DashboardControl)

#End Region

        Protected ReadOnly Property DashboardControls() As List(Of DashboardControl)
            Get
                If _DashboardControls Is Nothing Then
                    _DashboardControls = DashboardController.GetDashboardControls(False)
                End If
                Return _DashboardControls
            End Get
        End Property


#Region "Private Methods"

        Private Function SupportsRichClient() As Boolean
            Return DotNetNuke.UI.Utilities.ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML)
        End Function

        Private Sub DeleteControl(ByVal index As Integer)
            Dim dashboardControl As DashboardControl = DashboardControls(index)

            Response.Redirect(Util.UnInstallURL(TabId, dashboardControl.PackageID, Server.UrlEncode(NavigateURL(TabId, "DashboardControls", "mid=" & ModuleId))), True)
        End Sub

        Private Sub MoveControl(ByVal index As Integer, ByVal destIndex As Integer)
            Dim dashboardControl As DashboardControl = DashboardControls(index)
            Dim nextControl As DashboardControl = dashboardControls(destIndex)

            Dim currentOrder As Integer = dashboardControl.ViewOrder
            Dim nextOrder As Integer = nextControl.ViewOrder

            'Swap ViewOrders
            dashboardControl.ViewOrder = nextOrder
            nextControl.ViewOrder = currentOrder

            ''Refresh Grid
            dashboardControls.Sort()
            BindGrid()
        End Sub

        Private Sub MoveControlDown(ByVal index As Integer)
            MoveControl(index, index + 1)
        End Sub

        Private Sub MoveControlUp(ByVal index As Integer)
            MoveControl(index, index - 1)
        End Sub

        Private Sub BindGrid()
            Dim allEnabled As Boolean = True

            'Check whether the checkbox column headers are true or false
            For Each dashboardControl As DashboardControl In dashboardControls
                If dashboardControl.IsEnabled = False Then
                    allEnabled = False
                End If

                If Not allEnabled Then
                    Exit For
                End If
            Next

            For Each column As DataGridColumn In grdDashboardControls.Columns
                If column.GetType Is GetType(CheckBoxColumn) Then
                    'Manage CheckBox column events
                    Dim cbColumn As CheckBoxColumn = CType(column, CheckBoxColumn)
                    If cbColumn.DataField = "IsEnabled" Then
                        cbColumn.Checked = allEnabled
                    End If
                End If
            Next

            grdDashboardControls.DataSource = dashboardControls
            grdDashboardControls.DataBind()
        End Sub

        Private Sub RefreshGrid()
            _DashboardControls = Nothing
            BindGrid()
        End Sub

        Private Sub UpdateControls()
            For Each dashboardControl As DashboardControl In DashboardControls
                If dashboardControl.IsDirty Then
                    DashboardController.UpdateDashboardControl(dashboardControl)
                End If
            Next
        End Sub

        Private Sub ProcessPostBack()
            Try
                Dim aryNewOrder() As String = DotNetNuke.UI.Utilities.ClientAPI.GetClientSideReorder(Me.grdDashboardControls.ClientID, Me.Page)
                Dim dashboardControl As DashboardControl
                Dim objItem As DataGridItem
                Dim chk As CheckBox
                For i As Integer = 0 To Me.grdDashboardControls.Items.Count - 1
                    objItem = Me.grdDashboardControls.Items(i)
                    dashboardControl = dashboardControls(i)
                    chk = CType(objItem.Cells(COLUMN_ENABLED).Controls(0), CheckBox)
                    dashboardControl.IsEnabled = chk.Checked
                Next
                'assign vieworder
                For i As Integer = 0 To aryNewOrder.Length - 1
                    dashboardControls(CInt(aryNewOrder(i))).ViewOrder = i
                Next
                dashboardControls.Sort()

            Catch ex As Exception
                Throw ex
            End Try
        End Sub

#End Region

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            If Not (savedState Is Nothing) Then
                ' Load State from the array of objects that was saved with SaveViewState.

                Dim myState As Object() = CType(savedState, Object())

                'Load Base Controls ViewState
                If Not (myState(0) Is Nothing) Then
                    MyBase.LoadViewState(myState(0))
                End If

                'Load ModuleID
                If Not (myState(1) Is Nothing) Then
                    _DashboardControls = CType(myState(1), List(Of DashboardControl))
                End If
            End If
        End Sub

        Protected Overrides Function SaveViewState() As Object
            Dim allStates(1) As Object

            ' Save the Base Controls ViewState
            allStates(0) = MyBase.SaveViewState()

            'Save the Profile Properties
            allStates(1) = DashboardControls

            Return allStates
        End Function

#Region "Event Handlers"

        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            For Each column As DataGridColumn In grdDashboardControls.Columns
                If column.GetType Is GetType(CheckBoxColumn) Then
                    If SupportsRichClient() = False Then
                        Dim cbColumn As CheckBoxColumn = CType(column, CheckBoxColumn)
                        AddHandler cbColumn.CheckedChanged, AddressOf grdDashboardControls_ItemCheckedChanged
                    End If
                ElseIf column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    Select Case imageColumn.CommandName
                        Case "Delete"
                            imageColumn.OnClickJS = Localization.GetString("DeleteItem")
                            imageColumn.Text = Localization.GetString("Delete", Me.LocalResourceFile)
                        Case "MoveUp"
                            imageColumn.Text = Localization.GetString("MoveUp", Me.LocalResourceFile)
                        Case "MoveDown"
                            imageColumn.Text = Localization.GetString("MoveDown", Me.LocalResourceFile)
                    End Select
                End If
            Next

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Try
                If Not Page.IsPostBack Then
                    Localization.LocalizeDataGrid(grdDashboardControls, Me.LocalResourceFile)
                    BindGrid()
                Else
                    ProcessPostBack()
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdCancel.Click
            Response.Redirect(NavigateURL(), True)
        End Sub

        Protected Sub cmdInstall_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdInstall.Click
            Response.Redirect(Util.InstallURL(TabId, Server.UrlEncode(NavigateURL(TabId, "DashboardControls", "mid=" & ModuleId)), "DashboardControl"), True)
        End Sub

        Private Sub cmdRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRefresh.Click
            RefreshGrid()
        End Sub

        Private Sub cmdUpdate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdUpdate.Click
            Try
                UpdateControls()

                RefreshGrid()
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        Private Sub grdDashboardControls_ItemCheckedChanged(ByVal sender As Object, ByVal e As UI.WebControls.DNNDataGridCheckChangedEventArgs)
            Dim propertyName As String = e.Field
            Dim propertyValue As Boolean = e.Checked
            Dim isAll As Boolean = e.IsAll
            Dim index As Integer = e.Item.ItemIndex

            Dim dashboardControl As DashboardControl

            If isAll Then
                'Update All the properties
                For Each dashboardControl In DashboardControls
                    Select Case propertyName
                        Case "IsEnabled"
                            dashboardControl.IsEnabled = propertyValue
                    End Select
                Next
            Else
                'Update the indexed property
                dashboardControl = DashboardControls(index)
                Select Case propertyName
                    Case "IsEnabled"
                        dashboardControl.IsEnabled = propertyValue
                End Select
            End If

            BindGrid()

        End Sub

        Private Sub grdDashboardControls_ItemCommand(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdDashboardControls.ItemCommand

            Dim commandName As String = e.CommandName
            Dim commandArgument As Integer = CType(e.CommandArgument, Integer)
            Dim index As Integer = e.Item.ItemIndex

            Select Case commandName
                Case "Delete"
                    DeleteControl(index)
                Case "MoveUp"
                    MoveControlUp(index)
                Case "MoveDown"
                    MoveControlDown(index)
            End Select

        End Sub

        Private Sub grdDashboardControls_ItemCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdDashboardControls.ItemCreated
            If SupportsRichClient() Then
                Select Case e.Item.ItemType
                    Case ListItemType.Header
                        'we combined the header label and checkbox in same place, so it is control 1 instead of 0
                        CType(e.Item.Cells(COLUMN_ENABLED).Controls(1), WebControl).Attributes.Add("onclick", "dnn.util.checkallChecked(this," & COLUMN_ENABLED & ");")
                        CType(e.Item.Cells(COLUMN_ENABLED).Controls(1), CheckBox).AutoPostBack = False
                    Case ListItemType.AlternatingItem, ListItemType.Item
                        CType(e.Item.Cells(COLUMN_ENABLED).Controls(0), CheckBox).AutoPostBack = False

                        DotNetNuke.UI.Utilities.ClientAPI.EnableClientSideReorder(e.Item.Cells(COLUMN_MOVE_DOWN).Controls(0), Me.Page, False, Me.grdDashboardControls.ClientID)
                        DotNetNuke.UI.Utilities.ClientAPI.EnableClientSideReorder(e.Item.Cells(COLUMN_MOVE_UP).Controls(0), Me.Page, True, Me.grdDashboardControls.ClientID)
                End Select
            End If
        End Sub

        Protected Sub grdDashboardControlss_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles grdDashboardControls.ItemDataBound
            Dim item As DataGridItem = e.Item

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim imgColumnControl As Control = item.Controls(0).Controls(0)
                If TypeOf imgColumnControl Is ImageButton Then
                    Dim delImage As ImageButton = CType(imgColumnControl, ImageButton)
                    Dim dashboardControl As DashboardControl = CType(item.DataItem, DashboardControl)

                    Select Case dashboardControl.DashboardControlKey
                        Case "Server", "Database", "Host", "Portals", "Modules", "Skins"
                            delImage.Visible = False
                        Case Else
                            delImage.Visible = True
                    End Select
                End If
            End If

        End Sub

#End Region

    End Class

End Namespace
