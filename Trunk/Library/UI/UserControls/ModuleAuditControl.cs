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
        private string _CreatedByUser = "";
        private string _CreatedDate = "";
        private BaseEntityInfo _Entity;
        private string _LastModifiedByUser = string.Empty;
        private string _LastModifiedDate = string.Empty;
        private string _SystemUser = string.Empty;
        protected Label lblCreatedBy;
        protected Label lblUpdatedBy;

        public string CreatedDate
        {
            set
            {
                _CreatedDate = value;
            }
        }

        public string CreatedByUser
        {
            set
            {
                _CreatedByUser = value;
            }
        }

        public string LastModifiedByUser
        {
            set
            {
                _LastModifiedByUser = value;
            }
        }

        public string LastModifiedDate
        {
            set
            {
                _LastModifiedDate = value;
            }
        }

        public BaseEntityInfo Entity
        {
            set
            {
                _Entity = value;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                if (_Entity != null)
                {
                    _CreatedByUser = _Entity.CreatedByUserID.ToString();
                    _CreatedDate = _Entity.CreatedOnDate.ToString();
                    _LastModifiedByUser = _Entity.LastModifiedByUserID.ToString();
                    _LastModifiedDate = _Entity.LastModifiedOnDate.ToString();
                }
				
                //check to see if updated check is redundant
                bool isCreatorAndUpdater = false;
                if (Regex.IsMatch(_CreatedByUser, "^\\d+$") && Regex.IsMatch(_LastModifiedByUser, "^\\d+$") && _CreatedByUser == _LastModifiedByUser)
                {
                    isCreatorAndUpdater = true;
                }

				_SystemUser = Localization.GetString("SystemUser", Localization.GetResourceFile(this, MyFileName));
				ShowCreatedString();
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
            if (Regex.IsMatch(_CreatedByUser, "^\\d+$"))
            {
                if (int.Parse(_CreatedByUser) == Null.NullInteger)
                {
                    _CreatedByUser = _SystemUser;
                }
                else
                {
					//contains a UserID
                    var objUsers = new UserController();
                    UserInfo objUser = UserController.GetUserById(PortalController.GetCurrentPortalSettings().PortalId, int.Parse(_CreatedByUser));
                    if (objUser != null)
                    {
                        _CreatedByUser = objUser.DisplayName;
                    }
                }
            }
            string str = Localization.GetString("CreatedBy", Localization.GetResourceFile(this, MyFileName));
            lblCreatedBy.Text = string.Format(str, _CreatedByUser, _CreatedDate);
            return str;
        }

        private void ShowUpdatedString(bool isCreatorAndUpdater)
        {
			//check to see if audit contains update information
            if (string.IsNullOrEmpty(_LastModifiedDate))
            {
                return;
            }
            string str = string.Empty;

            if (Regex.IsMatch(_LastModifiedByUser, "^\\d+$"))
            {
                if (isCreatorAndUpdater)
                {
                    _LastModifiedByUser = _CreatedByUser;
                }
                else if (int.Parse(_LastModifiedByUser) == Null.NullInteger)
                {
                    _LastModifiedByUser = _SystemUser;
                }
                else
                {
					//contains a UserID
                    var objUsers = new UserController();
                    UserInfo objUser = UserController.GetUserById(PortalController.GetCurrentPortalSettings().PortalId, int.Parse(_LastModifiedByUser));
                    if (objUser != null)
                    {
                        _LastModifiedByUser = objUser.DisplayName;
                    }
                }
            }
            
            str = Localization.GetString("UpdatedBy", Localization.GetResourceFile(this, MyFileName));
            lblUpdatedBy.Text = string.Format(str, _LastModifiedByUser, _LastModifiedDate);
        }
    }
}