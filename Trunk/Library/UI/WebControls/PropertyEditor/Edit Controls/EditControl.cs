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

using DotNetNuke.Security;

#endregion

namespace DotNetNuke.UI.WebControls
{
    [ValidationPropertyAttribute("Value")]
    public abstract class EditControl : WebControl, IPostBackDataHandler
    {
        private object[] _CustomAttributes;

        public object[] CustomAttributes
        {
            get
            {
                return _CustomAttributes;
            }
            set
            {
                _CustomAttributes = value;
                if ((_CustomAttributes != null) && _CustomAttributes.Length > 0)
                {
                    OnAttributesChanged();
                }
            }
        }

        public PropertyEditorMode EditMode { get; set; }

        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        public string LocalResourceFile { get; set; }

        public string Name { get; set; }

        public object OldValue { get; set; }

        public bool Required { get; set; }

        public string SystemType { get; set; }

        public object Value { get; set; }

        protected abstract string StringValue { get; set; }

        #region IPostBackDataHandler Members

        public virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            bool dataChanged = false;
            string presentValue = StringValue;
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
            OnDataChanged(EventArgs.Empty);
        }

        #endregion

        public event PropertyChangedEventHandler ItemAdded;
        public event PropertyChangedEventHandler ItemDeleted;
        public event PropertyChangedEventHandler ValueChanged;

        protected abstract void OnDataChanged(EventArgs e);

        protected virtual void OnAttributesChanged()
        {
        }

        protected virtual void OnItemAdded(PropertyEditorEventArgs e)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, e);
            }
        }

        protected virtual void OnItemDeleted(PropertyEditorEventArgs e)
        {
            if (ItemDeleted != null)
            {
                ItemDeleted(this, e);
            }
        }

        protected virtual void OnValueChanged(PropertyEditorEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        protected virtual void RenderViewMode(HtmlTextWriter writer)
        {
            string propValue = Page.Server.HtmlDecode(Convert.ToString(Value));
            ControlStyle.AddAttributesToRender(writer);
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            var security = new PortalSecurity();
            writer.Write(security.InputFilter(propValue, PortalSecurity.FilterFlag.NoScripting));
            writer.RenderEndTag();
        }

        protected virtual void RenderEditMode(HtmlTextWriter writer)
        {
            string propValue = Convert.ToString(Value);
            ControlStyle.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
            writer.AddAttribute(HtmlTextWriterAttribute.Value, propValue);
            writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID);
            writer.RenderBeginTag(HtmlTextWriterTag.Input);
            writer.RenderEndTag();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            var strOldValue = OldValue as string;
            if (EditMode == PropertyEditorMode.Edit || (Required && string.IsNullOrEmpty(strOldValue)))
            {
                RenderEditMode(writer);
            }
            else
            {
                RenderViewMode(writer);
            }
        }
    }
}
