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
using System.Text.RegularExpressions;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The admin language page object.
    /// </summary>
    public class LanguagePage : WatiNBase
    {
        #region Constructors

        public LanguagePage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public LanguagePage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region SelectLists
        /// <summary>
        /// The resource locale selectlist from the create language page/popup.
        /// </summary>
        public SelectList LanguagePackLocaleSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("LanguagePackWriter_cboLanguage")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("LanguagePackWriter_cboLanguage")));
            }
        }
        #endregion

        #region Links
        /// <summary>
        /// The drop down link for the Language telerik selectlist from the Edit/Add Language page.
        /// </summary>
        public Link AddLanguageDropDownArrow
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("_EditLanguage_ctl00_Arrow")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("_EditLanguage_ctl00_Arrow")));
            }
        }
        public Link AddNewLanguageLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Add New Language"))); }
        }
        public Link CreateLanguagePackLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("createLanguagePackLink"))); }
        }
        public Link CreateLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Create"));
                }
                return ContentPaneDiv.Link(Find.ByTitle("Create"));
            }
        }
        /// <summary>
        /// The drop down link for the defaul language telerik selectlist from the Language Management page.
        /// </summary>
        public Link DefaultLanguageDropDownLink
        {
            get { return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("languageEnabler_ctl01_Arrow"))); }
        }
        public Link DeleteResourceFileLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Delete Resource File"));
                }
                return IEInstance.Link(Find.ByTitle("Delete Resource File"));
            }
        }
        /// <summary>
        /// The Enable Localized Content link in the Enable Localized Content confirmation pop up.
        /// </summary>
        public Link EnableLocalizedContentConfirmation
        {
            get
            {
                if(PopUpFrame!= null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Enable Localized Content"));
                }
                return EnableLocalizedContentPopUp.Link(Find.ByTitle("Enable Localized Content"));
            }
        }
        /// <summary>
        /// The Enable Localized Content Link on the Language Management page.
        /// </summary>
        public Link EnableLocalizedContentLink
        {
            get { return LanguagesDiv.Link(Find.ById(s => s.EndsWith("languageEnabler_cmdEnableLocalizedContent"))); }
        }
        /// <summary>
        /// The drop down link for the Fallback Language telerik selectlist from the Edit/Add Language page.
        /// </summary>
        public Link FallbackLanguageDropDownArrow
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("_EditLanguage_ctl03_Arrow")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("_EditLanguage_ctl03_Arrow")));
            }
        }
        public Link InstallLanguagePackLink
        {
            get { return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("installLanguagePackLink"))); }
        }
        public Link ReturnLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText("Return"));
                }
                return ContentPaneDiv.Link(Find.ByText("Return"));
            }
        }
        public Link SaveResourceFileLink
        {
            get { return ContentPaneDiv.Link(Find.ByTitle("Save Resource File")); }
        }
        [Obsolete("Element no longer exists in 6.X")]
        public Link TimeZoneEditorLink
        {
            get { return IEInstance.Link(Find.ByTitle("Time Zone Editor")); }
        }
        public Link VerifyLanguageResourceFilesLink
        {
            get { return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("verifyLanguageResourcesLink"))); }
        }
        #endregion

        #region Tables
        public Table LanguageTable
        {
            get { return IEInstance.Table(Find.ById(new Regex(@"dnn_ctr\d*_languageEnabler_languagesGrid_ctl\d*"))); }
        }
        /// <summary>
        /// The table containing the list of available modules on the Create Language pack page.
        /// </summary>
        public Table ModuleCheckboxTable
        {
            get { return ContentPaneDiv.Table(Find.ById(s => s.EndsWith("LanguagePackWriter_lstItems"))); }
        }
        #endregion

        #region TextFields
        /// <summary>
        /// The admin localized value text field in the Language Editor.
        /// </summary>
        public TextField AdminStringTextField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextFields.Filter(Find.ById(s => s.EndsWith("_txtValue")))[1];
                }
                return IEInstance.TextFields.Filter(Find.ById(s => s.EndsWith("_txtValue")))[1];
            }
        }
        public TextField ResourcePackNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("LanguagePackWriter_txtFileName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("LanguagePackWriter_txtFileName")));
            }
        }
        #endregion

        #region Spans
        /// <summary>
        /// The Selected Language span on the Language Editor page.
        /// </summary>
        public Span EditorLanguageLabel
        {
            get { return ContentPaneDiv.Span(Find.ById(s => s.EndsWith("languageeditor_languageLabel"))); }
        }
        /// <summary>
        /// The System Default Language span on the Language Management page.
        /// </summary>
        public Span SystemDefaultLanguageSpan
        {
            get { return ContentPaneDiv.Span(Find.ById(s => s.EndsWith("languageEnabler_systemDefaultLanguageLabel"))); }
        }        
        #endregion

        #region Divs
        /// <summary>
        /// The outer div containing the Language Management module.
        /// </summary>
        public Div LanguagesDiv
        {
            get { return IEInstance.Div(Find.ById("dnnLanguages")); }
        }
        /// <summary>
        /// A div containing the telerik selectlist options for the Default Language.
        /// </summary>
        public Div DefaultLanguageComboBoxDiv
        {
            get { return IEInstance.Div(Find.ById(new Regex(@"dnn_ctr\d*_languageEnabler_ctl\d*_DropDown"))); }
        }
        /// <summary>
        /// The div containing the list of Translator roles
        /// </summary>
        public Div LanguageTranslatorsGrid
        {
            get { return ContentPaneDiv.Div(Find.ByClass("dnnRolesGrid")); }
        }
        /// <summary>
        /// A div containing the telerik selectlist options for a new language.
        /// </summary>
        public Div AddLanguageDiv
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ById(s => s.EndsWith("EditLanguage_ctl00_DropDown")));
                }
                return IEInstance.Div(Find.ById(s => s.EndsWith("EditLanguage_ctl00_DropDown")));
            }
        }
        /// <summary>
        /// A div containing the telerik selectlist options for the fallback language.
        /// </summary>
        public Div FallbackLanguageDiv
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ById(s => s.EndsWith("EditLanguage_ctl03_DropDown")));
                }
                return IEInstance.Div(Find.ById(s => s.EndsWith("EditLanguage_ctl03_DropDown")));
            }
        }
        #endregion

        #region FileUploads
        public FileUpload LanguagePackFileUpload
        {
            get { return ContentPaneDiv.FileUpload(Find.ById(s => s.EndsWith("WebUpload_cmdBrowse"))); }
        }
        #endregion

        #region RadioButtons
        public RadioButton EditorModeHostRadioButton
        {
            get { return ContentPaneDiv.Table(Find.ById(s => s.EndsWith("languageeditor_rbMode"))).RadioButton(Find.ByValue("Host")); }
        }
        #endregion

        #region Images
        public Image ContentLocalizationNoOfPagesImage
        {
            get { return LanguageTable.Image(Find.ByTitle("No of Pages")); }
        }
        public Image ContentLocalizationTranslatedPagesImage
        {
            get { return LanguageTable.Image(Find.ByTitle("Translated Pages")); }
        }
        #endregion

        /// <summary>
        /// The frame containing the Enable Localized Content confirmation elements. 
        /// </summary>
        public Frame EnableLocalizedContentPopUp
        {
            get { return PopUpFrame; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a new language from the language page (doesn't install any language pack).
        /// </summary>
        /// <param name="language">The name of the language. The name must match the text in the language drop down exactly.</param>
        /// <param name="fallbackLanguage">The name of the fallback language. The name must match the text in the language drop down exactly.</param>
        public void AddNewLanguage(string language, string fallbackLanguage)
        {
            AddNewLanguageLink.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
            SelectLanguage("Language", "rcbItem ", language);
            SelectLanguage("Fallback", "rcbItem ", fallbackLanguage);
            UpdateLink.Click();
        }

        /// <summary>
        /// Clicks the expand button for a resource in the resources tree on the language editor page.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        public void ClickExpandButtonForResource(string resourceName)
        {
            DivCollection TreeViewDivs = ContentPaneDiv.Divs.Filter(Find.ByClass("rtTop"));
            foreach (Div d in TreeViewDivs)
            {
                if (d.InnerHtml.Contains(resourceName))
                {
                    d.Span(Find.ByClass("rtPlus")).Click();
                    return;
                }
                continue;
            }
        }

        /// <summary>
        /// Finds the enabled checkbox for the language name.
        /// </summary>
        /// <param name="localeName">The localized name of the language. This must match the language name that appears in the language table exactly.</param>
        /// <returns>The enabled checkbox for the language.</returns>
        public CheckBox GetCheckBoxForLanguage(string localeName)
        {
            //Returns the checkbox for the Specified Langauge
            CheckBox enabled = null;
            foreach (TableRow row in LanguageTable.OwnTableRows)
            {
                if (row.OwnTableCells.Count == 0)
                {
                    continue;
                }
                if (row.OwnTableCells[0].InnerHtml.Contains(localeName))
                {
                    enabled = row.OwnTableCells[1].CheckBox(Find.Any);
                    break;
                }
                continue;
            }
            return enabled;
        }
       
        /// <summary>
        /// Finds the edit language button for the language name.
        /// </summary>
        /// <param name="localeName">The localized name of the language. This must match the language name that appears in the language table exactly.</param>
        /// <returns>The edit language image/button for the language.</returns>
        public Image GetEditLanguageButtonForLanguage(string localeName)
        {
            //Returns the checkbox for the Specified Langauge
            Image edit = null; ;
            foreach (TableRow row in LanguageTable.OwnTableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(localeName))
                {
                    edit = row.TableCells[2].Image(Find.ByTitle("Edit this language"));
                    break;
                }
                continue;
            }
            return edit;
        }
        
        /// <summary>
        /// The Host Static Resources button for the language.
        /// </summary>
        /// <param name="localName">The localized name of the language. This must match the language name that appears in the language table exactly.</param>
        /// <returns>The Host Static Resources image/button for the language.</returns>
        public Image GetHostLanguageEditButton(string localName)
        {
            //Returns the Edit Button for the language at the host level
            Image result = null;
            foreach (TableRow row in LanguageTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(localName))
                {
                    result = row.Images.Filter(Find.ByTitle("Edit resourcekeys for the host installation"))[0]; ;
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the span that contains the flag image and link for the language.
        /// </summary>
        /// <param name="localeName">The name of the language. Must match the name that will be in the span title.</param>
        /// <returns>A span containing the flag image and link for the language.</returns>
        public Span GetLanguageFlagSpanByLocaleName(string localeName)
        {
            return IEInstance.Span(Find.ByTitle(localeName));
        }
       
        /// <summary>
        /// Finds the site level edit resource keys button.
        /// </summary>
        /// <param name="localName">The name of the language as it appears in the language table.</param>
        /// <returns>The site level edit resource keys image/button.</returns>
        public Image GetSiteLanguageEditButton(string localName)
        {
            //Returns the Edit Button for the language at the portal level
            Image result = null;
            foreach (TableRow row in LanguageTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(localName))
                {
                    result = row.Images.Filter(Find.ByTitle("Edit resourcekeys for this portal"))[0];
                    break;
                }
                continue;
            }
            return result;
        }
        
        /// <summary>
        /// Finds the system level edit resource keys button.
        /// </summary>
        /// <param name="localName">The name of the language as it appears in the language table.</param>
        /// <returns>The system level edit resource keys image/button. </returns>
        public Image GetSystemLanguageEditButton(string localName)
        {
            //Returns the Edit Button for the language at the system level
            Image result = null;
            foreach (TableRow row in LanguageTable.TableRows)
            {
                if (row.TableCells[0].InnerHtml.Contains(localName))
                {
                    result = row.Images.Filter(Find.ByTitle("Edit system resourcekeys"))[0]; ;
                    break;
                }
                continue;
            }
            return result;
        }
        
        /// <summary>
        /// Selects and updates the Default language for the site.
        /// This method will not work once content localization has been enabled.
        /// </summary>
        /// <param name="localeName">The name of the language as it will appear in the drop down.</param>
        /// <param name="selectListClass">The class to filter items by. 
        /// The currently selected item will have the class "rcbHovered ", all others will have the class "rcbItem ".</param>
        public void SelectAndUpdateNewDefaultLanguage(string localeName, string selectListClass)
        {
            //Click the drop down Arrow 
            DefaultLanguageDropDownLink.ClickNoWait();

            //Find Item to Select
            //Find all List Item elements.
            ElementCollection SelectListElements = DefaultLanguageComboBoxDiv.Elements.Filter(Find.ByClass(selectListClass));
            Element result = null;
            //Search for the desired Element
            foreach (Element e in SelectListElements)
            {
                if (e.InnerHtml.ToLower().Contains(localeName.ToLower()))
                {
                    //Select the Element
                    result = e;
                    break;
                }
                continue;
            }
            result.FireEvent("onmouseover");
            System.Threading.Thread.Sleep(1000);
            result.ClickNoWait();
            UpdateLink.Click();
        }
        
        /// <summary>
        /// Selects a language from the Add/Edit Language page.
        /// </summary>
        /// <param name="languageType">The language type that will be selected. Either "Language" or "Fallback".</param>
        /// <param name="SelectListClass">The class to filter items by. 
        /// The currently selected item will have the class "rcbHovered ", all others will have the class "rcbItem ".</param>
        /// <param name="ItemText">The text of the item (or language) that will be selected.</param>
        public void SelectLanguage(string languageType, string SelectListClass, string ItemText)
        {
            Div ComboBox = null;
            //Click Drop down
            if (languageType.ToLower().Equals("language"))
            {
                AddLanguageDropDownArrow.Click();
                System.Threading.Thread.Sleep(3000);
                ComboBox = AddLanguageDiv;

            }
            else if (languageType.ToLower().Equals("fallback"))
            {
                FallbackLanguageDropDownArrow.Click();
                System.Threading.Thread.Sleep(3000);
                ComboBox = FallbackLanguageDiv;
            }
            //Find Item to Select
            //Find all List Item elements.
            ElementCollection SelectListElements = ComboBox.Elements.Filter(Find.ByClass(SelectListClass));
            Element result = null;
            //Search for the desired Element
            foreach (Element e in SelectListElements)
            {
                if (e.InnerHtml.ToLower().Contains(ItemText.ToLower()))
                {
                    //Select the Element
                    result = e;
                    break;
                }
                continue;
            }
            result.FireEvent("onmouseover");
            result.Click();

        }

        /// <summary>
        /// Selects a Resource pack type on the create language pack.
        /// </summary>
        /// <param name="packageType">The package type to select. Ex. "Core", "Module", "Provider" etc.</param>
        public void SelectLanguagePackType(string packageType)
        {
            if (!string.IsNullOrEmpty(packageType))
            {
                if(PopUpFrame != null)
                {
                    WatiNUtil.SelectRadioButtonByName(PopUpFrame, "LanguagePackWriter$rbPackType", packageType);
                }
                WatiNUtil.SelectRadioButtonByName(IEInstance, "LanguagePackWriter$rbPackType", packageType);
            }
        }

        /// <summary>
        /// Selects a module checkbox on the create language page. Any module selected here will be included in the resource pack that is being created. 
        /// This method can only be used if the Module Resource Pack Type has been selected.
        /// </summary>
        /// <param name="moduleName">The name of the module to include in the resource pack.</param>
        public void SelectModuleCheckbox(string moduleName)
        {
            foreach (TableCell cell in ModuleCheckboxTable.TableCells)
            {
                if (cell.InnerHtml.Contains(moduleName))
                {
                    cell.CheckBox(Find.Any).Checked = true;
                    break;
                }
                continue;
            }
        }

        #endregion

    }
}
