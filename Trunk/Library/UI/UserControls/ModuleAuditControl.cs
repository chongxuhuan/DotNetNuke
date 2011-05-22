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
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.UserControls
{
    public abstract class ModuleAuditControl : UserControl
    {
        private string MyFileName = "ModuleAuditControl.ascx";
        private string _SystemUser = Localization.GetString("SystemUser");
        protected Label lblCreatedBy;
        protected Label lblUpdatedBy;

        public ModuleAuditControl()
        {
            LastModifiedDate = string.Empty;
            LastModifiedByUser = string.Empty;
            CreatedByUser = "";
            CreatedDate = "";
        }

        public string CreatedDate { private get; set; }

        public string CreatedByUser { private get; set; }

        public string LastModifiedByUser { private get; set; }

        public string LastModifiedDate { private get; set; }

        public BaseEntityInfo Entity { private get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (Entity != null)
                {
                    CreatedByUser = Entity.CreatedByUserID.ToString();
                    CreatedDate = Entity.CreatedOnDate.ToString();
                    LastModifiedByUser = Entity.LastModifiedByUserID.ToString();
                    LastModifiedDate = Entity.LastModifiedOnDate.ToString();
                }
				
                //check to see if updated check is redundant
                bool isCreatorAndUpdater = false;
                if (Regex.IsMatch(CreatedByUser, "^\\d+$") && Regex.IsMatch(LastModifiedByUser, "^\\d+$") && CreatedByUser == LastModifiedByUser)
                {
                    isCreatorAndUpdater = true;
                }

				_SystemUser = Localization.GetString("SystemUser", Localization.GetResourceFile(this, MyFileName));
				ShowCreatedString();
                ShowUpdatedString(isCreatorAndUpdater);
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private string ShowCreatedString()
        {
            if (Regex.IsMatch(CreatedByUser, @"^-?\d+$"))
            {
                if (int.Parse(CreatedByUser) == Null.NullInteger)
                {
                    CreatedByUser = _SystemUser;
                }
                else
                {
					//contains a UserID
                    var objUsers = new UserController();
                    UserInfo objUser = UserController.GetUserById(PortalController.GetCurrentPortalSettings().PortalId, int.Parse(CreatedByUser));
                    if (objUser != null)
                    {
                        CreatedByUser = objUser.DisplayName;
                    }
                }
            }
            string str = Localization.GetString("CreatedBy", Localization.GetResourceFile(this, MyFileName));
            lblCreatedBy.Text = string.Format(str, CreatedByUser, CreatedDate);
            return str;
        }

        private void ShowUpdatedString(bool isCreatorAndUpdater)
        {
			//check to see if audit contains update information
            if (string.IsNullOrEmpty(LastModifiedDate))
            {
                return;
            }
            string str = string.Empty;

            if (Regex.IsMatch(LastModifiedByUser, @"^-?\d+$"))
            {
                if (isCreatorAndUpdater)
                {
                    LastModifiedByUser = CreatedByUser;
                }
                else if (int.Parse(LastModifiedByUser) == Null.NullInteger)
                {
                    LastModifiedByUser = _SystemUser;
                }
                else
                {
					//contains a UserID
                    var objUsers = new UserController();
                    UserInfo objUser = UserController.GetUserById(PortalController.GetCurrentPortalSettings().PortalId, int.Parse(LastModifiedByUser));
                    if (objUser != null)
                    {
                        LastModifiedByUser = objUser.DisplayName;
                    }
                }
            }
            
            str = Localization.GetString("UpdatedBy", Localization.GetResourceFile(this, MyFileName));
            lblUpdatedBy.Text = string.Format(str, LastModifiedByUser, LastModifiedDate);
        }
    }
}