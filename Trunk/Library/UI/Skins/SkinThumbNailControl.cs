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
using System.IO;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Framework;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Localization;

using Image = System.Drawing.Image;

#endregion

namespace DotNetNuke.UI.Skins
{
    public abstract class SkinThumbNailControl : UserControlBase
    {
        protected HtmlGenericControl ControlContainer;
        protected RadioButtonList optSkin;

        public string Border
        {
            get
            {
                return Convert.ToString(ViewState["SkinControlBorder"]);
            }
            set
            {
                ViewState["SkinControlBorder"] = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ControlContainer.Style.Add("border-top", value);
                    ControlContainer.Style.Add("border-bottom", value);
                    ControlContainer.Style.Add("border-left", value);
                    ControlContainer.Style.Add("border-right", value);
                }
            }
        }

        public int Columns
        {
            get
            {
                return Convert.ToInt32(ViewState["SkinControlColumns"]);
            }
            set
            {
                ViewState["SkinControlColumns"] = value;
                if (value > 0)
                {
                    optSkin.RepeatColumns = value;
                }
            }
        }

        public string Height
        {
            get
            {
                return Convert.ToString(ViewState["SkinControlHeight"]);
            }
            set
            {
                ViewState["SkinControlHeight"] = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ControlContainer.Style.Add("height", value);
                }
            }
        }

        public string SkinRoot
        {
            get
            {
                return Convert.ToString(ViewState["SkinRoot"]);
            }
            set
            {
                ViewState["SkinRoot"] = value;
            }
        }

        public string SkinSrc
        {
            get
            {
                if (optSkin.SelectedItem != null)
                {
                    return optSkin.SelectedItem.Value;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                int intIndex;
                for (intIndex = 0; intIndex <= optSkin.Items.Count - 1; intIndex++)
                {
                    if (optSkin.Items[intIndex].Value == value)
                    {
                        optSkin.Items[intIndex].Selected = true;
                        break;
                    }
                }
            }
        }

        public string Width
        {
            get
            {
                return Convert.ToString(ViewState["SkinControlWidth"]);
            }
            set
            {
                ViewState["SkinControlWidth"] = value;
                if (!String.IsNullOrEmpty(value))
                {
                    ControlContainer.Style.Add("width", value);
                }
            }
        }

        private void AddDefaultSkin()
        {
            string strDefault = Localization.GetString("Not_Specified") + "<br>";
            strDefault += "<img src=\"" + Globals.ApplicationPath.Replace("\\", "/") + "/images/spacer.gif\" width=\"140\" height=\"135\" border=\"0\">";
            optSkin.Items.Insert(0, new ListItem(strDefault, ""));
        }

        private void AddSkin(string root, string strFolder, string strFile)
        {
            string strImage = "";
            if (File.Exists(strFile.Replace(".ascx", ".jpg")))
            {
                strImage += "<a href=\"" + CreateThumbnail(strFile.Replace(".ascx", ".jpg")).Replace("thumbnail_", "") + "\" target=\"_new\"><img src=\"" +
                            CreateThumbnail(strFile.Replace(".ascx", ".jpg")).Replace("\\", "/") + "\" border=\"1\"></a>";
            }
            else
            {
                strImage += "<img src=\"" + Globals.ApplicationPath.Replace("\\", "/") + "/images/thumbnail.jpg\" border=\"1\">";
            }
            optSkin.Items.Add(new ListItem(FormatSkinName(strFolder, Path.GetFileNameWithoutExtension(strFile)) + "<br>" + strImage, root + "/" + strFolder + "/" + Path.GetFileName(strFile)));
        }

        private string FormatSkinName(string strSkinFolder, string strSkinFile)
        {
            if (strSkinFolder.ToLower() == "_default")
            {
                return strSkinFile;
            }
            else
            {
                switch (strSkinFile.ToLower())
                {
                    case "skin":
                    case "container":
                    case "default":
                        return strSkinFolder;
                    default:
                        return strSkinFolder + " - " + strSkinFile;
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
                int intSize = 140;
                Image objImage;
                try
                {
                    objImage = Image.FromFile(strImage);
                    if (objImage.Height > objImage.Width)
                    {
                        dblScale = intSize/objImage.Height;
                        intHeight = intSize;
                        intWidth = Convert.ToInt32(objImage.Width*dblScale);
                    }
                    else
                    {
                        dblScale = intSize/objImage.Width;
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
            strThumbnail = Globals.ApplicationPath + "\\" + strThumbnail.Substring(strThumbnail.ToLower().IndexOf("portals\\"));
            return strThumbnail;
        }

        public void Clear()
        {
            optSkin.Items.Clear();
        }

        public void LoadAllSkins(bool includeNotSpecified)
        {
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            LoadHostSkins(false);
            LoadPortalSkins(false);
        }

        public void LoadHostSkins(bool includeNotSpecified)
        {
            string strRoot;
            string[] arrFolders;
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            strRoot = Globals.HostMapPath + SkinRoot;
            if (Directory.Exists(strRoot))
            {
                arrFolders = Directory.GetDirectories(strRoot);
                foreach (string strFolder in arrFolders)
                {
                    if (!strFolder.EndsWith(Globals.glbHostSkinFolder))
                    {
                        LoadSkins(strFolder, "[G]", false);
                    }
                }
            }
        }

        public void LoadPortalSkins(bool includeNotSpecified)
        {
            string strRoot;
            string[] arrFolders;
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            strRoot = PortalSettings.HomeDirectoryMapPath + SkinRoot;
            if (Directory.Exists(strRoot))
            {
                arrFolders = Directory.GetDirectories(strRoot);
                foreach (string strFolder in arrFolders)
                {
                    LoadSkins(strFolder, "[L]", false);
                }
            }
        }

        public void LoadSkins(string strFolder, string skinType, bool includeNotSpecified)
        {
            string[] arrFiles;
            if (includeNotSpecified)
            {
                AddDefaultSkin();
            }
            if (Directory.Exists(strFolder))
            {
                arrFiles = Directory.GetFiles(strFolder, "*.ascx");
                strFolder = strFolder.Substring(strFolder.LastIndexOf("\\") + 1);
                foreach (string strFile in arrFiles)
                {
                    AddSkin(skinType + SkinRoot, strFolder, strFile);
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}
