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
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The schedule page object
    /// </summary>
    public class SchedulePage : WatiNBase
    {
        #region Constructors

        public SchedulePage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public SchedulePage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Tables
        public Table ScheduleEventsTable
        {
            get { return ContentPaneDiv.Table(Find.ById(s => s.Contains("ViewSchedule_dgSchedule")));}
        }
        #endregion

        #region SelectLists
        public SelectList TimeLapseMeasurementSelectList
        {
            get { return ContentPaneDiv.SelectList(Find.ById(s => s.EndsWith("EditSchedule_ddlTimeLapseMeasurement"))); }
        }
        #endregion

        #region TextFields
        public TextField TimeLapseTextField
        {
            get { return ContentPaneDiv.TextField(Find.ById(s => s.EndsWith("EditSchedule_txtTimeLapse"))); }
        }
        #endregion

        #region Links
        public Link ViewScheduleStatusLink
        {
            get { return ContentPaneDiv.Link(Find.ByTitle("View Schedule Status")); }
        }
        #endregion

        #region CheckBoxes
        public CheckBox ScheduleEnabledCheckbox
        {
            get { return ContentPaneDiv.CheckBox(Find.ById(s => s.EndsWith("EditSchedule_chkEnabled"))); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the edit button for the schedule item.
        /// </summary>
        /// <param name="name">The name of the schedule item.</param>
        /// <returns>The edit image/button for the schedule item.</returns>
        public Image GetScheduleItemEditButton(string name)
        {
            //Returns the Edit Button for the schedule item specified 
            Image result = null;
            foreach (TableRow row in ScheduleEventsTable.TableRows)
            {
                if (row.TableCells.Count > 1 && row.TableCells[1].InnerHtml.Contains(name))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
                continue;
            }
            return result;
        }

        #endregion
    }
}
