using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using log4net;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml;

namespace DotNetNuke.MSBuild.Tasks
{
    using System.Xml.XPath;
    using System.IO;
    using System.Xml.Linq;

    public class DotNetNukeDeploy : Task
    {
        private readonly IISManager iisMgr = new IISManager();
        public string PhysicalPath { get; set; }
        public string WebsiteName { get; set; }
        public string AppPool { get; set; }
        bool autoFailed;
        [Output]
        public string Error { get; set; }

        public override bool Execute()
        {
            try
            {
                iisMgr.CreateVirtualDirectory(WebsiteName, PhysicalPath, AppPool);
                DirectoryManager.SetFolderPermissions("NETWORK SERVICE", new DirectoryInfo(PhysicalPath));
                autoFailed = false;
                var url = string.Format("http://localhost/{0}/Install/Install.aspx?mode=install", WebsiteName);
                LogFormat("Install URL: - " + url + "\r\n");
                var wc = new WebClient();
                var data = wc.DownloadString(url);
                autoFailed = (data.Contains("Error") || data.Contains("bypasses"));
                LogFormat("-----------------------------");
                LogFormat("DNN INSTALL LOGGING INFO");
                LogFormat("-----------------------------");
                iisMgr.DeleteVirtualDirectory("localhost", WebsiteName);
                if (autoFailed)
                {
                    Log.LogMessage(data);
                    return false;
                }
                LogFormat("-------------------------------------------\r\n");
                return true;
            }
            catch (Exception ex)
            {
                Error = "ERROR OCCURRED DURING AUTO-INSTALL" + ex.Message + "-- Stack: " + ex.StackTrace;
                LogFormat(Error);
                return false;
            }
        }
        private void LogFormat(string message, params object[] args)
        {
            if (BuildEngine != null)
            {
                Log.LogMessage(message, args);
            } 
            else
            {
                Debug.Print(message, args);
            }
        }

    }
}
