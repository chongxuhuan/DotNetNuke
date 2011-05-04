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
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;

#endregion

namespace DotNetNuke.UI.Containers
{
    public partial class Icon : SkinObjectBase
    {
        public string BorderWidth { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                if (!String.IsNullOrEmpty(BorderWidth))
                {
                    imgIcon.BorderWidth = Unit.Parse(BorderWidth);
                }
                Visible = false;
                if ((ModuleControl != null) && (ModuleControl.ModuleContext.Configuration != null))
                {
                    if (!String.IsNullOrEmpty(ModuleControl.ModuleContext.Configuration.IconFile))
                    {
                        if (ModuleControl.ModuleContext.Configuration.IconFile.StartsWith("~/"))
                        {
                            imgIcon.ImageUrl = ModuleControl.ModuleContext.Configuration.IconFile;
                        }
                        else
                        {
                            if (Globals.IsAdminControl())
                            {
                                imgIcon.ImageUrl = ModuleControl.ModuleContext.Configuration.DesktopModule.FolderName + "/" + ModuleControl.ModuleContext.Configuration.IconFile;
                            }
                            else
                            {
                                imgIcon.ImageUrl = ModuleControl.ModuleContext.PortalSettings.HomeDirectory + ModuleControl.ModuleContext.Configuration.IconFile;
                            }
                        }
                        imgIcon.AlternateText = ModuleControl.ModuleContext.Configuration.ModuleTitle;
                        Visible = true;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}