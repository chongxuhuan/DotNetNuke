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

using DotNetNuke.Common.Utilities;
using DotNetNuke.ComponentModel;
using DotNetNuke.Entities.Host;

using Microsoft.VisualBasic;

#endregion

namespace DotNetNuke.Services.Scheduling
{
    public enum EventName
    {
        APPLICATION_START
    }

    public enum ScheduleSource
    {
        NOT_SET,
        STARTED_FROM_SCHEDULE_CHANGE,
        STARTED_FROM_EVENT,
        STARTED_FROM_TIMER,
        STARTED_FROM_BEGIN_REQUEST
    }

    public enum ScheduleStatus
    {
        NOT_SET,
        WAITING_FOR_OPEN_THREAD,
        RUNNING_EVENT_SCHEDULE,
        RUNNING_TIMER_SCHEDULE,
        RUNNING_REQUEST_SCHEDULE,
        WAITING_FOR_REQUEST,
        SHUTTING_DOWN,
        STOPPED
    }

    public enum SchedulerMode
    {
        DISABLED = 0,
        TIMER_METHOD = 1,
        REQUEST_METHOD = 2
    }

    public delegate void WorkStarted(SchedulerClient objSchedulerClient);

    public delegate void WorkProgressing(SchedulerClient objSchedulerClient);

    public delegate void WorkCompleted(SchedulerClient objSchedulerClient);

    public delegate void WorkErrored(SchedulerClient objSchedulerClient, Exception objException);

    public abstract class SchedulingProvider
    {
        private static bool _Debug;
        private static int _MaxThreads;
        private readonly string _providerPath;
        public EventName EventName;

        public SchedulingProvider()
        {
            _providerPath = Settings["providerPath"];
            if (!string.IsNullOrEmpty(Settings["debug"]))
            {
                _Debug = Convert.ToBoolean(Settings["debug"]);
            }
            if (!string.IsNullOrEmpty(Settings["maxThreads"]))
            {
                _MaxThreads = Convert.ToInt32(Settings["maxThreads"]);
            }
            else
            {
                _MaxThreads = 1;
            }
        }

        public static bool Debug
        {
            get
            {
                return _Debug;
            }
        }

        public static bool Enabled
        {
            get
            {
                if (SchedulerMode != SchedulerMode.DISABLED)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static int MaxThreads
        {
            get
            {
                return _MaxThreads;
            }
        }

        public string ProviderPath
        {
            get
            {
                return _providerPath;
            }
        }

        public static bool ReadyForPoll
        {
            get
            {
                if (DataCache.GetCache("ScheduleLastPolled") == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static DateTime ScheduleLastPolled
        {
            get
            {
                if (DataCache.GetCache("ScheduleLastPolled") != null)
                {
                    return Convert.ToDateTime(DataCache.GetCache("ScheduleLastPolled"));
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                DateTime ns;
                ScheduleItem s;
                s = Instance().GetNextScheduledTask(ServerController.GetExecutingServerName());
                if (s != null)
                {
                    DateTime NextStart = s.NextStart;
                    if (NextStart >= DateTime.Now)
                    {
                        ns = NextStart;
                    }
                    else
                    {
                        ns = DateTime.Now.AddMinutes(1);
                    }
                }
                else
                {
                    ns = DateTime.Now.AddMinutes(1);
                }
                DataCache.SetCache("ScheduleLastPolled", value, ns);
            }
        }

        public static SchedulerMode SchedulerMode
        {
            get
            {
                return Host.SchedulerMode;
            }
        }

        public virtual Dictionary<string, string> Settings
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }

        public static SchedulingProvider Instance()
        {
            return ComponentFactory.GetComponent<SchedulingProvider>();
        }

        public abstract void Start();

        public abstract void ExecuteTasks();

        public abstract void ReStart(string SourceOfRestart);

        public abstract void StartAndWaitForResponse();

        public abstract void Halt(string SourceOfHalt);

        public abstract void PurgeScheduleHistory();

        public abstract void RunEventSchedule(EventName objEventName);

        public abstract ArrayList GetSchedule();

        public abstract ArrayList GetSchedule(string Server);

        public abstract ScheduleItem GetSchedule(int ScheduleID);

        public abstract ScheduleItem GetSchedule(string TypeFullName, string Server);

        public abstract ScheduleItem GetNextScheduledTask(string Server);

        public abstract ArrayList GetScheduleHistory(int ScheduleID);

        public abstract Hashtable GetScheduleItemSettings(int ScheduleID);

        public abstract void AddScheduleItemSetting(int ScheduleID, string Name, string Value);

        public abstract Collection GetScheduleQueue();

        public abstract Collection GetScheduleProcessing();

        public abstract int GetFreeThreadCount();

        public abstract int GetActiveThreadCount();

        public abstract int GetMaxThreadCount();

        public abstract ScheduleStatus GetScheduleStatus();

        public abstract int AddSchedule(ScheduleItem objScheduleItem);

        public abstract void UpdateSchedule(ScheduleItem objScheduleItem);

        public abstract void DeleteSchedule(ScheduleItem objScheduleItem);

        public virtual void RunScheduleItemNow(ScheduleItem objScheduleItem)
        {
        }
    }
}