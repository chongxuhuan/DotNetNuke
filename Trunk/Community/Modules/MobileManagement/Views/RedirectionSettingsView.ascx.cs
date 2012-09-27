#region Copyright
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// All Rights Reserved
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Modules.MobileManagement.Presenters;
using DotNetNuke.Modules.MobileManagement.Views;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mobile;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Web.Mvp;
using Telerik.Web.UI;
using WebFormsMvp;

using Globals = DotNetNuke.Common.Globals;

namespace DotNetNuke.Modules.MobileManagement
{
    /// <summary>
    /// 
    /// </summary>
    [PresenterBinding(typeof(RedirectionSettingsPresenter))]
    public partial class RedirectionSettingsView : ModuleView, IRedirectionSettingsView
    {
        #region "Properties
        public int RedirectId
        {
            get
            {
                if (Request.QueryString["Id"] != null)
                {
                    return Convert.ToInt32(Request.QueryString["Id"]);
                }
                else
                {
                    return Null.NullInteger;
                }
            }
        }
        public IList<IMatchRule> Capabilities
        {
            get
            {
                if (ViewState["capabilities"] == null)
                {
                    ViewState["capabilities"] = RedirectId > Null.NullInteger ? new RedirectionController().GetRedirectionById(ModuleContext.PortalId, RedirectId).MatchRules : new List<IMatchRule>();
                }
                return (List<IMatchRule>)ViewState["capabilities"];
            }
            set
            {
                ViewState["capabilities"] = value;
            }            
        }

        private string _capability = string.Empty;
        private string _capabilityValue = string.Empty;
        #endregion

        #region "Event Handlers"

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            dgCapabilityList.ItemCommand += OnCapabilitiesItemCommand;
            dgCapabilityList.ItemDataBound += CapabilitiesItemDataBound;
            cboCapabilityValue.ItemsRequested += LoadCapabilityValues;
            cboCapabilityValue.SelectedIndexChanged += SetCapabilityValue;

