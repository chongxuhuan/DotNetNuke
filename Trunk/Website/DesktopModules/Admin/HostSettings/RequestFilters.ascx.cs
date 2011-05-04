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
using System.Collections.Generic;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.HttpModules.RequestFilter;
using DotNetNuke.Instrumentation;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Utilities;

#endregion

namespace DotNetNuke.Modules.Admin.Host
{
    public partial class RequestFilters : PortalModuleBase
    {
        private List<RequestFilterRule> _Rules;

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

        private List<RequestFilterRule> Rules
        {
            get
            {
                if (_Rules == null)
                {
                    _Rules = RequestFilterSettings.GetSettings().Rules;
                }
                return _Rules;
            }
            set
            {
                _Rules = value;
            }
        }

        private static void AddConfirmActiontoDeleteButton(DataListItemEventArgs e)
        {
            var cmdDelete = (ImageButton) e.Item.FindControl("cmdDelete");
            ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteItem"));
        }

        private void BindDropDownValues(DataListItemEventArgs e)
        {
            var rule = (RequestFilterRule) e.Item.DataItem;
            var ddlOperator = (DropDownList) e.Item.FindControl("ddlOperator");
            if (ddlOperator != null && rule != null)
            {
                ddlOperator.SelectedValue = rule.Operator.ToString();
            }
            var ddlAction = (DropDownList) e.Item.FindControl("ddlAction");
            if (ddlAction != null && rule != null)
            {
                ddlAction.SelectedValue = rule.Action.ToString();
            }
        }

        private void BindRules()
        {
            rptRules.DataSource = Rules;
            rptRules.DataBind();
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
                var configRules = new List<RequestFilterRule>();
                configRules = (List<RequestFilterRule>) XmlUtils.Deserialize(Convert.ToString(myState[1]), configRules.GetType());
                Rules = configRules;
            }
        }

        protected override object SaveViewState()
        {
            var configRules = new List<RequestFilterRule>();
            configRules = Rules;
            object baseState = base.SaveViewState();
            var allStates = new object[2];
            allStates[0] = baseState;
            allStates[1] = XmlUtils.Serialize(configRules);
            return allStates;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cmdAddRule.Click += AddRule;
            rptRules.ItemDataBound += rptRules_ItemDataBound;
            rptRules.EditCommand += EditRule;
            rptRules.DeleteCommand += DeleteRule;
            rptRules.UpdateCommand += SaveRule;
            rptRules.CancelCommand += CancelEdit;

            if (!Page.IsPostBack)
            {
                BindRules();
            }
        }

        protected void AddRule(object sender, EventArgs e)
        {
            Rules.Add(new RequestFilterRule());
            rptRules.EditItemIndex = Rules.Count - 1;
            AddMode = true;
            BindRules();
        }

        protected void DeleteRule(object source, DataListCommandEventArgs e)
        {
            int index = e.Item.ItemIndex;
            Rules.RemoveAt(index);
            try
            {
                RequestFilterSettings.Save(Rules);
            }
            catch (UnauthorizedAccessException exc)
            {
                DnnLog.Debug(exc);

                lblErr.InnerText = Localization.GetString("unauthorized", LocalResourceFile);
                lblErr.Visible = true;
                Rules = null;
            }
            BindRules();
        }

        protected void EditRule(object source, DataListCommandEventArgs e)
        {
            lblErr.Visible = true;
            AddMode = false;
            rptRules.EditItemIndex = e.Item.ItemIndex;
            BindRules();
        }

        protected void SaveRule(object source, DataListCommandEventArgs e)
        {
            int index = rptRules.EditItemIndex;
            RequestFilterRule rule = Rules[index];
            var txtServerVar = (TextBox) e.Item.FindControl("txtServerVar");
            var txtValue = (TextBox) e.Item.FindControl("txtValue");
            var txtLocation = (TextBox) e.Item.FindControl("txtLocation");
            var ddlOperator = (DropDownList) e.Item.FindControl("ddlOperator");
            var ddlAction = (DropDownList) e.Item.FindControl("ddlAction");
            if (!String.IsNullOrEmpty(txtServerVar.Text) && !String.IsNullOrEmpty(txtValue.Text))
            {
                rule.ServerVariable = txtServerVar.Text;
                rule.Location = txtLocation.Text;
                rule.Operator = (RequestFilterOperatorType) Enum.Parse(typeof (RequestFilterOperatorType), ddlOperator.SelectedValue);
                rule.Action = (RequestFilterRuleType) Enum.Parse(typeof (RequestFilterRuleType), ddlAction.SelectedValue);
                rule.SetValues(txtValue.Text, rule.Operator);
                RequestFilterSettings.Save(Rules);
            }
            else
            {
                if (AddMode)
                {
                    Rules.RemoveAt(Rules.Count - 1);
                }
            }
            AddMode = false;
            rptRules.EditItemIndex = -1;
            BindRules();
        }

        protected void CancelEdit(object source, DataListCommandEventArgs e)
        {
            if (AddMode)
            {
                Rules.RemoveAt(Rules.Count - 1);
                AddMode = false;
            }
            rptRules.EditItemIndex = -1;
            BindRules();
        }

        protected void rptRules_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            switch (e.Item.ItemType)
            {
                case ListItemType.AlternatingItem:
                    AddConfirmActiontoDeleteButton(e);
                    break;
                case ListItemType.Item:
                    AddConfirmActiontoDeleteButton(e);
                    break;
                case ListItemType.EditItem:
                    BindDropDownValues(e);
                    break;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            cmdAddRule.Enabled = rptRules.EditItemIndex == -1;
        }
    }
}