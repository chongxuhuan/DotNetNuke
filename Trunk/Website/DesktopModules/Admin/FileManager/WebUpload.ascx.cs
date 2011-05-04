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
using System.Web;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;

#endregion

namespace DotNetNuke.Modules.Admin.FileManager
{
    public partial class WebUpload : PortalModuleBase
    {
        private string _DestinationFolder;
        private UploadType _FileType;
        private string _FileTypeName;
        private string _RootFolder;
        private string _UploadRoles;

        public string DestinationFolder
        {
            get
            {
                if (_DestinationFolder == null)
                {
                    _DestinationFolder = string.Empty;
                    if ((Request.QueryString["dest"] != null))
                    {
                        _DestinationFolder = Globals.QueryStringDecode(Request.QueryString["dest"]);
                    }
                }
                return PathUtils.Instance.RemoveTrailingSlash(_DestinationFolder.Replace("\\", "/"));
            }
        }

        public UploadType FileType
        {
            get
            {
                _FileType = UploadType.File;
                if ((Request.QueryString["ftype"] != null))
                {
                    switch (Request.QueryString["ftype"].ToLower())
                    {
                        case "file":
                        case "container":
                        case "skin":
                            _FileType = (UploadType) Enum.Parse(typeof (UploadType), Request.QueryString["ftype"]);
                            break;
                    }
                }
                return _FileType;
            }
        }

        public string FileTypeName
        {
            get
            {
                if (_FileTypeName == null)
                {
                    _FileTypeName = Localization.GetString(FileType.ToString(), LocalResourceFile);
                }
                return _FileTypeName;
            }
        }

        public int FolderPortalID
        {
            get
            {
                if (IsHostMenu)
                {
                    return Null.NullInteger;
                }
                else
                {
                    return PortalId;
                }
            }
        }

        public string RootFolder
        {
            get
            {
                if (_RootFolder == null)
                {
                    if (IsHostMenu)
                    {
                        _RootFolder = Globals.HostMapPath;
                    }
                    else
                    {
                        _RootFolder = PortalSettings.HomeDirectoryMapPath;
                    }
                }
                return _RootFolder;
            }
        }

        public string UploadRoles
        {
            get
            {
                if (_UploadRoles == null)
                {
                    _UploadRoles = string.Empty;
                    var objModules = new ModuleController();
                    ModuleInfo ModInfo;
                    if (IsHostMenu)
                    {
                        ModInfo = objModules.GetModuleByDefinition(Null.NullInteger, "File Manager");
                    }
                    else
                    {
                        ModInfo = objModules.GetModuleByDefinition(PortalId, "File Manager");
                    }
                    Hashtable settings = new ModuleController().GetModuleSettings(ModInfo.ModuleID);
                    if (Convert.ToString(settings["uploadroles"]) != null)
                    {
                        _UploadRoles = Convert.ToString(settings["uploadroles"]);
                    }
                }
                return _UploadRoles;
            }
        }

        private void CheckSecurity()
        {
            if (!IsEditable)
            {
                Response.Redirect(Globals.NavigateURL("Access Denied"), true);
            }
        }

        private void LoadFolders()
        {
            ddlFolders.Items.Clear();
            
            var user = UserController.GetCurrentUserInfo();

            var folders = FolderManager.Instance.GetFolders(user, "ADD", true, true);

            foreach (FolderInfo folder in folders)
            {
                var FolderItem = new ListItem();
                if (folder.FolderPath == Null.NullString)
                {
                    if (IsHostMenu)
                    {
                        FolderItem.Text = Localization.GetString("HostRoot", LocalResourceFile);
                    }
                    else
                    {
                        FolderItem.Text = Localization.GetString("PortalRoot", LocalResourceFile);
                    }
                }
                else
                {
                    FolderItem.Text = PathUtils.Instance.RemoveTrailingSlash(folder.DisplayPath);
                }
                FolderItem.Value = folder.FolderPath;
                ddlFolders.Items.Add(FolderItem);
            }
            
            if (!String.IsNullOrEmpty(DestinationFolder))
            {
                if (ddlFolders.Items.FindByText(DestinationFolder) != null)
                {
                    ddlFolders.Items.FindByText(DestinationFolder).Selected = true;
                }
            }
        }

