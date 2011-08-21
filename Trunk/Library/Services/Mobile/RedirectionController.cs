﻿#region Copyright

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
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.Services.Mobile
{
	public class RedirectionController : IRedirectController
	{
		public static void Redirect(HttpContext context)
		{
			
		}

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
		}

		public void Delete(int id)
		{
			DataProvider.Instance().DeleteRedirection(id);
		}

		public void DeleteRule(int redirectionId, int ruleId)
		{
			DataProvider.Instance().DeleteRedirectionRule(ruleId);
		}

		public IList<IRedirection> GetRedirectionsByPortal(int portalId)
		{
			//string cacheKey = string.Format("MobileRedirections{0}", portalId);
			//return CBO.GetCachedObject<IList<IRedirection>>(new CacheItemArgs(cacheKey, 20, CacheItemPriority.Default, portalId), GetRedirectionsByPortalCallBack);
			return CBO.FillCollection<Redirection>(DataProvider.Instance().GetRedirections(portalId)).Cast<IRedirection>().ToList();
		}

		public IRedirection GetRedirectionById(int portalId, int id)
		{
			return GetRedirectionsByPortal(portalId).Where(r => r.Id == id).FirstOrDefault();
		}

		private IList<IRedirection> GetRedirectionsByPortalCallBack(CacheItemArgs cacheItemArgs)
		{
			int portalId = (int)cacheItemArgs.ParamList[0];
			return CBO.FillCollection<IRedirection>(DataProvider.Instance().GetRedirections(portalId));
		}
	}
}