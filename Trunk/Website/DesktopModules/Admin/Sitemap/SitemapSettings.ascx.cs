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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Sitemap;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.Web.UI.WebControls;

using Telerik.Web.UI;

#endregion

namespace DotNetNuke.Modules.Admin.Sitemap
{
    public partial class SitemapSettings : PortalModuleBase
    {
        #region "Page Events"

        public SitemapSettings()
        {
            jQuery.RequestRegistration();
            AJAX.RegisterScriptManager();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cboSearchEngine.SelectedIndexChanged += cboSearchEngine_SelectedIndexChanged;
            grdProviders.UpdateCommand += grdProviders_UpdateCommand;
            grdProviders.ItemCommand += grdProviders_ItemCommand;
            lnkResetCache.Click += lnkResetCache_Click;
            lnkSaveAll.Click += lnkSaveAll_Click;
            cmdVerification.Click += cmdVerification_Click;
            try
            {
                if (Page.IsPostBack == false)
                {
                    LoadConfiguration();

                    if (IsChildPortal(PortalSettings, Context))
                    {
                        lnkSiteMapUrl.Text = Globals.AddHTTP(Globals.GetDomainName(Request)) + "/SiteMap.aspx?portalid=" + PortalId;
                    }
                    else
                    {
                        lnkSiteMapUrl.Text = Globals.AddHTTP(PortalSettings.PortalAlias.HTTPAlias) + "/SiteMap.aspx";
                    }

                    lnkSiteMapUrl.NavigateUrl = lnkSiteMapUrl.Text;

                    //Dim ProviderEditCol = New DnnGridEditColumn()
                    //grdProviders.MasterTableView.Columns.Add(ProviderEditCol)

                    //Dim ProviderNameCol = New DnnGridBoundColumn()
                    //grdProviders.MasterTableView.Columns.Add(ProviderNameCol)

                    //Dim ProviderDescriptionCol = New DnnGridBoundColumn()
                    //grdProviders.MasterTableView.Columns.Add(ProviderDescriptionCol)

                    //Dim ProviderPriorityOverrideCol = New GridCheckBoxColumn()
                    //grdProviders.MasterTableView.Columns.Add(ProviderPriorityOverrideCol)

                    //Dim ProviderPriorityCol = New DnnGridBoundColumn()
                    //grdProviders.MasterTableView.Columns.Add(ProviderPriorityCol)

                    //Dim ProviderEnabledCol = New GridCheckBoxColumn()
                    //grdProviders.MasterTableView.Columns.Add(ProviderEnabledCol)

                    //grdProviders.MasterTableView.EditMode = GridEditMode.InPlace

                    //ProviderEditCol.HeaderText = String.Empty
                    //ProviderEditCol.HeaderStyle.Width = 0
                    //ProviderEditCol.EditText = "Edit"
                    //ProviderEditCol.CancelText = "Cancel"
                    //ProviderEditCol.UpdateText = "Update"

                    //ProviderNameCol.DataField = "Name"
                    //ProviderNameCol.ReadOnly = True
                    //ProviderNameCol.HeaderText = "Name"

                    //ProviderDescriptionCol.DataField = "Description"
                    //ProviderDescriptionCol.ReadOnly = True
                    //ProviderDescriptionCol.HeaderText = "Description"

                    //ProviderPriorityCol.DataField = "Priority"
                    //ProviderPriorityCol.HeaderStyle.Width = 0
                    //ProviderPriorityCol.HeaderText = "Priority"

                    //ProviderPriorityOverrideCol.DataField = "OverridePriority"
                    //ProviderPriorityOverrideCol.HeaderStyle.Width = 0
                    //ProviderPriorityOverrideCol.HeaderText = "OverridePriority"

                    //ProviderEnabledCol.DataField = "Enabled"
                    //ProviderEnabledCol.HeaderStyle.Width = 0
                    //ProviderEnabledCol.HeaderText = "Enabled"

                    BindProviders();
                    SetSearchEngineSubmissionURL();
                }

                grdProviders_needDataSorce();

                grdProviders.NeedDataSource += grdProviders_NeedDataSorce;
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        #region "Configuration Handlers"

        private void lnkSaveAll_Click(object sender, EventArgs e)
        {
            SavePrioritySettings();

            PortalController.UpdatePortalSetting(PortalId, "SitemapIncludeHidden", chkIncludeHidden.Checked.ToString());

            float excludePriority = float.Parse(txtExcludePriority.Text);
            PortalController.UpdatePortalSetting(PortalId, "SitemapExcludePriority", excludePriority.ToString(NumberFormatInfo.InvariantInfo));

            if ((cmbDaysToCache.SelectedIndex == 0))
            {
                ResetCache();
            }

            PortalController.UpdatePortalSetting(PortalId, "SitemapCacheDays", cmbDaysToCache.SelectedIndex.ToString());

            UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("MessageUpdated", LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess);


            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            // core settings
            chkLevelPriority.Checked = bool.Parse(PortalController.GetPortalSetting("SitemapLevelMode", PortalId, "False"));

            float minPriority = 0;
            minPriority = float.Parse(PortalController.GetPortalSetting("SitemapMinPriority", PortalId, "0.1"), NumberFormatInfo.InvariantInfo);
            txtMinPagePriority.Text = minPriority.ToString();

            chkIncludeHidden.Checked = bool.Parse(PortalController.GetPortalSetting("SitemapIncludeHidden", PortalId, "False"));

            // General settings
            float excludePriority = 0;
            excludePriority = float.Parse(PortalController.GetPortalSetting("SitemapExcludePriority", PortalId, "0.1"), NumberFormatInfo.InvariantInfo);
            txtExcludePriority.Text = excludePriority.ToString();

            cmbDaysToCache.SelectedIndex = Int32.Parse(PortalController.GetPortalSetting("SitemapCacheDays", PortalId, "1"));
        }

        private void SavePrioritySettings()
        {
            PortalController.UpdatePortalSetting(PortalId, "SitemapLevelMode", chkLevelPriority.Checked.ToString());

            if (float.Parse(txtMinPagePriority.Text) < 0)
            {
                txtMinPagePriority.Text = "0";
            }
            float minPriority = float.Parse(txtMinPagePriority.Text);

            PortalController.UpdatePortalSetting(PortalId, "SitemapMinPriority", minPriority.ToString(NumberFormatInfo.InvariantInfo));
        }

        #endregion

        private void ResetCache()
        {
            var CacheFolder = new DirectoryInfo(PortalSettings.HomeDirectoryMapPath + "sitemap\\");

            if (CacheFolder.Exists)
            {
                foreach (FileInfo file in CacheFolder.GetFiles("sitemap*.xml"))
                {
                    file.Delete();
                }

                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("ResetOK", LocalResourceFile), ModuleMessage.ModuleMessageType.GreenSuccess);
            }
        }

        private bool IsChildPortal(PortalSettings ps, HttpContext context)
        {
            bool isChild = false;
            string portalName = null;
            var aliasController = new PortalAliasController();
            ArrayList arr = aliasController.GetPortalAliasArrayByPortalID(ps.PortalId);
            string serverPath = Globals.GetAbsoluteServerPath(context.Request);

            if (arr.Count > 0)
            {
                var portalAlias = (PortalAliasInfo) arr[0];
                portalName = Globals.GetPortalDomainName(ps.PortalAlias.HTTPAlias, Request, true);
                if (portalAlias.HTTPAlias.IndexOf("/") > -1)
                {
                    portalName = portalAlias.HTTPAlias.Substring(portalAlias.HTTPAlias.LastIndexOf("/") + 1);
                }
                if (!string.IsNullOrEmpty(portalName) && Directory.Exists(serverPath + portalName))
                {
                    isChild = true;
                }
            }
            return isChild;
        }

        private void BindProviders()
        {
            var builder = new SitemapBuilder(PortalSettings);

            grdProviders.DataSource = builder.Providers;
        }

        protected void grdProviders_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.UpdateCommandName)
            {
                if (!Page.IsValid)
                {
                    e.Canceled = true;
                }
            }
        }

