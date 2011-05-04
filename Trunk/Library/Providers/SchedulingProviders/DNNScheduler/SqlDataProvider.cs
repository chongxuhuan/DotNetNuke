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
using System.Data.SqlTypes;

using DotNetNuke.Common.Utilities;

using Microsoft.ApplicationBlocks.Data;

#endregion

namespace DotNetNuke.Services.Scheduling.DNNScheduling
{
    public class SqlDataProvider : DataProvider
    {
        public string ConnectionString
        {
            get
            {
                return Data.DataProvider.Instance().ConnectionString;
            }
        }

        public string DatabaseOwner
        {
            get
            {
                return Data.DataProvider.Instance().DatabaseOwner;
            }
        }

        public string ObjectQualifier
        {
            get
            {
                return Data.DataProvider.Instance().ObjectQualifier;
            }
        }

        private DateTime FixDate(DateTime dateToFix)
        {
            if (dateToFix < SqlDateTime.MinValue.Value)
            {
                dateToFix = SqlDateTime.MinValue.Value;
            }
            return dateToFix;
        }

        private object GetNull(object Field)
        {
            object nullValue = Null.GetNull(Field, DBNull.Value);
            if (nullValue is DateTime)
            {
                nullValue = FixDate((DateTime) nullValue);
            }
            return nullValue;
        }

        public override int AddSchedule(string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum,
                                        string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers, int CreatedByUserID, string FriendlyName)
        {
            return
                Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString,
                                                        DatabaseOwner + ObjectQualifier + "AddSchedule",
                                                        TypeFullName,
                                                        TimeLapse,
                                                        TimeLapseMeasurement,
                                                        RetryTimeLapse,
                                                        RetryTimeLapseMeasurement,
                                                        RetainHistoryNum,
                                                        AttachToEvent,
                                                        CatchUpEnabled,
                                                        Enabled,
                                                        ObjectDependencies,
                                                        GetNull(Servers),
                                                        CreatedByUserID,
                                                        FriendlyName));
        }

        public override int AddScheduleHistory(int ScheduleID, DateTime StartDate, string Server)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, DatabaseOwner + ObjectQualifier + "AddScheduleHistory", ScheduleID, FixDate(StartDate), Server));
        }

        public override void AddScheduleItemSetting(int ScheduleID, string Name, string Value)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddScheduleItemSetting", ScheduleID, Name, Value);
        }

        public override void DeleteSchedule(int ScheduleID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteSchedule", ScheduleID);
        }

        public override IDataReader GetNextScheduledTask(string Server)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetScheduleNextTask", GetNull(Server));
        }

        public override IDataReader GetSchedule()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSchedule", DBNull.Value);
        }

        public override IDataReader GetSchedule(string Server)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetSchedule", GetNull(Server));
        }

        public override IDataReader GetSchedule(int ScheduleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetScheduleByScheduleID", ScheduleID);
        }

        public override IDataReader GetSchedule(string TypeFullName, string Server)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetScheduleByTypeFullName", TypeFullName, GetNull(Server));
        }

        public override IDataReader GetScheduleByEvent(string EventName, string Server)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetScheduleByEvent", EventName, GetNull(Server));
        }

        public override IDataReader GetScheduleHistory(int ScheduleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetScheduleHistory", ScheduleID);
        }

        public override IDataReader GetScheduleItemSettings(int ScheduleID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetScheduleItemSettings", ScheduleID);
        }

        public override void PurgeScheduleHistory()
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "PurgeScheduleHistory");
        }

        public override void UpdateSchedule(int ScheduleID, string TypeFullName, int TimeLapse, string TimeLapseMeasurement, int RetryTimeLapse, string RetryTimeLapseMeasurement, int RetainHistoryNum,
                                            string AttachToEvent, bool CatchUpEnabled, bool Enabled, string ObjectDependencies, string Servers, int LastModifiedByUserID, string FriendlyName)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateSchedule",
                                      ScheduleID,
                                      TypeFullName,
                                      TimeLapse,
                                      TimeLapseMeasurement,
                                      RetryTimeLapse,
                                      RetryTimeLapseMeasurement,
                                      RetainHistoryNum,
                                      AttachToEvent,
                                      CatchUpEnabled,
                                      Enabled,
                                      ObjectDependencies,
                                      GetNull(Servers),
                                      LastModifiedByUserID,
                                      FriendlyName);
        }

        public override void UpdateScheduleHistory(int ScheduleHistoryID, DateTime EndDate, bool Succeeded, string LogNotes, DateTime NextStart)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateScheduleHistory", ScheduleHistoryID, GetNull(EndDate), GetNull(Succeeded), LogNotes, GetNull(NextStart));
        }
    }
}