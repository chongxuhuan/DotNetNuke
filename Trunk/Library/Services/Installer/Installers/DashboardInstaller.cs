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
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Modules.Dashboard.Components;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    public class DashboardInstaller : ComponentInstallerBase
    {
        private string ControllerClass;
        private bool IsEnabled;
        private string Key;
        private string LocalResources;
        private string Src;
        private DashboardControl TempDashboardControl;
        private int ViewOrder;

        public override string AllowableFiles
        {
            get
            {
                return "ashx, aspx, ascx, vb, cs, resx, css, js, resources, config, vbproj, csproj, sln, htm, html";
            }
        }

        private void DeleteDashboard()
        {
            try
            {
                DashboardControl dashboardControl = DashboardController.GetDashboardControlByPackageId(Package.PackageID);
                if (dashboardControl != null)
                {
                    DashboardController.DeleteControl(dashboardControl);
                }
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.AUTHENTICATION_UnRegistered);
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void Commit()
        {
        }

        public override void Install()
        {
            bool bAdd = Null.NullBoolean;
            try
            {
                TempDashboardControl = DashboardController.GetDashboardControlByKey(Key);
                var dashboardControl = new DashboardControl();
                if (TempDashboardControl == null)
                {
                    dashboardControl.IsEnabled = true;
                    bAdd = true;
                }
                else
                {
                    dashboardControl.DashboardControlID = TempDashboardControl.DashboardControlID;
                    dashboardControl.IsEnabled = TempDashboardControl.IsEnabled;
                }
                dashboardControl.DashboardControlKey = Key;
                dashboardControl.PackageID = Package.PackageID;
                dashboardControl.DashboardControlSrc = Src;
                dashboardControl.DashboardControlLocalResources = LocalResources;
                dashboardControl.ControllerClass = ControllerClass;
                dashboardControl.ViewOrder = ViewOrder;
                if (bAdd)
                {
                    DashboardController.AddDashboardControl(dashboardControl);
                }
                else
                {
                    DashboardController.UpdateDashboardControl(dashboardControl);
                }
                Completed = true;
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.DASHBOARD_Registered);
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void ReadManifest(XPathNavigator manifestNav)
        {
            Key = Util.ReadElement(manifestNav, "dashboardControl/key", Log, Util.DASHBOARD_KeyMissing);
            Src = Util.ReadElement(manifestNav, "dashboardControl/src", Log, Util.DASHBOARD_SrcMissing);
            LocalResources = Util.ReadElement(manifestNav, "dashboardControl/localResources", Log, Util.DASHBOARD_LocalResourcesMissing);
            ControllerClass = Util.ReadElement(manifestNav, "dashboardControl/controllerClass");
            IsEnabled = bool.Parse(Util.ReadElement(manifestNav, "dashboardControl/isEnabled", "true"));
            ViewOrder = int.Parse(Util.ReadElement(manifestNav, "dashboardControl/viewOrder", "-1"));
            if (Log.Valid)
            {
                Log.AddInfo(Util.DASHBOARD_ReadSuccess);
            }
        }

        public override void Rollback()
        {
            if (TempDashboardControl == null)
            {
                DeleteDashboard();
            }
            else
            {
                DashboardController.UpdateDashboardControl(TempDashboardControl);
            }
        }

        public override void UnInstall()
        {
            try
            {
                DashboardControl dashboardControl = DashboardController.GetDashboardControlByPackageId(Package.PackageID);
                if (dashboardControl != null)
                {
                    DashboardController.DeleteControl(dashboardControl);
                }
                Log.AddInfo(dashboardControl.DashboardControlKey + " " + Util.DASHBOARD_UnRegistered);
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }
    }
}
