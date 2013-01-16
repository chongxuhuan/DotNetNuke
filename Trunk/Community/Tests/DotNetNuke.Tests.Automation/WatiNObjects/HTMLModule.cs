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
using System.Text.RegularExpressions;
using System.Threading;

using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The HTML module object.
    /// Contains elements from both the Edit Content page and the HTML module settings page.
    /// </summary>
    public class HTMLModule : ModulePage
    {
        private TelerikEditor telerikEditor;

        #region Constructors

        public HTMLModule(WatiNBase watinBase)
            : this(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName)
        {
        }

        public HTMLModule(IE ieInstance, string siteUrl, string dbName)
            : base(ieInstance, siteUrl, dbName)
        {
            telerikEditor = new TelerikEditor(ieInstance, siteUrl, dbName);
        }

        #endregion

        #region Public Properties

        #region TelerikEditor
        public TelerikEditor TelerikEditor
        {
            get { return telerikEditor; }
        }
        #endregion

        #region Links

        public new Link SaveContentLink
        {
            get { return FindElement<Link>(Find.ById(s => s.EndsWith("EditHTML_cmdSave"))); }
        }

        /// <summary>
        /// The manage workflows link found on the HTML module settings page.
        /// </summary>
        public Link ManageWorkFlowsLink
        {
            get { return FindElement<Link>(Find.ByText(s => s.Contains("Manage Workflows"))); }
        }
        public Link PreviewContentLink
        {
            get { return PageContentDiv.Link(Find.ByTitle("Preview")); }
        }   
        [Obsolete("Element no longer exists.")]
        public Link SaveVersionHistoryLink
        {
            get { return FindElement<Link>(Find.ById(s => s.EndsWith("Workflow_cmdSaveVersions"))); }
        }

        public Link AddCommentLink
        {
            get
            {
                return FindElement<Link>(Find.ById(s => s.EndsWith("EditHTML_cmdSubmitComment")));
            }
        }
        #endregion

        #region Tables
        [Obsolete("Element no longer exists.")]
        public Table PreviewTable
        {
            get { return PageContentDiv.Table(Find.ById(s => s.EndsWith("EditHTML_tblPreview"))); }
        }
        public Table VersionTable
        {
            get { return PageContentDiv.Table(Find.ById(s => s.Contains("EditHTML_grdVersions"))); }
        }
        #endregion

        #region SelectLists
        /// <summary>
        /// The workflow selectlist found on the HTML module settings page.
        /// </summary>
        public SelectList WorkflowSelectList
        {
            get
            {
                return FindElement<SelectList>(Find.ById(s => s.EndsWith("ModuleSettings_Settings_cboWorkflow")));
            }
        }
        #endregion

        #region Images
        [Obsolete("Element no longer exists in 6.X")]
        public Image VersionHistorySettingsExpandButton
        {
            get { return PageContentDiv.Image(Find.ById(s => s.EndsWith("Workflow_dshVersions_imgIcon"))); }
        }
        [Obsolete("Element no longer exists in 6.X")]
        public Image VersionHistoryTableExpandButton
        {
            get { return PageContentDiv.Image(Find.ById(s => s.EndsWith("EditHTML_dshVersions_imgIcon"))); }
        }
        #endregion

        #region TextFields
        public TextField ModuleTextBox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(new Regex(".*_EditHTML_txtContent_TxtDesktopHTML", RegexOptions.IgnoreCase)));
                }
                return IEInstance.TextField(Find.ById(new Regex(".*_EditHTML_txtContent_TxtDesktopHTML", RegexOptions.IgnoreCase)));
            }
        }
        public TextField VersionHistoryTextField
        {
            get { return PageContentDiv.TextField(Find.ById(s => s.EndsWith("Workflow_txtMaximumVersionHistory"))); }
        }
        public TextField MaximumHistoryField
        {
            get { return ContentPaneDiv.TextField(Find.ById(s => s.EndsWith("Settings_txtMaxVersions"))); }
        }

        public TextField CommentTextField
        {
            get
            {
                return FindElement<TextField>(Find.ById(s => s.EndsWith("EditHTML_txtComment")));
            }
        }

        #endregion

        #region CheckBoxes
        public CheckBox PublishCheckBox
        {
            get { return FindElement<CheckBox>(Find.ById(s => s.EndsWith("EditHTML_chkPublish"))); }
        }
        #endregion

        #region Divs
        /// <summary>
        /// A div containing the content of the first HTML module on the page.
        /// </summary>
        public Div HTMLContentDiv
        {
            get { return ContentPaneDiv.Div(Find.ById(s => s.EndsWith("HtmlModule_lblContent"))); }
        }
        public Div EditHTMLDiv
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ById(s => s.EndsWith("EditHTML_txtContent_txtContent")));
                }
                return ContentPaneDiv.Div(Find.ById(s => s.EndsWith("EditHTML_txtContent_txtContent")));
            }
        }

        #endregion

        #region Radio buttons
        public new RadioButton BasicTextBoxRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ById(s => s.EndsWith("EditHTML_txtContent_OptView_0")));
                }
                return IEInstance.RadioButton(Find.ById(s => s.EndsWith("EditHTML_txtContent_OptView_0")));
            }
        }
        public RadioButton RichTextBoxRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ById(s => s.EndsWith("EditHTML_txtContent_OptView_1")));
                }
                return IEInstance.RadioButton(Find.ById(s => s.EndsWith("EditHTML_txtContent_OptView_1")));
            }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds and publishes content to the html module.
        /// Content will be added using the basic text editor.
        /// </summary>
        /// <param name="content">The content to add to the HTML module.</param>
        /// <param name="moduleNum">The index of the module on the current page. To reference the first module on the page enter 0 as the moduleNum.</param>
        public void AddContentToModulePublishModule(string content, int moduleNum)
        {
            GetEditModuleContentLink(moduleNum).Click();
            SelectTextEditorStyleByName("BASIC");
            ModuleTextBox.Value = content;
            PublishCheckBox.Checked = true;
            SaveContentLink.Click();
        }

        /// <summary>
        /// Finds the table cell containing the version state of the version.
        /// </summary>
        /// <param name="versionNum">The number of the version.</param>
        /// <returns>The table cell with the version state.</returns>
        public TableCell GetStateCellByNumber(int versionNum)
        {
            //Gets the table cell that contains the versions state specified by the version number
            TableCell result = null;
            foreach (TableRow row in VersionTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(versionNum.ToString()))
                {
                    result = row.TableCells[5];
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the version preview button for the version.
        /// </summary>
        /// <param name="versionNum">The number of the version.</param>
        /// <returns>The Preview image/button for the version</returns>
        public Image GetVersionPreviewButtonByNumber(int versionNum)
        {
            //Returns the Preview Button for the version specified 
            Image result = null;
            foreach (TableRow row in VersionTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(versionNum.ToString()))
                {
                    result = row.Image(Find.ByTitle("Preview Content"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the rollback button for the version.
        /// </summary>
        /// <param name="versionNum">The number of the version.</param>
        /// <returns>The image/button for the version.</returns>
        public Image GetVersionRollbackButtonByNumber(int versionNum)
        {
            //Returns the Rollback Button for the version specified 
            Image result = null;
            foreach (TableRow row in VersionTable.TableRows)
            {
                if (row.TableCells[2].InnerHtml.Contains(versionNum.ToString()))
                {
                    result = row.Image(Find.ByTitle("Rollback Content To This Version"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Adds content to the module specified.
        /// Content will be added through the basic text editor.
        /// </summary>
        /// <param name="contentString">The content to add to the module.</param>
        /// <param name="moduleNum">The index of the module on the page. To find the action menu image for the first module on the page use 0.</param>
        public void AddContentToModule(string contentString, int moduleNum)
        {
            try
            {
                GetEditModuleContentLink(moduleNum).ClickNoWait();
                Thread.Sleep(2500);
                CurrentContentLink.ClickNoWait();
                Thread.Sleep(2500);
                if (!MainContentSectionLink.ClassName.Contains("SectionExpanded"))
                {
                    MainContentSectionLink.ClickNoWait();
                }
                Thread.Sleep(3500);
                BasicTextBoxRadioButton.Click();
                Thread.Sleep(3500);
                ModuleTextBox.Value = contentString;
                Thread.Sleep(2500);
                SaveContentLink.ClickNoWait();
                Thread.Sleep(3500);
            }
            catch (Exception exc)
            {
                Console.Write(exc.Message);
            }
        }

        #endregion
    }
}

