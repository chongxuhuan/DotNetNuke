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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;

using Image = System.Drawing.Image;

#endregion

namespace DotNetNuke.Modules.Admin.Skins
{
    public partial class EditSkins : PortalModuleBase
    {
        private readonly string NotSpecified = "<" + Localization.GetString("Not_Specified") + ">";

        protected string CurrentContainer
        {
            get
            {
                string _CurrentContainer = Null.NullString;
                if (ViewState["CurrentContainer"] != null)
                {
                    _CurrentContainer = Convert.ToString(ViewState["CurrentContainer"]);
                }
                return _CurrentContainer;
            }
            set
            {
                ViewState["CurrentContainer"] = value;
            }
        }

        protected string CurrentSkin
        {
            get
            {
                string _CurrentSkin = Null.NullString;
                if (ViewState["CurrentSkin"] != null)
                {
                    _CurrentSkin = Convert.ToString(ViewState["CurrentSkin"]);
                }
                return _CurrentSkin;
            }
            set
            {
                ViewState["CurrentSkin"] = value;
            }
        }

        private void AddSkinstoCombo(DropDownList combo, string strRoot)
        {
            string strName;
            if (Directory.Exists(strRoot))
            {
                foreach (string strFolder in Directory.GetDirectories(strRoot))
                {
                    strName = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                    if (strName != "_default")
                    {
                        combo.Items.Add(new ListItem(strName, strFolder.Replace(Globals.ApplicationMapPath, "").ToLower()));
                    }
                }
            }
        }

        private string CreateThumbnail(string strImage)
        {
            bool blnCreate = true;
            string strThumbnail = strImage.Replace(Path.GetFileName(strImage), "thumbnail_" + Path.GetFileName(strImage));
            if (File.Exists(strThumbnail))
            {
                DateTime d1 = File.GetLastWriteTime(strThumbnail);
                DateTime d2 = File.GetLastWriteTime(strImage);
                if (File.GetLastWriteTime(strThumbnail) == File.GetLastWriteTime(strImage))
                {
                    blnCreate = false;
                }
            }
            if (blnCreate)
            {
                double dblScale;
                int intHeight;
                int intWidth;
                int intSize = 150;
                Image objImage;
                try
                {
                    objImage = Image.FromFile(strImage);
                    if (objImage.Height > objImage.Width)
                    {
                        dblScale = (double) intSize/objImage.Height;
                        intHeight = intSize;
                        intWidth = Convert.ToInt32(objImage.Width*dblScale);
                    }
                    else
                    {
                        dblScale = (double) intSize/objImage.Width;
                        intWidth = intSize;
                        intHeight = Convert.ToInt32(objImage.Height*dblScale);
                    }
                    Image objThumbnail;
                    objThumbnail = objImage.GetThumbnailImage(intWidth, intHeight, null, IntPtr.Zero);
                    if (File.Exists(strThumbnail))
                    {
                        File.Delete(strThumbnail);
                    }
                    objThumbnail.Save(strThumbnail, objImage.RawFormat);
                    File.SetAttributes(strThumbnail, FileAttributes.Normal);
                    File.SetLastWriteTime(strThumbnail, File.GetLastWriteTime(strImage));
                    objImage.Dispose();
                    objThumbnail.Dispose();
                }
				catch (Exception ex)
				{
					DnnLog.Error(ex);
				}
            }
            strThumbnail = Globals.ApplicationPath + "/" + strThumbnail.Substring(strThumbnail.IndexOf("portals\\"));
            strThumbnail = strThumbnail.Replace("\\", "/");
            return strThumbnail;
        }

        private string GetSkinPath(string Type, string Root, string Name)
        {
            string strPath = Null.NullString;
            switch (Type)
            {
                case "G":
                    strPath = Globals.HostPath + Root + "/" + Name;
                    break;
                case "L":
                    strPath = PortalSettings.HomeDirectory + Root + "/" + Name;
                    break;
            }
            return strPath;
        }

