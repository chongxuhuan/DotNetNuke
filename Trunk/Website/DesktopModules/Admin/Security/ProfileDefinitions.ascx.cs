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
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Lists;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Users
{
    public partial class ProfileDefinitions : PortalModuleBase, IActionable
    {
        private const int COLUMN_REQUIRED = 11;
        private const int COLUMN_VISIBLE = 12;
        private const int COLUMN_MOVE_DOWN = 2;
        private const int COLUMN_MOVE_UP = 3;
        private ProfilePropertyDefinitionCollection _ProfileProperties;

        protected bool IsSuperUser
        {
            get
            {
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected ProfilePropertyDefinitionCollection ProfileProperties
        {
            get
            {
                if (_ProfileProperties == null)
                {
                    _ProfileProperties = ProfileController.GetPropertyDefinitionsByPortal(UsersPortalId, false);
                }
                return _ProfileProperties;
            }
        }

        public string ReturnUrl
        {
            get
            {
                string _ReturnURL;
                var FilterParams = new string[String.IsNullOrEmpty(Request.QueryString["filterproperty"]) ? 1 : 2];
                if (String.IsNullOrEmpty(Request.QueryString["filterProperty"]))
                {
                    FilterParams.SetValue("filter=" + Request.QueryString["filter"], 0);
                }
                else
                {
                    FilterParams.SetValue("filter=" + Request.QueryString["filter"], 0);
                    FilterParams.SetValue("filterProperty=" + Request.QueryString["filterProperty"], 1);
                }
                if (string.IsNullOrEmpty(Request.QueryString["filter"]))
                {
                    _ReturnURL = Globals.NavigateURL(TabId);
                }
                else
                {
                    _ReturnURL = Globals.NavigateURL(TabId, "", FilterParams);
                }
                return _ReturnURL;
            }
        }

        protected int UsersPortalId
        {
            get
            {
                int intPortalId = PortalId;
                if (IsSuperUser)
                {
                    intPortalId = Null.NullInteger;
                }
                return intPortalId;
            }
        }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                Actions.Add(GetNextActionID(),
                            Localization.GetString(ModuleActionType.AddContent, LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "add.gif",
                            EditUrl("EditProfileProperty"),
                            false,
                            SecurityAccessLevel.Admin,
                            true,
                            false);
                Actions.Add(GetNextActionID(),
                            Localization.GetString("Cancel.Action", LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "lt.gif",
                            ReturnUrl,
                            false,
                            SecurityAccessLevel.Admin,
                            true,
                            false);
                return Actions;
            }
        }

        #endregion

        private bool SupportsRichClient()
        {
            return ClientAPI.BrowserSupportsFunctionality(ClientAPI.ClientFunctionality.DHTML);
        }

        private void DeleteProperty(int index)
        {
            ProfilePropertyDefinition objProperty = ProfileProperties[index];
            ProfileController.DeletePropertyDefinition(objProperty);
            RefreshGrid();
        }

        private void MoveProperty(int index, int destIndex)
        {
            ProfilePropertyDefinition objProperty = ProfileProperties[index];
            ProfilePropertyDefinition objNext = ProfileProperties[destIndex];
            int currentOrder = objProperty.ViewOrder;
            int nextOrder = objNext.ViewOrder;
            objProperty.ViewOrder = nextOrder;
            objNext.ViewOrder = currentOrder;
            ProfileProperties.Sort();
            BindGrid();
        }

        private void MovePropertyDown(int index)
        {
            MoveProperty(index, index + 1);
        }

        private void MovePropertyUp(int index)
        {
            MoveProperty(index, index - 1);
        }

        private void BindGrid()
        {
            bool allRequired = true;
            bool allVisible = true;
            foreach (ProfilePropertyDefinition profProperty in ProfileProperties)
            {
                if (profProperty.Required == false)
                {
                    allRequired = false;
                }
                if (profProperty.Visible == false)
                {
                    allVisible = false;
                }
                if (!allRequired && !allVisible)
                {
                    break;
                }
            }
            foreach (DataGridColumn column in grdProfileProperties.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (CheckBoxColumn)))
                {
                    var cbColumn = (CheckBoxColumn) column;
                    if (cbColumn.DataField == "Required")
                    {
                        cbColumn.Checked = allRequired;
                    }
                    if (cbColumn.DataField == "Visible")
                    {
                        cbColumn.Checked = allVisible;
                    }
                }
            }
            grdProfileProperties.DataSource = ProfileProperties;
            grdProfileProperties.DataBind();
        }

        private void RefreshGrid()
        {
            _ProfileProperties = null;
            BindGrid();
        }

        private void UpdateProperties()
        {
            ProcessPostBack();
            foreach (ProfilePropertyDefinition objProperty in ProfileProperties)
            {
                if (objProperty.IsDirty)
                {
                    if (UsersPortalId == Null.NullInteger)
                    {
                        objProperty.Required = false;
                    }
                    ProfileController.UpdatePropertyDefinition(objProperty);
                }
            }
        }

        private void ProcessPostBack()
        {
            try
            {
                string[] aryNewOrder = ClientAPI.GetClientSideReorder(grdProfileProperties.ClientID, Page);
                ProfilePropertyDefinition objProperty;
                DataGridItem objItem;
                CheckBox chk;
                for (int i = 0; i <= grdProfileProperties.Items.Count - 1; i++)
                {
                    objItem = grdProfileProperties.Items[i];
                    objProperty = ProfileProperties[i];
                    chk = (CheckBox) objItem.Cells[COLUMN_REQUIRED].Controls[0];
                    objProperty.Required = chk.Checked;
                    chk = (CheckBox) objItem.Cells[COLUMN_VISIBLE].Controls[0];
                    objProperty.Visible = chk.Checked;
                }
                for (int i = 0; i <= aryNewOrder.Length - 1; i++)
                {
                    ProfileProperties[Convert.ToInt32(aryNewOrder[i])].ViewOrder = i;
                }
                ProfileProperties.Sort();
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
                    _ProfileProperties = (ProfilePropertyDefinitionCollection) myState[1];
                }
            }
        }

        protected override object SaveViewState()
        {
            var allStates = new object[2];
            allStates[0] = base.SaveViewState();
            allStates[1] = ProfileProperties;
            return allStates;
        }

        public string DisplayDataType(ProfilePropertyDefinition definition)
        {
            string retValue = Null.NullString;
            var objListController = new ListController();
            ListEntryInfo definitionEntry = objListController.GetListEntryInfo(definition.DataType);
            if (definitionEntry != null)
            {
                retValue = definitionEntry.Value;
            }
            return retValue;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (DataGridColumn column in grdProfileProperties.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (CheckBoxColumn)))
                {
                    var cbColumn = (CheckBoxColumn) column;
                    if (cbColumn.DataField == "Required" && UsersPortalId == Null.NullInteger)
                    {
                        cbColumn.Visible = false;
                    }
                    if (SupportsRichClient() == false)
                    {
                        cbColumn.CheckedChanged += grdProfileProperties_ItemCheckedChanged;
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
                        case "Edit":
                            string formatString = EditUrl("PropertyDefinitionID", "KEYFIELD", "EditProfileProperty");
                            formatString = formatString.Replace("KEYFIELD", "{0}");
                            imageColumn.NavigateURLFormatString = formatString;
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

            cmdRefresh.Click += cmdRefresh_Click;
            cmdUpdate.Click += cmdUpdate_Click;
            grdProfileProperties.ItemCommand += grdProfileProperties_ItemCommand;
            grdProfileProperties.ItemCreated += grdProfileProperties_ItemCreated;
            grdProfileProperties.ItemDataBound += grdProfileProperties_ItemDataBound;

            try
            {
                if (!Page.IsPostBack)
                {
                    Localization.LocalizeDataGrid(ref grdProfileProperties, LocalResourceFile);
                    BindGrid();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                UpdateProperties();
                Response.Redirect(Request.RawUrl, true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void grdProfileProperties_ItemCheckedChanged(object sender, DNNDataGridCheckChangedEventArgs e)
        {
            string propertyName = e.Field;
            bool propertyValue = e.Checked;
            bool isAll = e.IsAll;
            int index = e.Item.ItemIndex;
            if (isAll)
            {
                foreach (ProfilePropertyDefinition profProperty in ProfileProperties)
                {
                    switch (propertyName)
                    {
                        case "Required":
                            profProperty.Required = propertyValue;
                            break;
                        case "Visible":
                            profProperty.Visible = propertyValue;
                            break;
                    }
                }
            }
            else
            {
                ProfilePropertyDefinition profProperty = ProfileProperties[index];
                switch (propertyName)
                {
                    case "Required":
                        profProperty.Required = propertyValue;
                        break;
                    case "Visible":
                        profProperty.Visible = propertyValue;
                        break;
                }
            }
            BindGrid();
        }

        private void grdProfileProperties_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            string commandName = e.CommandName;
            int commandArgument = Convert.ToInt32(e.CommandArgument);
            int index = e.Item.ItemIndex;
            switch (commandName)
            {
                case "Delete":
                    DeleteProperty(index);
                    break;
                case "MoveUp":
                    MovePropertyUp(index);
                    break;
                case "MoveDown":
                    MovePropertyDown(index);
                    break;
            }
        }

        private void grdProfileProperties_ItemCreated(object sender, DataGridItemEventArgs e)
        {
            if (SupportsRichClient())
            {
                switch (e.Item.ItemType)
                {
                    case ListItemType.Header:
                        ((WebControl) e.Item.Cells[COLUMN_REQUIRED].Controls[1]).Attributes.Add("onclick", "dnn.util.checkallChecked(this," + COLUMN_REQUIRED + ");");
                        ((CheckBox) e.Item.Cells[COLUMN_REQUIRED].Controls[1]).AutoPostBack = false;
                        ((WebControl) e.Item.Cells[COLUMN_VISIBLE].Controls[1]).Attributes.Add("onclick", "dnn.util.checkallChecked(this," + COLUMN_VISIBLE + ");");
                        ((CheckBox) e.Item.Cells[COLUMN_VISIBLE].Controls[1]).AutoPostBack = false;
                        break;
                    case ListItemType.AlternatingItem:
                    case ListItemType.Item:
                        ((CheckBox) e.Item.Cells[COLUMN_REQUIRED].Controls[0]).AutoPostBack = false;
                        ((CheckBox) e.Item.Cells[COLUMN_VISIBLE].Controls[0]).AutoPostBack = false;
                        ClientAPI.EnableClientSideReorder(e.Item.Cells[COLUMN_MOVE_DOWN].Controls[0], Page, false, grdProfileProperties.ClientID);
                        ClientAPI.EnableClientSideReorder(e.Item.Cells[COLUMN_MOVE_UP].Controls[0], Page, true, grdProfileProperties.ClientID);
                        break;
                }
            }
        }

        protected void grdProfileProperties_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            DataGridItem item = e.Item;
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem)
            {
                Control imgColumnControl = item.Controls[1].Controls[0];
                if (imgColumnControl is ImageButton)
                {
                    var delImage = (ImageButton) imgColumnControl;
                    var profProperty = (ProfilePropertyDefinition) item.DataItem;
                    switch (profProperty.PropertyName.ToLower())
                    {
                        case "lastname":
                        case "firstname":
                        case "preferredtimezone":
                        case "preferredlocale":
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