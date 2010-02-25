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
Imports System.Web.UI.WebControls

Imports DotNetNuke.Services.Localization
Imports DotNetNuke.UI.Utilities
Imports DotNetNuke.UI.WebControls

Namespace DotNetNuke.Common.Lists

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Manages Entry List
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [tamttt]  20/10/2004	Created
    '''     [cnurse]  01/30/2007	Extracted to separate user control
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial Class ListEntries
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Events"

        Public Event ListCreated As EventHandler
        Public Event ListEntryCreated As EventHandler

#End Region

#Region "Protected Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Gets and sets the DefinitionID of the current List
        ''' </summary>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property DefinitionID() As Integer
            Get
                If ViewState("DefinitionID") Is Nothing Then
                    ViewState("DefinitionID") = Null.NullInteger
                End If
                Return CType(ViewState("DefinitionID"), Integer)
            End Get
            Set(ByVal [Value] As Integer)
                ViewState("DefinitionID") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Property to determine if this list has custom sort order
        ''' </summary>
        ''' <remarks>
        '''     Up/Down button in datagrid will be visibled based on this property.
        '''     If disable, list will be sorted anphabetically
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property EnableSortOrder() As Boolean
            Get
                If ViewState("EnableSortOrder") Is Nothing Then
                    ViewState("EnableSortOrder") = False
                End If
                Return CType(ViewState("EnableSortOrder"), Boolean)
            End Get
            Set(ByVal [Value] As Boolean)
                ViewState("EnableSortOrder") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the selected ListInfo
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]  01/31/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property SelectedList() As ListInfo
            Get
                Return GetList(SelectedKey, True)
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the selected collection of List Items
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]  01/31/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property SelectedListItems() As ListEntryInfoCollection
            Get
                Dim list As ListEntryInfoCollection = Nothing
                If Not SelectedList Is Nothing Then
                    Dim ctlLists As New ListController
                    list = ctlLists.GetListEntryInfoCollection(SelectedList.Name, SelectedList.ParentKey, SelectedList.PortalID)
                End If
                Return list
            End Get
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Property to determine if this list is system (DNN core)
        ''' </summary>
        ''' <remarks>
        '''     Default entries in system list can not be deleted
        '''     Entries in system list is sorted anphabetically
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Property SystemList() As Boolean
            Get
                If ViewState("SystemList") Is Nothing Then
                    ViewState("SystemList") = False
                End If
                Return CType(ViewState("SystemList"), Boolean)
            End Get
            Set(ByVal Value As Boolean)
                ViewState("SystemList") = Value
            End Set
        End Property

#End Region