        private bool IsFallbackContainer(string skinPath)
        {
            string strDefaultContainerPath = (Globals.HostMapPath + SkinController.RootContainer + SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo).Folder).Replace("/", "\\");
            if (strDefaultContainerPath.EndsWith("\\"))
            {
                strDefaultContainerPath = strDefaultContainerPath.Substring(0, strDefaultContainerPath.Length - 1);
            }
            return skinPath.IndexOf(strDefaultContainerPath, StringComparison.CurrentCultureIgnoreCase) != -1;
        }

        private bool IsFallbackSkin(string skinPath)
        {
            string strDefaultSkinPath = (Globals.HostMapPath + SkinController.RootSkin + SkinDefaults.GetSkinDefaults(SkinDefaultType.SkinInfo).Folder).Replace("/", "\\");
            if (strDefaultSkinPath.EndsWith("\\"))
            {
                strDefaultSkinPath = strDefaultSkinPath.Substring(0, strDefaultSkinPath.Length - 1);
            }
            return skinPath.ToLowerInvariant() == strDefaultSkinPath.ToLowerInvariant();
        }

        private void LoadCombos()
        {
            cboSkins.Items.Clear();
            CurrentSkin = NotSpecified;
            cboSkins.Items.Add(CurrentSkin);
            if (chkHost.Checked)
            {
                AddSkinstoCombo(cboSkins, Request.MapPath(Globals.HostPath + SkinController.RootSkin));
            }
            if (chkSite.Checked)
            {
                AddSkinstoCombo(cboSkins, PortalSettings.HomeDirectoryMapPath + SkinController.RootSkin);
            }
            cboContainers.Items.Clear();
            CurrentContainer = NotSpecified;
            cboContainers.Items.Add(CurrentContainer);
            if (chkHost.Checked)
            {
                AddSkinstoCombo(cboContainers, Request.MapPath(Globals.HostPath + SkinController.RootContainer));
            }
            if (chkSite.Checked)
            {
                AddSkinstoCombo(cboContainers, PortalSettings.HomeDirectoryMapPath + SkinController.RootContainer);
            }
        }

        private string ParseSkinPackage(string strType, string strRoot, string strName, string strFolder, string strParse)
        {
            string strRootPath = Null.NullString;
            switch (strType)
            {
                case "G":
                    strRootPath = Request.MapPath(Globals.HostPath);
                    break;
                case "L":
                    strRootPath = Request.MapPath(PortalSettings.HomeDirectory);
                    break;
            }
            var objSkinFiles = new SkinFileProcessor(strRootPath, strRoot, strName);
            var arrSkinFiles = new ArrayList();
            string[] arrFiles;
            if (Directory.Exists(strFolder))
            {
                arrFiles = Directory.GetFiles(strFolder);
                foreach (string strFile in arrFiles)
                {
                    switch (Path.GetExtension(strFile))
                    {
                        case ".htm":
                        case ".html":
                        case ".css":
                            if (strFile.ToLower().IndexOf(Globals.glbAboutPage.ToLower()) < 0)
                            {
                                arrSkinFiles.Add(strFile);
                            }
                            break;
                        case ".ascx":
                            if (File.Exists(strFile.Replace(".ascx", ".htm")) == false && File.Exists(strFile.Replace(".ascx", ".html")) == false)
                            {
                                arrSkinFiles.Add(strFile);
                            }
                            break;
                    }
                }
            }
            switch (strParse)
            {
                case "L":
                    return objSkinFiles.ProcessList(arrSkinFiles, SkinParser.Localized);
                case "P":
                    return objSkinFiles.ProcessList(arrSkinFiles, SkinParser.Portable);
            }
            return Null.NullString;
        }

