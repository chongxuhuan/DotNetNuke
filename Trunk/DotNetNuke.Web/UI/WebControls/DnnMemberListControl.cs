#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Social;
using DotNetNuke.Services.Tokens;

#endregion

namespace DotNetNuke.Web.UI.WebControls
{
    /// <summary>
    /// This control is used for displaying a template based list of users based upon various filter and sorting capabilities.
    /// </summary>
    [ToolboxData("<{0}:DnnMemberListControl runat=\"server\"></{0}:DnnMemberListControl>")]
    public class DnnMemberListControl : WebControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets the template for displaying the alternating items in a DnnMemberListControl object.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string AlternatingItemTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for displaying the alternating row footers in a DnnMemberListControl object.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string AlternatingRowFooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for displaying the alternating row headers in a DnnMemberListControl object.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string AlternatingRowHeaderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the collection of filters to apply when getting the list of members.
        /// </summary>
        /// <remarks>
        /// Posible keys are: RoleID, RelationshipType, UserID, Profile:PropertyName, FirstName, LastName, DisplayName, Username, Email.
        /// </remarks>
        public IDictionary<string, IList<object>> Filters { get; set; }

        /// <summary>
        /// Gets or sets the template for displaying the footer section of a DnnMemberListControl object.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string FooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for displaying the header section of a DnnMemberListControl object.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string HeaderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for displaying an item in a DnnMemberListControl object.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string ItemTemplate { get; set; }

        /// <summary>
        /// Gets or sets the index of the currently displayed page.
        /// </summary>
        [DefaultValue(1)]
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the number of records to display on a page in a DnnMemberListControl object.
        /// </summary>
        [DefaultValue(10)]
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the template for the row footer.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string RowFooterTemplate { get; set; }

        /// <summary>
        /// Gets or sets the template for the row header.
        /// </summary>
        [DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty)]
        public string RowHeaderTemplate { get; set; }

        /// <summary>
        /// Gets or sets the number of items displayed on each row.
        /// </summary>
        [DefaultValue(1)]
        public int RowSize { get; set; }

        #endregion

        #region Private Variables

        private UserInfo _currentUser;
        private RelationshipController _relationshipController;

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            _currentUser = UserController.GetCurrentUserInfo();
            _relationshipController = new RelationshipController();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (ItemTemplate == "") return;

            writer.Write(HeaderTemplate);

            var row = 0;
            var rowItem = 0;
            
            var users = new DataTable();
            users.Load(_relationshipController.GetUsersByFilters(0, _currentUser, PageSize, 0, null, null, "", "", "", true, false));

            foreach (DataRow user in users.Rows)
            {
               if (rowItem == 0)
                {
                    writer.Write(string.IsNullOrEmpty(AlternatingRowHeaderTemplate) || row % 2 == 0 ? RowHeaderTemplate : AlternatingRowHeaderTemplate);
                }
                
                var tokenReplace = new TokenReplace();
                var tokenKeyValues = new Dictionary<string, string>();
                // Reading data from datarow into a KeyValuePair dictionary for usage in TokenRepalace
                foreach (DataColumn col in user.Table.Columns.Cast<DataColumn>().Where(col => !tokenKeyValues.ContainsKey(col.ColumnName.Replace("Member:", "").Replace("MemberProfile:", ""))))
                {
                    tokenKeyValues.Add(col.ColumnName.Replace("Member:", "").Replace("MemberProfile:", ""), user[col.ColumnName].ToString());
                }

                var listItem = string.IsNullOrEmpty(AlternatingItemTemplate) || row%2 == 0 ? ItemTemplate : AlternatingItemTemplate;
                listItem = tokenReplace.ReplaceEnvironmentTokens(listItem, tokenKeyValues, "Member");
                listItem = tokenReplace.ReplaceEnvironmentTokens(listItem, tokenKeyValues, "MemberProfile");
                writer.Write(listItem);

                rowItem++;
                if (rowItem != RowSize) continue;

                writer.Write(string.IsNullOrEmpty(AlternatingRowFooterTemplate) || row % 2 == 0 ? RowFooterTemplate : AlternatingRowFooterTemplate);
                rowItem = 0;
                row++;
            }

            writer.Write(FooterTemplate);
        }

        #endregion
    }
}
