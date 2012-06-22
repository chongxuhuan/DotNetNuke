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
    /// The Event Viewer page object
    /// </summary>
    public class EventViewerPage : WatiNBase
    {
        #region Constructors

        public EventViewerPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public EventViewerPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Links
        public Link EditLogSettingsLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Edit Log Settings"))); }
        }
        #endregion

        #region Tables
        /// <summary>
        /// The log type table. 
        /// This object is only visible from the Edit Log Settings popup/page.
        /// </summary>
        public Table LogTypeTable
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Table(Find.ById(s => s.EndsWith("EditLogTypes_dgLogTypeConfigInfo")));
                }
                return IEInstance.Table(Find.ById(s => s.EndsWith("EditLogTypes_dgLogTypeConfigInfo")));
            }
        }
        
        [Obsolete("No Longer needed in 6.X")]
        public Table LogViewerTable
        {
            get
            {
                return IEInstance.Table(Find.ById(s => s.EndsWith("LogViewer_dlLog")));
            }
        }
        #endregion 

        #region Checkboxes
        public CheckBox EmailNotificationEnabledCheckbox
        {
            get { return ContentPaneDiv.CheckBox(Find.ById(s => s.EndsWith("EditLogTypes_chkEmailNotificationStatus"))); }
        }
        public CheckBox EnableLoggingCheckbox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("EditLogTypes_chkIsActive")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("EditLogTypes_chkIsActive")));
            }
        }        
        #endregion

        #region SelectLists
        public SelectList KeepMostRecentEntriesSelectList
        {
            get { return ContentPaneDiv.SelectList(Find.ById(s => s.EndsWith("EditLogTypes_ddlKeepMostRecent"))); }
        }
        public SelectList LogTypeSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("LogViewer_ddlLogType"))); }
        }
        public SelectList RecordsPerPageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("LogViewer_ddlRecordsPerPage"))); }
        }
        #endregion

        #region TextFields
        public TextField MailFromAddressField
        {
            get { return ContentPaneDiv.TextField(Find.ById(s => s.EndsWith("EditLogTypes_txtMailFromAddress"))); }
        }

        public TextField MailToAddressField
        {
            get { return ContentPaneDiv.TextField(Find.ById(s => s.EndsWith("EditLogTypes_txtMailToAddress"))); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The div containing the header of the log table.
        /// </summary>
        public Div LogHeaderDiv
        {
            get { return IEInstance.Div(Find.ByClass("dnnLogHeader dnnClear")); }
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the edit image for the log type.
        /// </summary>
        /// <param name="logType">The log type.</param>
        /// <returns>The edit image/button.</returns>
        public Image GetLogTypeEditButton(string logType)
        {
            //Returns the edit button for the specified log type
            Image button = null;
            foreach (TableRow row in LogTypeTable.TableRows)
            {
                if (row.TableCells[1].Text.Contains(logType))
                {
                    button = row.TableCells[0].Image(Find.Any);
                    break;
                }
                continue;
            }
            return button;
        }


        #endregion
    }
}