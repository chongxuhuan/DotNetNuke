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
using System;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The Host Lists page object.
    /// </summary>
    public class HostListPage : WatiNBase
    {
        #region Constructors

        public HostListPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public HostListPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Spans
        public Span DataTypeSpan
        {
            get { return PageContentDiv.Span(Find.ByText("DataType")); }
        }
        [Obsolete("Element no longer exists.")]
        public Span ListItemTitleSpan
        {
            get { return PageContentDiv.Span(Find.ById(s => s.EndsWith("ListEditor_dshDetails_lblTitle"))); }
        }
        #endregion

        #region Tables
        /// <summary>
        /// The table containing the contents of the currently selected list.
        /// </summary>
        public Table ListTable
        {
            get { return PageContentDiv.Table(Find.ById(s => s.EndsWith("ListEditor_ListEntries_grdEntries"))); }
        }

        #endregion

        #region TableRows
        [Obsolete("Element no longer exists")]
        public TableRow ListItemDetailsRow
        {
            get { return PageContentDiv.TableRow(Find.ById(s => s.EndsWith("ListEditor_ListeEntries_rowEntryEdit"))); }
        }
        #endregion

        #region TextFields
        public TextField ListEntryText
        {
            get { return PageContentDiv.TextField(Find.ById(s => s.EndsWith("ListEditor_ListEntries_txtEntryText"))); }
        }

        public TextField ListEntryValue
        {
            get { return PageContentDiv.TextField(Find.ById(s => s.EndsWith("ListEditor_ListEntries_txtEntryValue"))); }
        }
        public TextField ListEntryName
        {
            get { return PageContentDiv.TextField(Find.ById(s => s.EndsWith("ListEditor_ListEntries_txtEntryName"))); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The div containing the list tree
        /// </summary>
        public Div TypesOfListsDiv
        {
            get { return PageContentDiv.Div(Find.ByClass("dnnListEditorTree")); }
        }
        #endregion

        #region Buttons

        public Link AddListButton
        {
            get
            {
                return PageContentDiv.Link(Find.ByTitle("Add List"));
            }
        }

        public Link AddEntryButton
        {
            get
            {
                return PageContentDiv.Link(Find.ByTitle("Add Entry"));
            }
        }

        public Link SaveButton
        {
            get
            {
                return PageContentDiv.Link(Find.ByTitle("Save"));
            }
        }

        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the Edit button for the entry in the list.
        /// </summary>
        /// <param name="listText">The text of the list entry.</param>
        /// <returns>The edit image/button for the list entry.</returns>
        public Image GetHostListEditButton(string listText)
        {
            //Returns the Edit Button for the List Item specified 
            Image result = null;
            foreach (TableRow row in ListTable.TableRows)
            {
                if ((row.TableCells[1].InnerHtml != null && row.TableCells[1].InnerHtml.Contains(listText))
                    || row.TableCells[2].InnerHtml.Contains(listText))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the Edit button for the entry in the list.
        /// </summary>
        /// <param name="listText">The text of the list entry.</param>
        /// <returns>The edit image/button for the list entry.</returns>
        public Image GetHostListDeleteButton(string listText)
        {
            //Returns the Edit Button for the List Item specified 
            Image result = null;
            foreach (TableRow row in ListTable.TableRows)
            {
                if ((row.TableCells[1].InnerHtml != null && row.TableCells[1].InnerHtml.Contains(listText))
                    || row.TableCells[2].InnerHtml.Contains(listText))
                {
                    result = row.Image(Find.ByTitle("Delete"));
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Finds the row for the entry in the list.
        /// </summary>
        /// <param name="listText">The text of the list entry.</param>
        /// <returns>The row for the list entry.</returns>
        public TableRow GetHostListRow(string listText)
        {
            TableRow result = null;
            foreach (TableRow row in ListTable.TableRows)
            {
                if ((row.TableCells[1].InnerHtml != null &&row.TableCells[1].InnerHtml.Contains(listText))
                    || row.TableCells[2].InnerHtml.Contains(listText))
                {
                    result = row;
                    break;
                }
                continue;
            }
            return result;
        }

        #endregion

    }
}