        private void ProcessSkins(string strFolderPath, string type)
        {
            HtmlTable tbl;
            HtmlTableRow row = null;
            HtmlTableCell cell;
            string strFolder;
            string[] arrFiles;
            string strSkinType = "";
            string strRootSkin = "";
            string strURL;
            int intIndex = 0;
            bool fallbackSkin;
            if (Directory.Exists(strFolderPath))
            {
                if (type == "Skin")
                {
                    tbl = tblSkins;
                    strRootSkin = SkinController.RootSkin.ToLower();
                    fallbackSkin = IsFallbackSkin(strFolderPath);
                }
                else
                {
                    tbl = tblContainers;
                    strRootSkin = SkinController.RootContainer.ToLower();
                    fallbackSkin = IsFallbackContainer(strFolderPath);
                }
                if (strFolderPath.ToLower().IndexOf(Globals.HostMapPath.ToLower()) != -1)
                {
                    strSkinType = "G";
                }
                else
                {
                    strSkinType = "L";
                }
                bool canDeleteSkin = SkinController.CanDeleteSkin(strFolderPath, PortalSettings.HomeDirectoryMapPath);
                if (fallbackSkin || !canDeleteSkin)
                {
                    row = new HtmlTableRow();
                    cell = new HtmlTableCell();
                    cell.ColSpan = 3;
                    cell.Attributes["class"] = "NormalRed";
                    if (type == "Skin")
                    {
                        cell.InnerText = Localization.GetString("CannotDeleteSkin.ErrorMessage", LocalResourceFile);
                    }
                    else
                    {
                        cell.InnerText = Localization.GetString("CannotDeleteContainer.ErrorMessage", LocalResourceFile);
                    }
                    row.Cells.Add(cell);
                    tbl.Rows.Add(row);
                    cmdDelete.Visible = false;
                }
                arrFiles = Directory.GetFiles(strFolderPath, "*.ascx");
                if (arrFiles.Length == 0)
                {
                    row = new HtmlTableRow();
                    cell = new HtmlTableCell();
                    cell.ColSpan = 3;
                    cell.Attributes["class"] = "NormalRed";
                    if (type == "Skin")
                    {
                        cell.InnerText = Localization.GetString("NoSkin.ErrorMessage", LocalResourceFile);
                    }
                    else
                    {
                        cell.InnerText = Localization.GetString("NoContainer.ErrorMessage", LocalResourceFile);
                    }
                    row.Cells.Add(cell);
                    tbl.Rows.Add(row);
                }
                strFolder = strFolderPath.Substring(strFolderPath.LastIndexOf("\\") + 1);
                foreach (string strFile in arrFiles)
                {
                    string file = strFile.ToLower();
                    intIndex += 1;
                    if (intIndex == 4)
                    {
                        intIndex = 1;
                    }
                    if (intIndex == 1)
                    {
                        row = new HtmlTableRow();
                        tbl.Rows.Add(row);
                    }
                    cell = new HtmlTableCell();
                    cell.Align = "center";
                    cell.VAlign = "bottom";
                    cell.Attributes["class"] = "NormalBold";
                    var label = new Label();
                    label.Text = Path.GetFileNameWithoutExtension(file);
                    cell.Controls.Add(label);
                    cell.Controls.Add(new LiteralControl("<br/>"));
                    if (File.Exists(file.Replace(".ascx", ".jpg")))
                    {
                        var imgLink = new HyperLink();
                        strURL = file.Substring(strFile.LastIndexOf("\\portals\\"));
                        imgLink.NavigateUrl = ResolveUrl("~" + strURL.Replace(".ascx", ".jpg"));
                        imgLink.Target = "_new";
                        var img = new System.Web.UI.WebControls.Image();
                        img.ImageUrl = CreateThumbnail(file.Replace(".ascx", ".jpg"));
                        img.BorderWidth = new Unit(1);
                        imgLink.Controls.Add(img);
                        cell.Controls.Add(imgLink);
                    }
                    else
                    {
                        var img = new System.Web.UI.WebControls.Image();
                        img.ImageUrl = ResolveUrl("~/images/thumbnail.jpg");
                        img.BorderWidth = new Unit(1);
                        cell.Controls.Add(img);
                    }
                    cell.Controls.Add(new LiteralControl("<br/>"));
                    strURL = file.Substring(strFile.IndexOf("\\" + strRootSkin + "\\"));
                    strURL.Replace(".ascx", "");
                    var previewLink = new HyperLink();
                    if (type == "Skin")
                    {
                        previewLink.NavigateUrl = Globals.NavigateURL(PortalSettings.HomeTabId,
                                                                      Null.NullString,
                                                                      "SkinSrc=" + "[" + strSkinType + "]" + Globals.QueryStringEncode(strURL.Replace(".ascx", "").Replace("\\", "/")));
                    }
                    else
                    {
                        previewLink.NavigateUrl = Globals.NavigateURL(PortalSettings.HomeTabId,
                                                                      Null.NullString,
                                                                      "ContainerSrc=" + "[" + strSkinType + "]" + Globals.QueryStringEncode(strURL.Replace(".ascx", "").Replace("\\", "/")));
                    }
                    previewLink.CssClass = "CommandButton";
                    previewLink.Target = "_new";
                    previewLink.Text = Localization.GetString("cmdPreview", LocalResourceFile);
                    cell.Controls.Add(previewLink);
                    cell.Controls.Add(new LiteralControl("&nbsp;&nbsp;|&nbsp;&nbsp;"));
                    var applyButton = new LinkButton();
                    applyButton.Text = Localization.GetString("cmdApply", LocalResourceFile);
                    applyButton.CommandName = "Apply" + type;
                    applyButton.CommandArgument = "[" + strSkinType + "]" + strRootSkin + "/" + strFolder + "/" + Path.GetFileName(strFile);
                    applyButton.CssClass = "CommandButton";
                    applyButton.Command += OnCommand;
                    cell.Controls.Add(applyButton);
                    if ((UserInfo.IsSuperUser || strSkinType == "L") && (!fallbackSkin && canDeleteSkin))
                    {
                        cell.Controls.Add(new LiteralControl("&nbsp;&nbsp;|&nbsp;&nbsp;"));
                        var deleteButton = new LinkButton();
                        deleteButton.Text = Localization.GetString("cmdDelete");
                        deleteButton.CommandName = "Delete";
                        deleteButton.CommandArgument = "[" + strSkinType + "]" + strRootSkin + "/" + strFolder + "/" + Path.GetFileName(strFile);
                        deleteButton.CssClass = "CommandButton";
                        deleteButton.Command += OnCommand;
                        cell.Controls.Add(deleteButton);
                    }
                    row.Cells.Add(cell);
                }
                if (File.Exists(strFolderPath + "/" + Globals.glbAboutPage))
                {
                    row = new HtmlTableRow();
                    cell = new HtmlTableCell();
                    cell.ColSpan = 3;
                    cell.Align = "center";
                    cell.BgColor = "#CCCCCC";
                    string strFile = strFolderPath + "/" + Globals.glbAboutPage;
                    strURL = strFile.Substring(strFile.IndexOf("\\portals\\"));
                    var copyrightLink = new HyperLink();
                    copyrightLink.NavigateUrl = ResolveUrl("~" + strURL);
                    copyrightLink.CssClass = "CommandButton";
                    copyrightLink.Target = "_new";
                    copyrightLink.Text = string.Format(Localization.GetString("About", LocalResourceFile), strFolder);
                    cell.Controls.Add(copyrightLink);
                    row.Cells.Add(cell);
                    tbl.Rows.Add(row);
                }
            }
        }

