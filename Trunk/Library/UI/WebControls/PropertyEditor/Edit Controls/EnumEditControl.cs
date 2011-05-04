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

using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      EnumEditControl
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The EnumEditControl control provides a standard UI component for editing
    /// enumerated properties.
    /// </summary>
    /// <history>
    ///     [cnurse]	02/23/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    [ToolboxData("<{0}:EnumEditControl runat=server></{0}:EnumEditControl>")]
    public class EnumEditControl : EditControl
    {
        private readonly Type EnumType;

        public EnumEditControl()
        {
        }

        public EnumEditControl(string type)
        {
            SystemType = type;
            EnumType = Type.GetType(type);
        }

        protected override string StringValue
        {
            get
            {
                int retValue = Convert.ToInt32(Value);
                return retValue.ToString();
            }
            set
            {
                int setValue = Int32.Parse(value);
                Value = setValue;
            }
        }

        protected override void OnDataChanged(EventArgs e)
        {
            int intValue = Convert.ToInt32(Value);
            int intOldValue = Convert.ToInt32(OldValue);
            var args = new PropertyEditorEventArgs(Name);
            args.Value = Enum.ToObject(EnumType, intValue);
            args.OldValue = Enum.ToObject(EnumType, intOldValue);
            base.OnValueChanged(args);
        }

        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            Int32 propValue = Convert.ToInt32(Value);
            Array enumValues = Enum.GetValues(EnumType);
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Select);
            for (int I = 0; I <= enumValues.Length - 1; I++)
            {
                int enumValue = Convert.ToInt32(enumValues.GetValue(I));
                string enumName = Enum.GetName(EnumType, enumValue);
                enumName = Localization.GetString(enumName, LocalResourceFile);
                writer.AddAttribute(HtmlTextWriterAttribute.Value, enumValue.ToString());
                if (enumValue == propValue)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Selected, "selected");
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Option);
                writer.Write(enumName);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        protected override void RenderViewMode(HtmlTextWriter writer)
        {
            Int32 propValue = Convert.ToInt32(Value);
            string enumValue = Enum.Format(EnumType, propValue, "G");
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(enumValue);
            writer.RenderEndTag();
        }
    }
}
