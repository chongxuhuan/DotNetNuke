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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Tabs;

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Portals;
using Telerik.Web.UI;

namespace DotNetNuke.Providers.RadEditorProvider
{

	public class PageDropDownList : RadComboBox
	{

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			Entities.Users.UserInfo userInfo = Entities.Users.UserController.GetCurrentUserInfo();
			if (! Page.IsPostBack && userInfo != null && userInfo.UserID != Null.NullInteger)
			{
				//check view permissions - Yes?
				PortalSettings _PortalSettings = PortalController.GetCurrentPortalSettings();
				string pageCulture = _PortalSettings.ActiveTab.CultureCode;
				if (string.IsNullOrEmpty(pageCulture))
				{
					pageCulture = PortalController.GetActivePortalLanguage(_PortalSettings.PortalId);
				}

				List<TabInfo> tabs = TabController.GetTabsBySortOrder(_PortalSettings.PortalId, pageCulture, true);
				var sortedTabList = TabController.GetPortalTabs(tabs, Null.NullInteger, false, Null.NullString, true, false, true, true, true);

				Items.Clear();
				foreach (var _tab in sortedTabList)
				{
					RadComboBoxItem tabItem = new RadComboBoxItem(_tab.IndentedTabName, _tab.FullUrl);
					tabItem.Enabled = ! _tab.DisableLink;

					Items.Add(tabItem);
				}

				Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("", ""));
			}

			Width = Unit.Pixel(245);

		}

	}

}

