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
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{
    [ToolboxData("<{0}:DNNTimeZoneEditControl runat=server></{0}:DNNTimeZoneEditControl>")]
    [Obsolete("Deprecated in DNN 6.0. Replaced by new DnnTimeZoneComboBox control and use of .NET TimeZoneInfo class")]
    public class DNNTimeZoneEditControl : IntegerEditControl
    {
        protected override void RenderViewMode(HtmlTextWriter writer)
        {
            PortalSettings _portalSettings = Globals.GetPortalSettings();
            var cboTimeZones = new DropDownList();
            Localization.LoadTimeZoneDropDownList(cboTimeZones, ((PageBase) Page).PageCulture.Name, Convert.ToString(_portalSettings.TimeZoneOffset));
            if (cboTimeZones.Items.FindByValue(StringValue) != null)
            {
                cboTimeZones.ClearSelection();
                cboTimeZones.Items.FindByValue(StringValue).Selected = true;
            }
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(cboTimeZones.SelectedItem.Text);
            writer.RenderEndTag();
        }

        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            PortalSettings _portalSettings = Globals.GetPortalSettings();
            var cboTimeZones = new DropDownList();
            Localization.LoadTimeZoneDropDownList(cboTimeZones, ((PageBase) Page).PageCulture.Name, Convert.ToString(_portalSettings.TimeZoneOffset));
            if (cboTimeZones.Items.FindByValue(StringValue) != null)
            {
                cboTimeZones.ClearSelection();
                cboTimeZones.Items.FindByValue(StringValue).Selected = true;
            }
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);
            for (int I = 0; I <= cboTimeZones.Items.Count - 1; I++)
            {
                string timeZoneValue = cboTimeZones.Items[I].Value;
                string timeZoneName = cboTimeZones.Items[I].Text;
                bool isSelected = cboTimeZones.Items[I].Selected;
                writer.AddAttribute(HtmlTextWriterAttribute.Value, timeZoneValue);
                if (isSelected)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(timeZoneName.PadRight(100).Substring(0, 50));
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }
    }
}
