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

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI.Containers
{
    public partial class Title : SkinObjectBase
    {
        private const string MyFileName = "Title.ascx";
        public string CssClass { get; set; }

        private bool CanEditModule()
        {
            bool blnCanEdit = false;
            if ((ModuleControl != null) && (ModuleControl.ModuleContext.ModuleId > Null.NullInteger))
            {
                blnCanEdit = (PortalSettings.UserMode == PortalSettings.Mode.Edit) && TabPermissionController.CanAdminPage() && !Globals.IsAdminControl();
            }
            return blnCanEdit;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            lblTitle.UpdateLabel += lblTitle_UpdateLabel;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (!String.IsNullOrEmpty(CssClass))
                {
                    lblTitle.CssClass = CssClass;
                }
                string strTitle = Null.NullString;
                if (ModuleControl != null)
                {
                    strTitle = Localization.LocalizeControlTitle(ModuleControl);
                }
                if (strTitle == Null.NullString)
                {
                    strTitle = "&nbsp;";
                }
                lblTitle.Text = strTitle;
                lblTitle.EditEnabled = false;
                tbEIPTitle.Visible = false;
                if (CanEditModule() && PortalSettings.InlineEditorEnabled)
                {
                    lblTitle.EditEnabled = true;
                    tbEIPTitle.Visible = true;
                }
                else
                {
                    foreach (DNNToolBarButton objButton in tbEIPTitle.Buttons)
                    {
                        objButton.ToolTip = Localization.GetString("cmd" + objButton.ToolTip, Localization.GetResourceFile(this, MyFileName));
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void lblTitle_UpdateLabel(object source, DNNLabelEditEventArgs e)
        {
            if (CanEditModule())
            {
                var objModule = new ModuleController();
                ModuleInfo objModInfo = objModule.GetModule(ModuleControl.ModuleContext.ModuleId, ModuleControl.ModuleContext.TabId, false);
                objModInfo.ModuleTitle = e.Text;
                objModule.UpdateModule(objModInfo);
            }
        }
    }
}