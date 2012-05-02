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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;

#endregion

namespace DotNetNuke.Modules.Console.Components
{
    public class ConsoleController : IPortable
    {
        private readonly string[] _SettingKeys = new[] {"Mode", "ParentTabID", "IncludeParent", "DefaultSize", "AllowSizeChange", "DefaultView", "AllowViewChange", "ShowTooltip", "ConsoleWidth"};

        #region IPortable Members

        public string ExportModule(int moduleID)
        {
            var moduleCtrl = new ModuleController();
            var xmlStr = new StringBuilder();
            xmlStr.Append("<ConsoleSettings>");
            Hashtable settings = moduleCtrl.GetModuleSettings(moduleID);
            if ((settings != null))
            {
                foreach (string key in _SettingKeys)
                {
                    AddToXmlStr(xmlStr, settings, key);
                }
            }
            xmlStr.Append("</ConsoleSettings>");
            return xmlStr.ToString();
        }

        public void ImportModule(int ModuleID, string Content, string Version, int UserId)
        {
            XmlNode xmlSettings = Globals.GetContent(Content, "ConsoleSettings");
            var moduleCtrl = new ModuleController();
            foreach (string key in _SettingKeys)
            {
                XmlNode node = xmlSettings.SelectSingleNode(key);
                bool doUpdate = true;
                string value = string.Empty;
                try
                {
                    if ((node == null))
                    {
                        doUpdate = false;
                    }
                    else
                    {
                        value = node.InnerText;
                        switch (key)
                        {
                            case "ParentTabID":
								//does tab exist?
                                int parentTabID = int.Parse(value);
                                TabInfo tabInfo = new TabController().GetTab(parentTabID, PortalController.GetCurrentPortalSettings().PortalId, false);
                                break;
                            case "DefaultSize":
                                doUpdate = GetSizeValues().Contains(value);
                                break;
                            case "DefaultView":
                                doUpdate = GetViewValues().Contains(value);
                                break;
                            case "AllowSizeChange":
                            case "AllowViewChange":
                            case "ShowTooltip":
                                bool.Parse(value);
                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    Exceptions.LogException(new Exception("Unable to import value [" + key + "] for Console Module moduleid [" + ModuleID + "]"));
                    doUpdate = false;
                }
                if ((doUpdate))
                {
                    moduleCtrl.UpdateModuleSetting(ModuleID, key, value);
                }
            }
        }

        #endregion

        public static IList<string> GetSizeValues()
        {
            IList<string> returnValue = new List<string>();
            returnValue.Add("IconFile");
            returnValue.Add("IconFileLarge");
            returnValue.Add("IconNone");
            return returnValue;
        }

        public static IList<string> GetViewValues()
        {
            IList<string> returnValue = new List<string>();
            returnValue.Add("Hide");
            returnValue.Add("Show");
            return returnValue;
        }

        private void AddToXmlStr(StringBuilder xmlStr, Hashtable settings, string key)
        {
            if ((settings.ContainsKey(key)))
            {
                xmlStr.AppendFormat("<{0}>{1}</{0}>", key, settings[key]);
            }
        }
    }
}