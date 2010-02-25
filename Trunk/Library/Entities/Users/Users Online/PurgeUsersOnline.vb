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
Imports System.Collections
Imports System.Configuration
Imports System.Data

Namespace DotNetNuke.Entities.Users

    ''' -----------------------------------------------------------------------------
    ''' Project:    DotNetNuke
    ''' Namespace:  DotNetNuke.Entities.Users
    ''' Class:      PurgeUsersOnline
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' The PurgeUsersOnline class provides a Scheduler for purging the Users Online
    ''' data
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    '''     [cnurse]	03/14/2006	documented
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class PurgeUsersOnline
        Inherits DotNetNuke.Services.Scheduling.SchedulerClient

#Region "Constructors"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' Constructs a PurgeUsesOnline SchedulerClient
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <param name="objScheduleHistoryItem">A SchedulerHistiryItem</param>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
            MyBase.new()
            Me.ScheduleHistoryItem = objScheduleHistoryItem
        End Sub

#End Region

#Region "Private Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' UpdateUsersOnline updates the Users Online information
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Sub UpdateUsersOnline()

            Dim objUserOnlineController As UserOnlineController = New UserOnlineController

            ' Is Users Online Enabled?
            If (objUserOnlineController.IsEnabled()) Then

                ' Update the Users Online records from Cache
                Me.Status = "Updating Users Online"
                objUserOnlineController.UpdateUsersOnline()
                Me.Status = "Update Users Online Successfully"
                Me.ScheduleHistoryItem.Succeeded = True
            End If

        End Sub

#End Region

#Region "Public Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' DoWork does th4 Scheduler work
        ''' </summary>
        ''' <history>
        '''     [cnurse]	03/14/2006	documented
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Overrides Sub DoWork()
            Try

                'notification that the event is progressing
                Me.Progressing()    'OPTIONAL
                UpdateUsersOnline()
                Me.ScheduleHistoryItem.Succeeded = True     'REQUIRED
                Me.ScheduleHistoryItem.AddLogNote("UsersOnline purge completed.")

            Catch exc As Exception    'REQUIRED
                Me.ScheduleHistoryItem.Succeeded = False    'REQUIRED
                Me.ScheduleHistoryItem.AddLogNote("UsersOnline purge failed." + exc.ToString)    'OPTIONAL

                'notification that we have errored
                Me.Errored(exc)    'REQUIRED

                'log the exception
                LogException(exc)    'OPTIONAL
            End Try
        End Sub

#End Region

    End Class

End Namespace
