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
Imports System.Data
Imports DotNetNuke
Imports DotNetNuke.Entities.Tabs

Namespace DotNetNuke.Entities.Portals

    Public Class PortalAliasController

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' GetPortalAliasLookupCallBack gets a Dictionary of Host Settings from 
        ''' the Database.
        ''' </summary>
        ''' <param name="cacheItemArgs">The CacheItemArgs object that contains the parameters
        ''' needed for the database call</param>
        ''' <history>
        ''' 	[cnurse]	07/15/2008   Created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function GetPortalAliasLookupCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return New PortalAliasController().GetPortalAliases()
        End Function

#Region "Public Shared methods"

        Public Shared Function GetPortalAliasByPortal(ByVal PortalId As Integer, ByVal PortalAlias As String) As String
            Dim retValue As String = ""

            ' get the portal alias collection from the cache
            Dim objPortalAliasCollection As PortalAliasCollection = PortalAliasController.GetPortalAliasLookup()
            Dim strHTTPAlias As String
            Dim bFound As Boolean = False

            'Do a specified PortalAlias check first
            Dim objPortalAliasInfo As PortalAliasInfo = objPortalAliasCollection(PortalAlias.ToLower)
            If Not objPortalAliasInfo Is Nothing Then
                If objPortalAliasInfo.PortalID = PortalId Then
                    ' set the alias
                    retValue = objPortalAliasInfo.HTTPAlias
                    bFound = True
                End If
            End If

            'No match so iterate through the alias keys
            If Not bFound Then
                For Each key As String In objPortalAliasCollection.Keys
                    ' check if the alias key starts with the portal alias value passed in - we use
                    ' StartsWith because child portals are redirected to the parent portal domain name
                    ' eg. child = 'www.domain.com/child' and parent is 'www.domain.com'
                    ' this allows the parent domain name to resolve to the child alias ( the tabid still identifies the child portalid )
                    objPortalAliasInfo = objPortalAliasCollection(key)

                    strHTTPAlias = objPortalAliasInfo.HTTPAlias.ToLower()
                    If strHTTPAlias.StartsWith(PortalAlias.ToLower) = True And objPortalAliasInfo.PortalID = PortalId Then
                        ' set the alias
                        retValue = objPortalAliasInfo.HTTPAlias
                        Exit For
                    End If

                    ' domain.com and www.domain.com should be synonymous
                    If strHTTPAlias.StartsWith("www.") Then
                        ' try alias without the "www." prefix
                        strHTTPAlias = strHTTPAlias.Replace("www.", "")
                    Else ' try the alias with the "www." prefix
                        strHTTPAlias = String.Concat("www.", strHTTPAlias)
                    End If
                    If strHTTPAlias.StartsWith(PortalAlias.ToLower) = True And objPortalAliasInfo.PortalID = PortalId Then
                        ' set the alias
                        retValue = objPortalAliasInfo.HTTPAlias
                        Exit For
                    End If
                Next
            End If

            Return retValue
        End Function

        Public Shared Function GetPortalAliasByTab(ByVal TabID As Integer, ByVal PortalAlias As String) As String
            Dim retValue As String = Null.NullString
            Dim intPortalId As Integer = -2

            ' get the tab
            Dim objTabs As New TabController
            Dim objTab As TabInfo = objTabs.GetTab(TabID, Null.NullInteger, False)
            If Not objTab Is Nothing Then
                ' ignore deleted tabs
                If Not objTab.IsDeleted Then
                    intPortalId = objTab.PortalID
                End If
            End If

            Select Case intPortalId
                Case -2 ' tab does not exist
                Case -1 ' host tab
                    ' host tabs are not verified to determine if they belong to the portal alias
                    retValue = PortalAlias
                Case Else ' portal tab
                    retValue = GetPortalAliasByPortal(intPortalId, PortalAlias)
            End Select

            Return retValue
        End Function

        Public Shared Function GetPortalAliasInfo(ByVal PortalAlias As String) As PortalAliasInfo

            Dim strPortalAlias As String

            ' try the specified alias first
            Dim objPortalAliasInfo As PortalAliasInfo = GetPortalAliasLookup(PortalAlias.ToLower)

            ' domain.com and www.domain.com should be synonymous
            If objPortalAliasInfo Is Nothing Then
                If PortalAlias.ToLower.StartsWith("www.") Then
                    ' try alias without the "www." prefix
                    strPortalAlias = PortalAlias.Replace("www.", "")
                Else ' try the alias with the "www." prefix
                    strPortalAlias = String.Concat("www.", PortalAlias)
                End If
                ' perform the lookup
                objPortalAliasInfo = GetPortalAliasLookup(strPortalAlias.ToLower)
            End If

            ' allow domain wildcards 
            If objPortalAliasInfo Is Nothing Then
                ' remove the domain prefix ( ie. anything.domain.com = domain.com )
                If PortalAlias.IndexOf(".") <> -1 Then
                    strPortalAlias = PortalAlias.Substring(PortalAlias.IndexOf(".") + 1)
                Else ' be sure we have a clean string (without leftovers from preceding 'if' block)
                    strPortalAlias = PortalAlias
                End If
                If objPortalAliasInfo Is Nothing Then
                    ' try an explicit lookup using the wildcard entry ( ie. *.domain.com )
                    objPortalAliasInfo = GetPortalAliasLookup("*." & strPortalAlias.ToLower)
                End If
                If objPortalAliasInfo Is Nothing Then
                    ' try a lookup using the raw domain
                    objPortalAliasInfo = GetPortalAliasLookup(strPortalAlias.ToLower)
                End If
                If objPortalAliasInfo Is Nothing Then
                    ' try a lookup using "www." + raw domain
                    objPortalAliasInfo = GetPortalAliasLookup("www." & strPortalAlias.ToLower)
                End If
            End If

            If objPortalAliasInfo Is Nothing Then
                ' check if this is a fresh install ( no alias values in collection )
                Dim objPortalAliasCollection As PortalAliasCollection = GetPortalAliasLookup()
                If Not objPortalAliasCollection.HasKeys OrElse _
                    (objPortalAliasCollection.Count = 1 And objPortalAliasCollection.Contains("_default")) Then
                    ' relate the PortalAlias to the default portal on a fresh database installation
                    DataProvider.Instance().UpdatePortalAlias(PortalAlias.ToLower, UserController.GetCurrentUserInfo.UserID)
                    Dim objEventLog As New Services.Log.EventLog.EventLogController
                    objEventLog.AddLog("PortalAlias", PortalAlias.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALALIAS_UPDATED)

                    'clear the cachekey "GetPortalByAlias" otherwise portalalias "_default" stays in cache after first install
                    DataCache.RemoveCache("GetPortalByAlias")

                    'try again
                    objPortalAliasInfo = GetPortalAliasLookup(PortalAlias.ToLower)
                End If
            End If

            Return objPortalAliasInfo

        End Function

        Public Shared Function GetPortalAliasLookup() As PortalAliasCollection
            Return CBO.GetCachedObject(Of PortalAliasCollection)(New CacheItemArgs(DataCache.PortalAliasCacheKey, _
                                                                                                DataCache.PortalAliasCacheTimeOut, _
                                                                                                DataCache.PortalAliasCachePriority), _
                                                                                                AddressOf GetPortalAliasLookupCallBack)
        End Function

