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
using System.Collections;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{
    public class LanguageSelector : Control, INamingContainer
    {
        #region LanguageItemStyle enum

        public enum LanguageItemStyle
        {
            FlagOnly = 1,
            FlagAndCaption = 2,
            CaptionOnly = 3
        }

        #endregion

        #region LanguageListDirection enum

        public enum LanguageListDirection
        {
            Horizontal = 1,
            Vertical = 2
        }

        #endregion

        #region LanguageSelectionMode enum

        public enum LanguageSelectionMode
        {
            Multiple = 1,
            Single = 2
        }

        #endregion

        #region LanguageSelectionObject enum

        public enum LanguageSelectionObject
        {
            NeutralCulture = 1,
            SpecificCulture = 2
        }

        #endregion

        private Panel pnlControl;

        public LanguageSelectionMode SelectionMode
        {
            get
            {
                if (ViewState["SelectionMode"] == null)
                {
                    return LanguageSelectionMode.Single;
                }
                else
                {
                    return (LanguageSelectionMode) ViewState["SelectionMode"];
                }
            }
            set
            {
                if (SelectionMode != value)
                {
                    ViewState["SelectionMode"] = value;
                    if (Controls.Count > 0)
                    {
                        CreateChildControls();
                    }
                }
            }
        }

        public LanguageSelectionObject SelectionObject
        {
            get
            {
                if (ViewState["SelectionObject"] == null)
                {
                    return LanguageSelectionObject.SpecificCulture;
                }
                else
                {
                    return (LanguageSelectionObject) ViewState["SelectionObject"];
                }
            }
            set
            {
                if ((int) SelectionMode != (int) value)
                {
                    ViewState["SelectionObject"] = value;
                    if (Controls.Count > 0)
                    {
                        CreateChildControls();
                    }
                }
            }
        }

        public LanguageItemStyle ItemStyle
        {
            get
            {
                if (ViewState["ItemStyle"] == null)
                {
                    return LanguageItemStyle.FlagAndCaption;
                }
                else
                {
                    return (LanguageItemStyle) ViewState["ItemStyle"];
                }
            }
            set
            {
                if (ItemStyle != value)
                {
                    ViewState["ItemStyle"] = value;
                    if (Controls.Count > 0)
                    {
                        CreateChildControls();
                    }
                }
            }
        }

        public LanguageListDirection ListDirection
        {
            get
            {
                if (ViewState["ListDirection"] == null)
                {
                    return LanguageListDirection.Vertical;
                }
                else
                {
                    return (LanguageListDirection) ViewState["ListDirection"];
                }
            }
            set
            {
                if (ListDirection != value)
                {
                    ViewState["ListDirection"] = value;
                    if (Controls.Count > 0)
                    {
                        CreateChildControls();
                    }
                }
            }
        }

        public string[] SelectedLanguages
        {
            get
            {
                EnsureChildControls();
                var a = new ArrayList();
                if (GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture).Length < 2)
                {
                    PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
                    foreach (string strLocale in LocaleController.Instance.GetLocales(_Settings.PortalId).Keys)
                    {
                        a.Add(strLocale);
                    }
                }
                else
                {
                    foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
                    {
                        if (SelectionMode == LanguageSelectionMode.Single)
                        {
                            if (((RadioButton) pnlControl.FindControl("opt" + c.Name)).Checked)
                            {
                                a.Add(c.Name);
                            }
                        }
                        else
                        {
                            if (((CheckBox) pnlControl.FindControl("chk" + c.Name)).Checked)
                            {
                                a.Add(c.Name);
                            }
                        }
                    }
                }
                return a.ToArray(typeof (string)) as string[];
            }
            set
            {
                EnsureChildControls();
                if (SelectionMode == LanguageSelectionMode.Single && value.Length > 1)
                {
                    throw new ArgumentException("Selection mode 'single' cannot have more than one selected item.");
                }
                foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
                {
                    if (SelectionMode == LanguageSelectionMode.Single)
                    {
                        ((RadioButton) pnlControl.FindControl("opt" + c.Name)).Checked = false;
                    }
                    else
                    {
                        ((CheckBox) pnlControl.FindControl("chk" + c.Name)).Checked = false;
                    }
                }
                foreach (string strLocale in value)
                {
                    if (SelectionMode == LanguageSelectionMode.Single)
                    {
                        Control ctl = pnlControl.FindControl("opt" + strLocale);
                        if (ctl != null)
                        {
                            ((RadioButton) ctl).Checked = true;
                        }
                    }
                    else
                    {
                        Control ctl = pnlControl.FindControl("chk" + strLocale);
                        if (ctl != null)
                        {
                            ((CheckBox) ctl).Checked = true;
                        }
                    }
                }
            }
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            pnlControl = new Panel();
            Controls.Add(pnlControl);
            foreach (CultureInfo c in GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture))
            {
                var lblLocale = new HtmlGenericControl("label");
                if (SelectionMode == LanguageSelectionMode.Single)
                {
                    var optLocale = new RadioButton();
                    optLocale.ID = "opt" + c.Name;
                    optLocale.GroupName = pnlControl.ID + "_Locale";
                    if (c.Name == Localization.SystemLocale)
                    {
                        optLocale.Checked = true;
                    }
                    pnlControl.Controls.Add(optLocale);
                    lblLocale.Attributes["for"] = optLocale.ClientID;
                }
                else
                {
                    var chkLocale = new CheckBox();
                    chkLocale.ID = "chk" + c.Name;
                    pnlControl.Controls.Add(chkLocale);
                    lblLocale.Attributes["for"] = chkLocale.ClientID;
                }
                pnlControl.Controls.Add(lblLocale);
                if (ItemStyle != LanguageItemStyle.CaptionOnly)
                {
                    var imgLocale = new Image();
                    imgLocale.ImageUrl = ResolveUrl("~/images/Flags/" + c.Name + ".gif");
                    imgLocale.AlternateText = c.DisplayName;
                    imgLocale.Style["vertical-align"] = "middle";
                    lblLocale.Controls.Add(imgLocale);
                }
                if (ItemStyle != LanguageItemStyle.FlagOnly)
                {
                    lblLocale.Attributes["class"] = "Normal";
                    lblLocale.Controls.Add(new LiteralControl("&nbsp;" + c.DisplayName));
                }
                if (ListDirection == LanguageListDirection.Vertical)
                {
                    pnlControl.Controls.Add(new LiteralControl("<br />"));
                }
                else
                {
                    pnlControl.Controls.Add(new LiteralControl(" "));
                }
            }
            if (GetCultures(SelectionObject == LanguageSelectionObject.SpecificCulture).Length < 2)
            {
                Visible = false;
            }
        }

        private CultureInfo[] GetCultures(bool specific)
        {
            var a = new ArrayList();
            PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
            foreach (string strLocale in LocaleController.Instance.GetLocales(_Settings.PortalId).Keys)
            {
                var c = new CultureInfo(strLocale);
                if (specific)
                {
                    a.Add(c);
                }
                else
                {
                    CultureInfo p = c.Parent;
                    if (!a.Contains(p))
                    {
                        a.Add(p);
                    }
                }
            }
            return (CultureInfo[]) a.ToArray(typeof (CultureInfo));
        }
    }
}
