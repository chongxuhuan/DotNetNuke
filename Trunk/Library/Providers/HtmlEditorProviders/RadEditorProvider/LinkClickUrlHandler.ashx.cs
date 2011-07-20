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
using DotNetNuke.Services.FileSystem;

namespace DotNetNuke.Providers.RadEditorProvider
{

	/// <summary>
	/// Returns a LinkClickUrl if provided a tabid and LinkUrl.
	/// </summary>
	/// <remarks>This uses the new BaseHttpHandler which encapsulates most common scenarios including the retrieval of AJAX request data.
	/// See http://blog.theaccidentalgeek.com/post/2008/10/28/An-Updated-Abstract-Boilerplate-HttpHandler.aspx for more information on 
	/// the BaseHttpHandler.
	/// </remarks>
	public class LinkClickUrlHandler : BaseHttpHandler
	{

		private PortalAliasController _portalAliasController = new PortalAliasController();
		private UrlController _urlController = new UrlController();
		private FileController _fileController = new FileController();

#region Private Functions

		private string GetLinkClickURL(ref DialogParams @params, ref string link)
		{
			link = GetLinkUrl(ref @params, link);
			return "http://" + this.Context.Request.Url.Host + DotNetNuke.Common.Globals.LinkClick(link, @params.TabId, @params.ModuleId, true, false, @params.PortalId, @params.EnableUrlLanguage, @params.PortalGuid);

		}

		private string GetLinkUrl(ref DialogParams @params, string link)
		{
			ArrayList aliasList = _portalAliasController.GetPortalAliasArrayByPortalID(@params.PortalId);

			if (@params.LinkUrl.Contains(@params.HomeDirectory))
			{
				string filePath = @params.LinkUrl.Substring(@params.LinkUrl.IndexOf(@params.HomeDirectory)).Replace(@params.HomeDirectory, "");
				string linkedFileId = _fileController.ConvertFilePathToFileId(filePath, @params.PortalId).ToString();
				link = string.Format("fileID={0}", linkedFileId);
			}
			else
			{
				foreach (PortalAliasInfo portalAlias in aliasList)
				{
					@params.LinkUrl = @params.LinkUrl.Replace(portalAlias.HTTPAlias, "");
				}

				string tabPath = @params.LinkUrl.Replace("http://", "").Replace("/", "//").Replace(".aspx", "");
				string cultureCode = Localization.SystemLocale;

				//Try HumanFriendlyUrl TabPath
				link = TabController.GetTabByTabPath(@params.PortalId, tabPath, cultureCode).ToString();

				if (link == Null.NullInteger.ToString())
				{
					//Try getting the tabId from the querystring
					string[] arrParams = @params.LinkUrl.Split('/');
					for (int i = 0; i < arrParams.Length; i++)
					{
						if (arrParams[i].ToLowerInvariant() == "tabid")
						{
							link = arrParams[i + 1];
							break;
						}
					}
                    if (link == Null.NullInteger.ToString())
					{
						link = @params.LinkUrl;
					}
				}

			}

			return link;

		}

		private string GetURLType(TabType tabType)
		{
//INSTANT C# NOTE: The following VB 'Select Case' included either a non-ordinal switch expression or non-ordinal, range-type, or non-constant 'Case' expressions and was converted to C# 'if-else' logic:
//			Select Case tabType
//ORIGINAL LINE: Case Entities.Tabs.TabType.File
			if (tabType == Entities.Tabs.TabType.File)
			{
					return "F";
			}
//ORIGINAL LINE: Case Entities.Tabs.TabType.Member
			else if (tabType == Entities.Tabs.TabType.Member)
			{
					return "M";
			}
//ORIGINAL LINE: Case Entities.Tabs.TabType.Normal, Entities.Tabs.TabType.Tab
			else if ((tabType == Entities.Tabs.TabType.Normal) || (tabType == Entities.Tabs.TabType.Tab))
			{
					return "T";
			}
//ORIGINAL LINE: Case Else
			else
			{
					return "U";
			}
		}

		private string GetUrlLoggingInfo(ArrayList urlLog)
		{
			StringBuilder fullTableContent = new StringBuilder();
			int maxCount = ((urlLog.Count > 100) ? 100 : urlLog.Count);

			fullTableContent.Append("<div class='UrlLoggingInfo' style='width: 100%;'>");

			if (urlLog.Count > 100)
			{
				fullTableContent.Append("<span>Your search returned <strong>" + urlLog.Count.ToString() + "</strong> results. Showing only the first 100 records ordered by date.</span><br /><br />");
			}

			fullTableContent.Append("<table><tr><th>Date</th><th>User</th></tr>");

			if (maxCount == 0)
			{
				fullTableContent.Append("<tr><td colspan='2'>Your search did not return any results.</td></tr>");
			}
			else
			{
				for (var x = 0; x < maxCount; x++)
				{
					UrlLogInfo log = (UrlLogInfo)urlLog[x];
					fullTableContent.Append("<tr><td>" + log.ClickDate + "</td><td>" + log.FullName + "</td></tr>");
				}
			}

			fullTableContent.Append("</table></div>");

			return fullTableContent.ToString();
		}

#endregion

#region Public Methods

