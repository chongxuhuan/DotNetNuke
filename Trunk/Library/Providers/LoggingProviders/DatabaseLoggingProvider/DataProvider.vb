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
Imports System.Web.Caching
Imports System.Reflection
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Services.Log.EventLog.DBLoggingProvider
Imports DotNetNuke.Framework.Providers

Namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider.Data

    Public MustInherit Class DataProvider

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Shadows Function Instance() As DataProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of DataProvider)()
        End Function

#End Region

#Region "Abstract Methods"

        Public MustOverride Sub AddLog(ByVal LogGUID As String, ByVal LogTypeKey As String, ByVal LogUserID As Integer, ByVal LogUserName As String, ByVal LogPortalID As Integer, ByVal LogPortalName As String, ByVal LogCreateDate As Date, ByVal LogServerName As String, ByVal LogProperties As String, ByVal LogConfigID As Integer)
        Public MustOverride Sub DeleteLog(ByVal LogGUID As String)
        Public MustOverride Sub PurgeLog()
        Public MustOverride Sub ClearLog()

        Public MustOverride Sub AddLogTypeConfigInfo(ByVal LoggingIsActive As Boolean, ByVal LogTypeKey As String, ByVal LogTypePortalID As String, ByVal KeepMostRecent As Integer, ByVal EmailNotificationIsActive As Boolean, ByVal Threshold As Integer, ByVal NotificationThresholdTime As Integer, ByVal NotificationThresholdTimeType As Integer, ByVal MailFromAddress As String, ByVal MailToAddress As String)
        Public MustOverride Sub DeleteLogTypeConfigInfo(ByVal ID As String)
        Public MustOverride Function GetLogTypeConfigInfo() As IDataReader
        Public MustOverride Function GetLogTypeConfigInfoByID(ByVal ID As Integer) As IDataReader
        Public MustOverride Sub UpdateLogTypeConfigInfo(ByVal ID As String, ByVal LoggingIsActive As Boolean, ByVal LogTypeKey As String, ByVal LogTypePortalID As String, ByVal KeepMostRecent As Integer, ByVal LogFileName As String, ByVal EmailNotificationIsActive As Boolean, ByVal Threshold As Integer, ByVal NotificationThresholdTime As Integer, ByVal NotificationThresholdTimeType As Integer, ByVal MailFromAddress As String, ByVal MailToAddress As String)

        Public MustOverride Sub AddLogType(ByVal LogTypeKey As String, ByVal LogTypeFriendlyName As String, ByVal LogTypeDescription As String, ByVal LogTypeCSSClass As String, ByVal LogTypeOwner As String)
        Public MustOverride Sub UpdateLogType(ByVal LogTypeKey As String, ByVal LogTypeFriendlyName As String, ByVal LogTypeDescription As String, ByVal LogTypeCSSClass As String, ByVal LogTypeOwner As String)
        Public MustOverride Sub DeleteLogType(ByVal LogTypeKey As String)
        Public MustOverride Function GetLogTypeInfo() As IDataReader

        Public MustOverride Function GetLog() As IDataReader
        Public MustOverride Function GetLog(ByVal PortalID As Integer) As IDataReader
        Public MustOverride Function GetLog(ByVal PortalID As Integer, ByVal LogType As String) As IDataReader
        Public MustOverride Function GetLog(ByVal LogType As String) As IDataReader

        Public MustOverride Function GetLog(ByVal PageSize As Integer, ByVal PageIndex As Integer) As IDataReader
        Public MustOverride Function GetLog(ByVal PortalID As Integer, ByVal PageSize As Integer, ByVal PageIndex As Integer) As IDataReader
        Public MustOverride Function GetLog(ByVal PortalID As Integer, ByVal LogType As String, ByVal PageSize As Integer, ByVal PageIndex As Integer) As IDataReader
        Public MustOverride Function GetLog(ByVal LogType As String, ByVal PageSize As Integer, ByVal PageIndex As Integer) As IDataReader

        Public MustOverride Function GetSingleLog(ByVal LogGUID As String) As IDataReader
        Public MustOverride Function GetEventLogPendingNotifConfig() As IDataReader
        Public MustOverride Function GetEventLogPendingNotif(ByVal LogConfigID As Integer) As IDataReader
        Public MustOverride Sub UpdateEventLogPendingNotif(ByVal LogConfigID As Integer)

#End Region

    End Class

End Namespace
