/*
' Copyright (c) 2011  DotNetNuke Corporation
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;

using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Security;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Modules.Journal.Components;
using DotNetNuke.Security.Roles;

namespace DotNetNuke.Modules.Journal {

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ViewJournal class displays the content
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : JournalModuleBase {

        public int PageSize = 20;
        public bool AllowPhotos = true;
        public bool AllowFiles = true;
        public int MaxMessageLength = 250;
        public bool CanRender = true;
        public bool ShowEditor = true;
        public bool CanComment = true;
        #region Event Handlers

        override protected void OnInit(EventArgs e) {
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
            
            ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Journal/Scripts/journal.js");
            ClientResourceManager.RegisterScript(this.Page, "~/DesktopModules/Journal/Scripts/journalcomments.js");
            ClientResourceManager.RegisterScript(this.Page, "~/Resources/Shared/Scripts/json2.js");

            if (!Request.IsAuthenticated)
            {
                ShowEditor = false;
            } else
            {
                ShowEditor = EditorEnabled;
            }
           
            ctlJournalList.Enabled = true;
            ctlJournalList.ProfileId = -1;
            ctlJournalList.PageSize = 20;
            ctlJournalList.ModuleId = ModuleId;
            

            var moduleController = new ModuleController();
            foreach (var module in moduleController.GetTabModules(TabId).Values) {
                if (module.ModuleDefinition.FriendlyName == "Social Groups") {
                    if (GroupId == -1 && FilterMode == JournalMode.Auto) {
                        ShowEditor = false;
                        ctlJournalList.Enabled = false;
                    }
                    if (GroupId > 0) {
                        RoleController roleController = new RoleController();
                        RoleInfo roleInfo = roleController.GetRole(GroupId, PortalId);
                        if (roleInfo != null) {
                            if (UserInfo.IsInRole(roleInfo.RoleName)) {
                                ShowEditor = true;
                                CanComment = true;
                            } else {
                                ShowEditor = false;
                                CanComment = false;
                            }
                            
                            if (roleInfo.IsPublic == false && ShowEditor == false) {
                                ctlJournalList.Enabled = false;                               
                            }
                            if (roleInfo.IsPublic == true && ShowEditor == false) {
                                ctlJournalList.Enabled = true;
                            }
                            if (roleInfo.IsPublic && ShowEditor) {
                                ctlJournalList.Enabled = true;
                            }
                        } else {
                            ShowEditor = false;
                            ctlJournalList.Enabled = false;
                        }
                    }
                   
                }
            }
            if (!String.IsNullOrEmpty(Request.QueryString["userId"])) {
                ctlJournalList.ProfileId = Convert.ToInt32(Request.QueryString["userId"]);
            } else if (GroupId > 0) {
                ctlJournalList.SocialGroupId = Convert.ToInt32(Request.QueryString["groupId"]);
            }
            
         
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent() {
            this.Load += new System.EventHandler(this.Page_Load);
        }


        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// -----------------------------------------------------------------------------
        private void Page_Load(object sender, System.EventArgs e) {
            try {
               
                if (Settings.ContainsKey(Constants.DefaultPageSize)) {
                    PageSize = Convert.ToInt16(Settings[Constants.DefaultPageSize]);
                }
                if (Settings.ContainsKey(Constants.MaxCharacters)) {
                    MaxMessageLength = Convert.ToInt16(Settings[Constants.MaxCharacters]);
                }
                if (Settings.ContainsKey(Constants.AllowPhotos)) {
                    AllowPhotos = Convert.ToBoolean(Settings[Constants.AllowPhotos]);
                }
                if (Settings.ContainsKey(Constants.AllowFiles)) {
                    AllowFiles = Convert.ToBoolean(Settings[Constants.AllowFiles]);
                }
                var path = Common.Globals.ApplicationPath;
                path = path.EndsWith("/") ? path : path + "/";
                path += "DesktopModules/Journal/";

                litScripts.Text = "var pagesize=" + PageSize.ToString();
                litScripts.Text += ";var profilePage='" + DotNetNuke.Common.Globals.NavigateURL(PortalSettings.UserTabId, string.Empty, new String[] { "userId=xxx" }) + "'";
                litScripts.Text += ";var maxlength=" + MaxMessageLength.ToString();
                litScripts.Text += ";var baseUrl='" + path + "'"; 
                litScripts.Text += ";var resxLike='" + Utilities.GetSharedResource("{resx:like}") + "'";
                litScripts.Text += ";var resxUnLike='" + Utilities.GetSharedResource("{resx:unlike}") + "'";
                if (!String.IsNullOrEmpty(Request.QueryString["userId"])) {
                    litScripts.Text += ";var pid=" + Convert.ToInt32(Request.QueryString["userId"]).ToString();
                    litScripts.Text += ";var gid=-1";
                    ctlJournalList.ProfileId = Convert.ToInt32(Request.QueryString["userId"]);
                    ctlJournalList.PageSize = PageSize;
                } else if (GroupId > 0) {
                    litScripts.Text += ";var pid=-1";
                    litScripts.Text += ";var gid=" + GroupId.ToString();
                    ctlJournalList.SocialGroupId = GroupId;
                    ctlJournalList.PageSize = PageSize;
                } else {
                    litScripts.Text += ";var pid=-1";
                    litScripts.Text += ";var gid=-1";
                }
            } catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

      

    }

}
