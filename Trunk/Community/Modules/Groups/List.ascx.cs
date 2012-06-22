using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Framework;

namespace DotNetNuke.Modules.Groups {
    public partial class List : GroupsModuleBase {
        protected void Page_Load(object sender, EventArgs e) {
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            ctlGroupList.GroupViewTabId = GroupViewTabId;
            ctlGroupList.RoleGroupId = DefaultRoleGroupId;
            ctlGroupList.PageSize = 20;
            ctlGroupList.TabId = TabId;
            if (!String.IsNullOrEmpty(GroupListTemplate)) {
                ctlGroupList.ItemTemplate = GroupListTemplate;
            }
           
        }
    }
}