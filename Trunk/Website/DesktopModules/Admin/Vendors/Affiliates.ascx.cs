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

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Vendors;

#endregion

namespace DotNetNuke.Modules.Admin.Vendors
{
    public partial class Affiliates : PortalModuleBase, IActionable
    {
        public int VendorID;

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var Actions = new ModuleActionCollection();
                if ((Request.QueryString["VendorID"] != null))
                {
                    VendorID = Int32.Parse(Request.QueryString["VendorID"]);
                }
                else
                {
                    VendorID = Null.NullInteger;
                }
                Actions.Add(GetNextActionID(),
                            Localization.GetString(ModuleActionType.AddContent, LocalResourceFile),
                            ModuleActionType.AddContent,
                            "",
                            "",
                            EditUrl("VendorId", VendorID.ToString(), "Affiliate"),
                            false,
                            SecurityAccessLevel.Admin,
                            Null.IsNull(VendorID) == false,
                            false);
                return Actions;
            }
        }

        #endregion

        private void BindData()
        {
            var objAffiliates = new AffiliateController();
            Localization.LocalizeDataGrid(ref grdAffiliates, LocalResourceFile);
            grdAffiliates.DataSource = objAffiliates.GetAffiliates(VendorID);
            grdAffiliates.DataBind();
            cmdAdd.NavigateUrl = FormatURL("AffilId", "-1");
        }

        public string DisplayDate(DateTime DateValue)
        {
            if (Null.IsNull(DateValue))
            {
                return "";
            }
            else
            {
                return DateValue.ToShortDateString();
            }
        }

        public string FormatURL(string strKeyName, string strKeyValue)
        {
            return EditUrl(strKeyName, strKeyValue, "Affiliate", "VendorId=" + VendorID);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                if (!Null.IsNull(VendorID))
                {
                    BindData();
                }
                else
                {
                    Visible = false;
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void InitializeComponent()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeComponent();
        }
    }
}