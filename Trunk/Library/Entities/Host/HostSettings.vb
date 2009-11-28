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
Imports System.Configuration
Imports System.Data
Imports System.Web
Imports System.Collections
Imports System.IO
Imports System.Web.UI
Imports System.Collections.Generic

Namespace DotNetNuke.Entities.Host

    <Obsolete("Replaced in DotNetNuke 5.0 by Host class because of Namespace clashes with Obsolete Globals.HostSetting Property")> _
    Public Class HostSettings
        Inherits BaseEntityInfo

        Public Shared Function GetHostSetting(ByVal key As String) As String
            Dim setting As String = Null.NullString
            Host.GetHostSettingsDictionary.TryGetValue(key, setting)
            Return setting
        End Function

        <Obsolete("Replaced in DotNetNuke 5.0 by Host.GetHostSettingDictionary")> _
        Public Shared Function GetHostSettings() As Hashtable
            Dim h As New Hashtable
            For Each kvp As KeyValuePair(Of String, String) In Host.GetHostSettingsDictionary()
                h.Add(kvp.Key, kvp.Value)
            Next
            Return h
        End Function

        <Obsolete("Replaced in DotNetNuke 5.0 by Host.GetSecureHostSettingDictionary")> _
        Public Shared Function GetSecureHostSettings() As Hashtable
            Dim h As New Hashtable
            For Each kvp As KeyValuePair(Of String, String) In Host.GetSecureHostSettingsDictionary()
                h.Add(kvp.Key, kvp.Value)
            Next
            Return h
        End Function

    End Class

End Namespace
