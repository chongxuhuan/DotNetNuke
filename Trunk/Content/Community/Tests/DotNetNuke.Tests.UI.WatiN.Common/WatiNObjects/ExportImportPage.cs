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
    /// The Export Import page object.
    /// </summary>
    public class ExportImportPage : WatiNBase
    {
        #region Constructors

        public ExportImportPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ExportImportPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Spans
        /// <summary>
        /// The span containing the message after exporting a page.
        /// </summary>
        public Span ExportMessageSpan
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Span(Find.ById("dnn_ctr_Export_lblMessage"));
                }
                return IEInstance.Span(Find.ById("dnn_ctr_Export_lblMessage"));
            }
        }

        #endregion

        #region SelectLists
        public SelectList ExportTemplateFolderSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById("dnn_ctr_Export_cboFolders"));
                }
                return IEInstance.SelectList(Find.ById("dnn_ctr_Export_cboFolders"));
            }
        }
        public SelectList ImportFolderSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById("dnn_ctr_Import_cboFolders"));
                }
                return IEInstance.SelectList(Find.ById("dnn_ctr_Import_cboFolders"));
            }
        }
        public SelectList InsertPageSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById("dnn_ctr_Import_cboPositionTab"));
                }
                return IEInstance.SelectList(Find.ById("dnn_ctr_Import_cboPositionTab"));
            }
        }
        public SelectList TemplateSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById("dnn_ctr_Import_cboTemplate"));
                }
                return IEInstance.SelectList(Find.ById("dnn_ctr_Import_cboTemplate"));
            }
        }
        #endregion

        #region TextFields
        public TextField PageNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById("dnn_ctr_Import_txtTabName"));
                }
                return IEInstance.TextField(Find.ById("dnn_ctr_Import_txtTabName"));
            }
        }
        public TextField TemplateDescriptionField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById("dnn_ctr_Export_txtDescription"));
                }
                return IEInstance.TextField(Find.ById("dnn_ctr_Export_txtDescription"));
            }
        }
        public TextField TemplateNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById("dnn_ctr_Export_txtFile"));
                }
                return IEInstance.TextField(Find.ById("dnn_ctr_Export_txtFile"));
            }
        }
        #endregion

        #region CheckBoxes
        public CheckBox IncludeContentCheckbox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById("dnn_ctr_Export_chkContent"));
                }
                return IEInstance.CheckBox(Find.ById("dnn_ctr_Export_chkContent"));
            }
        }
        #endregion

        #region Links
        public Link ExportLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Export"));
                }
                return PageContentDiv.Link(Find.ByTitle("Export"));
            }
        }
        public Link ImportPageLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Import"));
                }
                return PageContentDiv.Link(Find.ByTitle("Import"));
            }
        }
        #endregion

        #region RadioButtons
        public RadioButton CreateNewPageRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("ADD"));
                }
                return IEInstance.RadioButton(Find.ByValue("ADD"));
            }
        }
        public RadioButton EditImportedPageRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("SETTINGS"));
                }
                return IEInstance.RadioButton(Find.ByValue("SETTINGS"));
            }
        }
        public RadioButton InsertAfterRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("After"));
                }
                return IEInstance.RadioButton(Find.ByValue("After"));
            }
        }
        public RadioButton InsertAtEndRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("AtEnd"));
                }
                return IEInstance.RadioButton(Find.ByValue("AtEnd"));
            }
        }
        public RadioButton InsertBeforeRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("Before"));
                }
                return IEInstance.RadioButton(Find.ByValue("Before"));
            }
        }
        public RadioButton ReplaceCurrentPageRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("REPLACE"));
                }
                return IEInstance.RadioButton(Find.ByValue("REPLACE"));
            }
        }
        public RadioButton ViewImportedPageRadioButton
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.RadioButton(Find.ByValue("VIEW"));
                }
                return PageContentDiv.RadioButton(Find.ByValue("VIEW"));
            }
        }

        #endregion

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a page with an HTML module and some content on it and then exports it.
        /// </summary>
        /// <param name="pageName">The page that will be created.</param>
        /// <param name="pageContent">The content to add to the HTML module on the page.</param>
        /// <param name="templateName">The name for the template.</param>
        /// <param name="withContent">Will the page be exported with content.</param>
        /// <param name="siteURL">The site url.</param>
        /// <param name="community">Is the site running community edition. Enter false if it uses professional or Enterprise.</param>
        public void CreateAndExportPage(string pageName, string pageContent, string templateName, bool withContent, string siteURL, bool community)
        {
            IconBar controlPanel = null;
            RibbonBar ribbonBar = null;
            bool usesRibbonBar = false;

            //Add a page to the site
            if (ControlPanelElement.Id.Contains(RibbonBarID))
            {
                ribbonBar = new RibbonBar(this);
                ribbonBar.NewPageLink.ClickNoWait();
                usesRibbonBar = true;
            }
            else
            {
                controlPanel = new IconBar(this);
                controlPanel.AddPageLink.ClickNoWait();
            }
            PageSettingsPage newPage = new PageSettingsPage(this);
            newPage.PageNameField.Value = pageName;
            newPage.PageTemplateSelect.Select("None Specified");
            newPage.SetPermissionForRole("Grant", "View", "All Users");
            newPage.AddPageLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);

            //Add content to the page 
            IEInstance.GoTo(siteURL + "/testpage.aspx");
            if (!usesRibbonBar)
            {
                if (community)
                {
                    controlPanel.AddHTMLModuleToPageWithContent("Export Module", "HTML", pageContent, 0);
                }
                else
                {
                    controlPanel.AddHTMLModuleToPageWithContent("Export Module", "HTML Pro", pageContent, 0);
                }
            }
            else
            {
                if (community)
                {
                    ribbonBar.AddHTMLModuleToPageWithContent("Export Module", "HTML", pageContent, 0);

                }
                else
                {
                    ribbonBar.AddHTMLModuleToPageWithContent("Export Module", "HTML Pro", pageContent, 0);

                }
            }

            //Act 
            if (usesRibbonBar)
            {
                ribbonBar.ExportPageLink.Click();
            }
            else
            {
                controlPanel.ExportPageLink.Click();
            }
            ExportTemplateFolderSelectList.Select("Templates");
            TemplateNameField.Value = templateName;
            TemplateDescriptionField.Value = "This is a template for testing";
            IncludeContentCheckbox.Checked = withContent;
            ExportLink.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
        }

        #endregion
    }
}
