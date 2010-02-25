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

Imports System.Reflection

Namespace DotNetNuke.Services.Search

    ''' -----------------------------------------------------------------------------
    ''' Namespace:  DotNetNuke.Services.Search
    ''' Project:    DotNetNuke
    ''' Class:      SearchEngine
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The SearchEngine  manages the Indexing of the Portal content
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''		[cnurse]	11/15/2004	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class SearchEngine

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IndexContent indexes all Portal content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''		[cnurse]	11/15/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub IndexContent()
            Dim Indexer As IndexingProvider = IndexingProvider.Instance

            SearchDataStoreProvider.Instance.StoreSearchItems(GetContent(Indexer))
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' IndexContent indexes the Portal's content
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <history>
        '''		[cnurse]	11/15/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub IndexContent(ByVal PortalID As Integer)
            Dim Indexer As IndexingProvider = IndexingProvider.Instance

            SearchDataStoreProvider.Instance.StoreSearchItems(GetContent(PortalID, Indexer))
        End Sub

#End Region


#Region "Protected Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetContent gets all the content and passes it to the Indexer
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="Indexer">The Index Provider that will index the content of the portal</param>
        ''' <history>
        '''		[cnurse]	11/15/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function GetContent(ByVal Indexer As IndexingProvider) As SearchItemInfoCollection
            Dim SearchItems As New SearchItemInfoCollection
            Dim objPortals As New PortalController
            Dim objPortal As PortalInfo

            Dim arrPortals As ArrayList = objPortals.GetPortals
            Dim intPortal As Integer
            For intPortal = 0 To arrPortals.Count - 1
                objPortal = CType(arrPortals(intPortal), PortalInfo)

                SearchItems.AddRange(Indexer.GetSearchIndexItems(objPortal.PortalID))

            Next
            Return SearchItems
        End Function

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetContent gets the Portal's content and passes it to the Indexer
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="PortalID">The Id of the Portal</param>
        ''' <param name="Indexer">The Index Provider that will index the content of the portal</param>
        ''' <history>
        '''		[cnurse]	11/15/2004	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Protected Function GetContent(ByVal PortalID As Integer, ByVal Indexer As IndexingProvider) As SearchItemInfoCollection
            Dim SearchItems As New SearchItemInfoCollection

            SearchItems.AddRange(Indexer.GetSearchIndexItems(PortalID))

            Return SearchItems
        End Function

#End Region

    End Class
End Namespace

