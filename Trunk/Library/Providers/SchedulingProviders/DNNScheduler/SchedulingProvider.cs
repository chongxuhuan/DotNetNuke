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
using System.Collections.Generic;
using System.Threading;

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Framework;

using Microsoft.VisualBasic;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public class DNNScheduler : SchedulingProvider
    {
		#region "Constructors"
        public DNNScheduler()
        {
            if (DataProvider.Instance() == null)
            {
				//get the provider configuration based on the type
                DataProvider objProvider = null;
                string defaultprovider = Data.DataProvider.Instance().DefaultProviderName;
                string dataProviderNamespace = "DotNetNuke.Services.Scheduling.DNNScheduling";
                if (defaultprovider == "SqlDataProvider")
                {
                    objProvider = new SqlDataProvider();
                }
                else
                {
                    string providerType = dataProviderNamespace + "." + defaultprovider;
                    objProvider = (DataProvider) Reflection.CreateObject(providerType, providerType, true);
                }
                ComponentFactory.RegisterComponentInstance<DataProvider>(objProvider);
            }
        }
		
		#endregion

		#region "Public Properties"

        public override Dictionary<string, string> Settings
        {
            get
            {
                return ComponentFactory.GetComponentSettings<DNNScheduler>() as Dictionary<string, string>;
            }
        }
		
		#endregion
		
		#region "Private Methods"

        private bool CanRunOnThisServer(string Servers)
        {
            string lwrServers = "";
            if (lwrServers != null)
            {
                lwrServers = Servers.ToLower();
            }
            if (String.IsNullOrEmpty(lwrServers) || lwrServers.Contains(Globals.ServerName.ToLower()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
		
		#endregion

		#region "Public Methods"
		
        public override int AddSchedule(ScheduleItem objScheduleItem)
        {
			//Remove item from queue
            Scheduler.CoreScheduler.RemoveFromScheduleQueue(objScheduleItem);
			//save item
            objScheduleItem.ScheduleID = SchedulingController.AddSchedule(objScheduleItem.TypeFullName,
                                                                          objScheduleItem.TimeLapse,
                                                                          objScheduleItem.TimeLapseMeasurement,
                                                                          objScheduleItem.RetryTimeLapse,
                                                                          objScheduleItem.RetryTimeLapseMeasurement,
                                                                          objScheduleItem.RetainHistoryNum,
                                                                          objScheduleItem.AttachToEvent,
                                                                          objScheduleItem.CatchUpEnabled,
                                                                          objScheduleItem.Enabled,
                                                                          objScheduleItem.ObjectDependencies,
                                                                          objScheduleItem.Servers,
                                                                          objScheduleItem.FriendlyName);
            //Add schedule to queue
			RunScheduleItemNow(objScheduleItem);

            //Return Id
			return objScheduleItem.ScheduleID;
        }

        public override void AddScheduleItemSetting(int ScheduleID, string Name, string Value)
        {
            SchedulingController.AddScheduleItemSetting(ScheduleID, Name, Value);
        }

        public override void DeleteSchedule(ScheduleItem objScheduleItem)
        {
            SchedulingController.DeleteSchedule(objScheduleItem.ScheduleID);
            Scheduler.CoreScheduler.RemoveFromScheduleQueue(objScheduleItem);
            DataCache.RemoveCache("ScheduleLastPolled");
        }

        public override void ExecuteTasks()
        {
            if (Enabled)
            {
                var s = new Scheduler.CoreScheduler(Debug, MaxThreads);
                Scheduler.CoreScheduler.KeepRunning = true;
                Scheduler.CoreScheduler.KeepThreadAlive = false;
                Scheduler.CoreScheduler.Start();
            }
        }

        public override int GetActiveThreadCount()
        {
            return SchedulingController.GetActiveThreadCount();
        }

        public override int GetFreeThreadCount()
        {
            return SchedulingController.GetFreeThreadCount();
        }

        public override int GetMaxThreadCount()
        {
            return SchedulingController.GetMaxThreadCount();
        }

        public override ScheduleItem GetNextScheduledTask(string Server)
        {
            return SchedulingController.GetNextScheduledTask(Server);
        }

        public override ArrayList GetSchedule()
        {
            return new ArrayList(SchedulingController.GetSchedule().ToArray());
        }

        public override ArrayList GetSchedule(string Server)
        {
            return new ArrayList(SchedulingController.GetSchedule(Server).ToArray());
        }

        public override ScheduleItem GetSchedule(int ScheduleID)
        {
            return SchedulingController.GetSchedule(ScheduleID);
        }

        public override ScheduleItem GetSchedule(string TypeFullName, string Server)
        {
            return SchedulingController.GetSchedule(TypeFullName, Server);
        }

        public override ArrayList GetScheduleHistory(int ScheduleID)
        {
            return new ArrayList(SchedulingController.GetScheduleHistory(ScheduleID).ToArray());
        }

        public override Hashtable GetScheduleItemSettings(int ScheduleID)
        {
            return SchedulingController.GetScheduleItemSettings(ScheduleID);
        }

        public override Collection GetScheduleProcessing()
        {
            return SchedulingController.GetScheduleProcessing();
        }

        public override Collection GetScheduleQueue()
        {
            return SchedulingController.GetScheduleQueue();
        }

        public override ScheduleStatus GetScheduleStatus()
        {
            return SchedulingController.GetScheduleStatus();
        }

        public override void Halt(string SourceOfHalt)
        {
            var s = new Scheduler.CoreScheduler(Debug, MaxThreads);
            Scheduler.CoreScheduler.Halt(SourceOfHalt);
            Scheduler.CoreScheduler.KeepRunning = false;
        }

        public override void PurgeScheduleHistory()
        {
            var s = new Scheduler.CoreScheduler(MaxThreads);
            Scheduler.CoreScheduler.PurgeScheduleHistory();
        }

        public override void ReStart(string SourceOfRestart)
        {
            Halt(SourceOfRestart);
            StartAndWaitForResponse();
        }

        public override void RunEventSchedule(EventName objEventName)
        {
            if (Enabled)
            {
                var s = new Scheduler.CoreScheduler(Debug, MaxThreads);
                Scheduler.CoreScheduler.RunEventSchedule(objEventName);
            }
        }

        public override void RunScheduleItemNow(ScheduleItem objScheduleItem)
        {
			//Remove item from queue
            Scheduler.CoreScheduler.RemoveFromScheduleQueue(objScheduleItem);
            var objScheduleHistoryItem = new ScheduleHistoryItem(objScheduleItem);
            objScheduleHistoryItem.NextStart = DateTime.Now;
            if (objScheduleHistoryItem.TimeLapse != Null.NullInteger && objScheduleHistoryItem.TimeLapseMeasurement != Null.NullString && objScheduleHistoryItem.Enabled &&
                CanRunOnThisServer(objScheduleItem.Servers))
            {
                objScheduleHistoryItem.ScheduleSource = ScheduleSource.STARTED_FROM_SCHEDULE_CHANGE;
                Scheduler.CoreScheduler.AddToScheduleQueue(objScheduleHistoryItem);
            }
            DataCache.RemoveCache("ScheduleLastPolled");
        }

        public override void Start()
        {
            if (Enabled)
            {
                var s = new Scheduler.CoreScheduler(Debug, MaxThreads);
                Scheduler.CoreScheduler.KeepRunning = true;
                Scheduler.CoreScheduler.KeepThreadAlive = true;
                Scheduler.CoreScheduler.Start();
            }
        }

        public override void StartAndWaitForResponse()
        {
            if (Enabled)
            {
                var newThread = new Thread(Start);
                newThread.IsBackground = true;
                newThread.Start();

                //wait for up to 30 seconds for thread
                //to start up
                for (int i = 0; i <= 30; i++)
                {
                    if (GetScheduleStatus() != ScheduleStatus.STOPPED)
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        public override void UpdateSchedule(ScheduleItem objScheduleItem)
        {
			//Remove item from queue
            Scheduler.CoreScheduler.RemoveFromScheduleQueue(objScheduleItem);
			//save item
            SchedulingController.UpdateSchedule(objScheduleItem.ScheduleID,
                                                objScheduleItem.TypeFullName,
                                                objScheduleItem.TimeLapse,
                                                objScheduleItem.TimeLapseMeasurement,
                                                objScheduleItem.RetryTimeLapse,
                                                objScheduleItem.RetryTimeLapseMeasurement,
                                                objScheduleItem.RetainHistoryNum,
                                                objScheduleItem.AttachToEvent,
                                                objScheduleItem.CatchUpEnabled,
                                                objScheduleItem.Enabled,
                                                objScheduleItem.ObjectDependencies,
                                                objScheduleItem.Servers,
                                                objScheduleItem.FriendlyName);
            //Add schedule to queue
			RunScheduleItemNow(objScheduleItem);
        }
		
		#endregion
    }
}