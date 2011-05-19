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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Vendors;
using DotNetNuke.UI.Utilities;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Vendors
{
    public partial class Vendors : PortalModuleBase, IActionable
    {
        protected int CurrentPage = -1;
        protected int TotalPages = -1;
        protected Label lblMessage;
        private string strFilter;

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection();
                actions.Add(GetNextActionID(),
                            Localization.GetString(ModuleActionType.AddContent, LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "",
                            EditUrl(),
                            false,
                            SecurityAccessLevel.Edit,
                            true,
                            false);
                if(IsEditable)
                {
                    actions.Add(GetNextActionID(),
                                Localization.GetString("cmdDelete", LocalResourceFile),
                                ModuleActionType.AddContent,
                                "Delete",
                                "delete.gif",
                                "",
                                "confirm('" + ClientAPI.GetSafeJSString(Localization.GetString("DeleteItems.Confirm")) + "')",
                                true,
                                SecurityAccessLevel.Admin,
                                true,
                                false);
                }
                return actions;
            }
        }

        #endregion

        private void BindData()
        {
            BindData(null, null);
        }

        private void BindData(string searchText, string searchField)
        {
            CreateLetterSearch();
            Localization.LocalizeDataGrid(ref grdVendors, LocalResourceFile);
            if (searchText == Localization.GetString("All"))
            {
                strFilter = "";
            }
            else if (searchText == Localization.GetString("Unauthorized"))
            {
                strFilter = "";
            }
            else
            {
                strFilter = searchText;
            }
            var PageSize = Convert.ToInt32(ddlRecordsPerPage.SelectedItem.Value);
            var TotalRecords = 0;
            var objVendors = new VendorController();
            int Portal;
            Portal = PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId ? Null.NullInteger : PortalId;
            if (String.IsNullOrEmpty(strFilter))
            {
                if (searchText == Localization.GetString("Unauthorized"))
                {
                    grdVendors.DataSource = objVendors.GetVendors(Portal, true, CurrentPage - 1, PageSize, ref TotalRecords);
                }
                else
                {
                    grdVendors.DataSource = objVendors.GetVendors(Portal, false, CurrentPage - 1, PageSize, ref TotalRecords);
                }
            }
            else
            {
                if (searchField == "email")
                {
                    grdVendors.DataSource = objVendors.GetVendorsByEmail(strFilter, Portal, CurrentPage - 1, PageSize, ref TotalRecords);
                }
                else
                {
                    grdVendors.DataSource = objVendors.GetVendorsByName(strFilter, Portal, CurrentPage - 1, PageSize, ref TotalRecords);
                }
            }
            grdVendors.DataBind();
            ctlPagingControl.TotalRecords = TotalRecords;
            ctlPagingControl.PageSize = PageSize;
            ctlPagingControl.CurrentPage = CurrentPage;
            string strQuerystring = "";
            if (ddlRecordsPerPage.SelectedIndex != 0)
            {
                strQuerystring = "PageRecords=" + ddlRecordsPerPage.SelectedValue;
            }
            if (!String.IsNullOrEmpty(strFilter))
            {
                strQuerystring += "&filter=" + strFilter;
            }
            ctlPagingControl.QuerystringParams = strQuerystring;
            ctlPagingControl.TabID = TabId;
        }

        private void CreateLetterSearch()
        {
            string[] strAlphabet = {
                                       "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", Localization.GetString("All"),
                                       Localization.GetString("Unauthorized")
                                   };
            rptLetterSearch.DataSource = strAlphabet;
            rptLetterSearch.DataBind();
        }

        public string DisplayAddress(object Unit, object Street, object City, object Region, object Country, object PostalCode)
        {
            return Globals.FormatAddress(Unit, Street, City, Region, Country, PostalCode);
        }

        public string FormatURL(string strKeyName, string strKeyValue)
        {
            return !String.IsNullOrEmpty(strFilter) ? EditUrl(strKeyName, strKeyValue, "", "filter=" + strFilter) : EditUrl(strKeyName, strKeyValue);
        }

        protected string FilterURL(string Filter, string CurrentPage)
        {
            if (!String.IsNullOrEmpty(Filter))
            {
                if (!String.IsNullOrEmpty(CurrentPage))
                {
                    return Globals.NavigateURL(TabId, "", "filter=" + Filter, "currentpage=" + CurrentPage, "PageRecords=" + ddlRecordsPerPage.SelectedValue);
                }
                else
                {
                    return Globals.NavigateURL(TabId, "", "filter=" + Filter, "PageRecords=" + ddlRecordsPerPage.SelectedValue);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(CurrentPage))
                {
                    return Globals.NavigateURL(TabId, "", "currentpage=" + CurrentPage, "PageRecords=" + ddlRecordsPerPage.SelectedValue);
                }
                else
                {
                    return Globals.NavigateURL(TabId, "", "PageRecords=" + ddlRecordsPerPage.SelectedValue);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            grdVendors.ItemCommand += grdVendors_ItemCommand;
            ddlRecordsPerPage.SelectedIndexChanged += OnRecordsPerPageIndexChanged;
            btnSearch.Click += OnSearchClick;

            try
            {
            	AddActionHandler(OnModuleActionClick);

                CurrentPage = Request.QueryString["CurrentPage"] != null ? Convert.ToInt32(Request.QueryString["CurrentPage"]) : 1;
                strFilter = Request.QueryString["filter"] ?? "";
                if (!Page.IsPostBack)
                {
                    if (Request.QueryString["PageRecords"] != null)
                    {
                        ddlRecordsPerPage.SelectedValue = Request.QueryString["PageRecords"];
                    }
                    BindData(strFilter, "username");
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnModuleActionClick(object sender, ActionEventArgs e)
        {
            switch (e.Action.CommandArgument)
            {
                case "Delete":
                    try
                    {
                        var objVendors = new VendorController();
                        if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                        {
                            objVendors.DeleteVendors();
                        }
                        else
                        {
                            objVendors.DeleteVendors(PortalId);
                        }
                        Response.Redirect(Globals.NavigateURL(), true);
                    }
                    catch (Exception exc)
                    {
                        Exceptions.ProcessModuleLoadException(this, exc);
                    }                    
                    break;
            }
        }

        protected void grdVendors_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                if (e.CommandName == "filter")
                {
                    strFilter = e.CommandArgument.ToString();
                    CurrentPage = 1;
                    txtSearch.Text = "";
                    BindData(strFilter, "username");
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnRecordsPerPageIndexChanged(Object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindData();
        }

        protected void OnSearchClick(Object sender, EventArgs e)
        {
            CurrentPage = 1;
            BindData(txtSearch.Text, ddlSearchType.SelectedItem.Value);
        }

    }
}