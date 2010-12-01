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

Imports DotNetNuke.Entities.Modules.Actions
Imports DotNetNuke.UI.WebControls
Imports System.IO

Namespace DotNetNuke.Modules.Admin.Portals

    Partial Class PortalAliases
        Inherits DotNetNuke.Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private intPortalId As Integer = -1
        Private _Aliases As ArrayList

#End Region

#Region "Private Methods"

        Private Sub BindAliases()
            dgPortalAlias.DataSource = Aliases
            dgPortalAlias.DataBind()
        End Sub

#End Region

#Region "Properties"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets and sets the mode of the control
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <history>
        ''' 	[cnurse]	12/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property AddMode() As Boolean
            Get
                Dim _Mode As Boolean = Null.NullBoolean
                If Not ViewState("Mode") Is Nothing Then
                    _Mode = CType(ViewState("Mode"), Boolean)
                End If
                Return _Mode
            End Get
            Set(ByVal value As Boolean)
                ViewState("Mode") = value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the collection of Portal ALiases
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <history>
        ''' 	[cnurse]	12/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Property Aliases() As ArrayList
            Get
                If _Aliases Is Nothing Then
                    _Aliases = New PortalAliasController().GetPortalAliasArrayByPortalID(intPortalId)
                End If
                Return _Aliases
            End Get
            Set(ByVal Value As ArrayList)
                _Aliases = Value
            End Set
        End Property

#End Region

#Region "Protected methods"

        Protected Overrides Sub LoadViewState(ByVal savedState As Object)
            Dim myState As Object() = CType(savedState, Object())
            If Not (myState(0) Is Nothing) Then
                MyBase.LoadViewState(myState(0))
            End If
            If Not (myState(1) Is Nothing) Then
                Dim aliasCount As Integer = DirectCast(myState(1), Integer)
                Aliases.Clear()
                For i As Integer = 0 To aliasCount - 1
                    Dim aliasString As String = CStr(myState(i + 2))
                    Dim sr As New StringReader(aliasString)
                    Aliases.Add(CBO.DeserializeObject(Of PortalAliasInfo)(sr))
                Next
            End If
        End Sub

        Protected Overrides Function SaveViewState() As Object
            Dim baseState As Object = MyBase.SaveViewState()
            Dim allStates(Aliases.Count + 1) As Object
            allStates(0) = baseState
            allStates(1) = Aliases.Count
            For i As Integer = 0 To Aliases.Count - 1
                Dim portalAlias As PortalAliasInfo = CType(Aliases(i), PortalAliasInfo)
                Dim sw As New StringWriter
                CBO.SerializeObject(portalAlias, sw)
                allStates(i + 2) = sw.ToString()
            Next

            Return allStates
        End Function

#End Region