        private void SetContainer(string strContainer)
        {
            if (cboContainers.Items.FindByValue(CurrentContainer) != null)
            {
                cboContainers.Items.FindByValue(CurrentContainer).Selected = false;
            }
            if (cboContainers.Items.FindByValue(strContainer) != null)
            {
                cboContainers.Items.FindByValue(strContainer).Selected = true;
                CurrentContainer = strContainer;
            }
            else
            {
                CurrentContainer = NotSpecified;
            }
        }

        private void SetSkin(string strSkin)
        {
            if (cboSkins.Items.FindByValue(CurrentSkin) != null)
            {
                cboSkins.Items.FindByValue(CurrentSkin).Selected = false;
            }
            if (cboSkins.Items.FindByValue(strSkin) != null)
            {
                cboSkins.Items.FindByValue(strSkin).Selected = true;
                CurrentSkin = strSkin;
            }
            else
            {
                CurrentSkin = NotSpecified;
            }
        }

        private void ShowContainers()
        {
            tblContainers.Rows.Clear();
            int intPortalId = PortalId;
            string strContainerPath = Globals.ApplicationMapPath.ToLower() + cboContainers.SelectedItem.Value;
            if (strContainerPath.ToLowerInvariant().Contains(Globals.HostMapPath.ToLowerInvariant()))
            {
                intPortalId = Null.NullInteger;
            }
            SkinPackageInfo skinPackage = SkinController.GetSkinPackage(intPortalId, cboContainers.SelectedItem.Text, "Container");
            if (skinPackage == null && !lblLegacy.Visible)
            {
                lblLegacy.Visible = (cboContainers.SelectedIndex > 0);
            }
            if (cboContainers.SelectedIndex > 0)
            {
                ProcessSkins(strContainerPath, "Container");
                pnlSkin.Visible = true;
                if (UserInfo.IsSuperUser || strContainerPath.IndexOf(Globals.HostMapPath.ToLower()) == -1)
                {
                    cmdParse.Visible = true;
                    pnlParse.Visible = true;
                }
                else
                {
                    cmdParse.Visible = false;
                    pnlParse.Visible = false;
                }
            }
            else
            {
                pnlSkin.Visible = false;
                pnlParse.Visible = false;
            }
        }

