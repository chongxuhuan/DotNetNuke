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
                if (iisMgr.VDirExists(WebsiteName))
                {
                    iisMgr.DeleteVirtualDirectory("localhost", WebsiteName);
                }

                iisMgr.CreateVirtualDirectory(WebsiteName, PhysicalPath, AppPool);
                DirectoryManager.SetFolderPermissions("NETWORK SERVICE", new DirectoryInfo(PhysicalPath));
                autoFailed = false;
                var url = string.Format("http://localhost/{0}/Install/Install.aspx?mode=install", WebsiteName);
                LogFormat("Message", "Install URL: - " + url + "\r\n");
                var wc = new WebClient();
                var data = wc.DownloadString(url);
                autoFailed = (data.Contains("Error") || data.Contains("bypasses"));
                if (!autoFailed)
                {
                    var homePage = wc.DownloadString(string.Format("http://localhost/{0}/default.aspx", WebsiteName));
                }
                LogFormat("Message", "-----------------------------");
                LogFormat("Message", "DNN INSTALL LOGGING INFO");
                LogFormat("Message", "-----------------------------");
                iisMgr.DeleteVirtualDirectory("localhost", WebsiteName);
                if (autoFailed)
                {
                    LogFormat("Error", data);
                    return false;
                }
                LogFormat("Message", "-------------------------------------------\r\n");
                return true;
            }
            catch (Exception ex)
            {
                Error = "ERROR OCCURRED DURING AUTO-INSTALL" + ex.Message + "-- Stack: " + ex.StackTrace;
                LogFormat("Error", Error);
                return false;
            }
        }
        private void LogFormat(string level, string message, params object[] args)
        {
            if (BuildEngine != null)
            {
                switch (level)
                {
                    case "Message":
                        Log.LogMessage(message, args);
                       break;
                    case "Error":
                       Log.LogError(message, args);
                        break;
                }
            }
            else
            {
                Debug.Print(message, args);
            }
        }

    }
}
