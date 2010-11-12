using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces; 

namespace DotNetNuke.MSBuild.Tasks
{
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public class Powershell : Task
    {
        [Required]
        public ITaskItem Script { get; set; }

        [Required]
        public string Function { get; set; }
        public ITaskItem[] Parameters { get; set; }

        public override bool Execute()
        {
            RunspaceConfiguration runspaceConfig = RunspaceConfiguration.Create();

            using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfig))
            {
                runspace.Open();

                StringBuilder commandLine = new StringBuilder();
                commandLine.Append(Function + " ");

                foreach (ITaskItem parameter in Parameters)
                {
                    commandLine.AppendFormat("\"{0}\" ", parameter.ItemSpec);
                }

                using (RunspaceInvoke scriptInvoker = new RunspaceInvoke(runspace))
                {
                    scriptInvoker.Invoke(Script.ItemSpec);
                    scriptInvoker.Invoke(commandLine.ToString());
                }
            }

            return true;
        }
    }
}
