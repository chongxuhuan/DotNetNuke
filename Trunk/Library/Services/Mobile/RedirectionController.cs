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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Log.EventLog;

#endregion

namespace DotNetNuke.Services.Mobile
{
	public class RedirectionController : IRedirectController
	{
		#region "Public Methods"
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

		#endregion
	}
}