        private void ShowSkins()
        {
            tblSkins.Rows.Clear();
            int intPortalId = PortalId;
            string strSkinPath = Globals.ApplicationMapPath.ToLower() + cboSkins.SelectedItem.Value;
            if (strSkinPath.ToLowerInvariant().Contains(Globals.HostMapPath.ToLowerInvariant()))
            {
                intPortalId = Null.NullInteger;
            }
            SkinPackageInfo skinPackage = SkinController.GetSkinPackage(intPortalId, cboSkins.SelectedItem.Text, "Skin");
            if (skinPackage == null)
            {
                lblLegacy.Visible = (cboSkins.SelectedIndex > 0);
            }
            if (cboSkins.SelectedIndex > 0)
            {
                ProcessSkins(strSkinPath, "Skin");
                pnlSkin.Visible = true;
                if (UserInfo.IsSuperUser || strSkinPath.IndexOf(Globals.HostMapPath.ToLower()) == -1)
                {
                    cmdParse.Visible = true;
                    pnlParse.Visible = true;
                }
                else
                {
                    cmdParse.Visible = false;
                    pnlParse.Visible = false;
                }
            }
            else
            {
                pnlSkin.Visible = false;
                pnlParse.Visible = false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            chkHost.CheckedChanged += chkHost_CheckedChanged;
            chkSite.CheckedChanged += chkSite_CheckedChanged;
            cmdDelete.Click += cmdDelete_Click;
            cmdParse.Click += cmdParse_Click;
            cmdRestore.Click += cmdRestore_Click;

            string strSkin = Null.NullString;
            string strContainer = Null.NullString;
            try
            {
                cmdDelete.Visible = true;
                if (Page.IsPostBack == false)
                {
                    LoadCombos();
                }
                if (PortalSettings.ActiveTab.IsSuperTab)
                {
                    typeRow.Visible = false;
                }
                else
                {
                    typeRow.Visible = true;
                }
                if (!Page.IsPostBack)
                {
                    string strURL;
                    if (Request.QueryString["Name"] != null)
                    {
                        strURL =
                            Request.MapPath(GetSkinPath(Convert.ToString(Request.QueryString["Type"]), Convert.ToString(Request.QueryString["Root"]), Convert.ToString(Request.QueryString["Name"])));
                        strSkin = strURL.Replace(Globals.ApplicationMapPath, "").ToLowerInvariant();
                    }
                    else
                    {
                        string skinSrc;
                        if (!string.IsNullOrEmpty(PortalSettings.DefaultPortalSkin))
                        {
                            skinSrc = PortalSettings.DefaultPortalSkin;
                        }
                        else
                        {
                            skinSrc = SkinController.GetDefaultPortalSkin();
                        }
                        strURL = Request.MapPath(SkinController.FormatSkinPath(SkinController.FormatSkinSrc(skinSrc, PortalSettings)));
                        strURL = strURL.Substring(0, strURL.LastIndexOf("\\"));
                        strSkin = strURL.Replace(Globals.ApplicationMapPath, "").ToLowerInvariant();
                    }
                    if (!string.IsNullOrEmpty(strSkin))
                    {
                        strContainer = strSkin.Replace("\\" + SkinController.RootSkin.ToLowerInvariant() + "\\", "\\" + SkinController.RootContainer.ToLowerInvariant() + "\\");
                    }
                    SetSkin(strSkin);
                    SetContainer(strContainer);
                }
                else
                {
                    strSkin = cboSkins.SelectedValue;
                    strContainer = cboContainers.SelectedValue;
                    if (strSkin != CurrentSkin)
                    {
                        strContainer = strSkin.Replace("\\" + SkinController.RootSkin.ToLowerInvariant() + "\\", "\\" + SkinController.RootContainer.ToLowerInvariant() + "\\");
                        SetSkin(strSkin);
                        SetContainer(strContainer);
                    }
                    else if (strContainer != CurrentContainer)
                    {
                        SetSkin(NotSpecified);
                        SetContainer(strContainer);
                    }
                }
                ShowSkins();
                ShowContainers();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void chkHost_CheckedChanged(object sender, EventArgs e)
        {
            LoadCombos();
            ShowSkins();
            ShowContainers();
        }

        private void chkSite_CheckedChanged(object sender, EventArgs e)
        {
            LoadCombos();
            ShowSkins();
            ShowContainers();
        }

        private void OnCommand(Object sender, CommandEventArgs e)
        {
            try
            {
                string strSrc = e.CommandArgument.ToString();
                string redirectUrl = Globals.NavigateURL(TabId);
                switch (e.CommandName)
                {
                    case "ApplyContainer":
                        if (chkPortal.Checked)
                        {
                            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Portal, strSrc);
                        }
                        if (chkAdmin.Checked)
                        {
                            SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Admin, strSrc);
                        }
                        break;
                    case "ApplySkin":
                        if (chkPortal.Checked)
                        {
                            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Portal, strSrc);
                        }
                        if (chkAdmin.Checked)
                        {
                            SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Admin, strSrc);
                        }
                        DataCache.ClearPortalCache(PortalId, true);
                        break;
                    case "Delete":
                        File.Delete(Request.MapPath(SkinController.FormatSkinSrc(strSrc, PortalSettings)));
                        DataCache.ClearPortalCache(PortalId, true);
                        string strType = "G";
                        if (strSrc.StartsWith("[L]"))
                        {
                            strType = "L";
                        }
                        string strRoot = strSrc.Substring(3, strSrc.IndexOf("/") - 3);
                        string strFolder = strSrc.Substring(strSrc.IndexOf("/") + 1, strSrc.LastIndexOf("/") - strSrc.IndexOf("/") - 2);
                        redirectUrl = Globals.NavigateURL(TabId, "", "Type=" + strType, "Root=" + strRoot, "Name=" + strFolder);
                        break;
                }
                Response.Redirect(redirectUrl, true);
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            bool failure = false;
            string strSkinPath = Globals.ApplicationMapPath.ToLower() + cboSkins.SelectedItem.Value;
            string strContainerPath = Globals.ApplicationMapPath.ToLower() + cboContainers.SelectedItem.Value;
            string strMessage;
            if (UserInfo.IsSuperUser == false && cboSkins.SelectedItem.Value.IndexOf("\\portals\\_default\\", 0) != -1)
            {
                strMessage = Localization.GetString("SkinDeleteFailure", LocalResourceFile);
                UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                failure = true;
            }
            else
            {
                if (cboSkins.SelectedIndex > 0)
                {
                    SkinPackageInfo skinPackage = SkinController.GetSkinPackage(PortalId, cboSkins.SelectedItem.Text, "Skin");
                    if (skinPackage != null)
                    {
                        strMessage = Localization.GetString("UsePackageUnInstall", LocalResourceFile);
                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                        return;
                    }
                    else
                    {
                        if (Directory.Exists(strSkinPath))
                        {
                            Globals.DeleteFolderRecursive(strSkinPath);
                        }
                        if (Directory.Exists(strSkinPath.Replace("\\" + SkinController.RootSkin.ToLower() + "\\", "\\" + SkinController.RootContainer + "\\")))
                        {
                            Globals.DeleteFolderRecursive(strSkinPath.Replace("\\" + SkinController.RootSkin.ToLower() + "\\", "\\" + SkinController.RootContainer + "\\"));
                        }
                    }
                }
                else if (cboContainers.SelectedIndex > 0)
                {
                    SkinPackageInfo skinPackage = SkinController.GetSkinPackage(PortalId, cboContainers.SelectedItem.Text, "Container");
                    if (skinPackage != null)
                    {
                        strMessage = Localization.GetString("UsePackageUnInstall", LocalResourceFile);
                        UI.Skins.Skin.AddModuleMessage(this, strMessage, ModuleMessage.ModuleMessageType.RedError);
                        return;
                    }
                    else if (Directory.Exists(strContainerPath))
                    {
                        Globals.DeleteFolderRecursive(strContainerPath);
                    }
                }
            }
            if (!failure)
            {
                LoadCombos();
                ShowSkins();
                ShowContainers();
            }
        }

