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
using System.Collections;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Upgrade;

#endregion

namespace DotNetNuke.Modules.Messaging
{
    public class MessagingBusinessController : IUpgradeable
    {
        #region IUpgradeable Members

        public string UpgradeModule(string Version)
        {
            try
            {
                switch (Version)
                {
                    case "01.00.00":
                        ModuleDefinitionInfo moduleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("Messaging");

                        if (moduleDefinition != null)
                        {
                            //Add Module to User Profile Page for all Portals
                            var objPortalController = new PortalController();
                            var objTabController = new TabController();
                            var objModuleController = new ModuleController();

                            ArrayList portals = objPortalController.GetPortals();
                            foreach (PortalInfo portal in portals)
                            {
                                int tabID = TabController.GetTabByTabPath(portal.PortalID, "//UserProfile", Null.NullString);
                                if ((tabID != Null.NullInteger))
                                {
                                    TabInfo tab = objTabController.GetTab(tabID, portal.PortalID, true);
                                    if ((tab != null))
                                    {
                                        int moduleId = Upgrade.AddModuleToPage(tab, moduleDefinition.ModuleDefID, "My Inbox", "", true);
                                        ModuleInfo objModule = objModuleController.GetModule(moduleId, tabID, false);

                                    	var settings = new PortalSettings(portal);

                                        ArrayList permissions = new PermissionController().GetPermissionByCodeAndKey("SYSTEM_MODULE_DEFINITION", "EDIT");
                                        PermissionInfo permission = null;
                                        if (permissions.Count == 1)
                                        {
                                            permission = permissions[0] as PermissionInfo;
                                        }
                                        if (permission != null)
                                        {
                                            var modulePermission = new ModulePermissionInfo(permission);
                                            modulePermission.ModuleID = moduleId;
                                            modulePermission.RoleID = settings.RegisteredRoleId;
                                            modulePermission.UserID = Null.NullInteger;
                                            modulePermission.AllowAccess = true;

                                            objModule.ModulePermissions.Add(modulePermission);

                                            ModulePermissionController.SaveModulePermissions(objModule);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }
                return "Success";
            }
            catch (Exception exc)
            {
                DnnLog.Error(exc);

                return "Failed";
            }
        }

        #endregion
    }
}