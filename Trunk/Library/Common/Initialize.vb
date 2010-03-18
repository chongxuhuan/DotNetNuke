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
Imports System.Security
Imports System.Security.Principal
Imports System.Web
Imports System.Web.Security
Imports System.IO
Imports DotNetNuke.Services.Log.EventLog
Imports DotNetNuke.Entities.Host

Namespace DotNetNuke.Common

    Public Class Initialize

        Private Shared InitializedAlready As Boolean
        Private Shared InitializeLock As Object = New Object()

#Region "Private Shared Methods"

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CacheMappedDirectory caches the Portal Mapped Directory(s)
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    1/27/2005   Moved back to App_Start from Caching Module
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Sub CacheMappedDirectory()
            'Cache the mapped physical home directory for each portal
            'so the mapped directories are available outside
            'of httpcontext.   This is especially necessary
            'when the /Portals or portal home directory has been 
            'mapped in IIS to another directory or server.
            Dim objFolderController As New Services.FileSystem.FolderController
            Dim objPortalController As New PortalController
            Dim arrPortals As ArrayList = objPortalController.GetPortals()
            Dim i As Integer
            For i = 0 To arrPortals.Count - 1
                Dim objPortalInfo As PortalInfo = CType(arrPortals(i), PortalInfo)
                objFolderController.SetMappedDirectory(objPortalInfo, HttpContext.Current)
            Next

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' CheckVersion determines whether the App is synchronized with the DB
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    2/17/2005   created
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Private Shared Function CheckVersion(ByVal app As HttpApplication) As String
            Dim Server As HttpServerUtility = app.Server

            Dim AutoUpgrade As Boolean
            If Config.GetSetting("AutoUpgrade") Is Nothing Then
                AutoUpgrade = True
            Else
                AutoUpgrade = Boolean.Parse(Config.GetSetting("AutoUpgrade"))
            End If

            Dim UseWizard As Boolean
            If Config.GetSetting("UseInstallWizard") Is Nothing Then
                UseWizard = True
            Else
                UseWizard = Boolean.Parse(Config.GetSetting("UseInstallWizard"))
            End If

            'Determine the Upgrade status and redirect as neccessary to InstallWizard.aspx
            Dim retValue As String = Null.NullString

            Select Case Globals.Status
                Case Globals.UpgradeStatus.Install
                    If AutoUpgrade Then
                        If UseWizard Then
                            retValue = "~/Install/InstallWizard.aspx"
                        Else
                            retValue = "~/Install/Install.aspx?mode=install"
                        End If
                    Else
                        CreateUnderConstructionPage(Server)
                        retValue = "~/Install/UnderConstruction.htm"
                    End If
                Case Globals.UpgradeStatus.Upgrade
                    If AutoUpgrade Then
                        retValue = "~/Install/Install.aspx?mode=upgrade"
                    Else
                        CreateUnderConstructionPage(Server)
                        retValue = "~/Install/UnderConstruction.htm"
                    End If
                Case Globals.UpgradeStatus.Error
                    CreateUnderConstructionPage(Server)
                    retValue = "~/Install/UnderConstruction.htm"
            End Select

            Return retValue
        End Function

        Private Shared Sub CreateUnderConstructionPage(ByVal server As HttpServerUtility)
            ' create an UnderConstruction page if it does not exist already
            If Not File.Exists(server.MapPath("~/Install/UnderConstruction.htm")) Then
                If File.Exists(server.MapPath("~/Install/UnderConstruction.template.htm")) Then
                    File.Copy(server.MapPath("~/Install/UnderConstruction.template.htm"), server.MapPath("~/Install/UnderConstruction.htm"))
                End If
            End If
        End Sub

        Private Shared Function InitializeApp(ByVal app As HttpApplication) As String
            Dim Server As HttpServerUtility = app.Server
            Dim Request As HttpRequest = app.Request
            Dim redirect As String = Null.NullString

            If HttpContext.Current.Request.ApplicationPath = "/" Then
                If Config.GetSetting("InstallationSubfolder") = "" Then
                    ApplicationPath = ""
                Else
                    ApplicationPath = Config.GetSetting("InstallationSubfolder") & "/"
                End If
            Else
                ApplicationPath = Request.ApplicationPath
            End If
            ApplicationMapPath = System.AppDomain.CurrentDomain.BaseDirectory.Substring(0, System.AppDomain.CurrentDomain.BaseDirectory.Length - 1)
            ApplicationMapPath = ApplicationMapPath.Replace("/", "\")

            HostPath = ApplicationPath & "/Portals/_default/"
            HostMapPath = Server.MapPath(HostPath)

            InstallPath = ApplicationPath & "/Install/"
            InstallMapPath = Server.MapPath(InstallPath)

            'Call the Global GetStatus function to determine the current status
            Globals.GetStatus()

            'Don't process some of the AppStart methods if we are installing
            If Not Request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") _
                    AndAlso Not Request.Url.LocalPath.ToLower.EndsWith("install.aspx") Then

                'Check whether the current App Version is the same as the DB Version
                redirect = CheckVersion(app)

                If String.IsNullOrEmpty(redirect) Then
                    'Cache Mapped Directory(s)
                    CacheMappedDirectory()

                    'Set globals
                    IISAppName = Request.ServerVariables("APPL_MD_PATH")
                    OperatingSystemVersion = Environment.OSVersion.Version
                    NETFrameworkVersion = GetNETFrameworkVersion()
                    DatabaseEngineVersion = GetDatabaseEngineVersion()

                    'Try and Upgrade to .NET 3.5
                    Upgrade.Upgrade.TryUpgradeNETFramework()

                    'Start Scheduler
                    StartScheduler()

                    'Log Application Start
                    LogStart()

                    'Process any messages in the EventQueue for the Application_Start event
                    EventQueue.EventQueueController.ProcessMessages("Application_Start")

                    'Set Flag so we can determine the first Page Request after Application Start
                    app.Context.Items.Add("FirstRequest", True)

                    'Log Server information
                    ServerController.UpdateServerActivity(New ServerInfo())
                End If
            Else
                'NET Framework version is neeed by Upgrade
                NETFrameworkVersion = GetNETFrameworkVersion()
            End If

            Return redirect
        End Function

        Private Shared Function GetNETFrameworkVersion() As System.Version
            Dim version As String = System.Environment.Version.ToString(2)

            If version = "2.0" Then
                'Try and load a 3.0 Assembly
                Dim assembly As System.Reflection.Assembly
                Try
                    assembly = AppDomain.CurrentDomain.Load("System.Runtime.Serialization, Version=3.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089")
                    version = "3.0"
                Catch ex As Exception
                End Try

                'Try and load a 3.5 Assembly
                Try
                    assembly = AppDomain.CurrentDomain.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089")
                    version = "3.5"
                Catch ex As Exception
                End Try
            End If

            Return New System.Version(version)
        End Function

        Private Shared Function GetDatabaseEngineVersion() As System.Version
            Return DataProvider.Instance.GetDatabaseEngineVersion()
        End Function

#End Region

        Public Shared Sub Init(ByVal app As HttpApplication)
            Dim Response As HttpResponse = app.Response
            Dim Request As HttpRequest = app.Request
            Dim redirect As String = Null.NullString

            'Check if app is initialised
            If (InitializedAlready AndAlso Globals.Status = UpgradeStatus.None) Then
                Exit Sub
            End If

            SyncLock InitializeLock
                'Double-Check if app was initialised by another request
                If (InitializedAlready AndAlso Globals.Status = UpgradeStatus.None) Then
                    Exit Sub
                End If

                'Initialize ...
                redirect = InitializeApp(app)

                'Set flag to indicate app has been initialised
                InitializedAlready = True
            End SyncLock

            If Not String.IsNullOrEmpty(redirect) Then
                Response.Redirect(redirect, True)
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LogStart logs the Application Start Event
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    1/27/2005   Moved back to App_Start from Logging Module
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub LogStart()
            Dim objEv As New EventLogController
            Dim objEventLogInfo As New LogInfo
            objEventLogInfo.BypassBuffering = True
            objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.APPLICATION_START.ToString
            objEv.AddLog(objEventLogInfo)

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StartScheduler starts the Scheduler
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    1/27/2005   Moved back to App_Start from Scheduling Module
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub StartScheduler()
            ' instantiate APPLICATION_START scheduled jobs
            If Services.Scheduling.SchedulingProvider.SchedulerMode = Scheduling.SchedulerMode.TIMER_METHOD Then
                Dim scheduler As Scheduling.SchedulingProvider = Scheduling.SchedulingProvider.Instance()
                scheduler.RunEventSchedule(Scheduling.EventName.APPLICATION_START)
                Dim newThread As New Threading.Thread(AddressOf Scheduling.SchedulingProvider.Instance.Start)
                newThread.IsBackground = True
                newThread.Start()
            End If
        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' LogEnd logs the Application Start Event
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    1/28/2005   Moved back to App_End from Logging Module
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub LogEnd()
            Try
                Dim shutdownReason As System.Web.ApplicationShutdownReason = System.Web.Hosting.HostingEnvironment.ShutdownReason
                Dim shutdownDetail As String = ""
                Select Case shutdownReason
                    Case ApplicationShutdownReason.BinDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the Bin folder or files contained in it."
                    Case ApplicationShutdownReason.BrowsersDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Browsers folder or files contained in it."
                    Case ApplicationShutdownReason.ChangeInGlobalAsax
                        shutdownDetail = "The AppDomain shut down because of a change to Global.asax."
                    Case ApplicationShutdownReason.ChangeInSecurityPolicyFile
                        shutdownDetail = "The AppDomain shut down because of a change in the code access security policy file."
                    Case ApplicationShutdownReason.CodeDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Code folder or files contained in it."
                    Case ApplicationShutdownReason.ConfigurationChange
                        shutdownDetail = "The AppDomain shut down because of a change to the application level configuration."
                    Case ApplicationShutdownReason.HostingEnvironment
                        shutdownDetail = "The AppDomain shut down because of the hosting environment."
                    Case ApplicationShutdownReason.HttpRuntimeClose
                        shutdownDetail = "The AppDomain shut down because of a call to Close."
                    Case ApplicationShutdownReason.IdleTimeout
                        shutdownDetail = "The AppDomain shut down because of the maximum allowed idle time limit."
                    Case ApplicationShutdownReason.InitializationError
                        shutdownDetail = "The AppDomain shut down because of an AppDomain initialization error."
                    Case ApplicationShutdownReason.MaxRecompilationsReached
                        shutdownDetail = "The AppDomain shut down because of the maximum number of dynamic recompiles of resources limit."
                    Case ApplicationShutdownReason.PhysicalApplicationPathChanged
                        shutdownDetail = "The AppDomain shut down because of a change to the physical path for the application."
                    Case ApplicationShutdownReason.ResourcesDirChangeOrDirectoryRename
                        shutdownDetail = "The AppDomain shut down because of a change to the App_GlobalResources folder or files contained in it."
                    Case ApplicationShutdownReason.UnloadAppDomainCalled
                        shutdownDetail = "The AppDomain shut down because of a call to UnloadAppDomain."
                    Case Else
                        shutdownDetail = "No shutdown reason provided."
                End Select

                Dim objEv As New EventLogController
                Dim objEventLogInfo As New LogInfo
                objEventLogInfo.BypassBuffering = True
                objEventLogInfo.LogTypeKey = Services.Log.EventLog.EventLogController.EventLogType.APPLICATION_SHUTTING_DOWN.ToString
                objEventLogInfo.AddProperty("Shutdown Details", shutdownDetail)

                objEv.AddLog(objEventLogInfo)
            Catch exc As Exception
                LogException(exc)
            End Try

            ' purge log buffer
            LoggingProvider.Instance.PurgeLogBuffer()
        End Sub

        Public Shared Sub RunSchedule(ByVal request As HttpRequest)
            'First check if we are upgrading/installing
            If request.Url.LocalPath.ToLower.EndsWith("install.aspx") OrElse request.Url.LocalPath.ToLower.EndsWith("installwizard.aspx") Then
                Exit Sub
            End If

            Try
                If Services.Scheduling.SchedulingProvider.SchedulerMode = Scheduling.SchedulerMode.REQUEST_METHOD _
                AndAlso Services.Scheduling.SchedulingProvider.ReadyForPoll Then

                    Dim scheduler As Scheduling.SchedulingProvider = Scheduling.SchedulingProvider.Instance
                    Dim RequestScheduleThread As Threading.Thread
                    RequestScheduleThread = New Threading.Thread(AddressOf scheduler.ExecuteTasks)
                    RequestScheduleThread.IsBackground = True
                    RequestScheduleThread.Start()

                    Services.Scheduling.SchedulingProvider.ScheduleLastPolled = Now
                End If
            Catch exc As Exception
                LogException(exc)
            End Try

        End Sub

        ''' -----------------------------------------------------------------------------
        ''' <summary>
        ''' StopScheduler stops the Scheduler
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        ''' <history>
        '''     [cnurse]    1/28/2005   Moved back to App_End from Scheduling Module
        ''' </history>
        ''' -----------------------------------------------------------------------------
        Public Shared Sub StopScheduler()
            ' stop scheduled jobs
            Scheduling.SchedulingProvider.Instance.Halt("Stopped by Application_End")
        End Sub

    End Class

End Namespace
