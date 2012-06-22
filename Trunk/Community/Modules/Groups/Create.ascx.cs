using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Security.Roles;
using DotNetNuke.Security.Roles.Internal;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.FileSystem;
using System.IO;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Modules.Groups.Components;
using DotNetNuke.Services.Journal;

namespace DotNetNuke.Modules.Groups {
    
    public partial class Create : GroupsModuleBase {

        protected override void OnInit(EventArgs e) {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent() {
            Load += Page_Load;
            btnCreate.Click += Create_Click;
            btnCancel.Click += Cancel_Click;
        }

        
        protected void Page_Load(object sender, EventArgs e) {
            
        }
        private void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(ModuleContext.NavigateUrl(TabId,string.Empty,false, null));
        }
        private void Create_Click(object sender, EventArgs e) {
            RoleController roleController = new RoleController();
            Security.PortalSecurity ps = new Security.PortalSecurity();
            txtGroupName.Text = ps.InputFilter(txtGroupName.Text, Security.PortalSecurity.FilterFlag.NoScripting);
            txtGroupName.Text = ps.InputFilter(txtGroupName.Text, Security.PortalSecurity.FilterFlag.NoMarkup);

            txtDescription.Text = ps.InputFilter(txtDescription.Text, Security.PortalSecurity.FilterFlag.NoScripting);
            txtDescription.Text = ps.InputFilter(txtDescription.Text, Security.PortalSecurity.FilterFlag.NoMarkup);
            if (roleController.GetRoleByName(PortalId, txtGroupName.Text) != null) {
                reqGroupName.Visible = true;
                return;
            }
            List<RoleInfo> modRoles = new List<RoleInfo>();
            foreach (ModulePermissionInfo modulePermissionInfo in ModulePermissionController.GetModulePermissions(ModuleId, TabId)) {
                if (modulePermissionInfo.PermissionKey == "MODGROUP" && modulePermissionInfo.AllowAccess) {
                    modRoles.Add(roleController.GetRole(modulePermissionInfo.RoleID, PortalId));
                }
            }
            RoleInfo roleInfo = new RoleInfo() {
                PortalID = PortalId,
                RoleName = txtGroupName.Text,
                Description = txtDescription.Text,
                SecurityMode = SecurityMode.SocialGroup,
                Status = RoleStatus.Approved,
                IsPublic = rdAccessTypePublic.Checked
            };
            var userRoleStatus = RoleStatus.Pending;
            if (GroupModerationEnabled)
            {
                roleInfo.Status = RoleStatus.Pending;
                userRoleStatus = RoleStatus.Pending;
            } else
            {
                userRoleStatus = RoleStatus.Approved;
            }
            if (ModulePermissionController.HasModulePermission(ModuleId, "MODGROUP")) {
                roleInfo.Status = RoleStatus.Approved;
                userRoleStatus = RoleStatus.Approved;
            }
            roleInfo.RoleGroupID = DefaultRoleGroupId;
            
            roleInfo.RoleID = roleController.AddRole(roleInfo);
            roleInfo = roleController.GetRole(roleInfo.RoleID, PortalId);
            roleInfo.Settings.Add("URL", Globals.NavigateURL(GroupViewTabId, "", new String[] { "groupid=" + roleInfo.RoleID.ToString() }));
            roleInfo.Settings.Add("GroupCreatorName", UserInfo.DisplayName);
            roleInfo.Settings.Add("ReviewMembers", chkMemberApproved.Checked.ToString());
            
            TestableRoleController.Instance.UpdateRoleSettings(roleInfo, true);
            if (inpFile.PostedFile.ContentLength > 0) {
                IFileManager _fileManager = FileManager.Instance;
                IFolderManager _folderManager = FolderManager.Instance;
                var rootFolderPath = PathUtils.Instance.FormatFolderPath(PortalSettings.HomeDirectory);
                
                IFolderInfo groupFolder = _folderManager.GetFolder(PortalSettings.PortalId, "Groups/" + roleInfo.RoleID);
                if (groupFolder == null) {
                    groupFolder = _folderManager.AddFolder(PortalSettings.PortalId, "Groups/" + roleInfo.RoleID);
                }
                if (groupFolder != null) {
                    var fileName = Path.GetFileName(inpFile.PostedFile.FileName);
                    var fileInfo = _fileManager.AddFile(groupFolder, fileName, inpFile.PostedFile.InputStream, true);
                    roleInfo.IconFile = "FileID=" + fileInfo.FileId;
                    roleController.UpdateRole(roleInfo);
                }
            }

            Components.Notifications notifications = new Components.Notifications();
            
           
            roleController.AddUserRole(PortalId, UserId, roleInfo.RoleID, userRoleStatus, true, Null.NullDate, Null.NullDate);
            if (roleInfo.Status == RoleStatus.Pending) {
                //Send notification to Group Moderators to approve/reject group.
                notifications.AddGroupNotification(Constants.GroupPendingNotification, GroupViewTabId, ModuleId, roleInfo, UserInfo, modRoles);
            } else {
                //Send notification to Group Moderators informing of new group.
                notifications.AddGroupNotification(Constants.GroupCreatedNotification, GroupViewTabId, ModuleId, roleInfo, UserInfo, modRoles);

                //Add entry to journal.
                GroupUtilities.CreateJournalEntry(roleInfo, UserInfo);
            }

            Response.Redirect(Globals.NavigateURL(GroupViewTabId, "", new String[] { "groupid=" + roleInfo.RoleID.ToString() }));
        }
    }
}