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

Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Common.Lists

    Public Class ListController

#Region "Private Methods"

        Private Sub ClearCache(ByVal PortalId As Integer)
            DataCache.ClearListsCache(PortalId)
        End Sub

        Private Function FillListInfo(ByVal dr As IDataReader, ByVal CheckForOpenDataReader As Boolean) As ListInfo
            Dim objListInfo As ListInfo = Nothing
            ' read datareader
            Dim canContinue As Boolean = True
            If CheckForOpenDataReader Then
                canContinue = False
                If dr.Read Then
                    canContinue = True
                End If
            End If

            If canContinue Then
                objListInfo = New ListInfo(Convert.ToString(dr("ListName")))
                With objListInfo
                    .Level = Convert.ToInt32(dr("Level"))
                    .PortalID = Convert.ToInt32(dr("PortalID"))
                    .DefinitionID = Convert.ToInt32(dr("DefinitionID"))
                    .EntryCount = Convert.ToInt32(dr("EntryCount"))
                    .ParentID = Convert.ToInt32(dr("ParentID"))
                    .ParentKey = Convert.ToString(dr("ParentKey"))
                    .Parent = Convert.ToString(dr("Parent"))
                    .ParentList = Convert.ToString(dr("ParentList"))
                    .EnableSortOrder = (Convert.ToInt32(dr("MaxSortOrder")) > 0)
                    .SystemList = Convert.ToBoolean(dr("SystemList"))
                End With
            End If
            Return objListInfo

        End Function

        Private Function FillListInfoDictionary(ByVal dr As IDataReader) As Dictionary(Of String, ListInfo)
            Dim dic As New Dictionary(Of String, ListInfo)
            Try
                Dim obj As ListInfo
                While dr.Read
                    ' fill business object
                    obj = FillListInfo(dr, False)

                    If Not dic.ContainsKey(obj.Key) Then
                        dic.Add(obj.Key, obj)
                    End If

                End While
            Catch exc As Exception
                LogException(exc)
            Finally
                ' close datareader
                CBO.CloseDataReader(dr, True)
            End Try

            Return dic
        End Function

        Private Function GetListInfoDictionaryCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Dim portalId As Integer = DirectCast(cacheItemArgs.ParamList(0), Integer)
            Return FillListInfoDictionary(DataProvider.Instance().GetLists(portalId))
        End Function

        Private Function GetListInfoDictionary(ByVal PortalId As Integer) As Dictionary(Of String, ListInfo)

            Dim cacheKey As String = String.Format(DataCache.ListsCacheKey, PortalId.ToString())
            Return CBO.GetCachedObject(Of Dictionary(Of String, ListInfo))(New CacheItemArgs(cacheKey, DataCache.ListsCacheTimeOut, DataCache.ListsCachePriority, PortalId), _
                                                                                                         AddressOf GetListInfoDictionaryCallBack)
        End Function

#End Region

