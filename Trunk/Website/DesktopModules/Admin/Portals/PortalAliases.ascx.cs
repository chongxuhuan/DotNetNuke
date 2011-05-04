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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.Portals
{
    public partial class PortalAliases : PortalModuleBase
    {
        private ArrayList _Aliases;
        private int intPortalId = -1;

        private bool AddMode
        {
            get
            {
                bool _Mode = Null.NullBoolean;
                if (ViewState["Mode"] != null)
                {
                    _Mode = Convert.ToBoolean(ViewState["Mode"]);
                }
                return _Mode;
            }
            set
            {
                ViewState["Mode"] = value;
            }
        }

        private ArrayList Aliases
        {
            get
            {
                if (_Aliases == null)
                {
                    _Aliases = new PortalAliasController().GetPortalAliasArrayByPortalID(intPortalId);
                }
                return _Aliases;
            }
            set
            {
                _Aliases = value;
            }
        }

        private void BindAliases()
        {
            dgPortalAlias.DataSource = Aliases;
            dgPortalAlias.DataBind();
        }

        protected override void LoadViewState(object savedState)
        {
            var myState = (object[]) savedState;
            if ((myState[0] != null))
            {
                base.LoadViewState(myState[0]);
            }
            if ((myState[1] != null))
            {
                var aliasCount = (int) myState[1];
                Aliases.Clear();
                for (int i = 0; i <= aliasCount - 1; i++)
                {
                    string aliasString = Convert.ToString(myState[i + 2]);
                    var sr = new StringReader(aliasString);
                    Aliases.Add(CBO.DeserializeObject<PortalAliasInfo>(sr));
                }
            }
        }

        protected override object SaveViewState()
        {
            object baseState = base.SaveViewState();
            var allStates = new object[Aliases.Count + 2];
            allStates[0] = baseState;
            allStates[1] = Aliases.Count;
            for (int i = 0; i <= Aliases.Count - 1; i++)
            {
                var portalAlias = (PortalAliasInfo) Aliases[i];
                var sw = new StringWriter();
                CBO.SerializeObject(portalAlias, sw);
                allStates[i + 2] = sw.ToString();
            }
            return allStates;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (DataGridColumn column in dgPortalAlias.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (ImageCommandColumn)))
                {
                    var imageColumn = (ImageCommandColumn) column;
                    if (imageColumn.CommandName == "Delete")
                    {
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem");
                    }
                    if (!String.IsNullOrEmpty(imageColumn.CommandName))
                    {
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, LocalResourceFile);
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdAddAlias.Click += AddAlias;
            dgPortalAlias.ItemDataBound += grdPortals_ItemDataBound;
            dgPortalAlias.EditCommand += EditAlias;
            dgPortalAlias.UpdateCommand += SaveAlias;
            dgPortalAlias.DeleteCommand += DeleteAlias;
            dgPortalAlias.CancelCommand += CancelEdit;

            if ((Request.QueryString["pid"] != null))
            {
                intPortalId = Int32.Parse(Request.QueryString["pid"]);
            }
            else
            {
                intPortalId = PortalId;
            }
            if (!Page.IsPostBack)
            {
                Localization.LocalizeDataGrid(ref dgPortalAlias, LocalResourceFile);
                BindAliases();
            }
        }

        protected void grdPortals_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            string defaultAlias = PortalController.GetPortalSetting("DefaultPortalAlias", intPortalId, "");
            DataGridItem item = e.Item;
            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem || item.ItemType == ListItemType.SelectedItem)
            {
                Control imgColumnControl = item.Controls[1].Controls[0];
                if (imgColumnControl is ImageButton)
                {
                    var delImage = (ImageButton) imgColumnControl;
                    var portalAlias = (PortalAliasInfo) item.DataItem;
                    delImage.Visible = portalAlias.PortalAliasID != PortalAlias.PortalAliasID && portalAlias.HTTPAlias != defaultAlias;
                }
                imgColumnControl = item.Controls[0].Controls[0];
                if (imgColumnControl is ImageButton)
                {
                    var editImage = (ImageButton) imgColumnControl;
                    var portalAlias = (PortalAliasInfo) item.DataItem;
                    editImage.Visible = portalAlias.PortalAliasID != PortalAlias.PortalAliasID;
                }
            }
            else if (item.ItemType == ListItemType.EditItem)
            {
                var chkChild = item.Cells[1].FindControl("chkChild") as CheckBox;
                chkChild.Visible = AddMode;
            }
        }

        private void AddAlias(object sender, EventArgs e)
        {
            var portalAlias = new PortalAliasInfo();
            portalAlias.PortalID = intPortalId;
            Aliases.Add(portalAlias);
            dgPortalAlias.EditItemIndex = Aliases.Count - 1;
            AddMode = true;
            BindAliases();
        }

        private void DeleteAlias(object source, DataGridCommandEventArgs e)
        {
            var controller = new PortalAliasController();
            int index = e.Item.ItemIndex;
            var portalAlias = (PortalAliasInfo) Aliases[index];
            controller.DeletePortalAlias(portalAlias.PortalAliasID);
            _Aliases = null;
            BindAliases();
        }

        private void EditAlias(object source, DataGridCommandEventArgs e)
        {
            AddMode = false;
            dgPortalAlias.EditItemIndex = e.Item.ItemIndex;
            BindAliases();
        }

        protected void SaveAlias(object source, CommandEventArgs e)
        {
            var controller = new PortalAliasController();
            bool isChild = false;
            string childPath = string.Empty;
            string message = string.Empty;

            //Get the index of the row to save
            int index = dgPortalAlias.EditItemIndex;

            var portalAlias = (PortalAliasInfo) Aliases[index];
            var ctlAlias = (TextBox) dgPortalAlias.Items[index].Cells[1].FindControl("txtHTTPAlias");
            var chkChild = (CheckBox) dgPortalAlias.Items[index].Cells[2].FindControl("chkChild");

            string strAlias = ctlAlias.Text.Trim();
            if (string.IsNullOrEmpty(strAlias))
            {
                message = Localization.GetString("InvalidAlias", LocalResourceFile);
            }
            else
            {
                if (strAlias.IndexOf("://") != -1)
                {
                    strAlias = strAlias.Remove(0, strAlias.IndexOf("://") + 3);
                }
                if (strAlias.IndexOf("\\\\") != -1)
                {
                    strAlias = strAlias.Remove(0, strAlias.IndexOf("\\\\") + 2);
                }

                isChild = (chkChild != null && chkChild.Checked);

                //Validate Alias
                if (!PortalAliasController.ValidateAlias(strAlias, false))
                {
                    message = Localization.GetString("InvalidAlias", LocalResourceFile);
                }

                //Validate Child Folder Name
                if (isChild)
                {
                    childPath = strAlias.Substring(strAlias.LastIndexOf("/") + 1);
                    if (!PortalAliasController.ValidateAlias(childPath, true))
                    {
                        message = Localization.GetString("InvalidAlias", LocalResourceFile);
                    }
                }
            }

            if (string.IsNullOrEmpty(message) && isChild)
            {
                //Attempt to create child folder
                string childPhysicalPath = Server.MapPath(childPath);

                if (Directory.Exists(childPhysicalPath))
                {
                    message = Localization.GetString("ChildExists", LocalResourceFile);
                }
                else
                {
                    message = PortalController.CreateChildPortalFolder(childPhysicalPath);
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                portalAlias.HTTPAlias = strAlias;
                if (AddMode)
                {
                    controller.AddPortalAlias(portalAlias);
                }
                else
                {
                    controller.UpdatePortalAliasInfo(portalAlias);
                }

                //Reset Edit Index
                lblError.Visible = false;
                dgPortalAlias.EditItemIndex = -1;
                _Aliases = null;
            }
            else
            {
                lblError.Text = message;
                lblError.Visible = true;
            }

            BindAliases();
        }


        protected void CancelEdit(object source, CommandEventArgs e)
        {
            if (AddMode)
            {
                Aliases.RemoveAt(Aliases.Count - 1);
                AddMode = false;
            }
            dgPortalAlias.EditItemIndex = -1;
            lblError.Visible = false;
            BindAliases();
        }
    }
}