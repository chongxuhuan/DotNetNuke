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
    /// The module definition page object.
    /// Base on the page that existed before 6.X. 
    /// Will need to be updated to handle pop ups and UI overhaul.
    /// </summary>
    public class ModuleDefinitionPage : WatiNBase
    {
        #region Constructors

        public ModuleDefinitionPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ModuleDefinitionPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Spans
        public Span EditExtensionTitle
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("EditExtension_lblTitle"))); }
        }
        public Span UninstallPackageFriendlyNameSpan
        {
            get { return IEInstance.Table(Find.ById("dnn_ctr_UnInstall_ctlPackage_tbl")).Div(Find.ById(s => s.EndsWith("ctlPackage_fldFriendlyName"))).Divs[3].Span(Find.Any); }
        }
        public Span ModuleHelpInfoSpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("Help_lblInfo"))); }
        }
        #endregion

        #region Divs
        public Div UsageDetailsDiv
        {
            get { return IEInstance.Div(Find.ById(s => s.EndsWith("UsageDetails_UP"))); }
        }
        #endregion

        #region Links
        public Link InstallModuleLink
        {
            get { return IEInstance.Link(Find.ByTitle("Install Module")); }
        }
        public Link CreateNewModuleLink
        {
            get { return IEInstance.Link(Find.ByTitle("Create New Module")); }
        }
        public Link WizardNextButton
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("StartNextLinkButton"))); }
        }
        public Link CancelLink
        {
            get { return IEInstance.Link(Find.ByTitle("Cancel")); }
        }
        #endregion

        #region Tables
        public Table ModuleTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("Extensions_grdExtensions"))); }
        }
        #endregion

        #region TextFields
        public TextField ModuleTitleField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("ModuleSettings_txtTitle"))); }
        }
        #endregion

        #endregion

        #region Public Methods

        public TableRow GetModuleRow(string moduleName)
        {
            //Returns the module Row specified from the module table
            TableRow result = null;
            foreach (TableRow row in ModuleTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(moduleName))
                {
                    result = row;
                    break;
                }
                continue;
            }
            return result;
        }

        public Image GetModuleDeleteButton(string moduleName)
        {
            //Returns the Edit Button for the module specified 
            Image result = null;
            foreach (TableRow row in ModuleTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(moduleName))
                {
                    result = row.Image(Find.ByTitle("Uninstall this Extension"));
                    break;
                }
                continue;
            }
            return result;
        }

        public Image GetModuleEditButton(string moduleName)
        {
            //Returns the Edit Button for the module specified 
            Image result = null;
            foreach (TableRow row in ModuleTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(moduleName))
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