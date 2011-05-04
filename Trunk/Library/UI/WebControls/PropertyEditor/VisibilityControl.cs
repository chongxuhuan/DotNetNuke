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
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{
    [ToolboxData("<{0}:VisibilityControl runat=server></{0}:VisibilityControl>")]
    public class VisibilityControl : WebControl, IPostBackDataHandler, INamingContainer
    {
        public string Caption { get; set; }

        public string Name { get; set; }

        public object Value { get; set; }

        #region IPostBackDataHandler Members

        public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            bool dataChanged = false;
            string presentValue = Convert.ToString(Value);
            string postedValue = postCollection[postDataKey];
            if (!presentValue.Equals(postedValue))
            {
                Value = postedValue;
                dataChanged = true;
            }
            return dataChanged;
        }

        public void RaisePostDataChangedEvent()
        {
            int intValue = Convert.ToInt32(Value);
            var args = new PropertyEditorEventArgs(Name);
            args.Value = Enum.ToObject(typeof (UserVisibilityMode), intValue);
            OnVisibilityChanged(args);
        }

        #endregion

        public event PropertyChangedEventHandler VisibilityChanged;

        protected virtual void OnVisibilityChanged(PropertyEditorEventArgs e)
        {
            if (VisibilityChanged != null)
            {
                VisibilityChanged(this, e);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var propValue = (UserVisibilityMode) (Convert.ToInt32(Value));
            ControlStyle.AddAttributesToRender(writer);
            AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(Caption);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if ((propValue == UserVisibilityMode.AllUsers))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            writer.Write(Localization.GetString("Public"));
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if ((propValue == UserVisibilityMode.MembersOnly))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "1");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            writer.Write(Localization.GetString("MemberOnly"));
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
            if ((propValue == UserVisibilityMode.AdminOnly))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Value, "2");
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
            writer.Write(Localization.GetString("AdminOnly"));
            writer.RenderEndTag();
        }
    }
}
