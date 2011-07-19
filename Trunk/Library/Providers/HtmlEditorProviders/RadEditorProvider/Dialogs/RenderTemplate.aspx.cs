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
//

//INSTANT C# NOTE: Formerly VB project-level imports:
using DotNetNuke;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Framework;
using DotNetNuke.Modules;
using DotNetNuke.Security;
using DotNetNuke.Services;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using DotNetNuke.Entities.Portals;

namespace DotNetNuke.Providers.RadEditorProvider
{

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// </remarks>
	/// <history>
	/// </history>
	public partial class RenderTemplate : System.Web.UI.Page
	{

#region Event Handlers

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				string renderUrl = Request.QueryString["rurl"];

				if (! (string.IsNullOrEmpty(renderUrl)))
				{
					string fileContents = string.Empty;
					FileSystem.FileController fileCtrl = new FileSystem.FileController();
					FileSystem.FileInfo fileInfo = null;
					int portalID = PortalController.GetCurrentPortalSettings().PortalId;

					if (renderUrl.ToLower().Contains("linkclick.aspx") && renderUrl.ToLower().Contains("fileticket"))
					{
						//File Ticket
						int fileID = GetFileIDFromURL(renderUrl);

						if (fileID > -1)
						{
							fileInfo = fileCtrl.GetFileById(fileID, portalID);
						}
					}
					else
					{
						//File URL
						string dbPath = (string)(string)FileSystemValidation.ToDBPath(renderUrl);
						string fileName = System.IO.Path.GetFileName(renderUrl);

						if (! (string.IsNullOrEmpty(fileName)))
						{
							FileSystem.FolderInfo dnnFolder = GetDNNFolder(dbPath);
							if (dnnFolder != null)
							{
								fileInfo = fileCtrl.GetFile(fileName, portalID, dnnFolder.FolderID);
							}
						}
					}

					if (fileInfo != null)
					{
						if (CanViewFile(fileInfo.Folder) && fileInfo.Extension.ToLower() == "htmtemplate")
						{
							byte[] fileBytes = FileSystemUtils.GetFileContent(fileInfo);
							fileContents = System.Text.Encoding.ASCII.GetString(fileBytes);
						}
					}

					if (! (string.IsNullOrEmpty(fileContents)))
					{
						Content.Text = Server.HtmlEncode(fileContents);
					}
				}
			}
			catch (Exception ex)
			{
				DotNetNuke.Services.Exceptions.LogException(ex);
				Content.Text = string.Empty;
			}
		}

#endregion

#region Methods

		private int GetFileIDFromURL(string url)
		{
			int returnValue = -1;
			//add http
			if (! (url.ToLower().StartsWith("http")))
			{
				if (url.ToLower().StartsWith("/"))
				{
					url = "http:/" + url;
				}
				else
				{
					url = "http://" + url;
				}
			}

			Uri u = new Uri(url);

			if (u != null && u.Query != null)
			{
				NameValueCollection @params = HttpUtility.ParseQueryString(u.Query);

				if (@params != null && @params.Count > 0)
				{
					string fileTicket = @params.Get("fileticket");

					if (! (string.IsNullOrEmpty(fileTicket)))
					{
						var strFileID = UrlUtils.DecryptParameter(fileTicket);

						try
						{
							returnValue = int.Parse(strFileID);
						}
						catch (Exception ex)
						{
							returnValue = -1;
						}
					}
				}
			}

			return returnValue;
		}

		protected bool CanViewFile(string dbPath)
		{
			return DotNetNuke.Security.Permissions.FolderPermissionController.CanViewFolder(GetDNNFolder(dbPath));
		}

		private DotNetNuke.Services.FileSystem.FolderInfo GetDNNFolder(string dbPath)
		{
			return new DotNetNuke.Services.FileSystem.FolderController().GetFolder(PortalController.GetCurrentPortalSettings().PortalId, dbPath, false);
		}

		private string DNNHomeDirectory
		{
			get
			{
				//todo: host directory
				string homeDir = PortalController.GetCurrentPortalSettings().HomeDirectory;
				homeDir = homeDir.Replace("\\", "/");

				if (homeDir.EndsWith("/"))
				{
					homeDir = homeDir.Remove(homeDir.Length - 1, 1);
				}

				return homeDir;
			}
		}

#endregion


	override protected void OnInit(EventArgs e)
	{
		base.OnInit(e);

//INSTANT C# NOTE: Converted event handler wireups:
		this.Load += new System.EventHandler(Page_Load);
	}
	}

}
