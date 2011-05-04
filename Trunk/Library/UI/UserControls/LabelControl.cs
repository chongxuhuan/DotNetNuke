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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

#endregion

namespace DotNetNuke.UI.UserControls
{
    public abstract class LabelControl : UserControl
    {
        protected LinkButton cmdHelp;
        protected HtmlGenericControl label;
        protected Label lblHelp;
        protected Label lblLabel;
        protected Panel pnlHelp;

        public string ControlName { get; set; }

        public string CssClass { get; set; }

        public string HelpKey { get; set; }

        public string HelpText
        {
            get
            {
                return lblHelp.Text;
            }
            set
            {
                lblHelp.Text = value;
            }
        }

        public string ResourceKey { get; set; }

        public string Suffix { get; set; }

        public string Text
        {
            get
            {
                return lblLabel.Text;
            }
            set
            {
                lblLabel.Text = value;
            }
        }

        public Unit Width
        {
            get
            {
                return lblLabel.Width;
            }
            set
            {
                lblLabel.Width = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, true, DNNClientAPI.MinMaxPersistanceType.None);
                if (String.IsNullOrEmpty(ResourceKey))
                {
                    ResourceKey = ID;
                }
                if ((!string.IsNullOrEmpty(ResourceKey)))
                {
                    string localText = Localization.GetString(ResourceKey, this);
                    if (!string.IsNullOrEmpty(localText))
                    {
                        Text = localText + Suffix;
                    }
                    else
                    {
                        Text += Suffix;
                    }
                }
                if (String.IsNullOrEmpty(HelpKey))
                {
                    HelpKey = ResourceKey + ".Help";
                }
                string helpText = Localization.GetString(HelpKey, this);
                if ((!string.IsNullOrEmpty(helpText)) || (string.IsNullOrEmpty(HelpText)))
                {
                    HelpText = helpText;
                }
                if (!string.IsNullOrEmpty(CssClass))
                {
                    lblLabel.CssClass = CssClass;
                }
                if (!String.IsNullOrEmpty(ControlName))
                {
                    Control c = Parent.FindControl(ControlName);
                    if (c != null)
                    {
                        label.Attributes["for"] = c.ClientID;
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

    }
}
