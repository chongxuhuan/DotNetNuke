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

using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Groups.Components;
using System;
using DotNetNuke.Security.Permissions;

#endregion

namespace DotNetNuke.Modules.Groups
{
    public class GroupsModuleBase : PortalModuleBase {
        public enum GroupMode {
            Setup = 0,
            List = 1,
            View = 2
        }
        #region Public Properties
        public GroupMode LoadView {
            get {
                 if (Settings.ContainsKey(Constants.GroupLoadView)) {
                     switch (Settings[Constants.GroupLoadView].ToString()) {
                         case "List":
                             return GroupMode.List;
                             break;
                         case "View":
                             return GroupMode.View;
                             break;
                         default:
                             return GroupMode.Setup;
                             break;
                     }
                     
             } else {
                    return GroupMode.Setup;
                }
            }
           
        }
        public int GroupId {
            get {
                int groupId = -1;
                if (string.IsNullOrEmpty(Request.QueryString["GroupId"])) {
                    return groupId;
                }
                if (int.TryParse(Request.QueryString["GroupId"].ToString(), out groupId)) {
                    return groupId;
                } else {
                    return -1;
                }

            }
        }
        public int DefaultRoleGroupId {
            get {
                if (Settings.ContainsKey(Constants.DefaultRoleGroupSetting)) {
                    return Convert.ToInt32(Settings[Constants.DefaultRoleGroupSetting].ToString());
                } else {
                    return -1;
                }
            }
        }
        public int GroupListTabId {
            get {
                if (Settings.ContainsKey(Constants.GroupListPage)) {
                    return Convert.ToInt32(Settings[Constants.GroupListPage].ToString());
                } else {
                    return TabId;
                }
            }
        }
        public int GroupViewTabId {
            get {
                if (Settings.ContainsKey(Constants.GroupViewPage)) {
                    return Convert.ToInt32(Settings[Constants.GroupViewPage].ToString());
                } else {
                    return TabId;
                }
            }
        }
        public string GroupViewTemplate {
            get {
                string template = LocalizeString("GroupViewTemplate.Text");
                if (Settings.ContainsKey(Constants.GroupViewTemplate)) {
                    if (!string.IsNullOrEmpty(Settings[Constants.GroupViewTemplate].ToString())) {
                        template = Settings[Constants.GroupViewTemplate].ToString();
                    }
                }
                return template;
            }
        }
        public string GroupListTemplate {
            get {
                string template = LocalizeString("GroupListTemplate.Text");
                if (Settings.ContainsKey(Constants.GroupListTemplate)) {
                    if (!string.IsNullOrEmpty(Settings[Constants.GroupListTemplate].ToString())) {
                        template = Settings[Constants.GroupListTemplate].ToString();
                    }
                }
                return template;
            }
        }
        public string DefaultGroupMode {
            get {
                if (Settings.ContainsKey(Constants.DefautlGroupViewMode)) {
                    return Settings[Constants.DefautlGroupViewMode].ToString();
                } else {
                    return "";
                }
            }
        }
        public bool GroupModerationEnabled {
            get {
                if (Settings.ContainsKey(Constants.GroupModerationEnabled)) {
                    return Convert.ToBoolean(Settings[Constants.GroupModerationEnabled].ToString());
                } else {
                    return false;
                }
            }
        }
        public bool CanCreate {
            get {
                if (Request.IsAuthenticated) {
                    if (UserInfo.IsSuperUser) {
                        return true;
                    } else {
                        return ModulePermissionController.HasModulePermission(this.ModuleConfiguration.ModulePermissions, "CREATEGROUP");
                    }
                   
                } else {
                    return false;
                }
            }
        }

        #endregion
        
        #region Public Methods
        public string GetCreateUrl() {
            return ModuleContext.EditUrl("Create"); //.NavigateUrl(GroupCreateTabId,"",true,null);
        }
        public string GetEditUrl()
        {
            return ModuleContext.EditUrl("GroupId", GroupId.ToString(), "Edit");
        }
        #endregion
    }
}