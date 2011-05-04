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
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Modules.Dashboard.Components;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Installer;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Dashboard
{
    public partial class DashboardControls : PortalModuleBase
    {
        private const int COLUMN_ENABLED = 5;
        private const int COLUMN_MOVE_DOWN = 1;
        private const int COLUMN_MOVE_UP = 2;
        private List<DashboardControl> _DashboardControls;

        protected List<DashboardControl> DashboardControlList
        {
            get
            {
                if (_DashboardControls == null)
                {
                    _DashboardControls = DashboardController.GetDashboardControls(false);
                }
                return _DashboardControls;
            }
        }

        private bool SupportsRichClient()
        {
            return ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML);
        }

        private void DeleteControl(int index)
        {
            DashboardControl dashboardControl = DashboardControlList[index];
            Response.Redirect(Util.UnInstallURL(TabId, dashboardControl.PackageID, Server.UrlEncode(Globals.NavigateURL(TabId, "DashboardControls", "mid=" + ModuleId))), true);
        }

        private void MoveControl(int index, int destIndex)
        {
            DashboardControl dashboardControl = DashboardControlList[index];
            DashboardControl nextControl = DashboardControlList[destIndex];
            int currentOrder = dashboardControl.ViewOrder;
            int nextOrder = nextControl.ViewOrder;
            dashboardControl.ViewOrder = nextOrder;
            nextControl.ViewOrder = currentOrder;
            DashboardControlList.Sort();
            BindGrid();
        }

        private void MoveControlDown(int index)
        {
            MoveControl(index, index + 1);
        }

        private void MoveControlUp(int index)
        {
            MoveControl(index, index - 1);
        }

        private void BindGrid()
        {
            bool allEnabled = true;
            foreach (DashboardControl dashboardControl in DashboardControlList)
            {
                if (dashboardControl.IsEnabled == false)
                {
                    allEnabled = false;
                }
                if (!allEnabled)
                {
                    break;
                }
            }
            foreach (DataGridColumn column in grdDashboardControls.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (CheckBoxColumn)))
                {
                    var cbColumn = (CheckBoxColumn) column;
                    if (cbColumn.DataField == "IsEnabled")
                    {
                        cbColumn.Checked = allEnabled;
                    }
                }
            }
            grdDashboardControls.DataSource = DashboardControlList;
            grdDashboardControls.DataBind();
        }

        private void RefreshGrid()
        {
            _DashboardControls = null;
            BindGrid();
        }

        private void UpdateControls()
        {
            foreach (DashboardControl dashboardControl in DashboardControlList)
            {
                if (dashboardControl.IsDirty)
                {
                    DashboardController.UpdateDashboardControl(dashboardControl);
                }
            }
        }

        private void ProcessPostBack()
        {
            try
            {
                string[] aryNewOrder = ClientAPI.GetClientSideReorder(grdDashboardControls.ClientID, Page);
                DashboardControl dashboardControl;
                DataGridItem objItem;
                CheckBox chk;
                for (int i = 0; i <= grdDashboardControls.Items.Count - 1; i++)
                {
                    objItem = grdDashboardControls.Items[i];
                    dashboardControl = DashboardControlList[i];
                    chk = (CheckBox) objItem.Cells[COLUMN_ENABLED].Controls[0];
                    dashboardControl.IsEnabled = chk.Checked;
                }
                for (int i = 0; i <= aryNewOrder.Length - 1; i++)
                {
                    DashboardControlList[Convert.ToInt32(aryNewOrder[i])].ViewOrder = i;
                }
                DashboardControlList.Sort();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void LoadViewState(object savedState)
        {
            if ((savedState != null))
            {
                var myState = (object[]) savedState;
                if ((myState[0] != null))
                {
                    base.LoadViewState(myState[0]);
                }
                if ((myState[1] != null))
                {
                    _DashboardControls = (List<DashboardControl>) myState[1];
                }
            }
        }

        protected override object SaveViewState()
        {
            var allStates = new object[2];
            allStates[0] = base.SaveViewState();
            allStates[1] = DashboardControlList;
            return allStates;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (DataGridColumn column in grdDashboardControls.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (CheckBoxColumn)))
                {
                    if (SupportsRichClient() == false)
                    {
                        var cbColumn = (CheckBoxColumn) column;
                        cbColumn.CheckedChanged += grdDashboardControls_ItemCheckedChanged;
                    }
                }
                else if (ReferenceEquals(column.GetType(), typeof (ImageCommandColumn)))
                {
                    var imageColumn = (ImageCommandColumn) column;
                    switch (imageColumn.CommandName)
                    {
                        case "Delete":
                            imageColumn.OnClickJS = Localization.GetString("DeleteItem");
                            imageColumn.Text = Localization.GetString("Delete", LocalResourceFile);
                            break;
                        case "MoveUp":
                            imageColumn.Text = Localization.GetString("MoveUp", LocalResourceFile);
                            break;
                        case "MoveDown":
                            imageColumn.Text = Localization.GetString("MoveDown", LocalResourceFile);
                            break;
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdInstall.Click += cmdInstall_Click;
            cmdRefresh.Click += cmdRefresh_Click;
            cmdUpdate.Click += cmdUpdate_Click;
            grdDashboardControls.ItemCommand += grdDashboardControls_ItemCommand;
            grdDashboardControls.ItemCreated += grdDashboardControls_ItemCreated;
            grdDashboardControls.ItemDataBound += grdDashboardControls_ItemDataBound;

            try
            {
                if (!Page.IsPostBack)
                {
                    Localization.LocalizeDataGrid(ref grdDashboardControls, LocalResourceFile);
                    BindGrid();
                }
                else
                {
                    ProcessPostBack();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(), true);
        }

        protected void cmdInstall_Click(object sender, EventArgs e)
        {
            Response.Redirect(Util.InstallURL(TabId, Server.UrlEncode(Globals.NavigateURL(TabId, "DashboardControls", "mid=" + ModuleId)), "DashboardControl"), true);
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateControls();
                RefreshGrid();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void grdDashboardControls_ItemCheckedChanged(object sender, DNNDataGridCheckChangedEventArgs e)
        {
            string propertyName = e.Field;
            bool propertyValue = e.Checked;
            bool isAll = e.IsAll;
            int index = e.Item.ItemIndex;
            DashboardControl dashboardControl;
            if (isAll)
            {
                foreach (DashboardControl dashboard in DashboardControlList)
                {
                    switch (propertyName)
                    {
                        case "IsEnabled":
                            dashboard.IsEnabled = propertyValue;
                            break;
                    }
                }
            }
            else
            {
                dashboardControl = DashboardControlList[index];
                switch (propertyName)
                {
                    case "IsEnabled":
                        dashboardControl.IsEnabled = propertyValue;
                        break;
                }
            }
            BindGrid();
        }

        private void grdDashboardControls_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;
            int commandArgument = Convert.ToInt32(e.CommandArgument);
            int index = e.Item.ItemIndex;
            switch (commandName)
            {
                case "Delete":
                    DeleteControl(index);
                    break;
                case "MoveUp":
                    MoveControlUp(index);
                    break;
                case "MoveDown":
                    MoveControlDown(index);
                    break;
            }
        }

        private void grdDashboardControls_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (SupportsRichClient())
            {
                switch (e.Item.ItemType)
                {
                    case ListItemType.Header:
                        ((WebControl) e.Item.Cells[COLUMN_ENABLED].Controls[1]).Attributes.Add("onclick", "dnn.util.checkallChecked(this," + COLUMN_ENABLED + ");");
                        ((CheckBox) e.Item.Cells[COLUMN_ENABLED].Controls[1]).AutoPostBack = false;
                        break;
                    case ListItemType.AlternatingItem:
                    case ListItemType.Item:
                        ((CheckBox) e.Item.Cells[COLUMN_ENABLED].Controls[0]).AutoPostBack = false;
                        ClientAPI.EnableClientSideReorder(e.Item.Cells[COLUMN_MOVE_DOWN].Controls[0], Page, false, grdDashboardControls.ClientID);
                        ClientAPI.EnableClientSideReorder(e.Item.Cells[COLUMN_MOVE_UP].Controls[0], Page, true, grdDashboardControls.ClientID);
                        break;
                }
            }
        }

        protected void grdDashboardControls_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            DataGridItem item = e.Item;
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem)
            {
                Control imgColumnControl = item.Controls[0].Controls[0];
                if (imgColumnControl is ImageButton)
                {
                    var delImage = (ImageButton) imgColumnControl;
                    var dashboardControl = (DashboardControl) item.DataItem;
                    switch (dashboardControl.DashboardControlKey)
                    {
                        case "Server":
                        case "Database":
                        case "Host":
                        case "Portals":
                        case "Modules":
                        case "Skins":
                            delImage.Visible = false;
                            break;
                        default:
                            delImage.Visible = true;
                            break;
                    }
                }
            }
        }
    }
}