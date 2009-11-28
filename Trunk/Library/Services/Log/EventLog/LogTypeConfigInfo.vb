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
Imports System.Xml.Serialization

Namespace DotNetNuke.Services.Log.EventLog

    <Serializable()> Public Class LogTypeConfigInfo
        Inherits LogTypeInfo

        Private _ID As String
        Private _LoggingIsActive As Boolean
        Private _LogFileName As String
        Private _LogFileNameWithPath As String
        Private _LogTypePortalID As String
        Private _KeepMostRecent As String
        Public Enum NotificationThresholdTimeTypes
            None = 0
            Seconds = 1
            Minutes = 2
            Hours = 3
            Days = 4
        End Enum

        Private _EmailNotificationIsActive As Boolean
        Private _MailFromAddress As String
        Private _MailToAddress As String
        Private _NotificationThreshold As Integer
        Private _NotificationThresholdTime As Integer
        Private _NotificationThresholdTimeType As NotificationThresholdTimeTypes

        Public ReadOnly Property StartDateTime() As Date
            Get
                Select Case Me.NotificationThresholdTimeType
                    Case NotificationThresholdTimeTypes.Seconds
                        Return Date.Now.AddSeconds(NotificationThresholdTime * -1)
                    Case NotificationThresholdTimeTypes.Minutes
                        Return Date.Now.AddMinutes(NotificationThresholdTime * -1)
                    Case NotificationThresholdTimeTypes.Hours
                        Return Date.Now.AddHours(NotificationThresholdTime * -1)
                    Case NotificationThresholdTimeTypes.Days
                        Return Date.Now.AddDays(NotificationThresholdTime * -1)
                    Case NotificationThresholdTimeTypes.None
                        Return Null.NullDate
                End Select
            End Get
        End Property

        Public Property EmailNotificationIsActive() As Boolean
            Get
                Return _EmailNotificationIsActive
            End Get
            Set(ByVal Value As Boolean)
                _EmailNotificationIsActive = Value
            End Set
        End Property
        Public Property MailFromAddress() As String
            Get
                Return _MailFromAddress
            End Get
            Set(ByVal Value As String)
                _MailFromAddress = Value
            End Set
        End Property
        Public Property MailToAddress() As String
            Get
                Return _MailToAddress
            End Get
            Set(ByVal Value As String)
                _MailToAddress = Value
            End Set
        End Property

        Public Property NotificationThreshold() As Integer
            Get
                Return _NotificationThreshold
            End Get
            Set(ByVal Value As Integer)
                _NotificationThreshold = Value
            End Set
        End Property

        Public Property NotificationThresholdTime() As Integer
            Get
                Return _NotificationThresholdTime
            End Get
            Set(ByVal Value As Integer)
                _NotificationThresholdTime = Value
            End Set
        End Property

        Public Property NotificationThresholdTimeType() As NotificationThresholdTimeTypes
            Get
                Return _NotificationThresholdTimeType
            End Get
            Set(ByVal Value As NotificationThresholdTimeTypes)
                _NotificationThresholdTimeType = Value
            End Set
        End Property

        Public Property ID() As String
            Get
                Return _ID
            End Get
            Set(ByVal Value As String)
                _ID = Value
            End Set
        End Property
        Public Property LoggingIsActive() As Boolean
            Get
                Return _LoggingIsActive
            End Get
            Set(ByVal Value As Boolean)
                _LoggingIsActive = Value
            End Set
        End Property
        Public Property LogFileName() As String
            Get
                Return _LogFileName
            End Get
            Set(ByVal Value As String)
                _LogFileName = Value
            End Set
        End Property

        Public Property LogFileNameWithPath() As String
            Get
                Return _LogFileNameWithPath
            End Get
            Set(ByVal Value As String)
                _LogFileNameWithPath = Value
            End Set
        End Property
        Public Property LogTypePortalID() As String
            Get
                Return _LogTypePortalID
            End Get
            Set(ByVal Value As String)
                _LogTypePortalID = Value
            End Set
        End Property
        Public Property KeepMostRecent() As String
            Get
                Return _KeepMostRecent
            End Get
            Set(ByVal Value As String)
                _KeepMostRecent = Value
            End Set
        End Property


    End Class

End Namespace




