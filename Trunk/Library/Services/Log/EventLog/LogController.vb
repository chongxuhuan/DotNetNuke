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
Imports System.IO
Imports System.Threading
Imports System.Xml.Serialization
Imports System.Xml


Namespace DotNetNuke.Services.Log.EventLog

    Public Class LogController

        Private Shared lockLog As New ReaderWriterLock
        Private Const ReaderLockTimeout As Integer = 10000    'milliseconds
        Private Const WriterLockTimeout As Integer = 10000    'milliseconds


        Public Sub AddLog(ByVal objLogInfo As LogInfo)

            If (Status = UpgradeStatus.Install) Then
                AddLogToFile(objLogInfo)
            Else
                Try
                    objLogInfo.LogCreateDate = Now
                    objLogInfo.LogServerName = Common.Globals.ServerName
                    If String.IsNullOrEmpty(objLogInfo.LogServerName) Then
                        objLogInfo.LogServerName = "NA"
                    End If

                    If objLogInfo.LogUserName = "" Then

                        If HttpContext.Current IsNot Nothing AndAlso HttpContext.Current.Request IsNot Nothing Then
                            If HttpContext.Current.Request.IsAuthenticated Then
                                Dim objUserInfo As UserInfo = UserController.GetCurrentUserInfo
                                objLogInfo.LogUserName = objUserInfo.Username
                            End If
                        End If
                    End If

                    LoggingProvider.Instance.AddLog(objLogInfo)
                Catch exc As Exception
                    AddLogToFile(objLogInfo)
                End Try
            End If
        End Sub

        Private Sub AddLogToFile(ByVal objLogInfo As LogInfo)

            Try
                Dim str As String = objLogInfo.Serialize
                Dim f As String
                f = Common.Globals.HostMapPath + "\Logs\LogFailures.xml.resources"
                WriteLog(f, str)
            Catch exc2 As Exception
                'critical error writing
            End Try
        End Sub

        Public Overridable Sub AddLogType(ByVal configFile As String, ByVal fallbackConfigFile As String)
            Dim xmlDoc As New XmlDocument
            Try
                xmlDoc.Load(configFile)
            Catch exc As FileNotFoundException
                xmlDoc.Load(fallbackConfigFile)
            End Try
            Dim LogType As XmlNodeList = xmlDoc.SelectNodes("/LogConfig/LogTypes/LogType")
            Dim LogTypeInfo As XmlNode
            For Each LogTypeInfo In LogType
                Dim objLogTypeInfo As New Log.EventLog.LogTypeInfo
                objLogTypeInfo.LogTypeKey = LogTypeInfo.Attributes("LogTypeKey").Value
                objLogTypeInfo.LogTypeFriendlyName = LogTypeInfo.Attributes("LogTypeFriendlyName").Value
                objLogTypeInfo.LogTypeDescription = LogTypeInfo.Attributes("LogTypeDescription").Value
                objLogTypeInfo.LogTypeCSSClass = LogTypeInfo.Attributes("LogTypeCSSClass").Value
                objLogTypeInfo.LogTypeOwner = LogTypeInfo.Attributes("LogTypeOwner").Value
                AddLogType(objLogTypeInfo)
            Next

            Dim LogTypeConfig As XmlNodeList = xmlDoc.SelectNodes("/LogConfig/LogTypeConfig")
            Dim LogTypeConfigInfo As XmlNode
            For Each LogTypeConfigInfo In LogTypeConfig
                Dim objLogTypeConfig As New Log.EventLog.LogTypeConfigInfo
                objLogTypeConfig.EmailNotificationIsActive = Convert.ToBoolean(IIf(LogTypeConfigInfo.Attributes("EmailNotificationStatus").Value = "On", True, False))
                objLogTypeConfig.KeepMostRecent = LogTypeConfigInfo.Attributes("KeepMostRecent").Value
                objLogTypeConfig.LoggingIsActive = Convert.ToBoolean(IIf(LogTypeConfigInfo.Attributes("LoggingStatus").Value = "On", True, False))
                objLogTypeConfig.LogTypeKey = LogTypeConfigInfo.Attributes("LogTypeKey").Value
                objLogTypeConfig.LogTypePortalID = LogTypeConfigInfo.Attributes("LogTypePortalID").Value
                objLogTypeConfig.MailFromAddress = LogTypeConfigInfo.Attributes("MailFromAddress").Value
                objLogTypeConfig.MailToAddress = LogTypeConfigInfo.Attributes("MailToAddress").Value
                objLogTypeConfig.NotificationThreshold = Convert.ToInt32(LogTypeConfigInfo.Attributes("NotificationThreshold").Value)
                objLogTypeConfig.NotificationThresholdTime = Convert.ToInt32(LogTypeConfigInfo.Attributes("NotificationThresholdTime").Value)
                objLogTypeConfig.NotificationThresholdTimeType = CType(LogTypeConfigInfo.Attributes("NotificationThresholdTimeType").Value, Services.Log.EventLog.LogTypeConfigInfo.NotificationThresholdTimeTypes)
                AddLogTypeConfigInfo(objLogTypeConfig)
            Next
        End Sub

        Public Overridable Sub AddLogType(ByVal objLogTypeInfo As LogTypeInfo)
            LoggingProvider.Instance.AddLogType(objLogTypeInfo.LogTypeKey, objLogTypeInfo.LogTypeFriendlyName, objLogTypeInfo.LogTypeDescription, objLogTypeInfo.LogTypeCSSClass, objLogTypeInfo.LogTypeOwner)
        End Sub

        Public Overridable Sub AddLogTypeConfigInfo(ByVal objLogTypeConfigInfo As LogTypeConfigInfo)
            LoggingProvider.Instance.AddLogTypeConfigInfo(objLogTypeConfigInfo.ID, objLogTypeConfigInfo.LoggingIsActive, objLogTypeConfigInfo.LogTypeKey, objLogTypeConfigInfo.LogTypePortalID, objLogTypeConfigInfo.KeepMostRecent, objLogTypeConfigInfo.LogFileName, objLogTypeConfigInfo.EmailNotificationIsActive, Convert.ToString(objLogTypeConfigInfo.NotificationThreshold), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTime), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTimeType), objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress)
        End Sub

        Public Sub ClearLog()
            LoggingProvider.Instance.ClearLog()
        End Sub

        Public Sub DeleteLog(ByVal objLogInfo As LogInfo)
            LoggingProvider.Instance.DeleteLog(objLogInfo)
        End Sub

        Public Overridable Sub DeleteLogType(ByVal objLogTypeInfo As LogTypeInfo)
            LoggingProvider.Instance.DeleteLogType(objLogTypeInfo.LogTypeKey)
        End Sub

        Public Overridable Sub DeleteLogTypeConfigInfo(ByVal objLogTypeConfigInfo As LogTypeConfigInfo)
            LoggingProvider.Instance.DeleteLogTypeConfigInfo(objLogTypeConfigInfo.ID)
        End Sub

        Public Overridable Function GetLog(ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray
            Return LoggingProvider.Instance.GetLog(PageSize, PageIndex, TotalRecords)
        End Function

        Public Overridable Function GetLog(ByVal PortalID As Integer, ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray
            Return LoggingProvider.Instance.GetLog(PortalID, PageSize, PageIndex, TotalRecords)
        End Function

        Public Overridable Function GetLog(ByVal PortalID As Integer, ByVal LogType As String, ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray
            Return LoggingProvider.Instance.GetLog(PortalID, LogType, PageSize, PageIndex, TotalRecords)
        End Function

        Public Overridable Function GetLog(ByVal LogType As String, ByVal PageSize As Integer, ByVal PageIndex As Integer, ByRef TotalRecords As Integer) As LogInfoArray
            Return LoggingProvider.Instance.GetLog(LogType, PageSize, PageIndex, TotalRecords)
        End Function

        Public Overridable Function GetLogTypeConfigInfo() As ArrayList
            Return CType(LoggingProvider.Instance.GetLogTypeConfigInfo(), ArrayList)
        End Function

        Public Overridable Function GetLogTypeConfigInfoByID(ByVal ID As String) As LogTypeConfigInfo
            Return LoggingProvider.Instance.GetLogTypeConfigInfoByID(ID)
        End Function

        Public Overridable Function GetLogTypeInfo() As ArrayList
            Return CType(LoggingProvider.Instance.GetLogTypeInfo(), ArrayList)
        End Function

        Public Overridable Function GetSingleLog(ByVal objLogInfo As LogInfo, ByVal objReturnType As LoggingProvider.ReturnType) As Object
            Return LoggingProvider.Instance.GetSingleLog(objLogInfo, objReturnType)
        End Function

        Public Function LoggingIsEnabled(ByVal LogType As String, ByVal PortalID As Integer) As Boolean
            Return LoggingProvider.Instance.LoggingIsEnabled(LogType, PortalID)
        End Function

        Public Sub PurgeLogBuffer()
            LoggingProvider.Instance.PurgeLogBuffer()
        End Sub

        Public Overridable Function SupportsEmailNotification() As Boolean
            Return LoggingProvider.Instance.SupportsEmailNotification()
        End Function

        Public Overridable Function SupportsInternalViewer() As Boolean
            Return LoggingProvider.Instance.SupportsInternalViewer()
        End Function

        Public Overridable Sub UpdateLogTypeConfigInfo(ByVal objLogTypeConfigInfo As LogTypeConfigInfo)
            LoggingProvider.Instance.UpdateLogTypeConfigInfo(objLogTypeConfigInfo.ID, objLogTypeConfigInfo.LoggingIsActive, objLogTypeConfigInfo.LogTypeKey, objLogTypeConfigInfo.LogTypePortalID, objLogTypeConfigInfo.KeepMostRecent, objLogTypeConfigInfo.LogFileName, objLogTypeConfigInfo.EmailNotificationIsActive, Convert.ToString(objLogTypeConfigInfo.NotificationThreshold), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTime), Convert.ToString(objLogTypeConfigInfo.NotificationThresholdTimeType), objLogTypeConfigInfo.MailFromAddress, objLogTypeConfigInfo.MailToAddress)
        End Sub

        Public Overridable Sub UpdateLogType(ByVal objLogTypeInfo As LogTypeInfo)
            LoggingProvider.Instance.UpdateLogType(objLogTypeInfo.LogTypeKey, objLogTypeInfo.LogTypeFriendlyName, objLogTypeInfo.LogTypeDescription, objLogTypeInfo.LogTypeCSSClass, objLogTypeInfo.LogTypeOwner)
        End Sub

#Region "Private Methods"

        Private Sub WriteLog(ByVal FilePath As String, ByVal Message As String)

            '--------------------------------------------------------------
            'Write the log entry
            '--------------------------------------------------------------
            Dim fs As FileStream = Nothing
            Dim sw As StreamWriter = Nothing

            Try

                '--------------------------------------------------------------
                ' Write the entry to the log. 
                '--------------------------------------------------------------
                lockLog.AcquireWriterLock(WriterLockTimeout)
                Dim intAttempts As Integer
                'wait for up to 100 milliseconds for the file
                'to be unlocked if it is not available
                Do Until Not fs Is Nothing Or intAttempts = 100
                    intAttempts += 1
                    Try
                        fs = New FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None)
                    Catch exc As IOException
                        Thread.Sleep(1)
                    End Try
                Loop

                If fs Is Nothing Then
                    If Not HttpContext.Current Is Nothing Then
                        HttpContext.Current.Response.Write("An error has occurred writing to the exception log.")
                        HttpContext.Current.Response.End()
                    End If
                Else
                    '--------------------------------------------------------------
                    'Instantiate a new StreamWriter
                    '--------------------------------------------------------------
                    sw = New StreamWriter(fs, System.Text.Encoding.UTF8)
                    Dim FileLength As Long
                    FileLength = fs.Length
                    '--------------------------------------------------------------
                    'check to see if this file is new
                    '--------------------------------------------------------------
                    If FileLength > 0 Then
                        '--------------------------------------------------------------
                        'file is not new, set the position to just before
                        'the closing root element tag
                        '--------------------------------------------------------------
                        fs.Position = FileLength - 9
                    Else
                        '--------------------------------------------------------------
                        'file is new, create the opening root element tag
                        '--------------------------------------------------------------
                        Message = "<logs>" + Message
                    End If

                    '--------------------------------------------------------------
                    'write out our exception
                    '--------------------------------------------------------------
                    sw.WriteLine(Message + "</logs>")
                    sw.Flush()
                End If
                If Not sw Is Nothing Then
                    sw.Close()
                End If
                If Not fs Is Nothing Then
                    fs.Close()
                End If
                '--------------------------------------------------------------
                'handle the more common exceptions up 
                'front, leave less common ones to the end
                '--------------------------------------------------------------
            Catch exc As UnauthorizedAccessException
                If Not HttpContext.Current Is Nothing Then
                    Dim response As HttpResponse = HttpContext.Current.Response
                    HtmlUtils.WriteHeader(response, "Unauthorized Access Error")

                    Dim strMessage As String = exc.Message & " The Windows User Account listed below must have Read/Write Privileges to this path."
                    HtmlUtils.WriteError(response, FilePath, strMessage)

                    HtmlUtils.WriteFooter(response)
                    response.End()
                End If
            Catch exc As DirectoryNotFoundException
                If Not HttpContext.Current Is Nothing Then
                    Dim response As HttpResponse = HttpContext.Current.Response
                    HtmlUtils.WriteHeader(response, "Directory Not Found Error")

                    Dim strMessage As String = exc.Message
                    HtmlUtils.WriteError(response, FilePath, strMessage)

                    HtmlUtils.WriteFooter(response)
                    response.End()
                End If
            Catch exc As PathTooLongException
                If Not HttpContext.Current Is Nothing Then
                    Dim response As HttpResponse = HttpContext.Current.Response
                    HtmlUtils.WriteHeader(response, "Path Too Long Error")

                    Dim strMessage As String = exc.Message
                    HtmlUtils.WriteError(response, FilePath, strMessage)

                    HtmlUtils.WriteFooter(response)
                    response.End()
                End If
            Catch exc As IOException
                If Not HttpContext.Current Is Nothing Then
                    Dim response As HttpResponse = HttpContext.Current.Response
                    HtmlUtils.WriteHeader(response, "IO Error")

                    Dim strMessage As String = exc.Message
                    HtmlUtils.WriteError(response, FilePath, strMessage)

                    HtmlUtils.WriteFooter(response)
                    response.End()
                End If
            Catch exc As Exception
                If Not HttpContext.Current Is Nothing Then
                    Dim response As HttpResponse = HttpContext.Current.Response
                    HtmlUtils.WriteHeader(response, "Unhandled Error")

                    Dim strMessage As String = exc.Message
                    HtmlUtils.WriteError(response, FilePath, strMessage)

                    HtmlUtils.WriteFooter(response)
                    response.End()
                End If
            Finally
                If Not sw Is Nothing Then
                    sw.Close()
                End If
                If Not fs Is Nothing Then
                    fs.Close()
                End If

                lockLog.ReleaseWriterLock()

            End Try

        End Sub

#End Region

#Region "Obsolete Methods"

        <Obsolete("This method has been replaced with one that supports record paging.")> _
        Public Overridable Function GetLog() As LogInfoArray
            Return LoggingProvider.Instance.GetLog()
        End Function

        <Obsolete("This method has been replaced with one that supports record paging.")> _
        Public Overridable Function GetLog(ByVal PortalID As Integer) As LogInfoArray
            Return LoggingProvider.Instance.GetLog(PortalID)
        End Function

        <Obsolete("This method has been replaced with one that supports record paging.")> _
        Public Overridable Function GetLog(ByVal PortalID As Integer, ByVal LogType As String) As LogInfoArray
            Return LoggingProvider.Instance.GetLog(PortalID, LogType)
        End Function

        <Obsolete("This method has been replaced with one that supports record paging.")> _
        Public Overridable Function GetLog(ByVal LogType As String) As LogInfoArray
            Return LoggingProvider.Instance.GetLog(LogType)
        End Function

#End Region


    End Class

End Namespace