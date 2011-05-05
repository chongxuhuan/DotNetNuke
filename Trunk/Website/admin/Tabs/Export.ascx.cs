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
using System.Web.UI.WebControls;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.Modules.Admin.Tabs
{
    public partial class Export : PortalModuleBase
    {
        private TabInfo _Tab;

        public TabInfo Tab
        {
            get
            {
                if (_Tab == null)
                {
                    var objTabs = new TabController();
                    _Tab = objTabs.GetTab(TabId, PortalId, false);
                }
                return _Tab;
            }
        }

        private void SerializeTab(XmlDocument xmlTemplate, XmlNode nodeTabs)
        {
            XmlNode nodeTab;
            XmlDocument xmlTab;
            xmlTab = new XmlDocument();
            nodeTab = TabController.SerializeTab(xmlTab, Tab, chkContent.Checked);
            nodeTabs.AppendChild(xmlTemplate.ImportNode(nodeTab, true));
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!TabPermissionController.CanExportPage())
            {
                Response.Redirect(Globals.AccessDeniedURL(), true);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdCancel.Click += cmdCancel_Click;
            cmdExport.Click += cmdExport_Click;

            try
            {
                if (!Page.IsPostBack)
                {
                    ArrayList folders = FileSystemUtils.GetFoldersByUser(PortalId, false, false, "ADD");
                    foreach (FolderInfo folder in folders)
                    {
                        var FolderItem = new ListItem();
                        if (folder.FolderPath == Null.NullString)
                        {
                            FolderItem.Text = Localization.GetString("Root", LocalResourceFile);
                        }
                        else
                        {
                            FolderItem.Text = PathUtils.Instance.RemoveTrailingSlash(folder.DisplayPath);
                        }
                        FolderItem.Value = folder.FolderPath;
                        cboFolders.Items.Add(FolderItem);
                    }
                    if (Tab != null)
                    {
                        txtFile.Text = Globals.CleanName(Tab.TabName);
                    }
                    if (cboFolders.Items.FindByValue("Templates/") != null)
                    {
                        cboFolders.Items.FindByValue("Templates/").Selected = true;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            try
            {
                Response.Redirect(Globals.NavigateURL(), true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdExport_Click(Object sender, EventArgs e)
        {
            try
            {
                XmlDocument xmlTemplate;
                XmlNode nodePortal;
                if (!Page.IsValid)
                {
                    return;
                }
                string filename;
                filename = PortalSettings.HomeDirectoryMapPath + cboFolders.SelectedItem.Value + txtFile.Text + ".page.template";
                filename = filename.Replace("/", "\\");
                xmlTemplate = new XmlDocument();
                nodePortal = xmlTemplate.AppendChild(xmlTemplate.CreateElement("portal"));
                nodePortal.Attributes.Append(XmlUtils.CreateAttribute(xmlTemplate, "version", "3.0"));
                XmlElement node = xmlTemplate.CreateElement("description");
                node.InnerXml = Server.HtmlEncode(txtDescription.Text);
                nodePortal.AppendChild(node);
                XmlNode nodeTabs;
                nodeTabs = nodePortal.AppendChild(xmlTemplate.CreateElement("tabs"));
                SerializeTab(xmlTemplate, nodeTabs);
                xmlTemplate.Save(filename);
                lblMessage.Text = string.Format(Localization.GetString("ExportedMessage", LocalResourceFile), filename);
                FileSystemUtils.AddFile(txtFile.Text + ".page.template", PortalId, cboFolders.SelectedItem.Value, PortalSettings.HomeDirectoryMapPath, "application/octet-stream");
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}