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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

#endregion

namespace DotNetNuke.UI.UserControls
{
    public abstract class HelpButtonControl : UserControl
    {
        private string _HelpKey;
        private string _ResourceKey;
        protected LinkButton cmdHelp;
        protected Image imgHelp;
        protected Label lblHelp;
        protected Panel pnlHelp;

        public string ControlName { get; set; }

        public string HelpKey
        {
            get
            {
                return _HelpKey;
            }
            set
            {
                _HelpKey = value;
            }
        }

        public string HelpText
        {
            get
            {
                return lblHelp.Text;
            }
            set
            {
                lblHelp.Text = value;
                imgHelp.AlternateText = HtmlUtils.Clean(value, false);
                if (String.IsNullOrEmpty(value))
                {
                    imgHelp.Visible = false;
                }
            }
        }

        public string ResourceKey
        {
            get
            {
                return _ResourceKey;
            }
            set
            {
                _ResourceKey = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdHelp.Click += cmdHelp_Click;

            try
            {
                DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, true, DNNClientAPI.MinMaxPersistanceType.None);
                if (String.IsNullOrEmpty(_HelpKey))
                {
                    _HelpKey = _ResourceKey + ".Help";
                }
                string helpText = Localization.GetString(_HelpKey, this);
                if (!String.IsNullOrEmpty(helpText))
                {
                    HelpText = helpText;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdHelp_Click(object sender, EventArgs e)
        {
            pnlHelp.Visible = true;
        }
    }
}
