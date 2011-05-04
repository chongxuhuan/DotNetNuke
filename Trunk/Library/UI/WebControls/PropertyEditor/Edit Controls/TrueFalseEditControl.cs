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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      TrueFalseEditControl
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The TrueFalseEditControl control provides a standard UI component for editing
    /// true/false (boolean) properties.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	02/21/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    [ToolboxData("<{0}:TrueFalseEditControl runat=server></{0}:TrueFalseEditControl>")]
    public class TrueFalseEditControl : EditControl
    {
        public TrueFalseEditControl()
        {
            SystemType = "System.Boolean";
        }

        protected bool BooleanValue
        {
            get
            {
                bool boolValue = Null.NullBoolean;
                try
                {
                    boolValue = Convert.ToBoolean(Value);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                }
                return boolValue;
            }
        }

        protected bool OldBooleanValue
        {
            get
            {
                bool boolValue = Null.NullBoolean;
                try
                {
                    boolValue = Convert.ToBoolean(OldValue);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                }
                return boolValue;
            }
        }

        protected override string StringValue
        {
            get
            {
                return BooleanValue.ToString();
            }
            set
            {
                bool setValue = bool.Parse(value);
                Value = setValue;
            }
        }

        protected override void OnDataChanged(EventArgs e)
        {
            var args = new PropertyEditorEventArgs(Name);
            args.Value = BooleanValue;
            args.OldValue = OldBooleanValue;
            args.StringValue = StringValue;
            base.OnValueChanged(args);
        }

        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if ((BooleanValue))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "True");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(Localization.GetString("True", Localization.SharedResourceFile));
            writer.RenderEndTag();
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if ((!BooleanValue))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "False");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(Localization.GetString("False", Localization.SharedResourceFile));
            writer.RenderEndTag();
        }
    }
}
