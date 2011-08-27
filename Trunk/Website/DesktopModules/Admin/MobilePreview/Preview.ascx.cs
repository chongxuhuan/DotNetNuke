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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Mobile;
using DotNetNuke.Web.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.MobilePreview
{
    public partial class Preview : PortalModuleBase
    {
    	protected string PreviewUrl
    	{
    		get
    		{
    			var tabId = PortalSettings.HomeTabId;

				if(Request.QueryString["previewTab"] != null)
				{
					int.TryParse(Request.QueryString["previewTab"], out tabId);
				}

				return Globals.NavigateURL(tabId, string.Empty, "dnnprintmode=true");
    		}
    	}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			this.Page.Title = LocalizeString("PageTitle");

			if(!IsPostBack)
			{
				BindProfiles();
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Page.ClientScript.RegisterClientScriptInclude("mobilepreview", string.Format("{0}Scripts/PreviewEmulator.js", this.ControlPath));
			Page.ClientScript.RegisterClientScriptInclude("jscrollpane", string.Format("{0}Scripts/jquery.jscrollpane.min.js", this.ControlPath));
			Page.ClientScript.RegisterClientScriptInclude("mousewheel", string.Format("{0}Scripts/jquery.mousewheel.js", this.ControlPath));

			PageBase.RegisterStyleSheet(Page, string.Format("{0}jquery.jscrollpane.css", this.ControlPath));
		}

		private void BindProfiles()
		{
			var profiles = new PreviewProfileController().GetProfilesByPortal(ModuleContext.PortalId);
			ddlProfileList.Items.Clear();

			foreach (var previewProfile in profiles)
			{
				var value = string.Format("width : \"{0}\", height : \"{1}\"", previewProfile.Width, previewProfile.Height);

				ddlProfileList.Items.Add(new ListItem(previewProfile.Name, value));
			}
		}
    }
}