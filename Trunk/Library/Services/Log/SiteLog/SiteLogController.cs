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
using System.Data;
using System.IO;
using System.Text;
using System.Threading;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Host;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security;

#endregion

namespace DotNetNuke.Services.Log.SiteLog
{
    public class SiteLogController
    {
        public void AddSiteLog(int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId, int SiteLogBuffer,
                               string SiteLogStorage)
        {
            var objSecurity = new PortalSecurity();
            try
            {
                if (Host.PerformanceSetting == Globals.PerformanceSettings.NoCaching)
                {
                    SiteLogBuffer = 1;
                }
                switch (SiteLogBuffer)
                {
                    case 0:
                        break;
                    case 1:
                        switch (SiteLogStorage)
                        {
                            case "D":
                                DataProvider.Instance().AddSiteLog(DateTime.Now,
                                                                   PortalId,
                                                                   UserId,
                                                                   objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                                                   objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                                                   objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                                                   objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                                                   objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                                                   TabId,
                                                                   AffiliateId);
                                break;
                            case "F":
                                W3CExtendedLog(DateTime.Now,
                                               PortalId,
                                               UserId,
                                               objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                               objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                               objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                               objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                               objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup),
                                               TabId,
                                               AffiliateId);
                                break;
                        }
                        break;
                    default:
                        string key = "SiteLog" + PortalId;
                        var arrSiteLog = (ArrayList) DataCache.GetCache(key);
                        if (arrSiteLog == null)
                        {
                            arrSiteLog = new ArrayList();
                            DataCache.SetCache(key, arrSiteLog);
                        }
                        var objSiteLog = new SiteLogInfo();
                        objSiteLog.DateTime = DateTime.Now;
                        objSiteLog.PortalId = PortalId;
                        objSiteLog.UserId = UserId;
                        objSiteLog.Referrer = objSecurity.InputFilter(Referrer, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.URL = objSecurity.InputFilter(URL, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.UserAgent = objSecurity.InputFilter(UserAgent, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.UserHostAddress = objSecurity.InputFilter(UserHostAddress, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.UserHostName = objSecurity.InputFilter(UserHostName, PortalSecurity.FilterFlag.NoScripting | PortalSecurity.FilterFlag.NoMarkup);
                        objSiteLog.TabId = TabId;
                        objSiteLog.AffiliateId = AffiliateId;
                        arrSiteLog.Add(objSiteLog);
                        if (arrSiteLog.Count >= SiteLogBuffer)
                        {
                            var objBufferedSiteLog = new BufferedSiteLog();
                            objBufferedSiteLog.SiteLogStorage = SiteLogStorage;
                            objBufferedSiteLog.SiteLog = arrSiteLog;
                            DataCache.RemoveCache(key);
                            var objThread = new Thread(objBufferedSiteLog.AddSiteLog);
                            objThread.Start();
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                Instrumentation.DnnLog.Error(exc);

            }
        }

        public IDataReader GetSiteLog(int PortalId, string PortalAlias, int ReportType, DateTime StartDate, DateTime EndDate)
        {
            return DataProvider.Instance().GetSiteLog(PortalId, PortalAlias, "GetSiteLog" + ReportType, StartDate, EndDate);
        }

        public void DeleteSiteLog(DateTime DateTime, int PortalId)
        {
            DataProvider.Instance().DeleteSiteLog(DateTime, PortalId);
        }

        public void W3CExtendedLog(DateTime DateTime, int PortalId, int UserId, string Referrer, string URL, string UserAgent, string UserHostAddress, string UserHostName, int TabId, int AffiliateId)
        {
            StreamWriter objStream;
            string LogFilePath = Globals.ApplicationMapPath + "\\Portals\\" + PortalId + "\\Logs\\";
            string LogFileName = "ex" + DateTime.Now.ToString("yyMMdd") + ".log";
            if (!File.Exists(LogFilePath + LogFileName))
            {
                try
                {
                    Directory.CreateDirectory(LogFilePath);
                    objStream = File.AppendText(LogFilePath + LogFileName);
                    objStream.WriteLine("#Software: Microsoft Internet Information Services 6.0");
                    objStream.WriteLine("#Version: 1.0");
                    objStream.WriteLine("#Date: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
                    objStream.WriteLine("#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) sc-status sc-substatus sc-win32-status");
                    objStream.Flush();
                    objStream.Close();
                }
				catch (Exception ex)
				{
					DnnLog.Error(ex);
				}
            }
            try
            {
                objStream = File.AppendText(LogFilePath + LogFileName);
                var objStringBuilder = new StringBuilder(1024);
                objStringBuilder.Append(DateTime.ToString("yyyy-MM-dd hh:mm:ss") + " ");
                objStringBuilder.Append(UserHostAddress + " ");
                objStringBuilder.Append("GET" + " ");
                objStringBuilder.Append(URL + " ");
                objStringBuilder.Append("-" + " ");
                objStringBuilder.Append("80" + " ");
                objStringBuilder.Append("-" + " ");
                objStringBuilder.Append(UserHostAddress + " ");
                objStringBuilder.Append(UserAgent.Replace(" ", "+") + " ");
                objStringBuilder.Append("200" + " ");
                objStringBuilder.Append("0" + " ");
                objStringBuilder.Append("0");
                objStream.WriteLine(objStringBuilder.ToString());
                objStream.Flush();
                objStream.Close();
            }
			catch (Exception ex)
			{
				DnnLog.Error(ex);
			}
        }
    }
}