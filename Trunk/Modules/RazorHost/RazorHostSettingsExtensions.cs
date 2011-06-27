using System.Web.UI;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security;

namespace DotNetNuke.Modules.RazorHost
{
    public static class RazorHostSettingsExtensions
    {
        public static Settings LoadRazorSettingsControl(this UserControl parent, ModuleInfo configuration, string localResourceFile)
        {
            var control = (Settings) parent.LoadControl("~/DesktopModules/RazorModules/RazorHost/Settings.ascx");
            control.ModuleConfiguration = configuration;
            control.LocalResourceFile = localResourceFile;
            EnsureEditScriptControlIsRegistered(configuration.ModuleDefID);
            return control;
        }


        private static void EnsureEditScriptControlIsRegistered(int moduleDefId)
        {
            if (ModuleControlController.GetModuleControlByControlKey("EditRazorScript", moduleDefId) != null) return;
            var m = new ModuleControlInfo
                        {
                            ControlKey = "EditRazorScript",
                            ControlSrc = "DesktopModules/RazorModules/RazorHost/EditScript.ascx",
                            ControlTitle = "Edit Script",
                            ControlType = SecurityAccessLevel.Host,
                            ModuleDefID = moduleDefId
                        };
            ModuleControlController.UpdateModuleControl(m);
        }
    }
}