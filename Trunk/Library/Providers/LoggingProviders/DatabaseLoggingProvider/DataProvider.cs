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

namespace DotNetNuke.Services.Log.EventLog.DBLoggingProvider
{
    public abstract class DataProvider
    {
        //return the provider
        public static DataProvider Instance()
        {
            return ComponentFactory.GetComponent<DataProvider>();
        }

        public abstract void AddLog(string logGUID, string logTypeKey, int logUserID, string logUserName, int logPortalID, string logPortalName, DateTime logCreateDate, string logServerName,
                                    string logProperties, int logConfigID);

        public abstract void DeleteLog(string logGUID);

        public abstract void PurgeLog();

        public abstract void ClearLog();

        public abstract void AddLogTypeConfigInfo(bool loggingIsActive, string logTypeKey, string logTypePortalID, int keepMostRecent, bool emailNotificationIsActive, int threshold,
                                                  int notificationThresholdTime, int notificationThresholdTimeType, string mailFromAddress, string mailToAddress);

        public abstract void DeleteLogTypeConfigInfo(string id);

        public abstract IDataReader GetLogTypeConfigInfo();

        public abstract IDataReader GetLogTypeConfigInfoByID(int id);

        public abstract void UpdateLogTypeConfigInfo(string id, bool loggingIsActive, string logTypeKey, string logTypePortalID, int keepMostRecent, bool emailNotificationIsActive,
                                                     int threshold, int notificationThresholdTime, int notificationThresholdTimeType, string mailFromAddress, string mailToAddress);

        public abstract void AddLogType(string logTypeKey, string logTypeFriendlyName, string logTypeDescription, string logTypeCSSClass, string logTypeOwner);

        public abstract void UpdateLogType(string logTypeKey, string logTypeFriendlyName, string logTypeDescription, string logTypeCSSClass, string logTypeOwner);

        public abstract void DeleteLogType(string logTypeKey);

        public abstract IDataReader GetLogTypeInfo();

        public abstract IDataReader GetLogs(int portalID, string logType, int pageSize, int pageIndex);

        public abstract IDataReader GetSingleLog(string logGUID);

        public abstract IDataReader GetEventLogPendingNotifConfig();

        public abstract IDataReader GetEventLogPendingNotif(int logConfigID);

        public abstract void UpdateEventLogPendingNotif(int logConfigID);
    }
}