#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.Web;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Search;

#endregion

namespace DotNetNuke.Services.Syndication
{
    public class RssHandler : SyndicationHandlerBase
    {
        /// <summary>
        /// This method
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="userName"></param>
        /// <remarks></remarks>
        protected override void PopulateChannel(string channelName, string userName)
        {
            var objModules = new ModuleController();
            ModuleInfo objModule;
            if (Request == null || Settings == null || Settings.ActiveTab == null || ModuleId == Null.NullInteger)
            {
                return;
            }
            Channel["title"] = Settings.PortalName;
            Channel["link"] = Globals.AddHTTP(Globals.GetDomainName(Request));
            if (!String.IsNullOrEmpty(Settings.Description))
            {
                Channel["description"] = Settings.Description;
            }
            else
            {
                Channel["description"] = Settings.PortalName;
            }
            Channel["language"] = Settings.DefaultLanguage;
            Channel["copyright"] = !string.IsNullOrEmpty(Settings.FooterText) ? 
                Settings.FooterText.Replace("[year]", DateTime.Now.Year.ToString()) : string.Empty;
            Channel["webMaster"] = Settings.Email;
            SearchResultsInfoCollection searchResults = null;
            try
            {
                searchResults = SearchDataStoreProvider.Instance().GetSearchItems(Settings.PortalId, TabId, ModuleId);
            }
            catch (Exception ex)
            {
                Exceptions.Exceptions.LogException(ex);
            }
            if (searchResults != null)
            {
                foreach (SearchResultsInfo objResult in searchResults)
                {
                    if (TabPermissionController.CanViewPage())
                    {
                        if (Settings.ActiveTab.StartDate < DateTime.Now && Settings.ActiveTab.EndDate > DateTime.Now)
                        {
                            objModule = objModules.GetModule(objResult.ModuleId, objResult.TabId);
                            if (objModule != null && objModule.DisplaySyndicate && objModule.IsDeleted == false)
                            {
                                if (ModulePermissionController.CanViewModule(objModule))
                                {
                                    if (Convert.ToDateTime(objModule.StartDate == Null.NullDate ? DateTime.MinValue : objModule.StartDate) < DateTime.Now &&
                                        Convert.ToDateTime(objModule.EndDate == Null.NullDate ? DateTime.MaxValue : objModule.EndDate) > DateTime.Now)
                                    {
                                        Channel.Items.Add(GetRssItem(objResult));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates an RSS Item
        /// </summary>
        /// <param name="SearchItem"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private GenericRssElement GetRssItem(SearchResultsInfo SearchItem)
        {
            var item = new GenericRssElement();
            string URL = Globals.NavigateURL(SearchItem.TabId);
            if (URL.ToLower().IndexOf(HttpContext.Current.Request.Url.Host.ToLower()) == -1)
            {
                URL = Globals.AddHTTP(HttpContext.Current.Request.Url.Host) + URL;
            }
            item["title"] = SearchItem.Title;
            item["description"] = SearchItem.Description;
            //TODO:  JMB: We need to figure out how to persist the dc prefix in the XML output.  See the Render method below.
            //item("dc:creator") = SearchItem.AuthorName
            item["pubDate"] = SearchItem.PubDate.ToUniversalTime().ToString("r");

            if (!string.IsNullOrEmpty(SearchItem.Guid))
            {
                if (URL.Contains("?"))
                {
                    URL += "&" + SearchItem.Guid;
                }
                else
                {
                    URL += "?" + SearchItem.Guid;
                }
            }
            item["link"] = URL;
            item["guid"] = URL;
            return item;
        }

        /// <summary>
        /// The PreRender event is used to set the Caching Policy for the Feed.  This mimics the behavior from the 
        /// OutputCache directive in the old Rss.aspx file.  @OutputCache Duration="60" VaryByParam="moduleid" 
        /// </summary>
		/// <param name="ea">Event Args.</param>
        /// <remarks></remarks>
        protected override void OnPreRender(EventArgs ea)
        {
            base.OnPreRender(ea);

            Context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(60));
            Context.Response.Cache.SetCacheability(HttpCacheability.Public);
            Context.Response.Cache.VaryByParams["moduleid"] = true;
        }
    }
}