        private void cmdParse_Click(object sender, EventArgs e)
        {
            string strFolder;
            string strType;
            string strRoot;
            string strName;
            string strSkinPath = Globals.ApplicationMapPath.ToLower() + cboSkins.SelectedItem.Value;
            string strContainerPath = Globals.ApplicationMapPath.ToLower() + cboContainers.SelectedItem.Value;
            string strParse = "";
            if (cboSkins.SelectedIndex > 0)
            {
                strFolder = strSkinPath;
                if (strFolder.IndexOf(Globals.HostMapPath.ToLower()) != -1)
                {
                    strType = "G";
                }
                else
                {
                    strType = "L";
                }
                strRoot = SkinController.RootSkin;
                strName = cboSkins.SelectedItem.Text;
                strParse += ParseSkinPackage(strType, strRoot, strName, strFolder, optParse.SelectedItem.Value);
                strFolder = strSkinPath.Replace("\\" + SkinController.RootSkin.ToLower() + "\\", "\\" + SkinController.RootContainer.ToLower() + "\\");
                strRoot = SkinController.RootContainer;
                strParse += ParseSkinPackage(strType, strRoot, strName, strFolder, optParse.SelectedItem.Value);
                DataCache.ClearPortalCache(PortalId, true);
            }
            if (cboContainers.SelectedIndex > 0)
            {
                strFolder = strContainerPath;
                if (strFolder.IndexOf(Globals.HostMapPath.ToLower()) != -1)
                {
                    strType = "G";
                }
                else
                {
                    strType = "L";
                }
                strRoot = SkinController.RootContainer;
                strName = cboContainers.SelectedItem.Text;
                strParse += ParseSkinPackage(strType, strRoot, strName, strFolder, optParse.SelectedItem.Value);
                DataCache.ClearPortalCache(PortalId, true);
            }
            lblOutput.Text = strParse;
            if (cboSkins.SelectedIndex > 0)
            {
                ShowSkins();
            }
            else if (cboContainers.SelectedIndex > 0)
            {
                ShowContainers();
            }
        }

        private void cmdRestore_Click(object sender, EventArgs e)
        {
            var objSkins = new SkinController();
            if (chkPortal.Checked)
            {
                SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Portal, "");
                SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Portal, "");
            }
            if (chkAdmin.Checked)
            {
                SkinController.SetSkin(SkinController.RootSkin, PortalId, SkinType.Admin, "");
                SkinController.SetSkin(SkinController.RootContainer, PortalId, SkinType.Admin, "");
            }
            DataCache.ClearPortalCache(PortalId, true);
            Response.Redirect(Request.RawUrl);
        }
    }
}