#End Region

#Region "Public Methods"

        Public Function AddPortalAlias(ByVal objPortalAliasInfo As PortalAliasInfo) As Integer
            Dim Id As Integer = DataProvider.Instance().AddPortalAlias(objPortalAliasInfo.PortalID, objPortalAliasInfo.HTTPAlias.ToLower, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objPortalAliasInfo, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.PORTALALIAS_CREATED)
            'clear portal alias cache
            DataCache.RemoveCache(DataCache.PortalAliasCacheKey)

            Return Id
        End Function

        Public Sub DeletePortalAlias(ByVal PortalAliasID As Integer)
            DataCache.RemoveCache(DataCache.PortalAliasCacheKey)
            DataProvider.Instance().DeletePortalAlias(PortalAliasID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog("PortalAliasID", PortalAliasID.ToString, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, Log.EventLog.EventLogController.EventLogType.PORTALALIAS_DELETED)
        End Sub

        Public Function GetPortalAlias(ByVal PortalAlias As String, ByVal PortalID As Integer) As PortalAliasInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetPortalAlias(PortalAlias, PortalID), GetType(PortalAliasInfo)), PortalAliasInfo)
        End Function

        Public Function GetPortalAliasArrayByPortalID(ByVal PortalID As Integer) As ArrayList
            Dim dr As IDataReader = DataProvider.Instance().GetPortalAliasByPortalID(PortalID)
            Try
                Dim arr As New ArrayList
                While dr.Read
                    Dim objPortalAliasInfo As New PortalAliasInfo
                    objPortalAliasInfo.PortalAliasID = Convert.ToInt32(dr("PortalAliasID"))
                    objPortalAliasInfo.PortalID = Convert.ToInt32(dr("PortalID"))
                    objPortalAliasInfo.HTTPAlias = Convert.ToString(dr("HTTPAlias")).ToLower
                    arr.Add(objPortalAliasInfo)
                End While
                Return arr
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
        End Function

        Public Function GetPortalAliasByPortalAliasID(ByVal PortalAliasID As Integer) As PortalAliasInfo
            Return CType(CBO.FillObject((DataProvider.Instance().GetPortalAliasByPortalAliasID(PortalAliasID)), GetType(PortalAliasInfo)), PortalAliasInfo)
        End Function

        Public Function GetPortalAliasByPortalID(ByVal PortalID As Integer) As PortalAliasCollection
            Dim dr As IDataReader = DataProvider.Instance().GetPortalAliasByPortalID(PortalID)
            Try
                Dim objPortalAliasCollection As New PortalAliasCollection
                While dr.Read
                    Dim objPortalAliasInfo As New PortalAliasInfo
                    objPortalAliasInfo.PortalAliasID = Convert.ToInt32(dr("PortalAliasID"))
                    objPortalAliasInfo.PortalID = Convert.ToInt32(dr("PortalID"))
                    objPortalAliasInfo.HTTPAlias = Convert.ToString(dr("HTTPAlias"))
                    objPortalAliasCollection.Add(Convert.ToString(dr("HTTPAlias")).ToLower, objPortalAliasInfo)
                End While
                Return objPortalAliasCollection
            Finally
                CBO.CloseDataReader(dr, True)
            End Try
        End Function

        Public Function GetPortalAliases() As PortalAliasCollection
            Return GetPortalAliasByPortalID(-1)
        End Function

        Public Function GetPortalByPortalAliasID(ByVal PortalAliasId As Integer) As PortalInfo
            Return CType(CBO.FillObject(DataProvider.Instance().GetPortalByPortalAliasID(PortalAliasId), GetType(PortalInfo)), PortalInfo)
        End Function

        Public Sub UpdatePortalAliasInfo(ByVal objPortalAliasInfo As PortalAliasInfo)
            DataCache.RemoveCache(DataCache.PortalAliasCacheKey)

            DataProvider.Instance().UpdatePortalAliasInfo(objPortalAliasInfo.PortalAliasID, objPortalAliasInfo.PortalID, objPortalAliasInfo.HTTPAlias.ToLower, UserController.GetCurrentUserInfo.UserID)
            Dim objEventLog As New Services.Log.EventLog.EventLogController
            objEventLog.AddLog(objPortalAliasInfo, PortalController.GetCurrentPortalSettings, UserController.GetCurrentUserInfo.UserID, "", Log.EventLog.EventLogController.EventLogType.PORTALALIAS_UPDATED)
        End Sub

#End Region

    End Class

End Namespace
