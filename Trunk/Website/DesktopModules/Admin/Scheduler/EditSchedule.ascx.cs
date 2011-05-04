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
using System.Collections.Generic;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Scheduler
{
    public partial class EditSchedule : PortalModuleBase, IActionable
    {
        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                Actions.Add(GetNextActionID(),
                            Localization.GetString(ModuleActionType.ContentOptions, LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "icon_scheduler_16px.gif",
                            EditUrl("", "", "Status"),
                            false,
                            SecurityAccessLevel.Host,
                            true,
                            false);
                if (Request.QueryString["ScheduleID"] != null)
                {
                    int ScheduleID = Convert.ToInt32(Request.QueryString["ScheduleID"]);
                    Actions.Add(GetNextActionID(),
                                Localization.GetString("ScheduleHistory.Action", LocalResourceFile),
                                ModuleActionType.AddContent,
                                "",
                                "icon_profile_16px.gif",
                                EditUrl("ScheduleID", ScheduleID.ToString(), "History"),
                                false,
                                SecurityAccessLevel.Host,
                                true,
                                false);
                }
                return Actions;
            }
        }

        #endregion

        private void BindData()
        {
            ScheduleItem objScheduleItem;
            if (Request.QueryString["ScheduleID"] != null)
            {
                ViewState["ScheduleID"] = Request.QueryString["ScheduleID"];
                objScheduleItem = SchedulingProvider.Instance().GetSchedule(Convert.ToInt32(Request.QueryString["ScheduleID"]));
                txtFriendlyName.Text = objScheduleItem.FriendlyName;
                txtType.Enabled = false;
                txtType.Text = objScheduleItem.TypeFullName;
                chkEnabled.Checked = objScheduleItem.Enabled;
                if (objScheduleItem.TimeLapse == Null.NullInteger)
                {
                    txtTimeLapse.Text = "";
                }
                else
                {
                    txtTimeLapse.Text = Convert.ToString(objScheduleItem.TimeLapse);
                }
                if (ddlTimeLapseMeasurement.Items.FindByValue(objScheduleItem.TimeLapseMeasurement) != null)
                {
                    ddlTimeLapseMeasurement.Items.FindByValue(objScheduleItem.TimeLapseMeasurement).Selected = true;
                }
                if (objScheduleItem.RetryTimeLapse == Null.NullInteger)
                {
                    txtRetryTimeLapse.Text = "";
                }
                else
                {
                    txtRetryTimeLapse.Text = Convert.ToString(objScheduleItem.RetryTimeLapse);
                }
                if (ddlRetryTimeLapseMeasurement.Items.FindByValue(objScheduleItem.RetryTimeLapseMeasurement) != null)
                {
                    ddlRetryTimeLapseMeasurement.Items.FindByValue(objScheduleItem.RetryTimeLapseMeasurement).Selected = true;
                }
                if (ddlRetainHistoryNum.Items.FindByValue(Convert.ToString(objScheduleItem.RetainHistoryNum)) != null)
                {
                    ddlRetainHistoryNum.Items.FindByValue(Convert.ToString(objScheduleItem.RetainHistoryNum)).Selected = true;
                }
                else
                {
                    ddlRetainHistoryNum.Items.Add(objScheduleItem.RetainHistoryNum.ToString());
                    ddlRetainHistoryNum.Items.FindByValue(Convert.ToString(objScheduleItem.RetainHistoryNum)).Selected = true;
                }
                if (ddlAttachToEvent.Items.FindByValue(objScheduleItem.AttachToEvent) != null)
                {
                    ddlAttachToEvent.Items.FindByValue(objScheduleItem.AttachToEvent).Selected = true;
                }
                chkCatchUpEnabled.Checked = objScheduleItem.CatchUpEnabled;
                txtObjectDependencies.Text = objScheduleItem.ObjectDependencies;
                BindServers(objScheduleItem.Servers);
            }
            else
            {
                cmdDelete.Visible = false;
                cmdRun.Visible = false;
                txtType.Enabled = true;
            }
        }

        private void BindServers(string selectedServers)
        {
            List<ServerInfo> servers = ServerController.GetServers();
            foreach (ServerInfo webServer in servers)
            {
                if (webServer.Enabled)
                {
                    string serverName = ServerController.GetServerName(webServer);
                    var serverItem = new ListItem(serverName, serverName);
                    if (string.IsNullOrEmpty(selectedServers))
                    {
                        serverItem.Selected = true;
                    }
                    else
                    {
                        serverItem.Selected = Null.NullBoolean;
                        foreach (string selectedServer in selectedServers.Split(','))
                        {
                            if (selectedServer == serverName)
                            {
                                serverItem.Selected = true;
                                break;
                            }
                        }
                    }
                    lstServers.Items.Add(serverItem);
                }
            }
        }

        private ScheduleItem CreateScheduleItem()
        {
            var objScheduleItem = new ScheduleItem();
            objScheduleItem.TypeFullName = txtType.Text;
            objScheduleItem.FriendlyName = txtFriendlyName.Text;
            if (String.IsNullOrEmpty(txtTimeLapse.Text) || txtTimeLapse.Text == "0" || txtTimeLapse.Text == "-1")
            {
                objScheduleItem.TimeLapse = Null.NullInteger;
            }
            else
            {
                objScheduleItem.TimeLapse = Convert.ToInt32(txtTimeLapse.Text);
            }
            objScheduleItem.TimeLapseMeasurement = ddlTimeLapseMeasurement.SelectedItem.Value;
            if (String.IsNullOrEmpty(txtRetryTimeLapse.Text) || txtRetryTimeLapse.Text == "0" || txtRetryTimeLapse.Text == "-1")
            {
                objScheduleItem.RetryTimeLapse = Null.NullInteger;
            }
            else
            {
                objScheduleItem.RetryTimeLapse = Convert.ToInt32(txtRetryTimeLapse.Text);
            }
            objScheduleItem.RetryTimeLapseMeasurement = ddlRetryTimeLapseMeasurement.SelectedItem.Value;
            objScheduleItem.RetainHistoryNum = Convert.ToInt32(ddlRetainHistoryNum.SelectedItem.Value);
            objScheduleItem.AttachToEvent = ddlAttachToEvent.SelectedItem.Value;
            objScheduleItem.CatchUpEnabled = chkCatchUpEnabled.Checked;
            objScheduleItem.Enabled = chkEnabled.Checked;
            objScheduleItem.ObjectDependencies = txtObjectDependencies.Text;
            string servers = Null.NullString;
            bool bAllSelected = true;
            foreach (ListItem item in lstServers.Items)
            {
                if (item.Selected)
                {
                    servers += "," + item.Value;
                }
                else
                {
                    bAllSelected = Null.NullBoolean;
                }
            }
            if (bAllSelected)
            {
                servers = Null.NullString;
            }
            if (!string.IsNullOrEmpty(servers))
            {
                objScheduleItem.Servers = servers + ",";
            }
            return objScheduleItem;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdDelete.Click += cmdDelete_Click;
            cmdRun.Click += cmdRun_Click;
            cmdUpdate.Click += cmdUpdate_Click;

            try
            {
                if (!Page.IsPostBack)
                {
                    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"));
                    BindData();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdCancel_Click(Object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }

        private void cmdDelete_Click(Object sender, EventArgs e)
        {
            var objScheduleItem = new ScheduleItem();
            objScheduleItem.ScheduleID = Convert.ToInt32(ViewState["ScheduleID"]);
            SchedulingProvider.Instance().DeleteSchedule(objScheduleItem);
            string strMessage;
            strMessage = Localization.GetString("DeleteSuccess", LocalResourceFile);
            UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
        }

        protected void cmdRun_Click(object sender, EventArgs e)
        {
            string strMessage;
            ScheduleItem objScheduleItem = CreateScheduleItem();
            if (ViewState["ScheduleID"] != null)
            {
                objScheduleItem.ScheduleID = Convert.ToInt32(ViewState["ScheduleID"]);
                SchedulingProvider.Instance().RunScheduleItemNow(objScheduleItem);
            }
            SchedulingProvider.Instance().RunScheduleItemNow(objScheduleItem);
            strMessage = Localization.GetString("RunNow", LocalResourceFile);
            UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
            if (SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD)
            {
                SchedulingProvider.Instance().ReStart("Change made to schedule.");
            }
        }

        private void cmdUpdate_Click(Object sender, EventArgs e)
        {
            string strMessage;
            ScheduleItem objScheduleItem = CreateScheduleItem();
            if (ViewState["ScheduleID"] != null)
            {
                objScheduleItem.ScheduleID = Convert.ToInt32(ViewState["ScheduleID"]);
                SchedulingProvider.Instance().UpdateSchedule(objScheduleItem);
            }
            else
            {
                SchedulingProvider.Instance().AddSchedule(objScheduleItem);
            }
            strMessage = Localization.GetString("UpdateSuccess", LocalResourceFile);
            UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.GreenSuccess);
            if (SchedulingProvider.SchedulerMode == SchedulerMode.TIMER_METHOD)
            {
                SchedulingProvider.Instance().ReStart("Change made to schedule.");
            }
            Response.Redirect(Globals.NavigateURL(), true);
        }
    }
}