#Region "Public Methods"

        Public Function AddListEntry(ByVal ListEntry As ListEntryInfo) As Integer
            Dim EnableSortOrder As Boolean = (ListEntry.SortOrder > 0)
            ClearCache(ListEntry.PortalID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(ListEntry, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LISTENTRY_CREATED)
            Return DataProvider.Instance().AddListEntry(ListEntry.ListName, ListEntry.Value, ListEntry.Text, ListEntry.ParentID, ListEntry.Level, EnableSortOrder, ListEntry.DefinitionID, ListEntry.Description, ListEntry.PortalID, UserController.GetCurrentUserInfo.UserID)
        End Function

        Public Sub DeleteList(ByVal ListName As String, ByVal ParentKey As String)
            Dim list As ListInfo = GetListInfo(ListName, ParentKey)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("ListName", ListName.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.LISTENTRY_DELETED)

            DataProvider.Instance().DeleteList(ListName, ParentKey)
            ClearCache(list.PortalID)
        End Sub

        Public Sub DeleteList(ByVal list As ListInfo, ByVal includeChildren As Boolean)
            Dim lists As New SortedList(Of String, ListInfo)
            lists.Add(list.Key, list)
            'add Children
            If includeChildren Then
                For Each listPair As KeyValuePair(Of String, ListInfo) In GetListInfoDictionary(list.PortalID)
                    If (listPair.Value.ParentList.StartsWith(list.Key)) Then
                        lists.Add(listPair.Value.Key.Replace(":", "."), listPair.Value)
                    End If
                Next

            End If

            'Delete items in reverse order so deeper descendants are removed before their parents
            For i As Integer = lists.Count - 1 To 0 Step -1
                DeleteList(lists.Values(i).Name, lists.Values(i).ParentKey)
            Next
        End Sub

        Public Sub DeleteListEntryByID(ByVal EntryID As Integer, ByVal DeleteChild As Boolean)
            Dim entry As ListEntryInfo = GetListEntryInfo(EntryID)
            DataProvider.Instance().DeleteListEntryByID(EntryID, DeleteChild)
            ClearCache(entry.PortalID)
        End Sub

        Public Sub DeleteListEntryByListName(ByVal ListName As String, ByVal Value As String, ByVal DeleteChild As Boolean)
            Dim entry As ListEntryInfo = GetListEntryInfo(ListName, Value)
            DataProvider.Instance().DeleteListEntryByListName(ListName, Value, DeleteChild)
            ClearCache(entry.PortalID)
        End Sub

        Public Function GetListEntryInfo(ByVal EntryID As Integer) As ListEntryInfo ' Get single entry by ID
            Return CType(CBO.FillObject(DataProvider.Instance().GetListEntry(EntryID), GetType(ListEntryInfo)), ListEntryInfo)
        End Function

        Public Function GetListEntryInfo(ByVal ListName As String, ByVal Value As String) As ListEntryInfo ' Get single entry by ListName/Value
            Return CType(CBO.FillObject(DataProvider.Instance().GetListEntry(ListName, Value), GetType(ListEntryInfo)), ListEntryInfo)
        End Function

        Public Function GetListEntryInfoCollection(ByVal ListName As String) As ListEntryInfoCollection ' Get collection of entry lists
            Return GetListEntryInfoCollection(ListName, "", Null.NullInteger)
        End Function

        Public Function GetListEntryInfoCollection(ByVal ListName As String, ByVal ParentKey As String) As ListEntryInfoCollection ' Get collection of entry lists
            Return GetListEntryInfoCollection(ListName, ParentKey, Null.NullInteger)
        End Function

        Public Function GetListEntryInfoCollection(ByVal ListName As String, ByVal ParentKey As String, ByVal PortalId As Integer) As ListEntryInfoCollection ' Get collection of entry lists
            Dim objListEntryInfoCollection As New ListEntryInfoCollection
            Dim arrListEntries As ArrayList = CBO.FillCollection(DataProvider.Instance().GetListEntriesByListName(ListName, ParentKey, PortalId), GetType(ListEntryInfo))
            For Each entry As ListEntryInfo In arrListEntries
                objListEntryInfoCollection.Add(entry.Key, entry)
            Next
            Return objListEntryInfoCollection
        End Function

        Public Function GetListInfo(ByVal ListName As String) As ListInfo
            Return GetListInfo(ListName, "")
        End Function

        Public Function GetListInfo(ByVal ListName As String, ByVal ParentKey As String) As ListInfo
            Return GetListInfo(ListName, ParentKey, -1)
        End Function

        Public Function GetListInfo(ByVal ListName As String, ByVal ParentKey As String, ByVal PortalID As Integer) As ListInfo
            Dim list As ListInfo = Nothing
            Dim key As String = Null.NullString
            If Not String.IsNullOrEmpty(ParentKey) Then
                key = ParentKey + ":"
            End If
            key += ListName

            Dim dicLists As Dictionary(Of String, ListInfo) = GetListInfoDictionary(PortalID)

            If Not dicLists.TryGetValue(key, list) Then
                Dim dr As IDataReader = DataProvider.Instance().GetList(ListName, ParentKey, PortalID)
                Try
                    list = FillListInfo(dr, True)
                Finally
                    CBO.CloseDataReader(dr, True)
                End Try
            End If
            Return list
        End Function

        Public Function GetListInfoCollection() As ListInfoCollection
            Return GetListInfoCollection("")
        End Function

        Public Function GetListInfoCollection(ByVal ListName As String) As ListInfoCollection
            Return GetListInfoCollection(ListName, "")
        End Function

        Public Function GetListInfoCollection(ByVal ListName As String, ByVal ParentKey As String) As ListInfoCollection
            Return GetListInfoCollection(ListName, ParentKey, -1)
        End Function

        Public Function GetListInfoCollection(ByVal ListName As String, ByVal ParentKey As String, ByVal PortalID As Integer) As ListInfoCollection
            Dim lists As IList = New ListInfoCollection

            For Each listPair As KeyValuePair(Of String, ListInfo) In GetListInfoDictionary(PortalID)
                Dim list As ListInfo = listPair.Value
                If (list.Name = ListName OrElse String.IsNullOrEmpty(ListName)) AndAlso _
                            (list.ParentKey = ParentKey OrElse String.IsNullOrEmpty(ParentKey)) AndAlso _
                            (list.PortalID = PortalID OrElse PortalID = Null.NullInteger) Then
                    lists.Add(list)
                End If
            Next
            Return CType(lists, ListInfoCollection)
        End Function

        Public Sub UpdateListEntry(ByVal ListEntry As ListEntryInfo)
            DataProvider.Instance().UpdateListEntry(ListEntry.EntryID, ListEntry.Value, ListEntry.Text, ListEntry.Description, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(ListEntry, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.LISTENTRY_UPDATED)
            ClearCache(ListEntry.PortalID)
        End Sub

        Public Sub UpdateListSortOrder(ByVal EntryID As Integer, ByVal MoveUp As Boolean)
            DataProvider.Instance().UpdateListSortOrder(EntryID, MoveUp)

            Dim entry As ListEntryInfo = GetListEntryInfo(EntryID)
            ClearCache(entry.PortalID)
        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("This method has been deprecated. PLease use GetListEntryInfo(ByVal ListName As String, ByVal Value As String)")> _
        Public Function GetListEntryInfo(ByVal ListName As String, ByVal Value As String, ByVal ParentKey As String) As ListEntryInfo ' Get single entry by ListName/Value
            Return GetListEntryInfo(ListName, Value)
        End Function

        <Obsolete("This method has been deprecated. PLease use GetListEntryInfoCollection(ByVal ListName As String, ByVal ParentKey As String)")> _
        Public Function GetListEntryInfoCollection(ByVal ListName As String, ByVal Value As String, ByVal ParentKey As String) As ListEntryInfoCollection ' Get collection of entry lists
            Return GetListEntryInfoCollection(ListName, ParentKey)
        End Function

#End Region

    End Class

End Namespace

