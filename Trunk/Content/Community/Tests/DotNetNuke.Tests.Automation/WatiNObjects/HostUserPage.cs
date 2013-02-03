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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The superuser accounts page object.
    /// Inherits most of its properties from the User Accounts page object.
    /// </summary>
    public class HostUserPage : UserPage
    {
        #region Constructors

        public HostUserPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public HostUserPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the edit button for the Superuser.
        /// </summary>
        /// <param name="displayName">The Superusers display name.</param>
        /// <returns>The edit image/button for the superuser.</returns>
        public override Image GetUserEditButton(string displayName)
        {
            //Returns the Edit Button for the user specified 
            Image result = null;
            foreach (TableRow row in UserTable.TableRows)
            {
                if (row.TableCells[6].InnerHtml.Contains(displayName))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the delete button for the Superuser.
        /// </summary>
        /// <param name="displayName">The Superuser's display name.</param>
        /// <returns>The delete image/button for the superuser.</returns>
        public override Image GetUserDeleteButton(string displayName)
        {
            //Returns the Delete Button for the user specified 
            Image result = null;
            foreach (TableRow row in UserTable.TableRows)
            {
                if (row.TableCells.Count > 6 && row.TableCells[6].InnerHtml.Contains(displayName))
                {
                    result = row.Image(Find.ByTitle("Delete"));
                    break;
                }
            }
            return result;

        }

        /// <summary>
        /// Finds the remove button (hard delete button) for the Superuser.
        /// </summary>
        /// <param name="displayName">The Superuser's display name.</param>
        /// <returns>The remove (hard delete) image/button for the superuser.</returns>
		public virtual Image GetUserRemoveButton(string displayName)
		{
			//Returns the Remove Button for the user specified 
			Image result = null;
			foreach (TableRow row in UserTable.TableRows)
			{
				if (row.TableCells[6].InnerHtml.Contains(displayName))
				{
					result = row.Image(Find.ByTitle("Remove"));
					break;
				}
			}
			return result;
		}
        
        /// <summary>
        /// Checks if the superuser table contains a user online image.
        /// </summary>
        /// <returns></returns>
        public bool UserGridContainsUserOnlineImage()
        {
            if (UserTable.Images.Filter(Find.ById(s => s.EndsWith("imgOnline"))).Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if after being soft deleted a superuser is displayed in the table as deleted.
        /// </summary>
        /// <param name="displayName">The superuser's display name.</param>
        /// <returns>Returns true if the superuser is displayed as deleted, and false otherwise. </returns>
        public override bool CheckUserDisplaysAsDeleted(string displayName)
        {
            bool deleted = true;
            TableCellCollection userRowCells = GetUserRow(displayName).OwnTableCells;
            foreach (TableCell cell in userRowCells)
            {
                if (cell.ClassName != "NormalDeleted")
                {
                    deleted = false;
                }
            }
            return deleted;
        }
        #endregion
    }
}