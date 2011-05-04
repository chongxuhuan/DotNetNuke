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
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.HttpModules.Config;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Modules.Admin.Host
{
    public partial class FriendlyUrls : PortalModuleBase
    {
        private RewriterRuleCollection _Rules;

        private bool AddMode
        {
            get
            {
                bool _Mode = Null.NullBoolean;
                if (ViewState["Mode"] != null)
                {
                    _Mode = Convert.ToBoolean(ViewState["Mode"]);
                }
                return _Mode;
            }
            set
            {
                ViewState["Mode"] = value;
            }
        }

        private RewriterRuleCollection Rules
        {
            get
            {
                if (_Rules == null)
                {
                    _Rules = RewriterConfiguration.GetConfig().Rules;
                }
                return _Rules;
            }
            set
            {
                _Rules = value;
            }
        }

        private void BindRules()
        {
            grdRules.DataSource = Rules;
            grdRules.DataBind();
        }

        protected override void LoadViewState(object savedState)
        {
            var myState = (object[]) savedState;
            if ((myState[0] != null))
            {
                base.LoadViewState(myState[0]);
            }
            if ((myState[1] != null))
            {
                var config = new RewriterConfiguration();
                config = (RewriterConfiguration) XmlUtils.Deserialize(Convert.ToString(myState[1]), config.GetType());
                Rules = config.Rules;
            }
        }

        protected override object SaveViewState()
        {
            var config = new RewriterConfiguration();
            config.Rules = Rules;
            object baseState = base.SaveViewState();
            var allStates = new object[2];
            allStates[0] = baseState;
            allStates[1] = XmlUtils.Serialize(config);
            return allStates;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            foreach (DataGridColumn column in grdRules.Columns)
            {
                if (ReferenceEquals(column.GetType(), typeof (ImageCommandColumn)))
                {
                    var imageColumn = (ImageCommandColumn) column;
                    if (imageColumn.CommandName == "Delete")
                    {
                        imageColumn.OnClickJS = Localization.GetString("DeleteItem");
                    }
                    if (!String.IsNullOrEmpty(imageColumn.CommandName))
                    {
                        imageColumn.Text = Localization.GetString(imageColumn.CommandName, LocalResourceFile);
                    }
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                Localization.LocalizeDataGrid(ref grdRules, LocalResourceFile);
                BindRules();
            }
        }

        private void AddRule(object sender, EventArgs e)
        {
            Rules.Add(new RewriterRule());
            grdRules.EditItemIndex = Rules.Count - 1;
            AddMode = true;
            BindRules();
        }

        private void DeleteRule(object source, DataGridCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Rules.RemoveAt(index);
            RewriterConfiguration.SaveConfig(Rules);
            BindRules();
        }

        private void EditRule(object source, DataGridCommandEventArgs e)
        {
            AddMode = false;
            grdRules.EditItemIndex = e.Item.ItemIndex;
            BindRules();
        }

        protected void SaveRule(object source, CommandEventArgs e)
        {
            int index = grdRules.EditItemIndex;
            RewriterRule rule = Rules[index];
            var ctlMatch = (TextBox) grdRules.Items[index].Cells[2].FindControl("txtMatch");
            var ctlReplace = (TextBox) grdRules.Items[index].Cells[2].FindControl("txtReplace");
            if (!String.IsNullOrEmpty(ctlMatch.Text) && !String.IsNullOrEmpty(ctlReplace.Text))
            {
                rule.LookFor = ctlMatch.Text;
                rule.SendTo = ctlReplace.Text;
                RewriterConfiguration.SaveConfig(Rules);
            }
            else
            {
                if (AddMode)
                {
                    Rules.RemoveAt(Rules.Count - 1);
                    AddMode = false;
                }
            }
            grdRules.EditItemIndex = -1;
            BindRules();
        }

        protected void CancelEdit(object source, CommandEventArgs e)
        {
            if (AddMode)
            {
                Rules.RemoveAt(Rules.Count - 1);
                AddMode = false;
            }
            grdRules.EditItemIndex = -1;
            BindRules();
        }
    }
}