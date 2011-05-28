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
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Instrumentation;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

using FileInfo = DotNetNuke.Services.FileSystem.FileInfo;
using DotNetNuke.Entities.Host;


#endregion

namespace DotNetNuke.Web.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    ///   The FilePicker Class provides a File Picker Control for DotNetNuke
    /// </summary>
    /// <history>
    ///   [cnurse]	07/31/2006
    /// </history>
    /// -----------------------------------------------------------------------------
    public class DnnFilePicker : CompositeControl, ILocalizable
    {
        #region "Public Enums"

        /// <summary>
        ///   Represents a possible mode for the File Control
        /// </summary>
        protected enum FileControlMode
        {
            /// <summary>
            ///   The File Control is in its Normal mode
            /// </summary>
            Normal,

            /// <summary>
            ///   The File Control is in the Upload File mode
            /// </summary>
            UpLoadFile,

            /// <summary>
            ///   The File Control is in the Preview mode
            /// </summary>
            Preview
        }

        #endregion

        #region "Controls"
		
		//Files
        private DropDownList cboFiles;
        private DropDownList cboFolders;
        private CommandButton cmdCancel;
        private CommandButton cmdSave;
        private CommandButton cmdUpload;
        private HtmlTableCell commandCell;
		//Command Row
        private HtmlTableRow commandRow;
        private HtmlTableCell fileCell;
        private HtmlTableRow fileRow;
        private HtmlTable fileTable;
        private HtmlTableCell folderCell;
        private HtmlTableRow folderRow;
        private Image imgPreview;
        private Label lblFile;
        private Label lblFolder;

        private Label lblMessage;
        private HtmlTableCell messageCell;
		//messages
        private HtmlTableRow messageRow;
        private HtmlTableCell preViewCell;
        private HtmlInputFile txtFile;

        #endregion

        private bool _Localize = true;
        private int _MaxHeight = 100;

        private int _MaxWidth = 135;

        #region "Protected Properties"

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets whether the control is on a Host or Portal Tab
        /// </summary>
        /// <value>A Boolean</value>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected bool IsHost
        {
            get
            {
                bool _IsHost = Null.NullBoolean;
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    _IsHost = true;
                }
                return _IsHost;
            }
        }

        public int MaxHeight
        {
            get
            {
                return _MaxHeight;
            }
            set
            {
                _MaxHeight = value;
            }
        }

        public int MaxWidth
        {
            get
            {
                return _MaxWidth;
            }
            set
            {
                _MaxWidth = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the current mode of the control
        /// </summary>
        /// <remarks>
        ///   Defaults to FileControlMode.Normal
        /// </remarks>
        /// <value>A FileControlMode enum</value>
        /// <history>
        ///   [cnurse]	7/13/2005  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected FileControlMode Mode
        {
            get
            {
                if (ViewState["Mode"] == null)
                {
                    return FileControlMode.Normal;
                }
                else
                {
                    return (FileControlMode) ViewState["Mode"];
                }
            }
            set
            {
                ViewState["Mode"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets the root folder for the control
        /// </summary>
        /// <value>A String</value>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected string ParentFolder
        {
            get
            {
                string _ParentFolder = null;
                if (IsHost)
                {
                    _ParentFolder = Globals.HostMapPath;
                }
                else
                {
                    _ParentFolder = PortalSettings.HomeDirectoryMapPath;
                }
                return _ParentFolder;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the file PortalId to use
        /// </summary>
        /// <remarks>
        ///   Defaults to PortalSettings.PortalId
        /// </remarks>
        /// <value>An Integer</value>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected int PortalId
        {
            get
            {
                int _PortalId = Null.NullInteger;
                if (!IsHost)
                {
                    _PortalId = PortalSettings.PortalId;
                }
                return _PortalId;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets the current Portal Settings
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected PortalSettings PortalSettings
        {
            get
            {
                return PortalController.GetCurrentPortalSettings();
            }
        }

        #endregion

        #region "Public Properties"

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the class to be used for the Labels
        /// </summary>
        /// <remarks>
        ///   Defaults to 'CommandButton'
        /// </remarks>
        /// <value>A String</value>
        /// <history>
        ///   [cnurse]	7/13/2005  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string CommandCssClass
        {
            get
            {
                string _Class = Convert.ToString(ViewState["CommandCssClass"]);
                if (string.IsNullOrEmpty(_Class))
                {
                    return "CommandButton";
                }
                else
                {
                    return _Class;
                }
            }
            set
            {
                ViewState["CommandCssClass"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the file Filter to use
        /// </summary>
        /// <remarks>
        ///   Defaults to ''
        /// </remarks>
        /// <value>a comma seperated list of file extenstions no wildcards or periods e.g. "jpg,png,gif"</value>
        /// -----------------------------------------------------------------------------
        public string FileFilter
        {
            get
            {
                if (ViewState["FileFilter"] != null)
                {
                    return (string) ViewState["FileFilter"];
                }
                else
                {
                    return "";
                }
            }
            set
            {
                ViewState["FileFilter"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the FileID for the control
        /// </summary>
        /// <value>An Integer</value>
        /// <history>
        ///   [cnurse]	7/13/2005  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public int FileID
        {
            get
            {
                EnsureChildControls();
                if (ViewState["FileID"] == null)
                {
                    //Get FileId from the file combo
                    int _FileId = Null.NullInteger;
                    if (cboFiles.SelectedItem != null)
                    {
                        _FileId = Int32.Parse(cboFiles.SelectedItem.Value);
                    }
                    ViewState["FileID"] = _FileId;
                }
                return Convert.ToInt32(ViewState["FileID"]);
            }
            set
            {
                EnsureChildControls();
                ViewState["FileID"] = value;
                if (string.IsNullOrEmpty(FilePath))
                {
                    var fileInfo = FileManager.Instance.GetFile(value);
                    if (fileInfo != null)
                    {
                        SetFilePath(fileInfo.Folder + fileInfo.FileName);
                    }
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the FilePath for the control
        /// </summary>
        /// <value>A String</value>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string FilePath
        {
            get
            {
                return Convert.ToString(ViewState["FilePath"]);
            }
            set
            {
                ViewState["FilePath"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets whether to Include Personal Folder
        /// </summary>
        /// <remarks>
        ///   Defaults to false
        /// </remarks>
        /// <value>A Boolean</value>
        /// -----------------------------------------------------------------------------
        public bool UsePersonalFolder
        {
            get
            {
                if (ViewState["UsePersonalFolder"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(ViewState["UsePersonalFolder"]);
                }
            }
            set
            {
                ViewState["UsePersonalFolder"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets the class to be used for the Labels
        /// </summary>
        /// <remarks>
        ///   Defaults to 'NormalBold'
        /// </remarks>
        /// <value>A String</value>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public string LabelCssClass
        {
            get
            {
                string _Class = Convert.ToString(ViewState["LabelCssClass"]);
                if (string.IsNullOrEmpty(_Class))
                {
                    return "NormalBold";
                }
                else
                {
                    return _Class;
                }
            }
            set
            {
                ViewState["LabelCssClass"] = value;
            }
        }

        public string Permissions
        {
            get
            {
                string _Permissions = Convert.ToString(ViewState["Permissions"]);
                if (string.IsNullOrEmpty(_Permissions))
                {
                    return "BROWSE,ADD";
                }
                else
                {
                    return _Permissions;
                }
            }
            set
            {
                ViewState["Permissions"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets whether the combos have a "Not Specified" option
        /// </summary>
        /// <remarks>
        ///   Defaults to True (ie no "Not Specified")
        /// </remarks>
        /// <value>A Boolean</value>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool Required
        {
            get
            {
                if (ViewState["Required"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(ViewState["Required"]);
                }
            }
            set
            {
                ViewState["Required"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets whether to Show Database Folders
        /// </summary>
        /// <remarks>
        ///   Defaults to false
        /// </remarks>
        /// <value>A Boolean</value>
        /// <history>
        ///   [cnurse]	7/31/2005  created
        /// </history>
        /// -----------------------------------------------------------------------------
        [Obsolete("Deprecated in DNN 6.0")]
        public bool ShowDatabase
        {
            get
            {
                if (ViewState["ShowDatabase"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(ViewState["ShowDatabase"]);
                }
            }
            set
            {
                ViewState["ShowDatabase"] = value;
            }
        }

        public bool ShowFolders
        {
            get
            {
                if (ViewState["ShowFolders"] == null)
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(ViewState["ShowFolders"]);
                }
            }
            set
            {
                ViewState["ShowFolders"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets whether to Show Secure Folders
        /// </summary>
        /// <remarks>
        ///   Defaults to false
        /// </remarks>
        /// <value>A Boolean</value>
        /// <history>
        ///   [cnurse]	7/31/2005  created
        /// </history>
        /// -----------------------------------------------------------------------------
        [Obsolete("Deprecated in DNN 6.0")]
        public bool ShowSecure
        {
            get
            {
                if (ViewState["ShowSecure"] == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(ViewState["ShowSecure"]);
                }
            }
            set
            {
                ViewState["ShowSecure"] = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets or sets whether to Show the Upload Button
        /// </summary>
        /// <remarks>
        ///   Defaults to True
        /// </remarks>
        /// <value>A Boolean</value>
        /// <history>
        ///   [cnurse]	7/31/2005  created
        /// </history>
        /// -----------------------------------------------------------------------------
        public bool ShowUpLoad
        {
            get
            {
                if (ViewState["ShowUpLoad"] == null)
                {
                    return true;
                }
                else
                {
                    return Convert.ToBoolean(ViewState["ShowUpLoad"]);
                }
            }
            set
            {
                ViewState["ShowUpLoad"] = value;
            }
        }

        #endregion

        #region "Private Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddButton adds a button to the Command Row
        /// </summary>
        /// <param name = "button">The button to add to the Row</param>
        /// <param name = "imageUrl">The url to the image for the button</param>
        /// <history>
        ///   [cnurse]	07/31/2006 created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void AddButton(ref CommandButton button, string imageUrl)
        {
            button = new CommandButton();
            button.EnableViewState = false;
            button.CausesValidation = false;
            button.ControlStyle.CssClass = CommandCssClass;
            if (!string.IsNullOrEmpty(imageUrl))
            {
                button.DisplayIcon = true;
                button.ImageUrl = imageUrl;
            }
            button.Visible = false;
            commandCell.Controls.Add(button);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddCommandRow adds the Command Row
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void AddCommandRow()
        {
            //Create Command Row
            commandRow = new HtmlTableRow();
            commandRow.Visible = false;
            commandCell = new HtmlTableCell();
            AddButton(ref cmdUpload, "~/images/up.gif");
            cmdUpload.Click += UploadFile;
            AddButton(ref cmdSave, "~/images/save.gif");
            cmdSave.Click += SaveFile;

            //Add separator
            commandCell.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));

            AddButton(ref cmdCancel, "~/images/lt.gif");
            cmdCancel.Click += CancelUpload;

            commandRow.Cells.Add(commandCell);
            fileTable.Rows.Add(commandRow);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddFileRow adds the Files Row
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void AddFileRow()
        {
            //Create Url Row
            fileRow = new HtmlTableRow();
            fileCell = new HtmlTableCell();

            //Create File Label
            lblFile = new Label();
            lblFile.EnableViewState = false;
            fileCell.Controls.Add(lblFile);

            //Add <br>
            fileCell.Controls.Add(new LiteralControl("<br/>"));

            //Create Files Combo
            cboFiles = new DropDownList();
            cboFiles.ID = "File";
            cboFiles.DataTextField = "Text";
            cboFiles.DataValueField = "Value";
            cboFiles.AutoPostBack = true;
            cboFiles.SelectedIndexChanged += FileChanged;
            fileCell.Controls.Add(cboFiles);

            //Create Upload Box
            txtFile = new HtmlInputFile();
            fileCell.Controls.Add(txtFile);

            //Add Cell/row/table
            fileRow.Cells.Add(fileCell);
            fileTable.Rows.Add(fileRow);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddFolderRow adds the Folders Row
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void AddFolderRow()
        {
            //Create Url Row
            folderRow = new HtmlTableRow();
            folderCell = new HtmlTableCell();

            //Create Folder Label
            lblFolder = new Label();
            lblFolder.EnableViewState = false;
            folderCell.Controls.Add(lblFolder);

            //Add <br>
            folderCell.Controls.Add(new LiteralControl("<br/>"));

            //Create Folders Combo
            cboFolders = new DropDownList();
            cboFolders.ID = "Folder";
            cboFolders.AutoPostBack = true;
            cboFolders.SelectedIndexChanged += FolderChanged;
            folderCell.Controls.Add(cboFolders);

            //Load Folders
            LoadFolders();

            //Add Preview
            preViewCell = new HtmlTableCell();
            preViewCell.VAlign = "top";
            preViewCell.RowSpan = 3;

            imgPreview = new Image();
            preViewCell.Controls.Add(imgPreview);

            //Add Cell/row/table
            folderRow.Cells.Add(folderCell);
            folderRow.Cells.Add(preViewCell);
            fileTable.Rows.Add(folderRow);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   AddMessageRow adds the Message Row
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void AddMessageRow()
        {
            //Create Type Row
            messageRow = new HtmlTableRow();
            messageCell = new HtmlTableCell();
            messageCell.ColSpan = 2;

            //Create Label
            lblMessage = new Label();
            lblMessage.EnableViewState = false;
            lblMessage.CssClass = "NormalRed";
            lblMessage.Text = "";
            messageCell.Controls.Add(lblMessage);

            //Add to Table
            messageRow.Cells.Add(messageCell);
            fileTable.Rows.Add(messageRow);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   GetFileList fetches the list of files to display in the File combo
        /// </summary>
        /// <param name = "NoneSpecified">A flag indicating whether the NoneSpecified item is 
        ///   shown in the list</param>
        /// <param name = "Folder">The folder to fetch the list of files</param>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private ArrayList GetFileList(bool NoneSpecified, string Folder)
        {
            ArrayList fileList = null;

            if (IsHost)
            {
                fileList = Globals.GetFileList(Null.NullInteger, FileFilter, NoneSpecified, cboFolders.SelectedItem.Value);
            }
            else
            {
                fileList = Globals.GetFileList(PortalId, FileFilter, NoneSpecified, cboFolders.SelectedItem.Value);
            }

            return fileList;
        }

        private bool IsUserFolder(string folderPath)
        {
            return (folderPath.ToLowerInvariant().StartsWith("users/") && folderPath.EndsWith(string.Format("/{0}/", UserController.GetCurrentUserInfo().UserID)));
        }

        private void LoadFiles()
        {
            cboFiles.DataSource = GetFileList(!Required, cboFolders.SelectedItem.Value);
            cboFiles.DataBind();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   LoadFolders fetches the list of folders from the Database
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void LoadFolders()
        {
            cboFolders.Items.Clear();

            //Add Personal Folder
            if (UsePersonalFolder)
            {
                string userFolder = PathUtils.Instance.GetUserFolderPath(UserController.GetCurrentUserInfo());
                ListItem userFolderItem = cboFolders.Items.FindByValue(userFolder);
                if (userFolderItem != null)
                {
                    userFolderItem.Text = Utilities.GetLocalizedString("MyFolder");
                }
                else
                {
                    //Add DummyFolder
                    cboFolders.Items.Add(new ListItem(Utilities.GetLocalizedString("MyFolder"), userFolder));
                }
            }
            else
            {
                var folders = FolderManager.Instance.GetFolders(UserController.GetCurrentUserInfo(), "READ,ADD");
                foreach (FolderInfo folder in folders)
                {
                    ListItem folderItem = new ListItem();
                    if (folder.FolderPath == Null.NullString)
                    {
                        folderItem.Text = Utilities.GetLocalizedString("PortalRoot");
                    }
                    else
                    {
                        folderItem.Text = folder.DisplayPath;
                    }
                    folderItem.Value = folder.FolderPath;
                    cboFolders.Items.Add(folderItem);
                }
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   SetFilePath sets the FilePath property
        /// </summary>
        /// <remarks>
        ///   This overload uses the selected item in the Folder combo
        /// </remarks>
        /// <history>
        ///   [cnurse]	08/01/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void SetFilePath()
        {
            SetFilePath(cboFiles.SelectedItem.Text);
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   SetFilePath sets the FilePath property
        /// </summary>
        /// <remarks>
        ///   This overload allows the caller to specify a file
        /// </remarks>
        /// <param name = "fileName">The filename to use in setting the property</param>
        /// <history>
        ///   [cnurse]	08/01/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void SetFilePath(string fileName)
        {
            if (string.IsNullOrEmpty(cboFolders.SelectedItem.Value))
            {
                FilePath = fileName;
            }
            else
            {
                FilePath = (cboFolders.SelectedItem.Value + "/") + fileName;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   ShowButton configures and displays a button
        /// </summary>
        /// <param name = "button">The button to configure</param>
        /// <param name = "command">The command name (amd key) of the button</param>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ShowButton(CommandButton button, string command)
        {
            button.Visible = true;
            if (!string.IsNullOrEmpty(command))
            {
                button.Text = Utilities.GetLocalizedString(command);
            }
            button.RegisterForPostback();
            commandRow.Visible = true;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   ShowImage displays the Preview Image
        /// </summary>
        /// <history>
        ///   [cnurse]	08/01/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ShowImage()
        {
            var image = (FileInfo)FileManager.Instance.GetFile(FileID);

            if (image != null)
            {
                imgPreview.ImageUrl = Globals.LinkClick("fileid=" + FileID, PortalSettings.ActiveTab.TabID, Null.NullInteger);

                try
                {
                    Utilities.CreateThumbnail(image, imgPreview, MaxWidth, MaxHeight);
                }
                catch (Exception)
                {
                    DnnLog.Warn("Unable to create thumbnail for {0}", image.PhysicalPath);
                    imgPreview.Visible = false;
                }

                imgPreview.Visible = true;
            }
            else
            {
                imgPreview.Visible = false;
            }
        }

        #endregion

        #region "Protected Methods"

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            LocalResourceFile = Utilities.GetLocalResourceFile(this);
            EnsureChildControls();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   CreateChildControls overrides the Base class's method to correctly build the
        ///   control based on the configuration
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void CreateChildControls()
        {
            //First clear the controls collection
            Controls.Clear();

            //Create Table
            fileTable = new HtmlTable();

            AddFolderRow();
            AddFileRow();

            AddCommandRow();
            AddMessageRow();

            //Add table to Control
            Controls.Add(fileTable);

            //Call base class's method

            base.CreateChildControls();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   OnPreRender runs just before the control is rendered
        /// </summary>
        /// <history>
        ///   [cnurse]	07/31/2006  created
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (cboFolders.Items.Count > 0)
            {
                //Configure Labels
                lblFolder.Text = Utilities.GetLocalizedString("Folder");
                lblFolder.CssClass = LabelCssClass;
                lblFile.Text = Utilities.GetLocalizedString("File");
                lblFile.CssClass = LabelCssClass;

                //select folder
                string fileName = null;
                string folderPath = null;
                if (!string.IsNullOrEmpty(FilePath))
                {
                    fileName = FilePath.Substring(FilePath.LastIndexOf("/") + 1);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        folderPath = FilePath;
                    }
                    else
                    {
                        folderPath = FilePath.Replace(fileName, "");
                    }
                }
                else
                {
                    fileName = FilePath;
                    folderPath = string.Empty;
                }

                if (cboFolders.Items.FindByValue(folderPath) != null)
                {
                    cboFolders.SelectedIndex = -1;
                    cboFolders.Items.FindByValue(folderPath).Selected = true;
                }
                cboFolders.Width = Width;

                //Get Files
                LoadFiles();
                if (cboFiles.Items.FindByText(fileName) != null)
                {
                    cboFiles.Items.FindByText(fileName).Selected = true;
                }
                if (cboFiles.SelectedItem == null || string.IsNullOrEmpty(cboFiles.SelectedItem.Value))
                {
                    FileID = -1;
                }
                else
                {
                    FileID = Int32.Parse(cboFiles.SelectedItem.Value);
                }
                cboFiles.Width = Width;

                if (cboFolders.Items.Count > 1 && ShowFolders)
                {
                    cboFolders.Visible = true;
                    lblFolder.Visible = true;
                }
                else
                {
                    cboFolders.Visible = false;
                    lblFolder.Visible = false;
                }

                //Configure Mode
                switch (Mode)
                {
                    case FileControlMode.Normal:
                        fileRow.Visible = true;
                        folderRow.Visible = true;
                        cboFiles.Visible = true;
                        ShowImage();
                        txtFile.Visible = false;
                        if ((FolderPermissionController.HasFolderPermission(PortalId, cboFolders.SelectedItem.Value, "ADD") || IsUserFolder(cboFolders.SelectedItem.Value)) && ShowUpLoad)
                        {
                            ShowButton(cmdUpload, "Upload");
                        }
                        break;

                    case FileControlMode.UpLoadFile:
                        cboFiles.Visible = false;
                        txtFile.Visible = true;
                        imgPreview.Visible = false;
                        ShowButton(cmdSave, "Save");
                        ShowButton(cmdCancel, "Cancel");
                        break;
                }
            }
            else
            {
                lblMessage.Text = Utilities.GetLocalizedString("NoPermission");
            }

            //Show message Row
            messageRow.Visible = (!string.IsNullOrEmpty(lblMessage.Text));
        }

        #endregion

        #region "Event Handlers"

        private void CancelUpload(object sender, EventArgs e)
        {
            Mode = FileControlMode.Normal;
        }

        private void FileChanged(object sender, EventArgs e)
        {
            SetFilePath();
        }

        private void FolderChanged(object sender, EventArgs e)
        {
            LoadFiles();
            SetFilePath();
        }

        private void SaveFile(object sender, EventArgs e)
        {
            //if file is selected exit
            if (!string.IsNullOrEmpty(txtFile.PostedFile.FileName))
            {
                string extension = Path.GetExtension(txtFile.PostedFile.FileName).Replace(".", "");

                if (!string.IsNullOrEmpty(FileFilter) && !FileFilter.ToLower().Contains(extension.ToLower()))
                {
                    // trying to upload a file not allowed for current filter
                    var localizedString = Localization.GetString("UploadError", LocalResourceFile);
                    if(String.IsNullOrEmpty(localizedString))
                    {
                        localizedString = Utilities.GetLocalizedString("UploadError");
                    }

                    lblMessage.Text = string.Format(localizedString, FileFilter, extension);
                }
                else
                {
                    var folderManager = FolderManager.Instance;

                    var folderPath = PathUtils.Instance.GetRelativePath(PortalId, ParentFolder) + cboFolders.SelectedItem.Value;

                    //Check if this is a User Folder
                    if (IsUserFolder(cboFolders.SelectedItem.Value))
                    {
                        //Make sure the user folder exists
                        var folder = folderManager.GetFolder(PortalId, folderPath);
                        if (folder == null)
                        {
                            //Add User folder
                            var user = UserController.GetUserById(PortalSettings.PortalId, PortalSettings.UserId);
                            ((FolderManager)folderManager).AddUserFolder(user);
                        }
                    }

                    var fileName = Path.GetFileName(txtFile.PostedFile.FileName);
                    var parentFolder = folderManager.GetFolder(PortalId, folderPath);

                    try
                    {
                        FileManager.Instance.AddFile(parentFolder, fileName, txtFile.PostedFile.InputStream, true);
                    }
                    catch (PermissionsNotMetException)
                    {
                        lblMessage.Text += "<br />" + string.Format(Localization.GetString("InsufficientFolderPermission"), parentFolder.FolderPath);
                    }
                    catch (NoSpaceAvailableException)
                    {
                        lblMessage.Text += "<br />" + string.Format(Localization.GetString("DiskSpaceExceeded"), fileName);
                    }
                    catch (InvalidFileExtensionException)
                    {
                        lblMessage.Text += "<br />" + string.Format(Localization.GetString("RestrictedFileType"), fileName, Host.AllowedExtensionWhitelist.ToDisplayString());
                    }
                    catch (Exception ex)
                    {
                        DnnLog.Error(ex);

                        lblMessage.Text += "<br />" + string.Format(Localization.GetString("SaveFileError"), fileName);
                    }
                }

                if (string.IsNullOrEmpty(lblMessage.Text))
                {
                    var fileName = txtFile.PostedFile.FileName.Substring(txtFile.PostedFile.FileName.LastIndexOf("\\") + 1);
                    SetFilePath(fileName);
                }
            }
            Mode = FileControlMode.Normal;
        }

        private void UploadFile(object sender, EventArgs e)
        {
            Mode = FileControlMode.UpLoadFile;
        }

        #endregion

        #region "ILocalizable Implementation"

        public bool Localize
        {
            get
            {
                return _Localize;
            }
            set
            {
                _Localize = value;
            }
        }

        public string LocalResourceFile { get; set; }

        public virtual void LocalizeStrings()
        {
        }

        #endregion
    }
}
