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

    public class ProcessSolutionFiles : Task
    {
        public string[] FileNames { get; set; }

        public override bool Execute()
        {
            if (FileNames == null)
            {
                return false;
            }
            foreach (var fileName in FileNames)
            {
                try
                {
                    var projectFileInfo = new FileInfo(fileName) { IsReadOnly = false };

                    //var lines = File.ReadAllLines(fileName).ToList();
                    //var newlines = lines.Where(l => !l.StartsWith("\t\tScc")).ToArray();
                    //File.WriteAllLines(fileName, newlines);
                    
                    var tr = new StreamReader(fileName);
                    var content = tr.ReadToEnd();
                    tr.Close();
                    //Remove Unit test projects and solution items
                    var globalIndex = content.IndexOf("Global");
                    var projectsSection = content.Substring(0, globalIndex);
                    var globalSection = content.Substring(globalIndex, content.Length - globalIndex);
                    var globalSections = globalSection.Split(new string[] { "GlobalSection(" }, StringSplitOptions.RemoveEmptyEntries);
                    var projects = projectsSection.Split(new string[] { "Project(" }, StringSplitOptions.RemoveEmptyEntries);

                    //Remoce the TFS section.
                    foreach (var glb in globalSections)
                    {
                        if (glb.Contains("TeamFoundationVersionControl"))
                        {
                            content = content.Replace(string.Concat("GlobalSection(", glb), string.Empty);
                        }
                    }
                    content = content.Replace("SAK", string.Empty);
     
                    var processedSolutionFile = new StreamWriter(fileName);
                    processedSolutionFile.WriteLine(content);
                    processedSolutionFile.Close();

                    foreach (var project in projects)
                    {
                        if (project.Contains("Tests"))
                        {
                            content = content.Replace(string.Concat("Project(", project), string.Empty);
                        }
                    }

                    var newSolutionFile = new StreamWriter(fileName.Replace("_UnitTests", string.Empty));
                    newSolutionFile.WriteLine(content);
                    newSolutionFile.Close();

                    //Convert a copy to VS2008
                    content = content.Replace("# Visual Studio 2010", "# Visual Studio 2008");
                    content = content.Replace("Microsoft Visual Studio Solution File, Format Version 11.00", "Microsoft Visual Studio Solution File, Format Version 10.00");
                    content = content.Replace("vbproj", "VS2008.vbproj").Replace("csproj", "VS2008.csproj");
                    var tw = new StreamWriter(fileName.Replace("UnitTests", "VS2008"));
                    tw.WriteLine(content);
                    tw.Close();
                }
                catch (Exception ex)
                {
                    var file = new StreamWriter("c:\\Builds\\log.txt");
                    file.WriteLine(ex.Message);
                    file.Close();
                    return false;
                }
            }
            return true;
        }
    }
}
