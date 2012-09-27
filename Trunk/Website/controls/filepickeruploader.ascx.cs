using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.FileSystem;


public partial class FilePickerUploader : UserControl
{
    #region protected properties

    public bool UsePersonalFolder { get; set; }
    public string FilePath
    {
        get
        {
            EnsureChildControls();
            return dnnFileUploadFilePath.Value;
        }
        set
        {
            EnsureChildControls();
            dnnFileUploadFilePath.Value = value;
        }
    }
    public int FileID
    {
        get
        {
            EnsureChildControls();
            try
            {
                return int.Parse(dnnFileUploadFileId.Value);
            }
            catch
            {
                return 0;
            }
        }
        set
        {
            EnsureChildControls();
            dnnFileUploadFileId.Value = value.ToString();
        }
    }
    public string FolderPath { get; set; }
    public string FileFilter { get; set; }
    public bool Required { get; set; }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadFolders();
        jQuery.RegisterFileUpload(Page);
        jQuery.RegisterDnnJQueryPlugins(Page);
        ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

        if(!IsPostBack && FileID == 0)
        {
            // set file id
            if (!string.IsNullOrEmpty(FilesComboBox.SelectedValue))
            {
                FileID = int.Parse(FilesComboBox.SelectedValue);
            }
            else FileID = 0;
        }
    }

    private void LoadFolders()
    {
        UserInfo user = UserController.GetCurrentUserInfo();
        FoldersComboBox.Items.Clear();

        //Add Personal Folder
        if (UsePersonalFolder)
        {
            var userFolder = FolderManager.Instance.GetUserFolder(user).FolderPath;
            FoldersComboBox.AddItem("My Folder", userFolder);
        }
        else
        {
            var folders = FolderManager.Instance.GetFolders(PortalSettings.Current.PortalId, "READ,ADD", user.UserID);
            foreach (FolderInfo folder in folders)
            {
                var folderItem = new ListItem
                {
                    Text =
                        folder.FolderPath == Null.NullString
                            ? "Site Root"
                            : folder.DisplayPath,
                    Value = folder.FolderPath
                };
                FoldersComboBox.AddItem(folderItem.Text, folderItem.Value);
            }
        }

        //select folder
        string fileName;
        string folderPath;
        if (!string.IsNullOrEmpty(FilePath))
        {
            fileName = FilePath.Substring(FilePath.LastIndexOf("/") + 1);
            folderPath = string.IsNullOrEmpty(fileName) ? FilePath : FilePath.Replace(fileName, "");
        }
        else
        {
            fileName = FilePath;
            folderPath = string.Empty;
        }

        if (FoldersComboBox.FindItemByValue(folderPath) != null)
        {
            FoldersComboBox.FindItemByValue(folderPath).Selected = true;
        }

        FolderPath = folderPath;

        //select file
        LoadFiles();

        var fileSelectedItem = FilesComboBox.FindItemByText(fileName);
        if (fileSelectedItem != null)
        {
            fileSelectedItem.Selected = true;
        }
    }

    private void LoadFiles()
    {
        int effectivePortalId = PortalSettings.Current.PortalId;
        if (IsUserFolder(FoldersComboBox.SelectedItem.Value))
        {
            effectivePortalId = PortalController.GetEffectivePortalId(effectivePortalId);
        }
        FilesComboBox.DataSource = DotNetNuke.Common.Globals.GetFileList(effectivePortalId, FileFilter, Required, FoldersComboBox.SelectedItem.Value);
        FilesComboBox.DataBind();
    }

    private bool IsUserFolder(string folderPath)
    {
        UserInfo user = UserController.GetCurrentUserInfo();
        return (folderPath.ToLowerInvariant().StartsWith("users/") && folderPath.EndsWith(string.Format("/{0}/", user.UserID)));
    }

    private void SetFilePath(string fileName)
    {
        if (FoldersComboBox.SelectedItem == null || string.IsNullOrEmpty(FoldersComboBox.SelectedItem.Value))
        {
            FilePath = fileName;
        }
        else
        {
            FilePath = (FoldersComboBox.SelectedItem.Value + "/") + fileName;
        }

        var fileSelectedItem = FilesComboBox.FindItemByText(fileName);
        if (fileSelectedItem != null)
        {
            fileSelectedItem.Selected = true;
        }
    }
}