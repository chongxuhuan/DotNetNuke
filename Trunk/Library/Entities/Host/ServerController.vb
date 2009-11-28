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
Imports System.Collections.Generic
Imports DotNetNuke.Common

Namespace DotNetNuke.Entities.Host
    Public Class ServerController

#Region "Private Members"

        Private Const cacheKey As String = "WebServers"
        Private Const cacheTimeout As Integer = 20
        Private Const cachePriority As System.Web.Caching.CacheItemPriority = Caching.CacheItemPriority.High
        Private Shared dataProvider As DataProvider = dataProvider.Instance()

#End Region

#Region "Private Methods"

        Private Shared Function GetServersCallBack(ByVal cacheItemArgs As CacheItemArgs) As Object
            Return CBO.FillCollection(Of ServerInfo)(dataProvider.GetServers())
        End Function

#End Region

#Region "Public Properties"

        Public Shared ReadOnly Property UseAppName() As Boolean
            Get
                Dim uniqueServers As New Dictionary(Of String, String)
                For Each server As ServerInfo In GetEnabledServers()
                    uniqueServers(server.ServerName) = server.IISAppName
                Next
                Return uniqueServers.Count < GetEnabledServers.Count
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Shared Sub ClearCachedServers()
            DataCache.RemoveCache(cacheKey)
        End Sub

        Public Shared Sub DeleteServer(ByVal serverID As Integer)
            dataProvider.Instance.DeleteServer(serverID)
            ClearCachedServers()
        End Sub

        Public Shared Function GetEnabledServers() As List(Of ServerInfo)
            Dim servers As New List(Of ServerInfo)
            For Each server As ServerInfo In GetServers()
                If server.Enabled Then
                    servers.Add(server)
                End If
            Next
            Return servers
        End Function

        Public Shared Function GetExecutingServerName() As String
            Dim executingServerName As String = ServerName
            If UseAppName Then
                executingServerName += "-" + IISAppName
            End If
            Return executingServerName
        End Function

        Public Shared Function GetServerName(ByVal webServer As ServerInfo) As String
            Dim serverName As String = webServer.ServerName
            If UseAppName Then
                serverName += "-" + webServer.IISAppName
            End If
            Return serverName
        End Function

        Public Shared Function GetServers() As List(Of ServerInfo)
            Dim servers As List(Of ServerInfo) = CBO.GetCachedObject(Of List(Of ServerInfo)) _
                                                        (New CacheItemArgs(cacheKey, cacheTimeout, cachePriority), _
                                                        AddressOf GetServersCallBack)
            Return servers
        End Function

        Public Shared Sub UpdateServer(ByVal server As ServerInfo)
            dataProvider.Instance.UpdateServer(server.ServerID, server.Url, server.Enabled)
            ClearCachedServers()
        End Sub

        Public Shared Sub UpdateServerActivity(ByVal server As ServerInfo)
            dataProvider.Instance.UpdateServerActivity(server.ServerName, server.IISAppName, server.CreatedDate, server.LastActivityDate)
            ClearCachedServers()
        End Sub

#End Region

    End Class
End Namespace
