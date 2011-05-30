using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using log4net;
using Microsoft.Build.Utilities;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DotNetNuke.MSBuild.Tasks
{
    public class TfsComments : Task
    {
        public string Server { get; set; }
        public string Root { get; set; }
        public string Committer { get; set; }

        public override bool Execute()
        {
            Server = "https://tfs.dotnetnuke.com:8088/tfs";
            Root = @"$/DotNetNuke/src/DotNetNuke_CS/";
            Committer = "MAXIMUMASPLOCAL\\DED275DNETCCNet";

            TeamFoundationServer tfs = TeamFoundationServerFactory.GetServer(Server);
            VersionControlServer vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));

            string path = Root;
            VersionSpec version = VersionSpec.Latest;
            int deletionId = 0;
            RecursionType recursion = RecursionType.Full;
            string user = null;// "mahesh_lambe";
            VersionSpec versionFrom = null; // VersionSpec.ParseSingleSpec("115894", "mahesh_lambe"); //keep it 'null' for first version
            VersionSpec versionTo = null; // VersionSpec.ParseSingleSpec("109014", "mahesh_lambe"); //keep it 'null' for latest version
            int maxCount = 100;// Int32.MaxValue;
            bool includeChanges = true;
            bool slotMode = true;
            bool includeDownloadInfo = true;

            IEnumerable enumerable =
              vcs.QueryHistory(path,
                              version,
                              deletionId,
                              recursion,
                              user,
                              versionFrom,
                              versionTo,
                              maxCount,
                              includeChanges,
                              slotMode,
                              includeDownloadInfo);


            var c = new List<Changeset>();

            foreach (var i in enumerable)
            {
                var cs = (i as Changeset);
                if (cs != null && cs.Committer != Committer)
                {
                    foreach(var change in cs.Changes)
                    {
                     if(!change.Item.ServerItem.Contains("Professional"))
                        {
                         c.Add(cs);
                       }    
                    }
                }
            }
            return true;
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
