using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override bool Execute()
        {
            if (ProjectNames == null)
            {
                return true;
            }
            try
            {
                var sFile = new FileInfo(OriginalFile);

                var tr = new StreamReader(OriginalFile);
                var content = tr.ReadToEnd();
                tr.Close();
                var globalIndex = content.IndexOf("Global");
                var globalSection = content.Substring(globalIndex, content.Length - globalIndex);
                var globalSections = globalSection.Split(new string[] { "GlobalSection(" }, StringSplitOptions.RemoveEmptyEntries);
                var projectsSection = content.Substring(0, globalIndex);
                var projects = projectsSection.Split(new string[] { "Project(" }, StringSplitOptions.RemoveEmptyEntries);
                //Remoce the TFS section.
                foreach (var glb in globalSections)
                {
                    if (glb.Contains("TeamFoundationVersionControl"))
                    {
                        content = content.Replace(string.Concat("GlobalSection(", glb), string.Empty);
                    }
                }

                //Remove the EE projects from EE sln file and save as PE Elite Unit tests sln file.
                foreach (var projectName in ProjectNames)
                {
                    foreach (var project in projects)
                    {
                        if (project.Contains(projectName))
                        {
                            content = content.Replace(string.Concat("Project(", project), string.Empty);
                        }
                    }
                }
                if (Replacements != null)
                {
                    for (var index = 0; index <= Replacements.Length - 1; index = index + 2)
                    {
                        content = content.Replace(Replacements[index], Replacements[index + 1]);
                    }
                }

                var newSolutionFile = new StreamWriter(CreatedFile);
                newSolutionFile.WriteLine(content);
                newSolutionFile.Close();
            }
            catch (Exception ex)
            {
                var file = new StreamWriter("c:\\Builds\\log.txt");
                file.WriteLine(ex.Message);
                file.Close();
                return false;
            }
            return true;
        }
    }
}
