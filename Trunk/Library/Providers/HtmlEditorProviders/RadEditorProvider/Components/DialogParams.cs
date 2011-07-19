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

namespace DotNetNuke.Providers.RadEditorProvider
{
	public class DialogParams
	{

		private int _portalId;
		public int PortalId
		{
			get
			{
				return _portalId;
			}
			set
			{
				_portalId = value;
			}
		}

		private int _tabId;
		public int TabId
		{
			get
			{
				return _tabId;
			}
			set
			{
				_tabId = value;
			}
		}

		private int _moduleId;
		public int ModuleId
		{
			get
			{
				return _moduleId;
			}
			set
			{
				_moduleId = value;
			}
		}

		private string _homeDirectory;
		public string HomeDirectory
		{
			get
			{
				return _homeDirectory;
			}
			set
			{
				_homeDirectory = value;
			}
		}

		private string _portalGuid;
		public string PortalGuid
		{
			get
			{
				return _portalGuid;
			}
			set
			{
				_portalGuid = value;
			}
		}

		private string _linkUrl;
		public string LinkUrl
		{
			get
			{
				return _linkUrl;
			}
			set
			{
				_linkUrl = value;
			}
		}

		private string _linkClickUrl;
		public string LinkClickUrl
		{
			get
			{
				return _linkClickUrl;
			}
			set
			{
				_linkClickUrl = value;
			}
		}

		private bool _track;
		public bool Track
		{
			get
			{
				return _track;
			}
			set
			{
				_track = value;
			}
		}

		private bool _trackUser;
		public bool TrackUser
		{
			get
			{
				return _trackUser;
			}
			set
			{
				_trackUser = value;
			}
		}

		private bool _enableUrlLanguage;
		public bool EnableUrlLanguage
		{
			get
			{
				return _enableUrlLanguage;
			}
			set
			{
				_enableUrlLanguage = value;
			}
		}

		private string _dateCreated;
		public string DateCreated
		{
			get
			{
				return _dateCreated;
			}
			set
			{
				_dateCreated = value;
			}
		}

		private string _clicks;
		public string Clicks
		{
			get
			{
				return _clicks;
			}
			set
			{
				_clicks = value;
			}
		}

		private string _lastClick;
		public string LastClick
		{
			get
			{
				return _lastClick;
			}
			set
			{
				_lastClick = value;
			}
		}

		private string _logStartDate;
		public string LogStartDate
		{
			get
			{
				return _logStartDate;
			}
			set
			{
				_logStartDate = value;
			}
		}

		private string _logEndDate;
		public string LogEndDate
		{
			get
			{
				return _logEndDate;
			}
			set
			{
				_logEndDate = value;
			}
		}

		private string _trackingLog;
		public string TrackingLog
		{
			get
			{
				return _trackingLog;
			}
			set
			{
				_trackingLog = value;
			}
		}

		private string _linkAction;
		public string LinkAction
		{
			get
			{
				return _linkAction;
			}
			set
			{
				_linkAction = value;
			}
		}


	}
}
