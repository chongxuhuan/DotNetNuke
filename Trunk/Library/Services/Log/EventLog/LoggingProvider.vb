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

Namespace DotNetNuke.Services.Log.EventLog

	Public MustInherit Class LoggingProvider

		Public Enum ReturnType
			LogInfoObjects
			XML
		End Enum

#Region "Shared/Static Methods"

        ' return the provider
        Public Shared Function Instance() As LoggingProvider
            Return DotNetNuke.ComponentModel.ComponentFactory.GetComponent(Of LoggingProvider)()
        End Function

#End Region

#Region "Abstract Methods"

		' methods to return functionality support indicators
		Public MustOverride Function SupportsEmailNotification() As Boolean
		Public MustOverride Function SupportsInternalViewer() As Boolean
		Public MustOverride Function LoggingIsEnabled(ByVal LogType As String, ByVal PortalID As Integer) As Boolean
		Public MustOverride Function SupportsSendToCoreTeam() As Boolean
		Public MustOverride Function SupportsSendViaEmail() As Boolean

		' method to add a log entry
        Public MustOverride Sub AddLog(ByVal LogInfo As LogInfo)
        Public MustOverride Sub DeleteLog(ByVal LogInfo As LogInfo)
        Public MustOverride Sub ClearLog()
        Public MustOverride Sub PurgeLogBuffer()
        Public MustOverride Sub SendLogNotifications()

        ' methods to get the log configuration info
        Public MustOverride Function GetLogTypeConfigInfo() As ArrayList
        Public MustOverride Function GetLogTypeConfigInfoByID(ByVal ID As String) As LogTypeConfigInfo
        Public MustOverride Function GetLogTypeInfo() As ArrayList

        Public MustOverride Sub AddLogTypeConfigInfo(ByVal ID As String, ByVal LoggingIsActive As Boolean, ByVal LogTypeKey As String, ByVal LogTypePortalID As String, ByVal KeepMostRecent As String, ByVal LogFileName As String, ByVal EmailNotificationIsActive As Boolean, ByVal Threshold As String, ByVal NotificationThresholdTime As String, ByVal NotificationThresholdTimeType As String, ByVal MailFromAddress As String, ByVal MailToAddress As String)
        Public MustOverride Sub UpdateLogTypeConfigInfo(ByVal ID As String, ByVal LoggingIsActive As Boolean, ByVal LogTypeKey As String, ByVal LogTypePortalID As String, ByVal KeepMostRecent As String, ByVal LogFileName As String, ByVal EmailNotificationIsActive As Boolean, ByVal Threshold As String, ByVal NotificationThresholdTime As String, ByVal NotificationThresholdTimeType As String, ByVal MailFromAddress As String, ByVal MailToAddress As String)
        Public MustOverride Sub DeleteLogTypeConfigInfo(ByVal ID As String)

        Public MustOverride Sub AddLogType(ByVal LogTypeKey As String, ByVal LogTypeFriendlyName As String, ByVal LogTypeDescription As String, ByVal LogTypeCSSClass As String, ByVal LogTypeOwner As String)
        Public MustOverride Sub UpdateLogType(ByVal LogTypeKey As String, ByVal LogTypeFriendlyName As String, ByVal LogTypeDescription As String, ByVal LogTypeCSSClass As String, ByVal LogTypeOwner As String)
        Public MustOverride Sub DeleteLogType(ByVal LogTypeKey As String)

        ' methods to get the log entries
        Public MustOverride Function GetSingleLog(ByVal LogInfo As LogInfo, ByVal objReturnType As ReturnType) As Object
        Public MustOverride Function GetLog() As LogInfoArray
        Public MustOverride Function GetLog(ByVal LogType As String) As LogInfoArray
        Public MustOverride Function GetLog(ByVal PortalID As Integer) As LogInfoArray
        Public MustOverride Function GetLog(ByVal PortalID As Integer, ByVal LogType As String) As LogInfoArray

        Public MustOverride Function GetLog(ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray
        Public MustOverride Function GetLog(ByVal LogType As String, ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray
        Public MustOverride Function GetLog(ByVal PortalID As Integer, ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray
        Public MustOverride Function GetLog(ByVal PortalID As Integer, ByVal LogType As String, ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray

#End Region

	End Class

End Namespace