		public override void HandleRequest()
		{
			string output = null;
			DialogParams dialogParams = Content.FromJson<DialogParams>(); // This uses the new JSON Extensions in DotNetNuke.Common.Utilities.JsonExtensionsWeb

			string link = dialogParams.LinkUrl;
			dialogParams.LinkClickUrl = link;

			if (dialogParams != null)
			{

				if (! (dialogParams.LinkAction == "GetLinkInfo"))
				{
					if (dialogParams.Track)
					{
						string tempVar = dialogParams.LinkUrl;
						dialogParams.LinkClickUrl = GetLinkClickURL(ref dialogParams, ref tempVar);
						dialogParams.LinkUrl = tempVar;
						UrlTrackingInfo linkTrackingInfo = _urlController.GetUrlTracking(dialogParams.PortalId, dialogParams.LinkUrl, dialogParams.ModuleId);

						if (linkTrackingInfo != null)
						{
							dialogParams.Track = linkTrackingInfo.TrackClicks;
							dialogParams.TrackUser = linkTrackingInfo.LogActivity;
							dialogParams.DateCreated = linkTrackingInfo.CreatedDate.ToString();
							dialogParams.LastClick = linkTrackingInfo.LastClick.ToString();
							dialogParams.Clicks = linkTrackingInfo.Clicks.ToString();
						}
						else
						{
							dialogParams.Track = false;
							dialogParams.TrackUser = false;
						}
						dialogParams.LinkUrl = link;

					}
				}

				switch (dialogParams.LinkAction)
				{
					case "GetLoggingInfo": //also meant for the tracking tab but this is to retrieve the user information
						DateTime logStartDate = DateTime.MinValue;
						DateTime logEndDate = DateTime.MinValue;
						string logText = "<table><tr><th>Date</th><th>User</th></tr><tr><td colspan='2'>The selected date-range did<br /> not return any results.</td></tr>";

						if (DateTime.TryParse(dialogParams.LogStartDate, out logStartDate))
						{
							if (! (DateTime.TryParse(dialogParams.LogEndDate, out logEndDate)))
							{
								logEndDate = logStartDate.AddDays(1);
							}

							UrlController _urlController = new UrlController();
							ArrayList urlLog = _urlController.GetUrlLog(dialogParams.PortalId, GetLinkUrl(ref dialogParams, dialogParams.LinkUrl), dialogParams.ModuleId, logStartDate, logEndDate);

							if (urlLog != null)
							{
								logText = GetUrlLoggingInfo(urlLog);
							}

						}

						dialogParams.TrackingLog = logText;

						break;
					case "GetLinkInfo":
						if (dialogParams.Track)
						{
							//this section is for when the user clicks ok in the dialog box, we actually create a record for the linkclick urls.
							if (! (dialogParams.LinkUrl.ToLower().Contains("linkclick.aspx")))
							{
								dialogParams.LinkClickUrl = GetLinkClickURL(ref dialogParams, ref link);
							}

							_urlController.UpdateUrl(dialogParams.PortalId, link, GetURLType(DotNetNuke.Common.Globals.GetURLType(link)), dialogParams.TrackUser, true, dialogParams.ModuleId, false);

						}
						else
						{
							//this section is meant for retrieving/displaying the original links and determining if the links are being tracked(making sure the track checkbox properly checked)
							UrlTrackingInfo linkTrackingInfo = null;

							if (dialogParams.LinkUrl.Contains("fileticket"))
							{
								var queryString = dialogParams.LinkUrl.Split('=');
								var encryptedFileId = queryString[1].Split('&')[0];

								string fileID = UrlUtils.DecryptParameter(encryptedFileId, dialogParams.PortalGuid);
								FileInfo savedFile = _fileController.GetFileById(Int32.Parse(fileID), dialogParams.PortalId);

								linkTrackingInfo = _urlController.GetUrlTracking(dialogParams.PortalId, string.Format("fileID={0}", fileID), dialogParams.ModuleId);
								dialogParams.LinkClickUrl = string.Format("{0}{1}{2}{3}/{4}", "http://", this.Context.Request.Url.Host, dialogParams.HomeDirectory, savedFile.Folder, savedFile.FileName).Replace("//", "/");
							}
							else
							{
								try
								{
									link = dialogParams.LinkUrl.Split(Convert.ToChar("?"))[1].Split(Convert.ToChar("&"))[0].Split(Convert.ToChar("="))[1];

									int tabId = 0;
									if (int.TryParse(link, out tabId)) //if it's a tabid get the tab path
									{
										TabController _tabController = new TabController();
										dialogParams.LinkClickUrl = _tabController.GetTab(tabId, dialogParams.PortalId, true).FullUrl;
										linkTrackingInfo = _urlController.GetUrlTracking(dialogParams.PortalId, tabId.ToString(), dialogParams.ModuleId);
									}
									else
									{
										dialogParams.LinkClickUrl = HttpContext.Current.Server.UrlDecode(link); //get the actual link
										linkTrackingInfo = _urlController.GetUrlTracking(dialogParams.PortalId, dialogParams.LinkClickUrl, dialogParams.ModuleId);
									}

								}
								catch (Exception ex)
								{
									dialogParams.LinkClickUrl = dialogParams.LinkUrl;
								}
							}

							if (linkTrackingInfo == null)
							{
								dialogParams.Track = false;
								dialogParams.TrackUser = false;
							}
							else
							{
								dialogParams.Track = linkTrackingInfo.TrackClicks;
								dialogParams.TrackUser = linkTrackingInfo.LogActivity;
							}

						}
						break;
				}
				output = dialogParams.ToJson();
			}

			Response.Write(output);
		}

		public override string ContentMimeType
		{
			get
			{
				//Normally we could use the ContentEncoding property, but because of an IE bug we have to ensure
				//that the UTF-8 is capitalized which requires inclusion in the mimetype property as shown here
				return "application/json; charset=UTF-8";
			}
		}

		public override bool ValidateParameters()
		{
			//TODO: This should be updated to validate the Content paramater and return false if the content can't be converted to a DialogParams
			return true;
		}

		public override bool HasPermission
		{
			get
			{
				//TODO: This should be updated to ensure the user has appropriate permissions for the passed in TabId.
				return Context.User.Identity.IsAuthenticated;
			}
		}

#endregion

	}

}