        //RadGrid1_ItemCommand

        protected void grdProviders_UpdateCommand(object source, GridCommandEventArgs e)
        {
            //grdProviders.Rebind()

            var editedItem = (GridEditableItem) e.Item;
            GridEditManager editMan = editedItem.EditManager;

            var editedSiteMap = (SitemapProvider) e.Item.DataItem;

            GridColumn nameCol = ((DnnGrid) source).Columns.FindByUniqueName("Name");
            IGridColumnEditor nameEditor = editMan.GetColumnEditor((IGridEditableColumn) nameCol);
            string key = ((GridTextColumnEditor) nameEditor).Text;

            var providers = (List<SitemapProvider>) grdProviders.DataSource;
            SitemapProvider editedProvider = null;

            foreach (SitemapProvider p in providers)
            {
                if ((string.Equals(key, p.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    editedProvider = p;
                }
            }

            bool providerEnabled = false;
            string providerPriorityString = string.Empty;
            bool providerOverride = false;

            foreach (GridColumn column in e.Item.OwnerTableView.Columns)
            {
                if (column is IGridEditableColumn)
                {
                    var editableCol = (IGridEditableColumn) column;


                    if ((editableCol.IsEditable))
                    {
                        IGridColumnEditor editor = editMan.GetColumnEditor(editableCol);

                        string editorType = (editor).ToString();
                        string editorText = "unknown";
                        object editorValue = null;

                        if ((editor is GridTextColumnEditor))
                        {
                            editorText = ((GridTextColumnEditor) editor).Text;
                            editorValue = ((GridTextColumnEditor) editor).Text;
                        }

                        if ((editor is GridBoolColumnEditor))
                        {
                            editorText = ((GridBoolColumnEditor) editor).Value.ToString();
                            editorValue = ((GridBoolColumnEditor) editor).Value;
                        }

                        if ((column.UniqueName == "Enabled"))
                        {
                            providerEnabled = Convert.ToBoolean(editorValue);
                        }
                        else if ((column.UniqueName == "Priority"))
                        {
                            providerPriorityString = editorValue.ToString();
                        }
                        else if ((column.UniqueName == "OverridePriority"))
                        {
                            providerOverride = Convert.ToBoolean(editorValue);
                        }
                    }
                }
            }

            float providerPriority = 0;

            if ((float.TryParse(providerPriorityString, out providerPriority)))
            {
                editedProvider.Enabled = providerEnabled;
                editedProvider.OverridePriority = providerOverride;
                editedProvider.Priority = providerPriority;
            }
            else
            {
                UI.Skins.Skin.AddModuleMessage(this, Localization.GetString("valPriority.Text", LocalResourceFile), ModuleMessage.ModuleMessageType.RedError);
            }
        }

        private void grdProviders_needDataSorce()
        {
            BindProviders();
        }

        private void grdProviders_NeedDataSorce(object sender, GridNeedDataSourceEventArgs e)
        {
            grdProviders_needDataSorce();
        }

        protected void lnkResetCache_Click(object sender, EventArgs e)
        {
            ResetCache();
        }

        #region "Site Submission"

        private void SetSearchEngineSubmissionURL()
        {
            try
            {
                if ((cboSearchEngine.SelectedItem != null))
                {
                    string strURL = "";
                    switch (cboSearchEngine.SelectedItem.Text.ToLower().Trim())
                    {
                        case "google":
                            strURL += "http://www.google.com/addurl?q=" + Globals.HTTPPOSTEncode(Globals.AddHTTP(Globals.GetDomainName(Request)));
                            strURL += "&dq=";
                            if (!string.IsNullOrEmpty(PortalSettings.PortalName))
                            {
                                strURL += Globals.HTTPPOSTEncode(PortalSettings.PortalName);
                            }
                            if (!string.IsNullOrEmpty(PortalSettings.Description))
                            {
                                strURL += Globals.HTTPPOSTEncode(PortalSettings.Description);
                            }
                            if (!string.IsNullOrEmpty(PortalSettings.KeyWords))
                            {
                                strURL += Globals.HTTPPOSTEncode(PortalSettings.KeyWords);
                            }
                            strURL += "&submit=Add+URL";
                            break;
                        case "yahoo!":
                            strURL = "http://siteexplorer.search.yahoo.com/submit";
                            break;
                        case "bing":
                            strURL = "http://www.bing.com/webmaster";
                            break;
                    }

                    cmdSubmitSitemap.NavigateUrl = strURL;

                    cmdSubmitSitemap.Target = "_blank";
                }
                //UrlUtils.OpenNewWindow(Me.Page, Me.GetType(), strURL)
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cboSearchEngine_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SetSearchEngineSubmissionURL();
        }

        protected void cmdVerification_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtVerification.Text) && txtVerification.Text.EndsWith(".html"))
            {
                if (!File.Exists(Globals.ApplicationMapPath + "\\" + txtVerification.Text))
                {
                    // write SiteMap verification file
                    StreamWriter objStream = default(StreamWriter);
                    objStream = File.CreateText(Globals.ApplicationMapPath + "\\" + txtVerification.Text);
                    objStream.WriteLine("Google SiteMap Verification File");
                    objStream.WriteLine(" - " + lnkSiteMapUrl.Text);
                    objStream.WriteLine(" - " + UserInfo.DisplayName);
                    objStream.Close();
                }
            }
        }

        #endregion
    }
}