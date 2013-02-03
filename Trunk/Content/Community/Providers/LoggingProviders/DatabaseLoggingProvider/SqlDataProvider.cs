#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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

using DotNetNuke.Common.Utilities;

using Microsoft.ApplicationBlocks.Data;

#endregion

namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider
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

        private static object GetNull(object field)
        {
            return Null.GetNull(field, DBNull.Value);
        }

        public override void AddLog(string logGUID, string logTypeKey, int logUserID, string logUserName, int logPortalID, string logPortalName, DateTime logCreateDate, string logServerName,
                                    string logProperties, int logConfigID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "AddEventLog",
                                      logGUID,
                                      logTypeKey,
                                      GetNull(logUserID),
                                      GetNull(logUserName),
                                      GetNull(logPortalID),
                                      GetNull(logPortalName),
                                      logCreateDate,
                                      logServerName,
                                      logProperties,
                                      logConfigID);
        }

        public override void AddLogType(string logTypeKey, string logTypeFriendlyName, string logTypeDescription, string logTypeCSSClass, string logTypeOwner)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "AddEventLogType", logTypeKey, logTypeFriendlyName, logTypeDescription, logTypeOwner, logTypeCSSClass);
        }

        public override void AddLogTypeConfigInfo(bool loggingIsActive, string logTypeKey, string logTypePortalID, int keepMostRecent, bool emailNotificationIsActive, int threshold,
                                                  int notificationThresholdTime, int notificationThresholdTimeType, string mailFromAddress, string mailToAddress)
        {
            int portalID;
            if (logTypeKey == "*")
            {
                logTypeKey = "";
            }
            if (logTypePortalID == "*")
            {
                portalID = -1;
            }
            else
            {
                portalID = Convert.ToInt32(logTypePortalID);
            }
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "AddEventLogConfig",
                                      GetNull(logTypeKey),
                                      GetNull(portalID),
                                      loggingIsActive,
                                      keepMostRecent,
                                      emailNotificationIsActive,
                                      GetNull(threshold),
                                      GetNull(notificationThresholdTime),
                                      GetNull(notificationThresholdTimeType),
                                      mailFromAddress,
                                      mailToAddress);
        }

        public override void ClearLog()
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLog", DBNull.Value);
        }

        public override void DeleteLog(string logGUID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLog", logGUID);
        }

        public override void DeleteLogType(string logTypeKey)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLogType", logTypeKey);
        }

        public override void DeleteLogTypeConfigInfo(string id)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "DeleteEventLogConfig", id);
        }

        public override IDataReader GetEventLogPendingNotif(int logConfigID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogPendingNotif", logConfigID);
        }

        public override IDataReader GetEventLogPendingNotifConfig()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogPendingNotifConfig");
        }

        public override IDataReader GetLogs(int portalID, string logType, int pageSize, int pageIndex)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLog", GetNull(portalID), GetNull(logType), pageSize, pageIndex);
        }

        public override IDataReader GetLogTypeConfigInfo()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogConfig", DBNull.Value);
        }

        public override IDataReader GetLogTypeConfigInfoByID(int id)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogConfig", id);
        }

        public override IDataReader GetLogTypeInfo()
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogType");
        }

        public override IDataReader GetSingleLog(string logGUID)
        {
            return SqlHelper.ExecuteReader(ConnectionString, DatabaseOwner + ObjectQualifier + "GetEventLogByLogGUID", logGUID);
        }

        public override void PurgeLog()
        {
			//Because event log is run on application end, app may not be fully installed, so check for the sproc first
            string sql = "IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'" + DatabaseOwner + ObjectQualifier + "PurgeEventLog') AND OBJECTPROPERTY(id, N'IsProcedure') = 1) " + " BEGIN " +
                         "    EXEC " + DatabaseOwner + ObjectQualifier + "PurgeEventLog" + " END ";
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, sql);
        }

        public override void UpdateEventLogPendingNotif(int logConfigID)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogPendingNotif", logConfigID);
        }

        public override void UpdateLogType(string logTypeKey, string logTypeFriendlyName, string logTypeDescription, string logTypeCSSClass, string logTypeOwner)
        {
            SqlHelper.ExecuteNonQuery(ConnectionString, DatabaseOwner + ObjectQualifier + "UpdateEventLogType", logTypeKey, logTypeFriendlyName, logTypeDescription, logTypeOwner, logTypeCSSClass);
        }

        public override void UpdateLogTypeConfigInfo(string id, bool loggingIsActive, string logTypeKey, string logTypePortalID, int keepMostRecent, bool emailNotificationIsActive,
                                                     int threshold, int notificationThresholdTime, int notificationThresholdTimeType, string mailFromAddress, string mailToAddress)
        {
            int portalID;
            if (logTypeKey == "*")
            {
                logTypeKey = "";
            }
            if (logTypePortalID == "*")
            {
                portalID = -1;
            }
            else
            {
                portalID = Convert.ToInt32(logTypePortalID);
            }
            SqlHelper.ExecuteNonQuery(ConnectionString,
                                      DatabaseOwner + ObjectQualifier + "UpdateEventLogConfig",
                                      id,
                                      GetNull(logTypeKey),
                                      GetNull(portalID),
                                      loggingIsActive,
                                      keepMostRecent,
                                      emailNotificationIsActive,
                                      GetNull(threshold),
                                      GetNull(notificationThresholdTime),
                                      GetNull(notificationThresholdTimeType),
                                      mailFromAddress,
                                      mailToAddress);
        }
    }
}