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
using System.Data;

using DotNetNuke.ComponentModel;

#endregion

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public abstract class DataProvider
    {
        public static DataProvider Instance()
        {
            return ComponentFactory.GetComponent<DataProvider>();
        }

        public abstract int AddSchedule(string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum,
                                        string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers, int CreatedByUserID, string FriendlyName);

        public abstract int AddScheduleHistory(int ScheduleID, DateTime StartDate, string Server);

        public abstract void AddScheduleItemSetting(int ScheduleID, string Name, string Value);

        public abstract void DeleteSchedule(int ScheduleID);

        public abstract IDataReader GetNextScheduledTask(string Server);

        public abstract IDataReader GetSchedule();

        public abstract IDataReader GetSchedule(string Server);

        public abstract IDataReader GetSchedule(int ScheduleID);

        public abstract IDataReader GetSchedule(string TypeFullName, string Server);

        public abstract IDataReader GetScheduleByEvent(string EventName, string Server);

        public abstract IDataReader GetScheduleHistory(int ScheduleID);

        public abstract IDataReader GetScheduleItemSettings(int ScheduleID);

        public abstract void PurgeScheduleHistory();

        public abstract void UpdateSchedule(int ScheduleID, string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum,
                                            string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers, int LastModifiedByUserID, string FriendlyName);

        public abstract void UpdateScheduleHistory(int ScheduleHistoryID, DateTime EndDate, bool Succeeded, string LogNotes, DateTime NextStart);
    }
}