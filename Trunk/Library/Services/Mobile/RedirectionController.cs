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
using System.Web;
using System.Web.Caching;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.ClientCapability;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Services.Mobile
{
	public class RedirectionController : IRedirectController
	{
		#region "Public Methods"
        /// <summary>
        /// Get Redirection Url based on Http Context and Portal Id.         
        /// </summary>
        /// <returns>string - Empty if redirection rules are not defined or no match found</returns>
        /// <param name="userAgent">User Agent - used for client capability.</param>
        /// <param name="portalId">Portal Id from which Redirection Rules should be applied.</param>
        /// <param name="currentTabId">Current Tab Id that needs to be evaluated.</param>
        public string GetRedirectUrl(string userAgent, int portalId, int currentTabId)
        {
            string redirectUrl = string.Empty;
            IList<IRedirection> allItems = GetRedirectionsByPortal(portalId);
            //check for redirect only when redirect rules are defined
            if (allItems != null && allItems.Count > 0)
            {
                var clientCapability = ClientCapabilityProvider.Instance().GetClientCapability(userAgent);
                var tabController = new TabController();
                foreach (var redirection in allItems)
                {
                    bool checkFurther = false;
                    //redirection is based on source tab
                    if (redirection.SourceTabId != Null.NullInteger)
                    {
                        //source tab matches current tab
                        if (currentTabId == redirection.SourceTabId)
                        {
                            checkFurther = true;
                        }
                        //is child tabs to be included as well
                        else if(redirection.IncludeChildTabs) 
                        {
                            //Get all the descendents of the source tab and find out if current tab is in source tab's hierarchy or not.
                            foreach (var childTab in tabController.GetTabsByPortal(portalId).DescendentsOf(redirection.SourceTabId))
                            {
                                if (childTab.TabID == currentTabId)
                                {
                                    checkFurther = true;
                                    break;
                                }
                            }
                        }
                    }
                    //redirection is based on portal
                    else if (redirection.SourceTabId == Null.NullInteger)
                    {
                        checkFurther = true;
                    }

                    if(checkFurther)
                    {
                        //check if client capability matches with this rule
                        if(DoesCapabilityMatchWithRule(clientCapability,redirection))
                        {   
                            //find the redirect url
                            redirectUrl = GetRedirectUrlFromRule(redirection);
                            break;
                        }
                    }                    
                }
            }

            return redirectUrl;
        }


		/// <summary>
		/// save a redirection. If redirection.Id equals Null.NullInteger(-1), that means need to add a new redirection;
		/// otherwise will update the redirection by redirection.Id.
		/// </summary>
		/// <param name="redirection">redirection object.</param>
		public void Save(IRedirection redirection)
		{
			if(redirection.Id == Null.NullInteger || redirection.SortOrder == 0)
			{
				redirection.SortOrder = GetRedirectionsByPortal(redirection.PortalId).Count + 1;
			}

			int id = DataProvider.Instance().SaveRedirection(redirection.Id,
			                                        redirection.PortalId,
			                                        redirection.Name,
			                                        (int) redirection.Type,
			                                        redirection.SortOrder,
			                                        redirection.SourceTabId,
													redirection.IncludeChildTabs,
			                                        (int) redirection.TargetType,
			                                        redirection.TargetValue,
													redirection.Enabled,
			                                        UserController.GetCurrentUserInfo().UserID);

			foreach (IMatchRules rule in redirection.MatchRules)
			{
				DataProvider.Instance().SaveRedirectionRule(rule.Id, id, rule.Capability, rule.Expression);
			}

			var logContent = string.Format("{0} Mobile Redirection '{1}'", redirection.Id == Null.NullInteger ? "Add" : "Update", redirection.Name);
			AddLog(logContent);

			ClearCache(redirection.PortalId);
		}

		/// <summary>
		/// delete a redirection.
		/// </summary>
		/// <param name="portalId">Portal's id.</param>
		/// <param name="id">the redirection's id.</param>
		public void Delete(int portalId, int id)
		{
			DataProvider.Instance().DeleteRedirection(id);

			var logContent = string.Format("Delete Mobile Redirection '{0}'", id);
			AddLog(logContent);

			ClearCache(portalId);
		}

		/// <summary>
		/// delete a redirection's match rule.
		/// </summary>
		/// <param name="portalId">Portal's id.</param>
		/// <param name="redirectionId">the redirection's id.</param>
		/// <param name="ruleId">the rule's id.</param>
		public void DeleteRule(int portalId, int redirectionId, int ruleId)
		{
			DataProvider.Instance().DeleteRedirectionRule(ruleId);

			var logContent = string.Format("Delete A Rule '{0}' from Redirection '{1}'", ruleId, redirectionId);
			AddLog(logContent);

			ClearCache(portalId);
		}

		/// <summary>
		/// get a redirection list for portal.
		/// </summary>
		/// <param name="portalId">redirection id.</param>
		/// <returns>List of redirection.</returns>
		public IList<IRedirection> GetRedirectionsByPortal(int portalId)
		{
			string cacheKey = string.Format(DataCache.RedirectionsCacheKey, portalId);
			var cacheArg = new CacheItemArgs(cacheKey, DataCache.RedirectionsCacheTimeOut, DataCache.RedirectionsCachePriority, portalId);
			return CBO.GetCachedObject<IList<IRedirection>>(cacheArg, GetRedirectionsByPortalCallBack);
		}

		/// <summary>
		/// get a specific redirection by id.
		/// </summary>
		/// <param name="portalId">the redirection belong's portal.</param>
		/// <param name="id">redirection's id.</param>
		/// <returns>redirection object.</returns>
		public IRedirection GetRedirectionById(int portalId, int id)
		{
			return GetRedirectionsByPortal(portalId).Where(r => r.Id == id).FirstOrDefault();
		}

		#endregion

		#region "Private Methods"

		private IList<IRedirection> GetRedirectionsByPortalCallBack(CacheItemArgs cacheItemArgs)
		{
			int portalId = (int)cacheItemArgs.ParamList[0];
			return CBO.FillCollection<Redirection>(DataProvider.Instance().GetRedirections(portalId)).Cast<IRedirection>().ToList();
		}

		private void ClearCache(int portalId)
		{
			DataCache.RemoveCache(string.Format(DataCache.RedirectionsCacheKey, portalId));
		}

		private void AddLog(string logContent)
		{
			var objEventLog = new EventLogController();
			objEventLog.AddLog("Message", logContent, PortalController.GetCurrentPortalSettings(), UserController.GetCurrentUserInfo().UserID, EventLogController.EventLogType.ADMIN_ALERT);
		}

        private bool DoesCapabilityMatchWithRule(IClientCapability clientCapability, IRedirection redirection)
        {
            bool match = false;
            //check for Tablet first as IsTablet = true also means IsMobile true
            if (redirection.Type == RedirectionType.Tablet && clientCapability.IsTablet)
            {
                match = true;
            }
            else if (redirection.Type == RedirectionType.MobilePhone && clientCapability.IsMobile)
            {
                match = true;
            }
            else if (redirection.Type == RedirectionType.AllMobile && clientCapability.IsMobile)
            {
                match = true;
            }
            else if (redirection.Type == RedirectionType.Other)
            {
                //match all the capabilities defined in the rule
                foreach (IMatchRules rule in redirection.MatchRules)
                {
                    if (clientCapability.Capabilities.ContainsKey(rule.Capability))
                    {
                        if (clientCapability.Capabilities[rule.Capability] == rule.Expression)
                        {
                            match = true;
                            break;
                        }
                    }
                }
            }
                           
            return match;
        }

        private string GetRedirectUrlFromRule(IRedirection redirection)
        {
            string redirectUrl = string.Empty;

            if (redirection.TargetType == TargetType.Url)
                redirectUrl = redirection.TargetValue.ToString();
            else if (redirection.TargetType == TargetType.Tab)
                redirectUrl = Globals.NavigateURL(int.Parse(redirection.TargetValue.ToString()));
            else if (redirection.TargetType == TargetType.Portal)
            {
                var portal = new PortalController().GetPortal(redirection.PortalId);
                if (portal != null && portal.HomeTabId != Null.NullInteger)
                    redirectUrl = Globals.NavigateURL(int.Parse(redirection.TargetValue.ToString()));                
            }

            return redirectUrl;
        }

		#endregion
	}
}
