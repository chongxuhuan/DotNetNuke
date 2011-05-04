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

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      TextEditControl
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The TextEditControl control provides a standard UI component for editing
    /// string/text properties.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	02/21/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    [ToolboxData("<{0}:TextEditControl runat=server></{0}:TextEditControl>")]
    public class TextEditControl : EditControl
    {
        public TextEditControl()
        {
        }

        public TextEditControl(string type)
        {
            SystemType = type;
        }

        protected string OldStringValue
        {
            get
            {
                return Convert.ToString(OldValue);
            }
        }

        protected override string StringValue
        {
            get
            {
                string strValue = Null.NullString;
                if (Value != null)
                {
                    strValue = Convert.ToString(Value);
                }
                return strValue;
            }
            set
            {
                Value = value;
            }
        }

        protected override void OnDataChanged(EventArgs e)
        {
            var args = new PropertyEditorEventArgs(Name);
            args.Value = StringValue;
            args.OldValue = OldStringValue;
            args.StringValue = StringValue;
            base.OnValueChanged(args);
        }

        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            int length = Null.NullInteger;
            if ((CustomAttributes != null))
            {
                foreach (Attribute attribute in CustomAttributes)
                {
                    if (attribute is MaxLengthAttribute)
                    {
                        var lengthAtt = (MaxLengthAttribute) attribute;
                        length = lengthAtt.Length;
                        break;
                    }
                }
            }
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, StringValue);
            if (length > Null.NullInteger)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Maxlength, length.ToString());
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }
    }
}
