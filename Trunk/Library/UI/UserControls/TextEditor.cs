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
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.HTMLEditorProvider;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Personalization;

#endregion

namespace DotNetNuke.UI.UserControls
{
    [ValidationPropertyAttribute("Text")]
    public class TextEditor : UserControl
    {
        private string MyFileName = "TextEditor.ascx";
        private HtmlEditorProvider RichTextEditor;
        private bool _ChooseMode = true;
        private bool _ChooseRender = true;
        private bool _HtmlEncode = true;
        protected HtmlTableCell celTextEditor;
        protected RadioButtonList optRender;
        protected RadioButtonList optView;
        protected LabelControl plView;
        protected PlaceHolder plcEditor;
        protected Panel pnlBasicRender;
        protected Panel pnlBasicTextBox;
        protected Panel pnlRichTextBox;
        protected HtmlTable tblTextEditor;
        protected HtmlTableRow trView;
        protected TextBox txtDesktopHTML;

        public bool ChooseMode
        {
            get
            {
                return _ChooseMode;
            }
            set
            {
                _ChooseMode = value;
            }
        }

        public bool ChooseRender
        {
            get
            {
                return _ChooseRender;
            }
            set
            {
                _ChooseRender = value;
            }
        }

        public string DefaultMode
        {
            get
            {
                if (ViewState["DefaultMode"] == null || String.IsNullOrEmpty(ViewState["DefaultMode"].ToString()))
                {
                    return "RICH";
                }
                else
                {
                    return ViewState["DefaultMode"].ToString();
                }
            }
            set
            {
                if (value.ToUpper() != "BASIC")
                {
                    ViewState["DefaultMode"] = "RICH";
                }
                else
                {
                    ViewState["DefaultMode"] = "BASIC";
                }
            }
        }

        public Unit Height { get; set; }

        public bool HtmlEncode
        {
            get
            {
                return _HtmlEncode;
            }
            set
            {
                _HtmlEncode = value;
            }
        }

        public string Mode
        {
            get
            {
                string strMode = "";
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                if (objUserInfo.UserID >= 0)
                {
                    if (Personalization.GetProfile("DotNetNuke.TextEditor", "PreferredTextEditor") != null)
                    {
                        strMode = Convert.ToString(Personalization.GetProfile("DotNetNuke.TextEditor", "PreferredTextEditor"));
                    }
                }
                if (String.IsNullOrEmpty(strMode))
                {
                    if (ViewState["DesktopMode"] != null && !String.IsNullOrEmpty(ViewState["DesktopMode"].ToString()))
                    {
                        strMode = Convert.ToString(ViewState["DesktopMode"]);
                    }
                }
                if (String.IsNullOrEmpty(strMode))
                {
                    strMode = DefaultMode;
                }
                return strMode;
            }
            set
            {
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                if (value.ToUpper() != "BASIC")
                {
                    ViewState["DesktopMode"] = "RICH";
                    if (objUserInfo.UserID >= 0)
                    {
                        Personalization.SetProfile("DotNetNuke.TextEditor", "PreferredTextEditor", "RICH");
                    }
                }
                else
                {
                    ViewState["DesktopMode"] = "BASIC";
                    if (objUserInfo.UserID >= 0)
                    {
                        Personalization.SetProfile("DotNetNuke.TextEditor", "PreferredTextEditor", "BASIC");
                    }
                }
            }
        }

        public string Text
        {
            get
            {
                if (optView.SelectedItem.Value == "BASIC")
                {
                    switch (optRender.SelectedItem.Value)
                    {
                        case "T":
                            return Encode(HtmlUtils.ConvertToHtml(txtDesktopHTML.Text));
                            //break;
                        case "R":
                            return txtDesktopHTML.Text;
                            //break;
                        default:
                            return Encode(txtDesktopHTML.Text);
                            //break;
                    }
                }
                else
                {
                    return Encode(RichTextEditor.Text);
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    txtDesktopHTML.Text = Decode(HtmlUtils.ConvertToText(value));
                    RichTextEditor.Text = Decode(value);
                }
            }
        }

        public string TextRenderMode
        {
            get
            {
                return Convert.ToString(ViewState["textrender"]);
            }
            set
            {
                string strMode;
                strMode = value.ToUpper().Substring(0, 1);
                if (strMode != "R" && strMode != "H" && strMode != "T")
                {
                    strMode = "H";
                }
                ViewState["textrender"] = strMode;
            }
        }

        public Unit Width { get; set; }

