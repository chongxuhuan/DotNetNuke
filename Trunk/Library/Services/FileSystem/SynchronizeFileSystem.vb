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
Imports System.Threading
Imports System.Web

Namespace DotNetNuke.Services.FileSystem

    Public Class SynchronizeFileSystem
        Inherits DotNetNuke.Services.Scheduling.SchedulerClient

        Public Sub New(ByVal objScheduleHistoryItem As DotNetNuke.Services.Scheduling.ScheduleHistoryItem)
            MyBase.new()

            Me.ScheduleHistoryItem = objScheduleHistoryItem

        End Sub

        Public Overrides Sub DoWork()
            Try
                'notification that the event is progressing
                Me.Progressing()    'OPTIONAL

                Synchronize()

                Me.ScheduleHistoryItem.Succeeded = True    'REQUIRED

                Me.ScheduleHistoryItem.AddLogNote("File System Synchronized.")    'OPTIONAL

            Catch exc As Exception    'REQUIRED

                Me.ScheduleHistoryItem.Succeeded = False    'REQUIRED

                Me.ScheduleHistoryItem.AddLogNote("File System Synchronization failed. " + exc.ToString)     'OPTIONAL

                'notification that we have errored
                Me.Errored(exc)    'REQUIRED

                'log the exception
                LogException(exc)    'OPTIONAL
            End Try
        End Sub

        Private Sub Synchronize()


            Dim objPortals As New PortalController
            Dim arrPortals As ArrayList = objPortals.GetPortals
            Dim objPortal As PortalInfo

            'Sync Host
            FileSystemUtils.Synchronize(Null.NullInteger, Null.NullInteger, Common.Globals.HostMapPath)

            'Sync Portals
            Dim intIndex As Integer
            For intIndex = 0 To arrPortals.Count - 1
				objPortal = CType(arrPortals(intIndex), PortalInfo)
				FileSystemUtils.Synchronize(objPortal.PortalID, objPortal.AdministratorRoleId, objPortal.HomeDirectoryMapPath)
            Next

        End Sub
    End Class

End Namespace
