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
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.UI.Utilities;

#endregion

namespace DotNetNuke.UI.WebControls
{
    [ToolboxData("<{0}:PropertyLabelControl runat=server></{0}:PropertyLabelControl>")]
    public class PropertyLabelControl : WebControl
    {
        private string _ResourceKey;
        protected LinkButton cmdHelp;
        protected HtmlGenericControl label;
        protected Label lblHelp;
        protected Label lblLabel;
        protected Panel pnlHelp;

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        [Browsable(true), Category("Appearance"), DefaultValue("Property"), Description("Enter Caption for the control.")]
        public string Caption
        {
            get
            {
                EnsureChildControls();
                return lblLabel.Text;
            }
            set
            {
                EnsureChildControls();
                lblLabel.Text = value;
            }
        }

        [Browsable(false)]
        public Control EditControl { get; set; }

        [Browsable(true), Category("Appearance"), DefaultValue(""), Description("Enter Help Text for the control.")]
        public string HelpText
        {
            get
            {
                EnsureChildControls();
                return lblHelp.Text;
            }
            set
            {
                EnsureChildControls();
                lblHelp.Text = value;
            }
        }

        [Browsable(true), Category("Localization"), DefaultValue(""), Description("Enter the Resource key for the control.")]
        public string ResourceKey
        {
            get
            {
                return _ResourceKey;
            }
            set
            {
                _ResourceKey = value;
                EnsureChildControls();
                lblHelp.Attributes["resourcekey"] = _ResourceKey + ".Help";
                lblLabel.Attributes["resourcekey"] = _ResourceKey + ".Text";
            }
        }

        [Browsable(true), Category("Behavior"), DefaultValue(false), Description("Set whether the Help icon is displayed.")]
        public bool ShowHelp
        {
            get
            {
                EnsureChildControls();
                return cmdHelp.Visible;
            }
            set
            {
                EnsureChildControls();
                cmdHelp.Visible = value;
            }
        }

        [Browsable(true), Category("Data"), DefaultValue(""), Description("Enter the name of the field that is data bound to the Label's Text property.")]
        public string DataField { get; set; }

        [Browsable(false)]
        public object DataSource { get; set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         Description("Set the Style for the Help Text.")]
        public Style HelpStyle
        {
            get
            {
                EnsureChildControls();
                return pnlHelp.ControlStyle;
            }
        }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         Description("Set the Style for the Label Text")]
        public Style LabelStyle
        {
            get
            {
                EnsureChildControls();
                return lblLabel.ControlStyle;
            }
        }

        protected override void CreateChildControls()
        {
            label = new HtmlGenericControl();
            label.TagName = "label";
            if (!DesignMode)
            {
                cmdHelp = new LinkButton();
                cmdHelp.ID = ID + "_cmdHelp";
                cmdHelp.CssClass = "dnnFormHelp";
                cmdHelp.CausesValidation = false;
                cmdHelp.EnableViewState = false;
                cmdHelp.TabIndex = -1;
                label.Controls.Add(cmdHelp);

                lblLabel = new Label();
                lblLabel.ID = ID + "_label";
                lblLabel.EnableViewState = false;
                cmdHelp.Controls.Add(lblLabel);
            }

            pnlHelp = new Panel();
            pnlHelp.ID = ID + "_pnlHelp";
            pnlHelp.EnableViewState = false;
            lblHelp = new Label();
            lblHelp.ID = ID + "_lblHelp";
            lblHelp.EnableViewState = false;
            pnlHelp.Controls.Add(lblHelp);
            Controls.Add(label);
            Controls.Add(pnlHelp);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            if (DataSource != null)
            {
                EnsureChildControls();
                if (!String.IsNullOrEmpty(DataField))
                {
                    var dataRow = (DataRowView) DataSource;
                    if (ResourceKey == string.Empty)
                    {
                        ResourceKey = Convert.ToString(dataRow[DataField]);
                    }
                    if (DesignMode)
                    {
                        label.InnerText = Convert.ToString(dataRow[DataField]);
                    }
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            EnsureChildControls();
            DNNClientAPI.EnableMinMax(cmdHelp, pnlHelp, true, DNNClientAPI.MinMaxPersistanceType.None);
            if (EditControl != null)
            {
                label.Attributes.Add("for", EditControl.ClientID);
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }
}
