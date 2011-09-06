using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public class GenerateSolutionFiles : Task
    {
        public string OriginalFile { get; set; }
        public string CreatedFile { get; set; }
        public string[] ProjectNames { get; set; }
        public string[] Replacements { get; set; }
        public bool RemoveTfsBindings { get; set; }

        public override bool Execute()
        {
            if (ProjectNames == null)
            {
                return true;
            }
            try
            {
                var builder = new StringBuilder();
                foreach (var value in ProjectNames)
                {
                    builder.Append(value);
                    builder.Append('.');
                }

                LogFormat("Message", "----------------------------------");
                LogFormat("Message", "GenerateSolutionFiles Logging Info");
                LogFormat("Message", "----------------------------------");
                LogFormat("Message", CreatedFile);
                LogFormat("Message", builder.ToString());

                var tr = new StreamReader(OriginalFile, true);
                tr.Peek();
                var fileEncoding = tr.CurrentEncoding;
                var content = tr.ReadToEnd();
                tr.Close();
                LogFormat("Message", "File Encoding " + fileEncoding);
                var globalIndex = content.IndexOf("Global");
                var globalSection = content.Substring(globalIndex, content.Length - globalIndex);
                var globalSections = globalSection.Split(new string[] { "GlobalSection(" }, StringSplitOptions.RemoveEmptyEntries);
                var projectsSection = content.Substring(0, globalIndex);
                projectsSection = projectsSection.Remove(0, projectsSection.IndexOf("Project("));
                var projects = projectsSection.Split(new string[] { "Project(" }, StringSplitOptions.RemoveEmptyEntries);
                var nestedProjectsSection = string.Empty;
                if (RemoveTfsBindings)
                {
                    //Remove the TFS section.
                    content = globalSections.Where(glb => glb.Contains("TeamFoundationVersionControl")).Aggregate(content, (current, glb) => current.Replace(string.Concat("GlobalSection(", glb), string.Empty));
                    content = content.Replace("SAK", string.Empty);
                }
                foreach (var glb in globalSections.Where(glb => glb.Contains("NestedProjects")))
                {
                    nestedProjectsSection = string.Concat("GlobalSection(", glb);
                }
                var nestedProjects = nestedProjectsSection.Split(new char[] { '\r', '\n', '\t', '\t' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var projectName in ProjectNames)
                {
                    foreach (var project in projects)
                    {
                        var projectEntry = "";
                        if (project.Contains("ProjectSection"))
                        {
                            var indexProjectSection = project.IndexOf("ProjectSection");
                            projectEntry = project.Substring(0,  indexProjectSection).Replace("EndProject", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
                        }
                        else
                        {
                            projectEntry = project.Replace("EndProject", string.Empty).Replace("\"", string.Empty).Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty).Replace("\t", string.Empty);
                        }

                        var indexFirstEquals = projectEntry.IndexOf("=") + 1;
                        projectEntry = projectEntry.Remove(0, indexFirstEquals);
                        var projectNameGuid = projectEntry.Split(',');
                        if (projectNameGuid[0] == projectName)
                        {
                            content = content.Replace(string.Concat("Project(", project), string.Empty);
                            var nestedProject = nestedProjects.FirstOrDefault(l => l.StartsWith(projectNameGuid[2]));
                            if (nestedProject != default(string))
                            {
                                nestedProjects.RemoveAll(l => l.StartsWith(projectNameGuid[2]));
                            }
                        }
                    }
                }
                var filteredNestedProjects = string.Concat(string.Join("\r\n\t\t", nestedProjects.ToArray()), "\r\n\t").Replace("\tEndGlobalSection", "EndGlobalSection");
                content = content.Replace(nestedProjectsSection, filteredNestedProjects);

                if (Replacements != null)
                {
                    for (var index = 0; index <= Replacements.Length - 1; index = index + 2)
                    {
                        content = content.Replace(Replacements[index], Replacements[index + 1]);
                    }
                }

                if (CreatedFile.Contains("_VS2008"))
                {
                    //Convert a copy to VS2008
                    content = content.Replace("# Visual Studio 2010", "# Visual Studio 2008");
                    content = content.Replace("Microsoft Visual Studio Solution File, Format Version 11.00", "Microsoft Visual Studio Solution File, Format Version 10.00");
                    content = content.Replace("vbproj", "VS2008.vbproj").Replace("csproj", "VS2008.csproj");
                }

                var newSolutionFile = new StreamWriter(CreatedFile, false, fileEncoding);
                newSolutionFile.WriteLine(content);
                newSolutionFile.Close();

                var newFileSr = new StreamReader(OriginalFile, true);
                newFileSr.Peek();
                var newFileEncoding = newFileSr.CurrentEncoding;

                LogFormat("Message", "New File " + CreatedFile);
                LogFormat("Message", "File Encoding " + newFileEncoding);
                LogFormat("Message", "--------------------------------------");
                LogFormat("Message", "End GenerateSolutionFiles Logging Info");
                LogFormat("Message", "--------------------------------------");
            }
            catch (Exception ex)
            {
                LogFormat("Error", ex.StackTrace);
                return false;
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
