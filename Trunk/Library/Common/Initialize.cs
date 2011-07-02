#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

#region Usings

using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Hosting;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.EventQueue;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Services.Upgrade;

#endregion

namespace DotNetNuke.Common
{
    /// <summary>
    /// The Object to initialize application.
    /// </summary>
    public class Initialize
    {
        private static bool InitializedAlready;
        private static readonly object InitializeLock = new object();

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CacheMappedDirectory caches the Portal Mapped Directory(s)
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    1/27/2005   Moved back to App_Start from Caching Module
        /// </history>
        /// -----------------------------------------------------------------------------
        private static void CacheMappedDirectory()
        {
            //This code is only retained for binary compatability.
#pragma warning disable 612,618
            var objFolderController = new FolderController();
            var objPortalController = new PortalController();
            ArrayList arrPortals = objPortalController.GetPortals();
            int i;
            for (i = 0; i <= arrPortals.Count - 1; i++)
            {
                var objPortalInfo = (PortalInfo)arrPortals[i];
                objFolderController.SetMappedDirectory(objPortalInfo, HttpContext.Current);
            }
#pragma warning restore 612,618
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// CheckVersion determines whether the App is synchronized with the DB
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    2/17/2005   created
        /// </history>
        /// -----------------------------------------------------------------------------
        private static string CheckVersion(HttpApplication app)
        {
            HttpServerUtility Server = app.Server;
            bool AutoUpgrade;
            if (Config.GetSetting("AutoUpgrade") == null)
            {
                AutoUpgrade = true;
            }
            else
            {
                AutoUpgrade = bool.Parse(Config.GetSetting("AutoUpgrade"));
            }
            bool UseWizard;
            if (Config.GetSetting("UseInstallWizard") == null)
            {
                UseWizard = true;
            }
            else
            {
                UseWizard = bool.Parse(Config.GetSetting("UseInstallWizard"));
            }
            //Determine the Upgrade status and redirect as neccessary to InstallWizard.aspx
            string retValue = Null.NullString;
            switch (Globals.Status)
            {
                case Globals.UpgradeStatus.Install:
                    if (AutoUpgrade)
                    {
                        if (UseWizard)
                        {
                            retValue = "~/Install/InstallWizard.aspx";
                        }
                        else
                        {
                            retValue = "~/Install/Install.aspx?mode=install";
                        }
                    }
                    else
                    {
                        CreateUnderConstructionPage(Server);
                        retValue = "~/Install/UnderConstruction.htm";
                    }
                    break;
                case Globals.UpgradeStatus.Upgrade:
                    if (AutoUpgrade)
                    {
                        retValue = "~/Install/Install.aspx?mode=upgrade";
                    }
                    else
                    {
                        CreateUnderConstructionPage(Server);
                        retValue = "~/Install/UnderConstruction.htm";
                    }
                    break;
                case Globals.UpgradeStatus.Error:
                    CreateUnderConstructionPage(Server);
                    retValue = "~/Install/UnderConstruction.htm";
                    break;
            }
            return retValue;
        }

        private static void CreateUnderConstructionPage(HttpServerUtility server)
        {
            //create an UnderConstruction page if it does not exist already
            if (!File.Exists(server.MapPath("~/Install/UnderConstruction.htm")))
            {
                if (File.Exists(server.MapPath("~/Install/UnderConstruction.template.htm")))
                {
                    File.Copy(server.MapPath("~/Install/UnderConstruction.template.htm"), server.MapPath("~/Install/UnderConstruction.htm"));
                }
            }
        }

        private static string InitializeApp(HttpApplication app)
        {
            DnnLog.MethodEntry();
            HttpServerUtility Server = app.Server;
            HttpRequest Request = app.Request;
            string redirect = Null.NullString;

            DnnLog.Trace("Request " + Request.Url.LocalPath);

            //Don't process some of the AppStart methods if we are installing
            if (!Request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx") && !Request.Url.LocalPath.ToLower().EndsWith("install.aspx"))
            {
                //Check whether the current App Version is the same as the DB Version
                redirect = CheckVersion(app);
                if (string.IsNullOrEmpty(redirect))
                {
                    DnnLog.Info("Application Initializing");
                    //Cache Mapped Directory(s)
                    CacheMappedDirectory();
                    //Set globals
                    Globals.IISAppName = Request.ServerVariables["APPL_MD_PATH"];
                    Globals.OperatingSystemVersion = Environment.OSVersion.Version;
                    Globals.NETFrameworkVersion = GetNETFrameworkVersion();
                    Globals.DatabaseEngineVersion = GetDatabaseEngineVersion();
                    //Try and Upgrade to Current Framewok
                    Upgrade.TryUpgradeNETFramework();

                    //Start Scheduler
                    StartScheduler();
                    //Log Application Start
                    LogStart();
                    //Process any messages in the EventQueue for the Application_Start event
                    EventQueueController.ProcessMessages("Application_Start");

                    //Set Flag so we can determine the first Page Request after Application Start
                    app.Context.Items.Add("FirstRequest", true);

                    //Log Server information
                    ServerController.UpdateServerActivity(new ServerInfo());
                    DnnLog.Info("Application Initialized");
                }
            }
            else
            {
                //NET Framework version is neeed by Upgrade
                Globals.NETFrameworkVersion = GetNETFrameworkVersion();
            }
            return redirect;
        }

        private static Version GetNETFrameworkVersion()
        {
            string version = Environment.Version.ToString(2);
            if (version == "2.0")
            {
                //Try and load a 3.0 Assembly
                try
                {
                    AppDomain.CurrentDomain.Load("System.Runtime.Serialization, Version=3.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089");
                    version = "3.0";
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                }
                //Try and load a 3.5 Assembly
                try
                {
                    AppDomain.CurrentDomain.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089");
                    version = "3.5";
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);
                }
            }
            return new Version(version);
        }

        private static Version GetDatabaseEngineVersion()
        {
            return DataProvider.Instance().GetDatabaseEngineVersion();
        }

        /// <summary>
        /// Inits the app.
        /// </summary>
        /// <param name="app">The app.</param>
        public static void Init(HttpApplication app)
        {
            HttpResponse Response = app.Response;
            string redirect = Null.NullString;
            //Check if app is initialised
            if ((InitializedAlready && Globals.Status == Globals.UpgradeStatus.None))
            {
                return;
            }
            lock (InitializeLock)
            {
                //Double-Check if app was initialised by another request
                if ((InitializedAlready && Globals.Status == Globals.UpgradeStatus.None))
                {
                    return;
                }
                //Initialize ...
                redirect = InitializeApp(app);
                //Set flag to indicate app has been initialised
                InitializedAlready = true;
            }
            if (!string.IsNullOrEmpty(redirect))
            {
                Response.Redirect(redirect, true);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LogStart logs the Application Start Event
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    1/27/2005   Moved back to App_Start from Logging Module
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void LogStart()
        {
            var objEv = new EventLogController();
            var objEventLogInfo = new LogInfo();
            objEventLogInfo.BypassBuffering = true;
            objEventLogInfo.LogTypeKey = EventLogController.EventLogType.APPLICATION_START.ToString();
            objEv.AddLog(objEventLogInfo);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LogEnd logs the Application Start Event
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    1/28/2005   Moved back to App_End from Logging Module
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void LogEnd()
        {
            try
            {
                ApplicationShutdownReason shutdownReason = HostingEnvironment.ShutdownReason;
                string shutdownDetail = "";
                switch (shutdownReason)
                {
                    case ApplicationShutdownReason.BinDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the Bin folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.BrowsersDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Browsers folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.ChangeInGlobalAsax:
                        shutdownDetail = "The AppDomain shut down because of a change to Global.asax.";
                        break;
                    case ApplicationShutdownReason.ChangeInSecurityPolicyFile:
                        shutdownDetail = "The AppDomain shut down because of a change in the code access security policy file.";
                        break;
                    case ApplicationShutdownReason.CodeDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the App_Code folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.ConfigurationChange:
                        shutdownDetail = "The AppDomain shut down because of a change to the application level configuration.";
                        break;
                    case ApplicationShutdownReason.HostingEnvironment:
                        shutdownDetail = "The AppDomain shut down because of the hosting environment.";
                        break;
                    case ApplicationShutdownReason.HttpRuntimeClose:
                        shutdownDetail = "The AppDomain shut down because of a call to Close.";
                        break;
                    case ApplicationShutdownReason.IdleTimeout:
                        shutdownDetail = "The AppDomain shut down because of the maximum allowed idle time limit.";
                        break;
                    case ApplicationShutdownReason.InitializationError:
                        shutdownDetail = "The AppDomain shut down because of an AppDomain initialization error.";
                        break;
                    case ApplicationShutdownReason.MaxRecompilationsReached:
                        shutdownDetail = "The AppDomain shut down because of the maximum number of dynamic recompiles of resources limit.";
                        break;
                    case ApplicationShutdownReason.PhysicalApplicationPathChanged:
                        shutdownDetail = "The AppDomain shut down because of a change to the physical path for the application.";
                        break;
                    case ApplicationShutdownReason.ResourcesDirChangeOrDirectoryRename:
                        shutdownDetail = "The AppDomain shut down because of a change to the App_GlobalResources folder or files contained in it.";
                        break;
                    case ApplicationShutdownReason.UnloadAppDomainCalled:
                        shutdownDetail = "The AppDomain shut down because of a call to UnloadAppDomain.";
                        break;
                    default:
                        shutdownDetail = "No shutdown reason provided.";
                        break;
                }
                var objEv = new EventLogController();
                var objEventLogInfo = new LogInfo();
                objEventLogInfo.BypassBuffering = true;
                objEventLogInfo.LogTypeKey = EventLogController.EventLogType.APPLICATION_SHUTTING_DOWN.ToString();
                objEventLogInfo.AddProperty("Shutdown Details", shutdownDetail);
                objEv.AddLog(objEventLogInfo);

                DnnLog.Info("Application shutting down. Reason: {0}", shutdownDetail);
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
            if (Globals.Status != Globals.UpgradeStatus.Install)
            {
                //purge log buffer
                LoggingProvider.Instance().PurgeLogBuffer();
            }
        }

        public static void RunSchedule(HttpRequest request)
        {
            DnnLog.MethodEntry();

            //First check if we are upgrading/installing
            if (request.Url.LocalPath.ToLower().EndsWith("install.aspx") || request.Url.LocalPath.ToLower().EndsWith("installwizard.aspx"))
            {
                return;
            }
            try
            {
                if (SchedulingProvider.SchedulerMode == SchedulerMode.REQUEST_METHOD && SchedulingProvider.ReadyForPoll)
                {
                    DnnLog.Trace("Running Schedule " + (SchedulingProvider.SchedulerMode));
                    SchedulingProvider scheduler = SchedulingProvider.Instance();
                    var requestScheduleThread = new Thread(scheduler.ExecuteTasks);
                    requestScheduleThread.IsBackground = true;
                    requestScheduleThread.Start();
                    SchedulingProvider.ScheduleLastPolled = DateTime.Now;
                }
            }
            catch (Exception exc)
            {
                Exceptions.LogException(exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// StopScheduler stops the Scheduler
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    1/28/2005   Moved back to App_End from Scheduling Module
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void StopScheduler()
        {
            //stop scheduled jobs
            SchedulingProvider.Instance().Halt("Stopped by Application_End");
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// StartScheduler starts the Scheduler
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        ///     [cnurse]    1/27/2005   Moved back to App_Start from Scheduling Module
        /// </history>
        /// -----------------------------------------------------------------------------
        public static void StartScheduler()
        {
            DnnLog.MethodEntry();

            //instantiate APPLICATION_START scheduled jobs
            if (SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD)
            {
                DnnLog.Trace("Running Schedule " + (SchedulingProvider.SchedulerMode));
                SchedulingProvider scheduler = SchedulingProvider.Instance();
                scheduler.RunEventSchedule(EventName.APPLICATION_START);
                var newThread = new Thread(SchedulingProvider.Instance().Start);
                newThread.IsBackground = true;
                newThread.Start();
            }
        }
    }
}
