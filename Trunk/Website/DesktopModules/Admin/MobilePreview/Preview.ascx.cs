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
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Mobile;
using DotNetNuke.Web.Client.ClientResourceManagement;
using DotNetNuke.Web.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.MobilePreview
{
    public partial class Preview : PortalModuleBase
    {
		#region "Public Properties"

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

		#endregion

		#region "Event Handlers"

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!string.IsNullOrEmpty(Request.QueryString["UserAgent"]))
			{
				var userAgent = UrlUtils.DecryptParameter(Request.QueryString["UserAgent"]);
				if (Request.QueryString["SendAgent"] != "true")
				{
					userAgent = Request.UserAgent;
				}

				CreateViewProxy(userAgent);
			}

			this.Page.Title = LocalizeString("PageTitle");

            ClientResourceManager.RegisterScript(this.Page, string.Format("{0}Scripts/PreviewEmulator.js", this.ControlPath));

			if(!IsPostBack)
			{
				BindProfiles();
			}
		}

		#endregion

		#region "Private Methods"

		private void BindProfiles()
		{
			var profiles = new PreviewProfileController().GetProfilesByPortal(ModuleContext.PortalId);
			ddlProfileList.Items.Clear();

			var selectedProfile = -1;
			if(Request.QueryString["profile"] != null)
			{
				selectedProfile = Convert.ToInt32(Request.QueryString["profile"]);
			}

			foreach (var previewProfile in profiles)
			{
				var value = string.Format("width : \"{0}\", height : \"{1}\", userAgent: \"{2}\"", previewProfile.Width, previewProfile.Height, UrlUtils.EncryptParameter(previewProfile.UserAgent));

				var listItem = new ListItem(previewProfile.Name, value);
				if(selectedProfile == previewProfile.Id)
				{
					listItem.Selected = true;
				}

				ddlProfileList.Items.Add(listItem);
			}
		}

		private void CreateViewProxy(string userAgent)
		{
			Response.Clear();

			Response.Write(GetHttpContent(PreviewUrl, userAgent));

			Response.End();
		}

		private string GetHttpContent(string url, string userAgent)
		{
			try
			{
				var wreq = (HttpWebRequest)WebRequest.Create(url);
				wreq.UserAgent = userAgent;
				wreq.Referer = Request.Url.ToString();
				wreq.Method = "GET";
				wreq.Timeout = Host.WebRequestTimeout;

				wreq.ContentType = "application/x-www-form-urlencoded";
				var responseStream = wreq.GetResponse().GetResponseStream();

				using (var reader = new StreamReader(responseStream))
				{
					return reader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				return "ERROR:" + ex.Message;
			}
		}

		#endregion
	}
}