        public string ReturnURL()
        {
            int TabID = PortalSettings.HomeTabId;
            if (Request.Params["rtab"] != null)
            {
                TabID = int.Parse(Request.Params["rtab"]);
            }
            return Globals.NavigateURL(TabID);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ModuleConfiguration.ModuleTitle = Localization.GetString("UploadType" + FileType, LocalResourceFile);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdAdd.Click += cmdAdd_Click;
            cmdReturn1.Click += cmdReturn_Click;
            cmdReturn2.Click += cmdReturn_Click;

            try
            {
                CheckSecurity();
                string strHost = Localization.GetString("HostRoot", LocalResourceFile);
                string strPortal = Localization.GetString("PortalRoot", LocalResourceFile);
                if (!Page.IsPostBack)
                {
                    cmdAdd.Text = Localization.GetString("UploadType" + FileType, LocalResourceFile);
                    if (FileType == UploadType.File)
                    {
                        trFolders.Visible = true;
                        trRoot.Visible = true;
                        trUnzip.Visible = true;
                        if (IsHostMenu)
                        {
                            lblRootType.Text = strHost + ":";
                            lblRootFolder.Text = RootFolder;
                        }
                        else
                        {
                            lblRootType.Text = strPortal + ":";
                            lblRootFolder.Text = RootFolder;
                        }
                        LoadFolders();
                    }
                    chkUnzip.Checked = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdAdd_Click(object sender, EventArgs e)
        {
            try
            {
                CheckSecurity();
                var strMessage = "";
                var postedFile = cmdBrowse.PostedFile;
                var strInvalid = Localization.GetString("InvalidExt", LocalResourceFile);
                var strFileName = Path.GetFileName(postedFile.FileName);
                var strExtension = Path.GetExtension(strFileName);
                if (!String.IsNullOrEmpty(postedFile.FileName))
                {
                    switch (FileType)
                    {
                        case UploadType.File:
                            try
                            {
                                var folder = FolderManager.Instance.GetFolder(PortalId, ddlFolders.SelectedValue);
                                var fileManager = Services.FileSystem.FileManager.Instance;
                                var file = fileManager.AddFile(folder, strFileName, postedFile.InputStream, true, true, postedFile.ContentType);
                                if (chkUnzip.Checked && file.Extension == "zip")
                                {
                                    fileManager.UnzipFile(file, folder);
                                }
                            }
                            catch (SecurityException exc)
                            {
                                DnnLog.Error(exc);

                                strMessage += "<br />" + string.Format(Localization.GetString("InsufficientFolderPermission"), ddlFolders.SelectedValue);
                            }
                            catch (NoSpaceAvailableException exc)
                            {
                                DnnLog.Error(exc);

                                strMessage += "<br />" + string.Format(Localization.GetString("DiskSpaceExceeded"), strFileName);
                            }
                            catch (InvalidFileExtensionException exc)
                            {
                                DnnLog.Error(exc);
                                strMessage += "<br />" + string.Format(Localization.GetString("RestrictedFileType"), strFileName, Host.AllowedExtensionWhitelist.ToDisplayString());
                            }
                            catch (Exception exc)
                            {
                                DnnLog.Error(exc);
                                strMessage += "<br />" + string.Format(Localization.GetString("SaveFileError"), strFileName);
                            }
                            break;
                        case UploadType.Skin:
                            if (strExtension.ToLower() == ".zip")
                            {
                                var objLbl = new Label();
                                objLbl.CssClass = "Normal";
                                objLbl.Text = SkinController.UploadLegacySkin(RootFolder, SkinController.RootSkin, Path.GetFileNameWithoutExtension(postedFile.FileName), postedFile.InputStream);
                                phPaLogs.Controls.Add(objLbl);
                            }
                            else
                            {
                                strMessage += strInvalid + " " + FileTypeName + " " + strFileName;
                            }
                            break;
                        case UploadType.Container:
                            if (strExtension.ToLower() == ".zip")
                            {
                                var objLbl = new Label();
                                objLbl.CssClass = "Normal";
                                objLbl.Text = SkinController.UploadLegacySkin(RootFolder, SkinController.RootContainer, Path.GetFileNameWithoutExtension(postedFile.FileName), postedFile.InputStream);
                                phPaLogs.Controls.Add(objLbl);
                            }
                            else
                            {
                                strMessage += strInvalid + " " + FileTypeName + " " + strFileName;
                            }
                            break;
                    }
                }
                else
                {
                    strMessage = Localization.GetString("NoFile", LocalResourceFile);
                }
                if (phPaLogs.Controls.Count > 0)
                {
                    tblLogs.Visible = true;
                }
                else if (String.IsNullOrEmpty(strMessage))
                {
                    UI.Skins.Skin.AddModuleMessage(this, String.Format(Localization.GetString("FileUploadSuccess", LocalResourceFile), strFileName), ModuleMessage.ModuleMessageType.GreenSuccess);
                }
                else
                {
                    lblMessage.Text = strMessage;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void cmdReturn_Click(Object sender, EventArgs e)
        {
            Response.Redirect(ReturnURL(), true);
        }
    }
}