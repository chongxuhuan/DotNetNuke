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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls;

using Globals = DotNetNuke.Common.Globals;

#endregion

namespace DotNetNuke.Modules.Admin.Portals
{

    public partial class Portals : PortalModuleBase, IActionable
    {

        protected int TotalPages = -1;
        protected int TotalRecords;
        private int _currentPage = 1;
        private string _Filter = "";
        private ArrayList _portals = new ArrayList();

        protected int CurrentPage
        {
            get
            {
                return _currentPage;
            }
            set
            {
                _currentPage = value;
            }
        }

        protected string Filter
        {
            get
            {
                return _Filter;
            }
            set
            {
                _Filter = value;
            }
        }

        protected ArrayList PortalsList
        {
            get
            {
                return _portals;
            }
            set
            {
                _portals = value;
            }
        }

        protected int PageSize
        {
            get
            {
                return 20;
            }
        }

        protected bool SuppressPager
        {
            get
            {
                return true;
            }
        }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                                  {
                                      {
                                          GetNextActionID(), Localization.GetString(ModuleActionType.AddContent, LocalResourceFile), ModuleActionType.AddContent, "", "add.gif", EditUrl("Signup"), false,
                                          SecurityAccessLevel.Host, true, false
                                          },
                                      {
                                          GetNextActionID(), Localization.GetString("ExportTemplate.Action", LocalResourceFile), ModuleActionType.AddContent, "", "lt.gif", EditUrl("Template"), false,
                                          SecurityAccessLevel.Admin, true, false
                                          },
                                      {
                                          GetNextActionID(), Localization.GetString("DeleteExpired.Action", LocalResourceFile), ModuleActionType.AddContent, "Delete", "delete.gif", "",
                                          "confirm('" + ClientAPI.GetSafeJSString(Localization.GetString("DeleteItems.Confirm")) + "')", true, SecurityAccessLevel.Admin, true, false
                                          }
                                  };
                return actions;
            }
        }

        #endregion

        private void BindData()
        {
            CreateLetterSearch();
            var strQuerystring = Null.NullString;
            if (!String.IsNullOrEmpty(Filter))
            {
                strQuerystring += "filter=" + Filter;
            }
            if (Filter == Localization.GetString("Expired", LocalResourceFile))
            {
                PortalsList = PortalController.GetExpiredPortals();
                ctlPagingControl.Visible = false;
            }
            else
            {
                PortalsList = PortalController.GetPortalsByName(Filter + "%", CurrentPage - 1, PageSize, ref TotalRecords);
            }
            grdPortals.DataSource = PortalsList;
            grdPortals.DataBind();
            ctlPagingControl.TotalRecords = TotalRecords;
            ctlPagingControl.PageSize = PageSize;
            ctlPagingControl.CurrentPage = CurrentPage;
            ctlPagingControl.QuerystringParams = strQuerystring;
            ctlPagingControl.TabID = TabId;
            if (SuppressPager && ctlPagingControl.Visible)
            {
                ctlPagingControl.Visible = (PageSize < TotalRecords);
            }
        }

        private void CheckSecurity()
        {
            if (!UserInfo.IsSuperUser)
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }
        }

        private void CreateLetterSearch()
        {
            var filters = Localization.GetString("Filter.Text", LocalResourceFile);
            filters += "," + Localization.GetString("All");
            filters += "," + Localization.GetString("Expired", LocalResourceFile);
            var strAlphabet = filters.Split(',');
            rptLetterSearch.DataSource = strAlphabet;
            rptLetterSearch.DataBind();
        }

        private void DeleteExpiredPortals()
        {
            try
            {
                CheckSecurity();
                PortalController.DeleteExpiredPortals(Globals.GetAbsoluteServerPath(Request));
                BindData();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected string FilterURL(string filter, string currentPage)
        {
            string url;
            if (!String.IsNullOrEmpty(filter))
            {
                url = !String.IsNullOrEmpty(currentPage) ? Globals.NavigateURL(TabId, "", "filter=" + filter, "currentpage=" + CurrentPage) : Globals.NavigateURL(TabId, "", "filter=" + filter);
            }
            else
            {
                url = !String.IsNullOrEmpty(currentPage) ? Globals.NavigateURL(TabId, "", "currentpage=" + CurrentPage) : Globals.NavigateURL(TabId, "");
            }
            return url;
        }

        public string FormatExpiryDate(DateTime dateTime)
        {
            var strDate = string.Empty;
            try
            {
                if (!Null.IsNull(dateTime))
                {
                    strDate = dateTime.ToShortDateString();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return strDate;
        }

        public string FormatPortalAliases(int portalID)
        {
            var str = new StringBuilder();
            try
            {
                var objPortalAliasController = new PortalAliasController();
                var arr = objPortalAliasController.GetPortalAliasArrayByPortalID(portalID);
                PortalAliasInfo objPortalAliasInfo;
                int i;
                for (i = 0; i <= arr.Count - 1; i++)
                {
                    objPortalAliasInfo = (PortalAliasInfo) arr[i];

                    var httpAlias = Globals.AddHTTP(objPortalAliasInfo.HTTPAlias);
                    var originalUrl = HttpContext.Current.Items["UrlRewrite:OriginalUrl"].ToString().ToLowerInvariant();

                    httpAlias = Globals.AddPort(httpAlias, originalUrl);

                    str.Append("<a href=\"" + httpAlias + "\">" + objPortalAliasInfo.HTTPAlias + "</a>" + "<BR>");
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
            return str.ToString();
        }

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (DataGridColumn column in grdPortals.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (ImageCommandColumn)))
                {
                    var imageColumn = (ImageCommandColumn) column;
                    if (imageColumn.CommandName == "Delete")
                    {
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem");
                    }
                    if (imageColumn.CommandName == "Edit")
                    {
                        var formatString = EditUrl("pid", "KEYFIELD", "Edit");
                        formatString = formatString.Replace("KEYFIELD", "{0}");
                        imageColumn.NavigateURLFormatString = formatString;
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

            grdPortals.DeleteCommand += OnGridDeleteCommand;
            grdPortals.ItemDataBound += OnGridItemDataBound;

            try
            {
                AddActionHandler(OnModuleActionClick);
                if (!UserInfo.IsSuperUser)
                {
                    Response.Redirect(Globals.NavigateURL("Access Denied"), true);
                }
                if (Request.QueryString["CurrentPage"] != null)
                {
                    CurrentPage = Convert.ToInt32(Request.QueryString["CurrentPage"]);
                }
                if (Request.QueryString["filter"] != null)
                {
                    Filter = Request.QueryString["filter"];
                }
                if (Filter == Localization.GetString("All"))
                {
                    Filter = "";
                }
                if (!Page.IsPostBack)
                {
                    Localization.LocalizeDataGrid(ref grdPortals, LocalResourceFile);
                    BindData();
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
                    DeleteExpiredPortals();
                    break;
            }
        }

        protected void OnGridDeleteCommand(object source, DataGridCommandEventArgs e)
        {
            try
            {
                var objPortalController = new PortalController();
                var portal = objPortalController.GetPortal(Int32.Parse(e.CommandArgument.ToString()));
                if (portal != null)
                {
                    var strMessage = PortalController.DeletePortal(portal, Globals.GetAbsoluteServerPath(Request));
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        var objEventLog = new EventLogController();
                        objEventLog.AddLog("PortalName", portal.PortalName, PortalSettings, UserId, EventLogController.EventLogType.PORTAL_DELETED);
                        UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("PortalDeleted", LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess);
                    }
                    else
                    {
                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                    }
                }
                BindData();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void OnGridItemDataBound(object sender, DataGridItemEventArgs e)
        {
            var item = e.Item;
            switch (item.ItemType)
            {
                case ListItemType.SelectedItem:
                case ListItemType.AlternatingItem:
                case ListItemType.Item:
                    {
                        var imgColumnControl = item.Controls[1].Controls[0];
                        if (imgColumnControl is ImageButton)
                        {
                            var delImage = (ImageButton) imgColumnControl;
                            var portal = (PortalInfo) item.DataItem;
                            delImage.Visible = portal.PortalID != PortalSettings.PortalId;
                        }
                    }
                    break;
            }
        }

        #endregion

    }
}