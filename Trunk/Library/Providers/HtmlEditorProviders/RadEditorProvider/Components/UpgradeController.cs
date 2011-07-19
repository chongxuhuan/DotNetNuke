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

using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Log.EventLog;
using DotNetNuke.Services.Upgrade;

namespace DotNetNuke.Providers.RadEditorProvider
{

	public class UpgradeController : IUpgradeable
	{

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Version"></param>
		/// <returns></returns>
		/// <remarks>This is not localizing Page Name or description.</remarks>
		public string UpgradeModule(string Version)
		{
			try
			{
				if (Version == "06.00.00")
				{
					string resourceFile = "~/DesktopModules/Admin/RadEditorProvider/App_LocalResources/ProviderConfig.ascx.resx";
					string pageName = Localization.GetString("HTMLEditorPageName", resourceFile);
					string pageDescription = Localization.GetString("HTMLEditorPageDescription", resourceFile);

					//Create Rad Editor Config Page (or get existing one)
					TabInfo newPage = Upgrade.AddHostPage(pageName, pageDescription, "~/DesktopModules/Admin/RadEditorProvider/images/radeditor_config_small.png", "~/DesktopModules/Admin/RadEditorProvider/images/radeditor_config_large.png", true);

					//Add Module To Page
					int moduleDefId = GetModuleDefinitionID();
					Upgrade.AddModuleToPage(newPage, moduleDefId, pageName, "~/DesktopModules/Admin/RadEditorProvider/images/radeditor_config_large.png", true);

					foreach (var item in DesktopModuleController.GetDesktopModules(Null.NullInteger))
					{
						DesktopModuleInfo moduleInfo = item.Value;

						if (moduleInfo.ModuleName == "DotNetNuke.RadEditorProvider")
						{
							moduleInfo.Category = "Host";
							DesktopModuleController.SaveDesktopModule(moduleInfo, false, false);
						}
					}

				}
			}
			catch (Exception ex)
			{
				ExceptionLogController xlc = new ExceptionLogController();
				xlc.AddLog(ex);

				return "Failed";
			}

			return "Success";
		}

		private int GetModuleDefinitionID()
		{
			// get desktop module
			DesktopModuleInfo desktopModule = DesktopModuleController.GetDesktopModuleByModuleName("DotNetNuke.RadEditorProvider", Null.NullInteger);
			if (desktopModule == null)
			{
				return -1;
			}

			//get module definition
			ModuleDefinitionInfo moduleDefinition = ModuleDefinitionController.GetModuleDefinitionByFriendlyName("RadEditor Manager", desktopModule.DesktopModuleID);
			if (moduleDefinition == null)
			{
				return -1;
			}

			return moduleDefinition.ModuleDefID;
		}

	}

}