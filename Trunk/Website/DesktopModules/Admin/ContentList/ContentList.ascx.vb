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

Imports System.Linq
Imports System.Web
Imports DotNetNuke
Imports System.Collections.Generic
Imports DotNetNuke.Entities.Content

Namespace DotNetNuke.Modules.ContentList

    Partial Class ContentList
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _CurrentPage As Integer = 1
        Private _TagQuery As String = Null.NullString

#End Region

#Region "Protected Members"

        Protected TotalPages As Integer = -1
        Protected TotalRecords As Integer

        Protected Property CurrentPage() As Integer
            Get
                Return _CurrentPage
            End Get
            Set(ByVal Value As Integer)
                _CurrentPage = Value
            End Set
        End Property

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Gets the Page Size for the Grid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	03/12/2008  Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected ReadOnly Property PageSize() As Integer
            Get
                Dim itemsPage As Integer = 10
                If CType(Settings("perpage"), String) <> "" Then
                    itemsPage = Integer.Parse(CType(Settings("perpage"), String))
                End If
                Return itemsPage
            End Get
        End Property

#End Region

#Region "Private Methods"

        Private Sub BindData()
            Using dt As New DataTable()
                dt.Columns.Add(New DataColumn("TabId", GetType(System.Int32)))
                dt.Columns.Add(New DataColumn("ContentKey", GetType(System.String)))
                dt.Columns.Add(New DataColumn("Title", GetType(System.String)))
                dt.Columns.Add(New DataColumn("Description", GetType(System.String)))
                dt.Columns.Add(New DataColumn("PubDate", GetType(System.DateTime)))

                Dim Results As List(Of ContentItem) = New ContentController().GetContentItemsByTerm(_TagQuery).ToList()

                If _TagQuery.Length > 0 Then
                    For Each item As ContentItem In Results
                        Dim dr As DataRow = dt.NewRow()
                        dr("TabId") = item.TabID
                        dr("ContentKey") = item.ContentKey
                        dr("Title") = item.Content
                        If item.Content.Length > 1000 Then
                            dr("Description") = item.Content.Substring(0, 1000)
                        Else
                            dr("Description") = item.Content
                        End If
                        dr("PubDate") = item.CreatedOnDate
                        dt.Rows.Add(dr)
                    Next
                End If

                'Bind Search Results Grid
                Dim dv As New DataView(dt)
                dgResults.PageSize = PageSize
                dgResults.DataSource = dv
                dgResults.DataBind()
                If Results.Count = 0 Then
                    dgResults.Visible = False
                    lblMessage.Text = String.Format(Localization.GetString("NoResults", LocalResourceFile), _TagQuery)
                Else
                    lblMessage.Text = String.Format(Localization.GetString("Results", LocalResourceFile), _TagQuery)
                End If
                If Results.Count <= dgResults.PageSize Then
                    ctlPagingControl.Visible = False
                Else
                    ctlPagingControl.Visible = True
                End If
                ctlPagingControl.TotalRecords = Results.Count
            End Using
            ctlPagingControl.PageSize = dgResults.PageSize
            ctlPagingControl.CurrentPage = CurrentPage
        End Sub

#End Region

#Region "Protected Methods"

        Protected Function FormatDate(ByVal pubDate As Date) As String
            Return pubDate.ToString()
        End Function

        Protected Function FormatURL(ByVal TabID As Integer, ByVal Link As String) As String
            Dim strURL As String

            If Link = "" Then
                strURL = Common.NavigateURL(TabID)
            Else
                strURL = Common.NavigateURL(TabID, "", Link)
            End If

            Return strURL
        End Function

        Protected Function ShowDescription() As String
            Dim strShow As String

            If CType(Settings("showdescription"), String) <> "" Then
                If CType(Settings("showdescription"), String) = "Y" Then
                    strShow = "True"
                Else
                    strShow = "False"
                End If
            Else
                strShow = "True"
            End If

            Return strShow
        End Function

#End Region

#Region "Event Handlers"

        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim objSecurity As New PortalSecurity
            If Not Request.Params("Tag") Is Nothing Then
                _TagQuery = HttpContext.Current.Server.HtmlEncode(objSecurity.InputFilter(Request.Params("Tag").ToString, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup))
            End If

            If _TagQuery.Length > 0 Then
                If Not Page.IsPostBack Then
                    BindData()
                End If
            Else
                If Me.IsEditable Then
                    Skin.AddModuleMessage(Me, Localization.GetString("ModuleHidden", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError)
                Else
                    Me.ContainerControl.Visible = False
                End If
            End If
        End Sub

        Private Sub dgResults_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgResults.PageIndexChanged
            dgResults.CurrentPageIndex = e.NewPageIndex
            BindData()
        End Sub

        Protected Sub ctlPagingControl_PageChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlPagingControl.PageChanged
            CurrentPage = ctlPagingControl.CurrentPage

            dgResults.CurrentPageIndex = CurrentPage - 1
            BindData()
        End Sub

#End Region
    End Class

End Namespace