            lnkSave.Click += lnkSave_OnClick;
            lnkCancel.Click += lnkCancel_OnClick;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                optRedirectType.Items[1].Enabled = optRedirectType.Items[2].Enabled = ClientCapabilityProvider.Instance().SupportsTabletDetection;
                BindSettingControls();
                BindRedirection(RedirectId);
            }
            BindRedirectionCapabilties();
        }

        protected void AddCapability(object sender, EventArgs e)
        {
            if (_capabilityValue.Trim() != string.Empty)
            {
                // Must rebind Capability values due to a telerik in dependant combos, losing initial dependant value on post back.
                BindCapabilityValues(cboCapabilityName.SelectedItem.Text);
                SetCapabilityAndValue();

                var capability = new MatchRule { Capability = _capability, Expression = _capabilityValue };
                var capabilitylist = Capabilities;
                capabilitylist.Add(capability);
                Capabilities = capabilitylist;
                BindRedirectionCapabilties();
                BindCapabilities();
                cboCapabilityName.SelectedIndex = 0;
                BindCapabilityValues(string.Empty);
            }
        }

        protected void OnCapabilitiesItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    var capability = Capabilities.Where(c => c.Capability == e.CommandArgument.ToString()).First();
                        var capabilitylist = Capabilities;
                        capabilitylist.Remove(capability);
                        Capabilities = capabilitylist;                        
                        BindRedirectionCapabilties();
                        BindCapabilities();
                        cboCapabilityName.SelectedIndex = 0;
                        BindCapabilityValues(string.Empty);
					break;
            }
        }

        protected void CapabilitiesItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.EditItem)
            {
                var rule = e.Item.DataItem as IMatchRule;
                e.Item.Attributes.Add("data", rule.Id.ToString());
            }
        }

        protected void LoadCapabilityValues(object source, RadComboBoxItemsRequestedEventArgs e)
        {
            BindCapabilityValues(e.Text);
            SetCapabilityAndValue();
        }

        protected void SetCapabilityValue(object source, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            _capabilityValue = e.Text;
        }

        #endregion

        #region "Private Methods"
        private void BindRedirection(int redirectId)
        {
            // Populating existing redirection settings
            if (redirectId != Null.NullInteger)
            {
                var redirectController = new RedirectionController();
                var redirect = redirectController.GetRedirectionById(ModuleContext.PortalId, redirectId);

                txtRedirectName.Text = redirect.Name;
                chkEnable.Checked = redirect.Enabled;
                chkChildPages.Checked = redirect.IncludeChildTabs;

                if (redirect.SourceTabId != -1)
                {
                    optRedirectSource.SelectedValue = "Tab";
                    cboSourcePage.SelectedIndex = cboSourcePage.Items.IndexOf(cboSourcePage.FindItemByValue(redirect.SourceTabId.ToString()));
                }
                else
                {
                    optRedirectSource.SelectedValue = "Portal";
                }

                optRedirectType.SelectedValue = redirect.Type.ToString();
                optRedirectTarget.SelectedValue = redirect.TargetType.ToString();

                //Other, populate Capabilities
                if (redirect.Type == RedirectionType.Other)
                {
                    BindRedirectionCapabilties();
                }

                switch (redirect.TargetType)
                {
                    case TargetType.Portal:
                        if (cboPortal.Items.Count < 1) optRedirectTarget.SelectedValue = "Tab";
                        else if (cboPortal.FindItemByValue(redirect.TargetValue.ToString()) != null)
                            cboPortal.SelectedValue = redirect.TargetValue.ToString();
                        break;
                    case TargetType.Tab:
                        if (cboTargetPage.FindItemByValue(redirect.TargetValue.ToString()) != null)
                            cboTargetPage.SelectedValue = redirect.TargetValue.ToString();
                        break;
                    case TargetType.Url:
                        txtTargetUrl.Text = redirect.TargetValue.ToString();
                        break;
                }
            }
        }

        private void BindRedirectionCapabilties()
        {
            dgCapabilityList.DataSource = Capabilities;
            dgCapabilityList.DataBind();
            dgCapabilityList.Visible = (Capabilities.Count > 0);
        }

        private void BindSettingControls()
        {
            BindTabs();
            BindPortals();
            BindCapabilities();
        }
        private void BindPortals()
        {
            // Populating Portals dropdown
            var portalController = new PortalController();
			var portals = portalController.GetPortals().Cast<PortalInfo>().Where(p => p.PortalID != ModuleContext.PortalId).ToList();
            if (portals.Count > 0)
            {
                cboPortal.DataSource = portals;
                cboPortal.DataTextField = "PortalName";
                cboPortal.DataValueField = "PortalID";
                cboPortal.DataBind();
            }
            else
            {
                optRedirectTarget.Items[0].Enabled = false;
                if (RedirectId == Null.NullInteger)
                {
                    optRedirectTarget.Items[0].Selected = false;
                    optRedirectTarget.Items[1].Selected = true;
                }
            }

        }
        private void BindTabs()
        {
            var tabs = new TabController().GetTabsByPortal(ModuleContext.PortalId).AsList().Where(IsVisible);
            cboSourcePage.DataTextField = "IndentedTabName";
            cboSourcePage.DataValueField = "TabID";
            cboSourcePage.DataSource = tabs;
            cboSourcePage.DataBind();
            cboTargetPage.DataTextField = "IndentedTabName";
            cboTargetPage.DataValueField = "TabID";
            cboTargetPage.DataSource = tabs;
            cboTargetPage.DataBind();
        }
        private void BindCapabilities()
        {
            //Bind only capabilities that have not yet been added
			var capabilities = new List<string>(ClientCapabilityProvider.Instance().GetAllClientCapabilityValues().Keys.Where(capability => Capabilities.Where( c => c.Capability == capability).Count() < 1));
            capabilities.Insert(0, LocalizeString("selectCapabilityName"));
            cboCapabilityName.DataSource = capabilities;
            cboCapabilityName.DataBind();
        }
        private void BindCapabilityValues(string capability)
        {
            if (capability != string.Empty)
            {
                var capabillityValues = new List<string>(ClientCapabilityProvider.Instance().GetAllClientCapabilityValues().Where(c => c.Key == capability).First().Value);
                capabillityValues.Sort();
                cboCapabilityValue.DataSource = capabillityValues;
            }
            else
            {
                cboCapabilityValue.DataSource = string.Empty;
                cboCapabilityValue.Text = string.Empty;
            }
            cboCapabilityValue.DataBind();                 
        }
        private void SetCapabilityAndValue()
        {
            if (_capability == string.Empty) _capability = cboCapabilityName.SelectedValue;
            if (_capabilityValue == string.Empty) _capabilityValue = cboCapabilityValue.SelectedValue;
        }
        private void SaveRedirection()
        {
            IRedirection redirection = new Redirection();
            var redirectionController = new RedirectionController();

            if (RedirectId > Null.NullInteger)
            {
                redirection = redirectionController.GetRedirectionById(ModuleContext.PortalId, RedirectId);
            }

            redirection.Name = txtRedirectName.Text;
            redirection.Enabled = chkEnable.Checked;
            redirection.PortalId = ModuleContext.PortalId;
            if (optRedirectSource.SelectedValue == "Tab")
            {
                redirection.SourceTabId = int.Parse(cboSourcePage.SelectedValue);
                redirection.IncludeChildTabs = chkChildPages.Checked;
            }
            else
            {
                redirection.SourceTabId = -1;
                redirection.IncludeChildTabs = false;
            }

            redirection.Type = (RedirectionType)Enum.Parse(typeof(RedirectionType), optRedirectType.SelectedValue);
            //Other, save new capabilities
            if (redirection.Type == RedirectionType.Other)
            {
                if (RedirectId > Null.NullInteger)
                {
                    // Delete capabilities that no longer exist in the grid
                    foreach (var rule in redirection.MatchRules.Where(rule => Capabilities.Where(c => c.Id == rule.Id).Count() < 1))
                    {
                        redirectionController.DeleteRule(ModuleContext.PortalId, redirection.Id, rule.Id);
                    }
                }

                // A new capabilities
                foreach (var capability in Capabilities.Where(capability => capability.Id == -1))
                {
                    redirection.MatchRules.Add(capability);
                }

                redirection.MatchRules = Capabilities;
            }
            else if (RedirectId > Null.NullInteger && redirection.MatchRules.Count > 0)
            {
                foreach(var rule in redirection.MatchRules)
                {
                    redirectionController.DeleteRule(ModuleContext.PortalId, redirection.Id, rule.Id);
                }
            }

            redirection.TargetType = (TargetType)Enum.Parse(typeof(TargetType), optRedirectTarget.SelectedValue);
            switch (redirection.TargetType)
            {
                case TargetType.Portal:
                    redirection.TargetValue = cboPortal.SelectedItem.Value;
                    break;
                case TargetType.Tab:
                    redirection.TargetValue = int.Parse(cboTargetPage.SelectedValue);
                    break;
                case TargetType.Url:
                    redirection.TargetValue = txtTargetUrl.Text;
                    break;
            }

            // Save the redirect
            redirectionController.Save(redirection);      
        }

		private bool IsVisible(TabInfo tab)
		{
            if (tab == null || tab.TabID == PortalSettings.Current.AdminTabId || tab.TabID == PortalSettings.Current.UserTabId)
            {
                return false;
            }
            
            if(tab.ParentId != Null.NullInteger)
            {
                var tabController = new TabController();
                do
                {
                    if (tab.ParentId == PortalSettings.Current.AdminTabId || tab.ParentId == PortalSettings.Current.UserTabId)
                    {
                        return false;
                    }

                    tab = tabController.GetTab(tab.ParentId, tab.PortalID, false);
                } while (tab != null && tab.ParentId != Null.NullInteger);
            }

			return true;
		}

        #endregion

        #region "Event Handlers"
        private void lnkSave_OnClick(object sender, EventArgs e)
        {
                     
            var redirectionController = new RedirectionController();
            var name = txtRedirectName.Text;
            int nameCount;
            // Checks for duplicate names   
            if (RedirectId > Null.NullInteger)
            {
                nameCount = redirectionController.GetRedirectionsByPortal(ModuleContext.PortalId).Where(r =>( r.Id != RedirectId && r.Name.ToLower() == name.ToLower())).Count();
            }
            else
            {
                nameCount = redirectionController.GetRedirectionsByPortal(ModuleContext.PortalId).Where(r => r.Name.ToLower() == name.ToLower()).Count();
            }

            if (nameCount < 1)
            {
                SaveRedirection();
                Response.Redirect(Globals.NavigateURL("", "type=RedirectionSettings"), true);
            }
            else
            {
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("DuplicateNameError.Text", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);                
            }            
        }

        private void lnkCancel_OnClick(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(ModuleContext.NavigateUrl(ModuleContext.TabId, "", true), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        #endregion
    }
}