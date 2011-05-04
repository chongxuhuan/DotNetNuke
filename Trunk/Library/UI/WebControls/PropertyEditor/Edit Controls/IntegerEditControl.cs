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

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      IntegerEditControl
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The IntegerEditControl control provides a standard UI component for editing
    /// integer properties.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	02/21/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    [ToolboxData("<{0}:IntegerEditControl runat=server></{0}:IntegerEditControl>")]
    public class IntegerEditControl : EditControl
    {
        public IntegerEditControl()
        {
            SystemType = "System.Int32";
        }

        protected override string StringValue
        {
            get
            {
                return IntegerValue.ToString();
            }
            set
            {
                int setValue = Int32.Parse(value);
                Value = setValue;
            }
        }

        protected int IntegerValue
        {
            get
            {
                int intValue = Null.NullInteger;
                try
                {
                    int.TryParse(Value.ToString(), out intValue);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                }
                return intValue;
            }
        }

        protected int OldIntegerValue
        {
            get
            {
                int intValue = Null.NullInteger;
                try
                {
                    int.TryParse(OldValue.ToString(), out intValue);
                }
                catch (Exception exc)
                {
                    DnnLog.Error(exc);

                }
                return intValue;
            }
        }

        protected override void OnDataChanged(EventArgs e)
        {
            var args = new PropertyEditorEventArgs(Name);
            args.Value = IntegerValue;
            args.OldValue = OldIntegerValue;
            args.StringValue = StringValue;
            base.OnValueChanged(args);
        }

        protected override void RenderEditMode(HtmlTextWriter writer)
        {
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Size, "5");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, StringValue);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }
    }
}
