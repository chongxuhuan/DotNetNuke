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
using System.IO;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The host SQL page object.
    /// </summary>
    public class HostSQLPage : WatiNBase
    {
        #region Constructors

        public HostSQLPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public HostSQLPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region TextFields
        public TextField SQLQueryTextField
        {
            get { return ContentPaneDiv.TextField(Find.ById(s => s.EndsWith("SQL_txtQuery")));}
        }
        #endregion

        #region Links
        public Link ExecuteSQLLink
        {
            get { return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("SQL_cmdExecute"))); }
        }
        #endregion

        #region Tables
        /// <summary>
        /// The table containing the results from the query
        /// </summary>
        public Table QueryResultsTable
        {
            get { return ContentPaneDiv.Table(Find.ById(s => s.EndsWith("SQL_gvResults"))); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Runs a query that checks the event log table for recent exceptions (within the last day).
        /// Writes any exceptions to a file.
        /// </summary>
        /// <param name="filePath">The path of the file that the exceptions will be recorded in.</param>
        /// <param name="qualifier">The qualifier for the database. </param>
        public void CheckForRecentExceptionsWriteToFile(string filePath, string qualifier)
        {
            DateTime currentDate = DateTime.Now;
            int year = currentDate.Year;
            int month = currentDate.Month;
            int day = currentDate.Day;
            if (day < 2)
            {
                month = month - 1;
                if (month == 01 || month == 03 || month == 05 || month == 07 || month == 08 || month == 10 || month == 12)
                {
                    day = 31;
                }
                else if (month == 04 || month == 06 || month == 09 || month == 11)
                {
                    day = 30;
                }
                else
                {
                    if (DateTime.IsLeapYear(currentDate.Year))
                    {
                        day = 29;
                    }
                    else
                    {
                        day = 28;
                    }

                }
            }
            else
            {
                day = day - 1;
            }
            if (qualifier != "")
            {
                SQLQueryTextField.Value = "SELECT LogTypeKey, LogCreateDate, LogProperties FROM dbo." + qualifier +  "_EventLog WHERE LogTypeKey LIKE '%EXCEPTION' AND LogCreateDate>'" + year + "-" + month + "-" + day + " 23:59:59.996'";
            }
            else
            {
                SQLQueryTextField.Value = "SELECT LogTypeKey, LogCreateDate, LogProperties FROM EventLog WHERE LogTypeKey LIKE '%EXCEPTION' AND LogCreateDate>'" + year + "-" + month + "-" + day + " 23:59:59.996'";
            }
            ExecuteSQLLink.Click();
            System.Threading.Thread.Sleep(2000);

            if (!QueryResultsTable.InnerHtml.Contains("The query did not return any data"))
            {
                TableRowCollection resultsRows = QueryResultsTable.TableRows.Filter(Find.ByClass("Normal"));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                StreamWriter sWriter = File.CreateText(filePath);
                foreach (TableRow row in resultsRows)
                {
                    sWriter.WriteLine(row.OwnTableCells[0].Text + " on " + row.OwnTableCells[1] + ":");
                    sWriter.WriteLine(row.OwnTableCells[2].Text);
                }
                sWriter.Close();
            }
        }

        /// <summary>
        /// Runs a query that checks the event log table for recent exceptions (within the last day).
        /// Writes any exceptions to a file.
        /// </summary>
        /// <param name="filePath">The path of the file that the exceptions will be recorded in.</param>
        public void CheckForRecentExceptionsWriteToFile(string filePath)
        {
            DateTime currentDate = DateTime.Now;
            int year = currentDate.Year;
            int month = currentDate.Month;
            int day = currentDate.Day;
            if (day < 2)
            {
                month = month - 1;
                if (month == 01 || month == 03 || month == 05 || month == 07 || month == 08 || month == 10 || month == 12)
                {
                    day = 31;
                }
                else if (month == 04 || month == 06 || month == 09 || month == 11)
                {
                    day = 30;
                }
                else
                {
                    if (DateTime.IsLeapYear(currentDate.Year))
                    {
                        day = 29;
                    }
                    else
                    {
                        day = 28;
                    }

                }
            }
            else
            {
                day = day - 1;
            }
            SQLQueryTextField.Value = "SELECT LogTypeKey, LogCreateDate, LogProperties FROM EventLog WHERE LogTypeKey LIKE '%EXCEPTION' AND LogCreateDate>'" + year + "-" + month + "-" + day + " 23:59:59.996'";
            ExecuteSQLLink.Click();
            System.Threading.Thread.Sleep(2000);

            if (!QueryResultsTable.InnerHtml.Contains("The query did not return any data"))
            {
                TableRowCollection resultsRows = QueryResultsTable.TableRows.Filter(Find.ByClass("Normal"));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                StreamWriter sWriter = File.CreateText(filePath);
                foreach (TableRow row in resultsRows)
                {
                    sWriter.WriteLine(row.OwnTableCells[0].Text + " on " + row.OwnTableCells[1] + ":");
                    sWriter.WriteLine(row.OwnTableCells[2].Text);
                }
                sWriter.Close();
            } 
        }


        #endregion
    }
}
