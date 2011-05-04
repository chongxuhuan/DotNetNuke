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
using System.Web.UI;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      DNNPageEditControl
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The DNNPageEditControl control provides a standard UI component for selecting
    /// a DNN Page
    /// </summary>
    /// <history>
    ///     [cnurse]	03/22/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    [ToolboxData("<{0}:DNNPageEditControl runat=server></{0}:DNNPageEditControl>")]
    public class DNNPageEditControl : IntegerEditControl
    {
        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            PortalSettings _portalSettings = Globals.GetPortalSettings();
            List<TabInfo> listTabs = TabController.GetPortalTabs(_portalSettings.PortalId, Null.NullInteger, true, "<" + Localization.GetString("None_Specified") + ">", false, false, true, true, false);
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);
            for (int tabIndex = 0; tabIndex <= listTabs.Count - 1; tabIndex++)
            {
                TabInfo tab = listTabs[tabIndex];
                writer.AddAttribute(HtmlTextWriterAttribute.Value, tab.TabID.ToString());
                if (tab.TabID == IntegerValue)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(tab.IndentedTabName);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        protected override void RenderViewMode(HtmlTextWriter writer)
        {
            var tabController = new TabController();
            TabInfo linkedTabInfo = tabController.GetTab(IntegerValue, Globals.GetPortalSettings().PortalId, false);
            if (linkedTabInfo != null)
            {
                int tabID = 0;
                int moduleID = 0;
                Int32.TryParse(Page.Request.QueryString["tabid"], out tabID);
                Int32.TryParse(Page.Request.QueryString["mid"], out moduleID);
                string url = Globals.LinkClick(StringValue, tabID, moduleID, true);
                ControlStyle.AddAttributesToRender(writer);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "Normal");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(linkedTabInfo.LocalizedTabName);
                writer.RenderEndTag();
            }
        }
    }
}
