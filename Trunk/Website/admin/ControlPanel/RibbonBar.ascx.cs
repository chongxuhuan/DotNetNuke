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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Application;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Upgrade;
using DotNetNuke.UI.Utilities;
using DotNetNuke.Web.UI.WebControls;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.UI.ControlPanels
{
    public partial class RibbonBar : ControlPanelBase
    {
      private bool _IsDockable;
      public override bool IsDockable
      {
        get
        {
          return _IsDockable;  //_IsDockable; 
        }
        set
        {
          _IsDockable = value;
        }
      }
      
        #region "Private Methods"

        private void Localize()
        {
            CommonTasksTabLiteral.Text = Localization.GetString("Tab_CommonTasks", LocalResourceFile);
            CurrentPageTabLiteral.Text = Localization.GetString("Tab_CurrentPage", LocalResourceFile);
            AdminTabLiteral.Text = Localization.GetString("Tab_Site", LocalResourceFile);

            Control ctrl = AdminPanel.FindControl("SiteNewPage");
            if (((ctrl != null) && ctrl is DnnRibbonBarTool))
            {
                var toolCtrl = (DnnRibbonBarTool) ctrl;
                toolCtrl.Text = Localization.GetString("SiteNewPage", LocalResourceFile);
                toolCtrl.ToolTip = Localization.GetString("SiteNewPage.ToolTip", LocalResourceFile);
            }

            ListItem lstItem = optMode.Items.FindByValue("VIEW");
            if (((lstItem != null)))
            {
                lstItem.Text = Localization.GetString("ModeView", LocalResourceFile);
            }
            lstItem = optMode.Items.FindByValue("EDIT");
            if (((lstItem != null)))
            {
                lstItem.Text = Localization.GetString("ModeEdit", LocalResourceFile);
            }
            lstItem = optMode.Items.FindByValue("LAYOUT");
            if (((lstItem != null)))
            {
                lstItem.Text = Localization.GetString("ModeLayout", LocalResourceFile);
            }
        }

        private void SetMode(bool update)
        {
            if (update)
            {
                SetUserMode(optMode.SelectedValue);
            }

            if (!TabPermissionController.CanAddContentToPage())
            {
                optMode.Items.Remove(optMode.Items.FindByValue("LAYOUT"));
            }

            switch (UserMode)
            {
                case PortalSettings.Mode.View:
                    optMode.Items.FindByValue("VIEW").Selected = true;
                    break;
                case PortalSettings.Mode.Edit:
                    optMode.Items.FindByValue("EDIT").Selected = true;
                    break;
                case PortalSettings.Mode.Layout:
                    optMode.Items.FindByValue("LAYOUT").Selected = true;
                    break;
            }
        }

        private void SetVisibility(bool toggle)
        {
            if (toggle)
            {
                SetVisibleMode(!IsVisible);
            }
        }

        protected string GetButtonConfirmMessage(string toolName)
        {
            if (toolName == "DeletePage")
            {
                return Localization.GetString("Tool.DeletePage.Confirm", this.LocalResourceFile);
            }

            if (toolName == "CopyPermissionsToChildren")
            {
                if (PortalSecurity.IsInRole("Administrators"))
                {
                    return Localization.GetString("Tool.CopyPermissionsToChildren.Confirm", this.LocalResourceFile);
                }

                return Localization.GetString("Tool.CopyPermissionsToChildrenPageEditor.Confirm", this.LocalResourceFile);
            }

            if (toolName == "CopyDesignToChildren")
            {
                if (PortalSecurity.IsInRole("Administrators"))
                {
                    return Localization.GetString("Tool.CopyDesignToChildren.Confirm", this.LocalResourceFile);
                }

                return Localization.GetString("Tool.CopyDesignToChildrenPageEditor.Confirm", this.LocalResourceFile);
            }

            return string.Empty;
        }

        #endregion

        #region "Event Handlers"

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ID = "RibbonBar";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdVisibility.Click += CmdVisibilityClick;
            optMode.SelectedIndexChanged += OptModeSelectedIndexChanged;

            try
            {
                AdminPanel.Visible = false;
                AdminTabListItem.Visible = false;
                AdvancedToolsPanel.Visible = false;

                jQuery.RequestDnnPluginsRegistration();

                Control copyPageButton = CurrentPagePanel.FindControl("CopyPage");
                if ((copyPageButton != null))
                {
                    copyPageButton.Visible = LocaleController.Instance.IsDefaultLanguage(LocaleController.Instance.GetCurrentLocale(PortalSettings.PortalId).Code);
                }


                if ((Request.IsAuthenticated))
                {
                    UserInfo user = UserController.GetCurrentUserInfo();
                    if (((user != null)))
                    {
                        bool isAdmin = user.IsInRole(PortalSettings.Current.AdministratorRoleName);
                        AdminPanel.Visible = isAdmin;
                        AdminTabListItem.Visible = isAdmin;
                    }
                }

                if (IsPageAdmin())
                {
                    ControlPanel.Visible = true;
                    cmdDock.Visible = true;
                    cmdVisibility.Visible = true;
                    BodyPanel.Visible = true;

                    if ((DotNetNukeContext.Current.Application.Name == "DNNCORP.CE"))
                    {
                        //Hide Support icon in CE
                        AdminPanel.FindControl("SupportTickets").Visible = false;
                    }
                    else
                    {
                        //Show PE/XE tools
                        AdvancedToolsPanel.Visible = true;
                    }

                    Localize();

                    if (!Page.IsPostBack)
                    {
                        UserInfo objUser = UserController.GetCurrentUserInfo();
                        if ((objUser != null))
                        {
                            if (objUser.IsSuperUser)
                            {
                                hypMessage.ImageUrl = Upgrade.UpgradeIndicator(DotNetNukeContext.Current.Application.Version, Request.IsLocal, Request.IsSecureConnection);
                                if (!string.IsNullOrEmpty(hypMessage.ImageUrl))
                                {
                                    hypMessage.ToolTip = Localization.GetString("hypUpgrade.Text", LocalResourceFile);
                                    hypMessage.NavigateUrl = Upgrade.UpgradeRedirect();
                                }
                            }
                            else
                            {
                                if (PortalSecurity.IsInRole(PortalSettings.AdministratorRoleName) && Host.DisplayCopyright)
                                {
                                    hypMessage.ImageUrl = "~/images/branding/iconbar_logo.png";
                                    hypMessage.ToolTip = DotNetNukeContext.Current.Application.Description;
                                    hypMessage.NavigateUrl = Localization.GetString("hypMessageUrl.Text", LocalResourceFile);
                                }
                                else
                                {
                                    hypMessage.Visible = false;
                                }
                            }
                        }
                        SetMode(false);
                        SetVisibility(false);
                    }
                }
                else if (IsModuleAdmin())
                {
                    ControlPanel.Visible = true;
                    cmdDock.Visible = false;
                    cmdVisibility.Visible = false;
                    BodyPanel.Visible = false;
                    if (!Page.IsPostBack)
                    {
                        SetMode(false);
                        SetVisibility(false);
                    }
                }
                else
                {
                    ControlPanel.Visible = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

          PageBase.RegisterStyleSheet(Page, "~/admin/ControlPanel/module.css");

          if (IsDockable)
          {
            ClientScriptManager cs = Page.ClientScript;
            if (!cs.IsClientScriptIncludeRegistered("hoverintent"))
            {
              cs.RegisterClientScriptInclude("hoverintent", Globals.ResolveUrl("~/Resources/Shared/Scripts/jquery/jquery.hoverIntent.min.js"));
            };

            if (!cs.IsClientScriptIncludeRegistered("controlpanel"))
            {
              cs.RegisterClientScriptInclude("controlpanel", Globals.ResolveUrl("~/Resources/ControlPanel/ControlPanel.js"));
            };
            cmdVisibility.Visible = false;
          }
          else
          {
            cmdDock.Visible = false;
          }
          

          //Set initial value
          DNNClientAPI.EnableMinMax(imgVisibility,
                                    BodyPanel,
                                    PortalSettings.DefaultControlPanelVisibility,
                                    Globals.ApplicationPath + "/images/collapse.gif",
                                    Globals.ApplicationPath + "/images/expand.gif",
                                    DNNClientAPI.MinMaxPersistanceType.Personalization,
                                    "Usability",
                                    "ControlPanelVisible" + PortalSettings.PortalId);
        }

        protected void CmdVisibilityClick(object sender, EventArgs e)
        {
          SetVisibility(true);
          Response.Redirect(Request.RawUrl, true);
        }

        protected void OptModeSelectedIndexChanged(object sender, EventArgs e)
        {
            if (Page.IsCallback)
            {
                return;
            }
            SetMode(true);
            Response.Redirect(Request.RawUrl, true);
        }

        #endregion
    }
}