        public HtmlEditorProvider RichText
        {
            get
            {
                return RichTextEditor;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                return TemplateSourceDirectory + "/" + Localization.LocalResourceDirectory + "/" + MyFileName;
            }
        }

        private string Decode(string strHtml)
        {
            if (HtmlEncode)
            {
                return Server.HtmlDecode(strHtml);
            }
            else
            {
                return strHtml;
            }
        }

        private string Encode(string strHtml)
        {
            if (HtmlEncode)
            {
                return Server.HtmlEncode(strHtml);
            }
            else
            {
                return strHtml;
            }
        }

        private void PopulateLists()
        {
            if (optRender.Items.Count == 0)
            {
                optRender.Items.Add(new ListItem(Localization.GetString("Text", Localization.GetResourceFile(this, MyFileName)), "T"));
                optRender.Items.Add(new ListItem(Localization.GetString("Html", Localization.GetResourceFile(this, MyFileName)), "H"));
                optRender.Items.Add(new ListItem(Localization.GetString("Raw", Localization.GetResourceFile(this, MyFileName)), "R"));
            }
            if (optView.Items.Count == 0)
            {
                optView.Items.Add(new ListItem(Localization.GetString("BasicTextBox", Localization.GetResourceFile(this, MyFileName)), "BASIC"));
                optView.Items.Add(new ListItem(Localization.GetString("RichTextBox", Localization.GetResourceFile(this, MyFileName)), "RICH"));
            }
        }

        private void SetPanels()
        {
            if (optView.SelectedIndex != -1)
            {
                Mode = optView.SelectedItem.Value;
            }
            if (!String.IsNullOrEmpty(Mode))
            {
                optView.Items.FindByValue(Mode).Selected = true;
            }
            else
            {
                optView.SelectedIndex = 0;
            }
            if (optRender.SelectedIndex != -1)
            {
                TextRenderMode = optRender.SelectedItem.Value;
            }
            if (!String.IsNullOrEmpty(TextRenderMode))
            {
                optRender.Items.FindByValue(TextRenderMode).Selected = true;
            }
            else
            {
                optRender.SelectedIndex = 0;
            }
            if (optView.SelectedItem.Value == "BASIC")
            {
                pnlBasicTextBox.Visible = true;
                pnlRichTextBox.Visible = false;
            }
            else
            {
                pnlBasicTextBox.Visible = false;
                pnlRichTextBox.Visible = true;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RichTextEditor = HtmlEditorProvider.Instance();
            RichTextEditor.ControlID = ID;
            RichTextEditor.Initialize();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            optRender.SelectedIndexChanged += optRender_SelectedIndexChanged;
            optView.SelectedIndexChanged += optView_SelectedIndexChanged;

            try
            {
                PopulateLists();
                UserInfo objUserInfo = UserController.GetCurrentUserInfo();
                RichTextEditor.Width = Width;
                RichTextEditor.Height = Height;
                txtDesktopHTML.Height = Height;
                txtDesktopHTML.Width = Width;
                tblTextEditor.Width = Width.ToString();
                celTextEditor.Width = Width.ToString();
                if (!ChooseMode)
                {
                    trView.Visible = false;
                }
                if (!ChooseRender)
                {
                    pnlBasicRender.Visible = false;
                }
                plcEditor.Controls.Add(RichTextEditor.HtmlEditorControl);
                SetPanels();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void optRender_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (optRender.SelectedIndex != -1)
            {
                TextRenderMode = optRender.SelectedItem.Value;
            }
            if (Mode == "BASIC")
            {
                if (TextRenderMode == "H")
                {
                    txtDesktopHTML.Text = HtmlUtils.ConvertToHtml(txtDesktopHTML.Text);
                }
                else
                {
                    txtDesktopHTML.Text = HtmlUtils.ConvertToText(txtDesktopHTML.Text);
                }
            }
            SetPanels();
        }

        protected void optView_SelectedIndexChanged(Object sender, EventArgs e)
        {
            if (optView.SelectedIndex != -1)
            {
                Mode = optView.SelectedItem.Value;
            }
            if (Mode == "BASIC")
            {
                if (TextRenderMode == "T")
                {
                    txtDesktopHTML.Text = HtmlUtils.ConvertToText(RichTextEditor.Text);
                }
                else
                {
                    txtDesktopHTML.Text = RichTextEditor.Text;
                }
            }
            else
            {
                if (TextRenderMode == "T")
                {
                    RichTextEditor.Text = HtmlUtils.ConvertToHtml(txtDesktopHTML.Text);
                }
                else
                {
                    RichTextEditor.Text = txtDesktopHTML.Text;
                }
            }
            SetPanels();
        }
    }
}
