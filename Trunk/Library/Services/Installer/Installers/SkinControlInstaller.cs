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
using System.Xml.XPath;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

#endregion

namespace DotNetNuke.Services.Installer.Installers
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The SkinControlInstaller installs SkinControl (SkinObject) Components to a DotNetNuke site
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	03/28/2008  created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class SkinControlInstaller : ComponentInstallerBase
    {
        private SkinControlInfo InstalledSkinControl;
        private SkinControlInfo SkinControl;

        public override string AllowableFiles
        {
            get
            {
                return "ascx, vb, cs, js, resx, xml, vbproj, csproj, sln";
            }
        }

        private void DeleteSkinControl()
        {
            try
            {
                SkinControlInfo skinControl = SkinControlController.GetSkinControlByPackageID(Package.PackageID);
                if (skinControl != null)
                {
                    SkinControlController.DeleteSkinControl(skinControl);
                }
                Log.AddInfo(string.Format(Util.MODULE_UnRegistered, skinControl.ControlKey));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void Commit()
        {
        }

        public override void Install()
        {
            try
            {
                InstalledSkinControl = SkinControlController.GetSkinControlByKey(SkinControl.ControlKey);
                if (InstalledSkinControl != null)
                {
                    SkinControl.SkinControlID = InstalledSkinControl.SkinControlID;
                }
                SkinControl.PackageID = Package.PackageID;
                SkinControl.SkinControlID = SkinControlController.SaveSkinControl(SkinControl);
                Completed = true;
                Log.AddInfo(string.Format(Util.MODULE_Registered, SkinControl.ControlKey));
            }
            catch (Exception ex)
            {
                Log.AddFailure(ex);
            }
        }

        public override void ReadManifest(XPathNavigator manifestNav)
        {
            SkinControl = CBO.DeserializeObject<SkinControlInfo>(new StringReader(manifestNav.InnerXml));
            if (Log.Valid)
            {
                Log.AddInfo(Util.MODULE_ReadSuccess);
            }
        }

        public override void Rollback()
        {
            if (InstalledSkinControl == null)
            {
                DeleteSkinControl();
            }
            else
            {
                SkinControlController.SaveSkinControl(InstalledSkinControl);
            }
        }

        public override void UnInstall()
        {
            DeleteSkinControl();
        }
    }
}