#Region "Public Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the ListName for this set of List Entries
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]  01/31/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ListName() As String
            Get
                If Not ViewState("ListName") Is Nothing Then
                    Return ViewState("ListName").ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                ViewState("ListName") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the portalId for this set of List Entries
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]  01/31/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ListPortalID() As Integer
            Get
                If ViewState("ListPortalID") Is Nothing Then
                    ViewState("ListPortalID") = Null.NullInteger
                End If
                Return CType(ViewState("ListPortalID"), Integer)
            End Get
            Set(ByVal [Value] As Integer)
                ViewState("ListPortalID") = Value
            End Set
        End Property

        Public Property Mode() As String
            Get
                If Not ViewState("Mode") Is Nothing Then
                    Return ViewState("Mode").ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal value As String)
                ViewState("Mode") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Get or set the ParentKey for this set of List Entries
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]  02/05/2007	Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property ParentKey() As String
            Get
                If Not ViewState("ParentKey") Is Nothing Then
                    Return ViewState("ParentKey").ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal Value As String)
                ViewState("ParentKey") = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets or sets the Selected key
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Property SelectedKey() As String
            Get
                If Not ViewState("SelectedKey") Is Nothing Then
                    Return ViewState("SelectedKey").ToString
                Else
                    Return ""
                End If
            End Get
            Set(ByVal [Value] As String)
                ViewState("SelectedKey") = Value
            End Set
        End Property

        Public Property ShowDelete() As Boolean
            Get
                If Not ViewState("ShowDelete") Is Nothing Then
                    Return Convert.ToBoolean(ViewState("ShowDelete"))
                Else
                    Return False
                End If
            End Get
            Set(ByVal value As Boolean)
                ViewState("ShowDelete") = value
            End Set
        End Property

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Loads top level entry list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindGrid()
            For Each column As DataGridColumn In grdEntries.Columns
                If column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    If imageColumn.CommandName = "Delete" Then
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem")
                        If SystemList Then
                            column.Visible = False
                        Else
                            column.Visible = True
                        End If
                    End If
                    'Localize Image Column Text
                    If imageColumn.CommandName <> "" Then
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, Me.LocalResourceFile)
                    End If

                End If
            Next

            grdEntries.DataSource = SelectedListItems 'selList
            grdEntries.DataBind()

            If SelectedListItems Is Nothing Then
                Me.lblEntryCount.Text = "0 " & Localization.GetString("Entries", Me.LocalResourceFile)
            Else
                Me.lblEntryCount.Text = SelectedListItems.Count.ToString & " " & Localization.GetString("Entries", Me.LocalResourceFile)
            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Loads top level entry list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindListInfo()
            lblListName.Text = ListName
            lblListParent.Text = ParentKey
            rowListParent.Visible = (ParentKey.Length > 0)
            chkEnableSortOrder.Checked = EnableSortOrder
            If Not SystemList And ShowDelete Then
                Me.cmdDeleteList.Visible = True
                ClientAPI.AddButtonConfirm(cmdDeleteList, Localization.GetString("DeleteItem"))
            Else
                Me.cmdDeleteList.Visible = False
            End If

            Select Case Mode
                Case "ListEntries"
                    EnableView(True)
                Case "EditEntry"
                    EnableView(False)
                    EnableEdit(False)
                Case "AddEntry"
                    EnableView(False)
                    EnableEdit(False)
                    If Not SelectedList Is Nothing Then
                        txtParentKey.Text = SelectedList.ParentKey
                    Else
                        rowEnableSortOrder.Visible = True
                    End If
                    txtEntryName.Text = ListName
                    rowListName.Visible = False
                    txtEntryValue.Text = ""
                    txtEntryText.Text = ""
                    cmdSaveEntry.CommandName = "SaveEntry"
                Case "AddList"
                    EnableView(False)
                    EnableEdit(True)

                    rowListName.Visible = True
                    txtParentKey.Text = ""
                    txtEntryName.Text = ""
                    txtEntryValue.Text = ""
                    txtEntryText.Text = ""
                    txtEntryName.ReadOnly = False
                    cmdSaveEntry.CommandName = "SaveList"

                    Dim ctlLists As New ListController
                    With ddlSelectList
                        .Enabled = True
                        .DataSource = ctlLists.GetListInfoCollection()
                        .DataTextField = "DisplayName"
                        .DataValueField = "Key"
                        .DataBind()
                        .Items.Insert(0, New ListItem(Localization.GetString("None_Specified"), ""))
                    End With

                    ' Reset dropdownlist
                    With ddlSelectParent
                        .ClearSelection()
                        .Enabled = False
                    End With

            End Select

        End Sub

        Private Sub DeleteItem(ByVal entryId As Integer)
            If SelectedListItems.Count > 1 Then
                Try
                    Dim ctlLists As New ListController
                    ctlLists.DeleteListEntryByID(entryId, True)
                    DataBind()
                Catch exc As Exception    'Module failed to load
                    ProcessModuleLoadException(Me, exc)
                End Try
            Else
                DeleteList()
            End If
        End Sub

        Private Sub DeleteList()
            Dim ctlLists As New ListController

            ctlLists.DeleteList(SelectedList, True)

            Response.Redirect(NavigateURL(TabId))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Switching to edit mode, change controls visibility for editing depends on AddList params
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub EnableEdit(ByVal AddList As Boolean)
            Me.rowListdetails.Visible = (Not AddList)
            Me.rowSelectList.Visible = AddList
            Me.rowSelectParent.Visible = AddList
            Me.rowEnableSortOrder.Visible = AddList
            Me.rowParentKey.Visible = False
            Me.cmdDelete.Visible = False
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Switching to view mode, change controls visibility for viewing
        ''' </summary>
        ''' <param name="ViewMode">Boolean value to determine View or Edit mode</param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub EnableView(ByVal [ViewMode] As Boolean)
            Me.rowListdetails.Visible = True
            Me.rowEntryGrid.Visible = ViewMode
            Me.rowEntryEdit.Visible = (Not ViewMode)
        End Sub

        Private Function GetList(ByVal key As String, ByVal update As Boolean) As ListInfo
            Dim ctlLists As New ListController
            Dim index As Integer = key.IndexOf(":")
            Dim _ListName As String = key.Substring(index + 1)
            Dim _ParentKey As String = Null.NullString
            If index > 0 Then
                _ParentKey = key.Substring(0, index)
            End If

            If update Then
                ListName = _ListName
                ParentKey = _ParentKey
            End If

            Return ctlLists.GetListInfo(_ListName, _ParentKey, ListPortalID)
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Loads top level entry list
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separate user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub InitList()
            If Not SelectedList Is Nothing Then
                DefinitionID = SelectedList.DefinitionID
                EnableSortOrder = SelectedList.EnableSortOrder
                SystemList = SelectedList.SystemList
            End If
        End Sub

