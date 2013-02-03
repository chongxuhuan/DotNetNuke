using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using log4net;
using Microsoft.Build.Utilities;

namespace DotNetNuke.MSBuild.Tasks
{
    public class CodePlexUpdate : Task
    {
        public string TFSServer { get; set; }
        public string TFSRoot { get; set; }
        public string TFSMappedFolder { get; set; }
        public string TFSCommitter { get; set; }

        public string CPServer { get; set; }
        public string CPRoot { get; set; }
        public string TempFolder { get; set; }

        public override bool Execute()
        {
#pragma warning disable 612,618
            TeamFoundationServer tfs = TeamFoundationServerFactory.GetServer(TFSServer);
#pragma warning restore 612,618
            var vcs = (VersionControlServer)tfs.GetService(typeof(VersionControlServer));

            string path = TFSRoot;
            VersionSpec version = VersionSpec.Latest;
            const int deletionId = 0;
            const RecursionType recursion = RecursionType.Full;
            string user = null;// "mahesh_lambe";
            VersionSpec versionFrom = new DateVersionSpec(DateTime.Today.AddDays(-1));
            VersionSpec versionTo = null;
            const int maxCount = 100;
            const bool includeChanges = true;
            const bool slotMode = true;
            const bool includeDownloadInfo = true;

            IEnumerable allChangeSets =
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


            var changes = new List<Change>();
            var comments = new StringBuilder();
            foreach (var changeSet in allChangeSets.Cast<Changeset>().Where(changeSet => changeSet != null && changeSet.Committer != TFSCommitter))
            {
                comments.Append(changeSet.Comment);
                changes.AddRange(changeSet.Changes.Where(change => !change.Item.ServerItem.Contains("Community")));
            }
            Directory.CreateDirectory(TempFolder);
            using (var tfsCodePlex = TeamFoundationServerFactory.GetServer(CPServer))
            {
                var vcsCodePlex = tfsCodePlex.GetService(typeof(VersionControlServer)) as VersionControlServer;
                vcsCodePlex.DeleteWorkspace("CodePlex Temporary Workspace", vcsCodePlex.AuthenticatedUser);
                var workspace = vcsCodePlex.CreateWorkspace("CodePlex Temporary Workspace", vcsCodePlex.AuthenticatedUser);
                try
                {
                    var workingFolder = new WorkingFolder(CPRoot, TempFolder);
                    workspace.CreateMapping(workingFolder);
                    foreach (Change change in changes)
                    {
                        var changedFileName = change.Item.ServerItem.Replace(TFSRoot, TFSMappedFolder);
                        var destFileName = change.Item.ServerItem.Replace(TFSRoot, TempFolder);
                        var destSccName = change.Item.ServerItem.Replace(TFSRoot, CPRoot);
                        var isDeleted = change.Item.DeletionId;
                        switch (change.ChangeType)
                        {
                            case ChangeType.Add:
                                Directory.CreateDirectory(destFileName.Remove(destFileName.LastIndexOf("/", System.StringComparison.InvariantCulture)));
                                File.Copy(changedFileName, destFileName);
                                workspace.PendAdd(destSccName);
                                break;
                            case ChangeType.Edit:
                                workspace.Get(new GetRequest(destSccName, RecursionType.None, VersionSpec.Latest), GetOptions.Overwrite);
                                workspace.PendEdit(destSccName);
                                //var sourceFile = File.OpenText(changedFileName).ReadToEnd();
                                File.Copy(changedFileName, destFileName, true);
                                break;
                            case ChangeType.Delete:
                                workspace.Get(new GetRequest(destSccName, RecursionType.None, VersionSpec.Latest), GetOptions.Overwrite);
                                workspace.PendDelete(destSccName);
                                File.Delete(destFileName);
                                break;
                        };
                        Debug.Print(changedFileName);
                        Debug.Print(destFileName);
                    }

                    // Check in the changes made.
                    var pendingChanges = workspace.GetPendingChanges();
                    if (pendingChanges.Any())
                    {
                        workspace.CheckIn(pendingChanges, comments.ToString());
                    }
                }
                catch (Exception ex)
                {
                    LogFormat("Error", ex.Message);
                }
                finally
                {
                    // Cleanup the workspace.
                    workspace.Delete();

                    // Remove the temp folder used.
                    Directory.Delete(TempFolder, true);
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