#Region "Event Handlers"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Init runs when the control is initialised
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            For Each column As DataGridColumn In dgPortalAlias.Columns
                If column.GetType Is GetType(ImageCommandColumn) Then
                    'Manage Delete Confirm JS
                    Dim imageColumn As ImageCommandColumn = CType(column, ImageCommandColumn)
                    If imageColumn.CommandName = "Delete" Then
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem")
                    End If

                    'Localize Image Column Text
                    If imageColumn.CommandName <> "" Then
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, Me.LocalResourceFile)
                    End If
                End If
            Next
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            If Not (Request.QueryString("pid") Is Nothing) Then
                intPortalId = Int32.Parse(Request.QueryString("pid"))
            Else
                intPortalId = PortalId
            End If

            If Not Page.IsPostBack Then
                Services.Localization.Localization.LocalizeDataGrid(dgPortalAlias, Me.LocalResourceFile)
                BindAliases()
            End If
        End Sub

        Protected Sub grdPortals_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.DataGridItemEventArgs) Handles dgPortalAlias.ItemDataBound
            Dim item As DataGridItem = e.Item

            Dim defaultAlias As String = PortalController.GetPortalSetting("DefaultPortalAlias", intPortalId, "")

            If item.ItemType = ListItemType.Item Or _
                    item.ItemType = ListItemType.AlternatingItem Or _
                    item.ItemType = ListItemType.SelectedItem Then

                Dim imgColumnControl As Control = item.Controls(1).Controls(0)
                If TypeOf imgColumnControl Is ImageButton Then
                    Dim delImage As ImageButton = CType(imgColumnControl, ImageButton)
                    Dim portalAlias As PortalAliasInfo = CType(item.DataItem, PortalAliasInfo)

                    delImage.Visible = Not (portalAlias.PortalAliasID = Me.PortalAlias.PortalAliasID) AndAlso Not (portalAlias.HTTPAlias = defaultAlias)
                End If
                imgColumnControl = item.Controls(0).Controls(0)
                If TypeOf imgColumnControl Is ImageButton Then
                    Dim editImage As ImageButton = CType(imgColumnControl, ImageButton)
                    Dim portalAlias As PortalAliasInfo = CType(item.DataItem, PortalAliasInfo)

                    editImage.Visible = Not (portalAlias.PortalAliasID = Me.PortalAlias.PortalAliasID)
                End If
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' AddAlias runs when the Add button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub AddAlias(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdAddAlias.Click

            'Add a new empty rule and set the editrow to the new row
            Dim portalAlias As New PortalAliasInfo
            portalAlias.PortalID = intPortalId
            Aliases.Add(portalAlias)
            dgPortalAlias.EditItemIndex = Aliases.Count - 1

            'Set the AddMode to true
            AddMode = True

            'Rebind the collection
            BindAliases()

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DeleteAlias runs when a delete button is clicked
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        ''' 	[cnurse]	12/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub DeleteAlias(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgPortalAlias.DeleteCommand
            Dim controller As New PortalAliasController()

            'Get the index of the row to delete
            Dim index As Integer = e.Item.ItemIndex

            'Remove the alias from the aliases collection
            Dim portalAlias As PortalAliasInfo = CType(Aliases(index), PortalAliasInfo)
            controller.DeletePortalAlias(portalAlias.PortalAliasID)

            'Rebind the collection
            _Aliases = Nothing
            BindAliases()
        End Sub

        Private Sub EditAlias(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridCommandEventArgs) Handles dgPortalAlias.EditCommand
            'Set the AddMode to false
            AddMode = False

            'Set the editrow
            dgPortalAlias.EditItemIndex = e.Item.ItemIndex

            'Rebind the collection
            BindAliases()
        End Sub

        Protected Sub SaveAlias(ByVal source As Object, ByVal e As CommandEventArgs)
            Dim controller As New PortalAliasController()
            Dim InvalidChars As String = "! @ # $ % ^ & * ( ) + = { } [ ] | \ ; "" ' < , > ?"

            'Get the index of the row to save
            Dim index As Integer = dgPortalAlias.EditItemIndex

            Dim portalAlias As PortalAliasInfo = CType(Aliases(index), PortalAliasInfo)
            Dim ctlAlias As TextBox = CType(dgPortalAlias.Items(index).Cells(2).FindControl("txtHTTPAlias"), TextBox)

            Dim strAlias As String = ctlAlias.Text.Trim()
            If strAlias <> "" Then
                If strAlias.IndexOf("://") <> -1 Then
                    strAlias = strAlias.Remove(0, strAlias.IndexOf("://") + 3)
                End If
                If strAlias.IndexOf("\\") <> -1 Then
                    strAlias = strAlias.Remove(0, strAlias.IndexOf("\\") + 2)
                End If

                For Each s As String In InvalidChars.Split(" ")
                    If strAlias.Contains(s) Then
                        lblError.Text = Localization.GetString("InvalidAlias", Me.LocalResourceFile)
                        lblError.Visible = True
                        Exit For
                    End If
                Next
            End If
            If Not lblError.Visible Then
                portalAlias.HTTPAlias = ctlAlias.Text
                If AddMode Then
                    controller.AddPortalAlias(portalAlias)
                Else
                    controller.UpdatePortalAliasInfo(portalAlias)
                End If

                'Reset Edit Index
                lblError.Visible = False
                dgPortalAlias.EditItemIndex = -1
                _Aliases = Nothing
            End If

            BindAliases()
        End Sub

        Protected Sub CancelEdit(ByVal source As Object, ByVal e As CommandEventArgs)

            If AddMode Then
                'Remove the temporary added row
                Aliases.RemoveAt(Aliases.Count - 1)
                AddMode = False
            End If

            'Clear editrow
            dgPortalAlias.EditItemIndex = -1
            lblError.Visible = False

            'Rebind the collection
            BindAliases()

        End Sub

#End Region

    End Class


End Namespace