#End Region

#Region "Public Methods"

        Public Overrides Sub DataBind()
            InitList()
            BindListInfo()
            BindGrid()
        End Sub

#End Region

#Region "EventMethods"

        Protected Sub OnListCreated(ByVal e As EventArgs)
            RaiseEvent ListCreated(Me, e)
        End Sub

        Protected Sub OnListEntryCreated(ByVal e As EventArgs)
            RaiseEvent ListEntryCreated(Me, e)
        End Sub

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Page load, bind tree and enable controls
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal [sender] As System.Object, ByVal [e] As System.EventArgs) Handles MyBase.Init
            Try
                If Not Page.IsPostBack Then
                    Mode = "ListEntries"
                End If
            Catch exc As Exception    'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Handles events when clicking image button in the grid (Edit/Up/Down)
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub grdEntries_ItemCommand(ByVal [source] As Object, ByVal [e] As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles grdEntries.ItemCommand
            Try
                Dim ctlLists As New ListController
                Dim entryID As Integer = CType(CType(source, DataGrid).DataKeys(e.Item.ItemIndex), Integer)

                Select Case e.CommandName.ToLower
                    Case "delete"
                        Mode = "ListEntries"
                        DeleteItem(entryID)
                    Case "edit"
                        Mode = "EditEntry"

                        Dim entry As ListEntryInfo = ctlLists.GetListEntryInfo(entryID)
                        Me.txtEntryID.Text = entryID.ToString
                        Me.txtParentKey.Text = entry.ParentKey
                        Me.txtEntryValue.Text = entry.Value
                        Me.txtEntryText.Text = entry.Text
                        rowListName.Visible = False
                        Me.cmdSaveEntry.CommandName = "Update"

                        If Not SystemList Then
                            Me.cmdDelete.Visible = True
                            ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"))
                        Else
                            Me.cmdDelete.Visible = False
                        End If

                        DataBind()
                    Case "up"
                        ctlLists.UpdateListSortOrder(entryID, True)
                        DataBind()
                    Case "down"
                        ctlLists.UpdateListSortOrder(entryID, False)
                        DataBind()
                End Select

            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Select a list in dropdownlist
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>        
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub ddlSelectList_SelectedIndexChanged(ByVal [sender] As Object, ByVal [e] As System.EventArgs) Handles ddlSelectList.SelectedIndexChanged
            Dim ctlLists As New ListController
            Dim selList As ListInfo = GetList(ddlSelectList.SelectedItem.Value, False)

            With ddlSelectParent
                .Enabled = True
                .DataSource = ctlLists.GetListEntryInfoCollection(selList.Name, selList.ParentKey)
                .DataTextField = "DisplayName"
                .DataValueField = "EntryID"
                .DataBind()
                '.Items.Insert(0, New ListItem(Localization.GetString("None_Specified"), ""))
            End With

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Handles Add New Entry command
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        '''     Using "CommandName" property of cmdSaveEntry to determine this is a new entry of an existing list
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdAddEntry_Click(ByVal [sender] As Object, ByVal [e] As System.EventArgs) Handles cmdAddEntry.Click
            Mode = "AddEntry"
            DataBind()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Handles cmdSaveEntry.Click
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        '''     Using "CommandName" property of cmdSaveEntry to determine action to take (ListUpdate/AddEntry/AddList)
        ''' </remarks>
        ''' <history>
        '''     [tamttt] 20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdSaveEntry_Click(ByVal [sender] As Object, ByVal [e] As System.EventArgs) Handles cmdSaveEntry.Click
            Dim ctlLists As New ListController
            Dim entry As New ListEntryInfo
            With entry
                .DefinitionID = Null.NullInteger
                .PortalID = ListPortalID
                .ListName = txtEntryName.Text
                .Value = txtEntryValue.Text
                .Text = txtEntryText.Text
            End With

            If Page.IsValid Then
                Mode = "ListEntries"
                Select Case cmdSaveEntry.CommandName.ToLower
                    Case "update"
                        entry.ParentKey = SelectedList.ParentKey
                        entry.EntryID = Int16.Parse(txtEntryID.Text)

                        ctlLists.UpdateListEntry(entry)

                        DataBind()
                    Case "saveentry"
                        If Not SelectedList Is Nothing Then
                            entry.ParentKey = SelectedList.ParentKey
                            entry.ParentID = SelectedList.ParentID
                            entry.Level = SelectedList.Level
                        End If
                        If chkEnableSortOrder.Checked Then
                            entry.SortOrder = 1
                        Else
                            entry.SortOrder = 0
                        End If

                        ctlLists.AddListEntry(entry)

                        DataBind()
                    Case "savelist"
                        If ddlSelectParent.SelectedIndex <> -1 Then
                            Dim parentID As Integer = Int32.Parse(ddlSelectParent.SelectedItem.Value)
                            Dim parentEntry As ListEntryInfo = ctlLists.GetListEntryInfo(parentID)
                            entry.ParentID = parentID
                            entry.DefinitionID = parentEntry.DefinitionID
                            entry.Level = parentEntry.Level + 1
                            entry.ParentKey = parentEntry.Key
                        End If

                        If chkEnableSortOrder.Checked Then
                            entry.SortOrder = 1
                        Else
                            entry.SortOrder = 0
                        End If

                        ctlLists.AddListEntry(entry)

                        SelectedKey = entry.ParentKey.Replace(":", ".") + ":" + entry.ListName

                        Response.Redirect(NavigateURL(TabId, "", "Key=" & SelectedKey))
                End Select

            End If

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Delete List
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDeleteList_Click(ByVal [sender] As Object, ByVal [e] As System.EventArgs) Handles cmdDeleteList.Click
            DeleteList()
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Delete List
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        '''     If deleting entry is not the last one in the list, rebinding the grid, otherwise return back to main page (rebinding DNNTree)
        ''' </remarks>
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdDelete_Click(ByVal [sender] As Object, ByVal [e] As System.EventArgs) Handles cmdDelete.Click
            DeleteItem(CType(txtEntryID.Text, Integer))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        '''     Cancel
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>       
        ''' <history>
        '''     [tamttt]  20/10/2004	Created
        '''     [cnurse]  01/30/2007	Extracted to separte user control
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub cmdCancel_Click(ByVal [sender] As Object, ByVal [e] As System.EventArgs) Handles cmdCancel.Click
            Try
                Mode = "ListEntries"
                If SelectedKey <> "" Then
                    DataBind()
                Else
                    Response.Redirect(NavigateURL(TabId))
                End If
            Catch exc As Exception           'Module failed to load
                ProcessModuleLoadException(Me, exc)
            End Try
        End Sub

#End Region


    End Class

End Namespace
