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

Imports System.Data.SqlClient
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Web
Imports DotNetNuke
Imports DotNetNuke.Services.Search

Namespace DotNetNuke.Modules.SearchResults

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Modules.SearchResults
    ''' Project:    DotNetNuke.SearchResults
    ''' Class:      SearchResults
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SearchResults Class provides the UI for displaying the Search Results
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/11/2004	Improved Formatting of results, and moved Search Options
    '''                             to Settings
    '''     [cnurse]    12/13/2004  Switched to using a DataGrid for Search Results
    '''     [cnurse]    01/04/2005  Modified so "Nos" stay in order
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Partial  Class SearchResults
        Inherits Entities.Modules.PortalModuleBase

#Region "Private Members"

        Private _CurrentPage As Integer = 1
        Private _SearchQuery As String

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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' BindData binds the Search Results to the Grid
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub BindData()

            Dim Results As SearchResultsInfoCollection = SearchDataStoreProvider.Instance.GetSearchResults(PortalId, _searchQuery)

            Dim dt As New DataTable
            Dim dc As New DataColumn("TabId")
            dt.Columns.Add(New DataColumn("TabId", GetType(System.Int32)))
            dt.Columns.Add(New DataColumn("Guid", GetType(System.String)))
            dt.Columns.Add(New DataColumn("Title", GetType(System.String)))
            dt.Columns.Add(New DataColumn("Relevance", GetType(System.Int32)))
            dt.Columns.Add(New DataColumn("Description", GetType(System.String)))
            dt.Columns.Add(New DataColumn("PubDate", GetType(System.DateTime)))


            'Get the maximum items to display
            Dim maxItems As Integer = 0
            If CType(Settings("maxresults"), String) <> "" Then
                maxItems = Integer.Parse(CType(Settings("maxresults"), String))
            Else
                maxItems = Results.Count
            End If
            If Results.Count < maxItems Or maxItems < 1 Then
                maxItems = Results.Count
            End If

            'Get the titlelength/descriptionlength
            Dim titleLength As Integer = 0
            If CType(Settings("titlelength"), String) <> "" Then
                titleLength = Integer.Parse(CType(Settings("titlelength"), String))
            End If
            Dim descLength As Integer = 0
            If CType(Settings("descriptionlength"), String) <> "" Then
                descLength = Integer.Parse(CType(Settings("descriptionlength"), String))
            End If

            Dim i As Integer = 0
            Dim ResultItem As SearchResultsInfo
            For i = 0 To maxItems - 1
                ResultItem = Results(i)
                Dim dr As DataRow = dt.NewRow()
                dr("TabId") = ResultItem.TabId
                dr("Guid") = ResultItem.Guid
                If titleLength > 0 And titleLength < ResultItem.Title.Length Then
                    dr("Title") = ResultItem.Title.Substring(0, titleLength)
                Else
                    dr("Title") = ResultItem.Title
                End If
                dr("Relevance") = ResultItem.Relevance
                If descLength > 0 And descLength < ResultItem.Description.Length Then
                    dr("Description") = ResultItem.Description.Substring(0, descLength)
                Else
                    dr("Description") = ResultItem.Description
                End If
                dr("PubDate") = ResultItem.PubDate
                dt.Rows.Add(dr)
            Next

            'Bind Search Results Grid
            Dim dv As New DataView(dt)
            dv.Sort = "Relevance DESC"
            dgResults.PageSize = PageSize
            dgResults.DataSource = dv
            dgResults.DataBind()

            If Results.Count = 0 Then
                dgResults.Visible = False
                lblMessage.Text = String.Format(Localization.GetString("NoResults", LocalResourceFile), _SearchQuery)
            Else
                lblMessage.Text = String.Format(Localization.GetString("Results", LocalResourceFile), _SearchQuery)
            End If
            If Results.Count <= dgResults.PageSize Then
                ctlPagingControl.Visible = False
            Else
                ctlPagingControl.Visible = True
            End If

            ctlPagingControl.TotalRecords = Results.Count
            ctlPagingControl.PageSize = dgResults.PageSize
            ctlPagingControl.CurrentPage = CurrentPage
        End Sub

#End Region

#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatDate displays the publication Date
        ''' </summary>
        ''' <param name="pubDate">The publication Date</param>
        ''' <returns>The formatted date</returns>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FormatDate(ByVal pubDate As Date) As String
            Return pubDate.ToString()
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatRelevance displays the relevance value
        ''' </summary>
        ''' <param name="relevance">The publication Date</param>
        ''' <returns>A String</returns>
        ''' <history>
        ''' 	[cnurse]	11/12/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FormatRelevance(ByVal relevance As Integer) As String
            Return Services.Localization.Localization.GetString("Relevance", Me.LocalResourceFile) & relevance.ToString
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' FormatURL the correctly formatted url to the Search Result
        ''' </summary>
        ''' <param name="TabID">The Id of the Tab where the content is located</param>
        ''' <param name="Link">The module provided querystring to access the correct content</param>
        ''' <returns>The formatted url</returns>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function FormatURL(ByVal TabID As Integer, ByVal Link As String) As String
            Dim strURL As String

            If Link = "" Then
                strURL = Common.NavigateURL(TabID)
            Else
                strURL = Common.NavigateURL(TabID, "", Link)
            End If

            Return strURL
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' ShowDescription determines whether the description should be shown
        ''' </summary>
        ''' <returns>True or False string</returns>
        ''' <history>
        ''' 	[cnurse]	12/13/2004	created
        ''' </history>
        ''' -----------------------------------------------------------------------------
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

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Page_Load runs when the control is loaded
        ''' </summary>
        ''' <history>
        ''' 	[cnurse]	11/11/2004	documented
        '''     [cnurse]    12/13/2004  Switched to using a DataGrid for Search Results
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Dim objSecurity As New PortalSecurity
            If Not Request.Params("Search") Is Nothing Then
                _SearchQuery = objSecurity.InputFilter(Request.Params("Search").ToString, PortalSecurity.FilterFlag.NoScripting Or PortalSecurity.FilterFlag.NoMarkup)
            Else
                _SearchQuery = ""
            End If

            If _SearchQuery.Length > 0 Then
                If Not Page.IsPostBack Then
                    BindData()
                End If
            Else
                lblMessage.Text = String.Format(Localization.GetString("NoSearch", LocalResourceFile), _SearchQuery)
                ctlPagingControl.Visible = False
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' dgResults_PageIndexChanged runs when one of the Page buttons is clicked
        ''' </summary>
        ''' <history>
        '''     [cnurse]    12/13/2004  created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub dgResults_PageIndexChanged(ByVal source As Object, ByVal e As System.Web.UI.WebControls.DataGridPageChangedEventArgs) Handles dgResults.PageIndexChanged
            dgResults.CurrentPageIndex = e.NewPageIndex
            BindData()
        End Sub

#End Region

        Protected Sub ctlPagingControl_PageChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ctlPagingControl.PageChanged
            CurrentPage = ctlPagingControl.CurrentPage

            dgResults.CurrentPageIndex = CurrentPage - 1
            BindData()
        End Sub
    End Class

End Namespace
