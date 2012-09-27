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
    /// The extensions page object (either host or admin).
    /// </summary>
    public class ExtensionsPage : WatiNBase
    {
        #region Constructors

        public ExtensionsPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ExtensionsPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Links
        public Link InstallExtensionsWizardLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Install Extension Wizard"))); }
        }
        /// <summary>
        /// The uninstall package link, found on the uninstall extension page.
        /// </summary>
        public Link UninstallPackageLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("UnInstall Package"));
                }
                return IEInstance.Link(Find.ByTitle("UnInstall Package"));
            }
        }
        /// <summary>
        /// The return link from the uninstall extension page.
        /// </summary>
        public Link UnInstallReturnLink
        {
            get { return IEInstance.Link(Find.ById(s => s.StartsWith("dnn_ctr_UnInstall_cmdReturn"))); }
        }
        /// <summary>
        /// The finish link in the install extension wizard.
        /// </summary>
        public Link WizardFinishLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("_ctr_Install_wizInstall_FinishNavigationTemplateContainerID_finishButtonStep")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("_ctr_Install_wizInstall_FinishNavigationTemplateContainerID_finishButtonStep")));
            }
        }
        /// <summary>
        /// The next link in the install extension wizard.
        /// </summary>
        public Link WizardNextLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Next"));
                }
                return IEInstance.Link(Find.ByTitle("Next"));
            }
        }

        public Link WizardReturnLink
        {
            get
            {
                return FindElement<Link>(Find.ByTitle("Return"));
            }
        }
        /// <summary>
        /// The next link on the first page of the install extension wizard.
        /// </summary>
        public Link WizardStartLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("ctr_Install_wizInstall_StartNavigationTemplateContainerID_nextButtonStart")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("ctr_Install_wizInstall_StartNavigationTemplateContainerID_nextButtonStart")));
            }
        }

        public CheckBox RepaireInstallCheckBox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(d => d.EndsWith("wizInstall_chkRepairInstall")));
                }
                return IEInstance.CheckBox(Find.ById(d => d.EndsWith("wizInstall_chkRepairInstall")));
            }
        }

        public Link CoreLanguagePackSectionLink
        {
            get { return IEInstance.Link(Find.ByText("Core Language Packs")); }
        }
        
        public Link AuthenticationSystemsSectionLink
        {
            get { return IEInstance.Link(Find.ByText("Authentication Systems")); }
        }
        
        public Link PackageSettingsSectionLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText("Package Settings"));
                }
                return IEInstance.Link(Find.ByText("Package Settings"));
            }
        }
        
        public Link ContainersSectionLink
        {
            get { return IEInstance.Link(Find.ByText("Containers")); }
        }
        
        public Link ExtensionLanguagePacksSectionLink
        {
            get { return IEInstance.Link(Find.ByText("Extension Language Packs")); }
        }
        
        public Link ModuleSectionLink
        {
            get { return IEInstance.Link(Find.ByText("Modules")); }
        }

        public Link UpdateExtensionLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Update Extension"));
                }

                return IEInstance.Link(Find.ByTitle("Update Extension"));
            }
        }

        public Link CreatePackageLink
        {
            get
            {
                return FindElement<Link>(Find.ByTitle("Create Package"));
            }
        }
        
        #endregion

        #region FileUploads
        public FileUpload SelectPackageFileUpload
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.FileUpload(Find.ById(s => s.EndsWith("ctr_Install_wizInstall_cmdBrowse")));
                }
                return IEInstance.FileUpload(Find.ById(s => s.EndsWith("ctr_Install_wizInstall_cmdBrowse")));
            }
        }
        #endregion

        #region CheckBoxes
        public CheckBox AcceptLicenseCheckBox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("_Install_wizInstall_chkAcceptLicense")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("_Install_wizInstall_chkAcceptLicense")));
            }
        }
        public CheckBox DeleteExtensionFilesCheckbox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("ctr_UnInstall_chkDelete")));
                }
                return ContentPaneDiv.CheckBox(Find.ById(s => s.EndsWith("ctr_UnInstall_chkDelete")));
            }
        }
        #endregion

        #region Divs
        /// <summary>
        /// The collection of Divs surrounding the extension categories.
        /// </summary>
        public DivCollection ExtensionTypeDivs
        {
            get { return IEInstance.Divs.Filter(Find.ByClass("dnnForm exieContent dnnClear")); }
        }
        /// <summary>
        /// The div containing the contents of the Installed Extensions tab.
        /// </summary>
        public Div InstalledExtensionDiv
        {
            get { return IEInstance.Div(Find.ById("installedExtensions")); }
        }
        /// <summary>
        /// The div containing the contents of the Available Extensions tab.
        /// </summary>
        public Div AvailableExtensionDiv
        {
            get { return IEInstance.Div(Find.ById("availableExtensions")); }
        }
        /// <summary>
        /// The div containing the contents of the More Extensions tab.
        /// </summary>
        public Div MoreExtensionDiv
        {
            get { return IEInstance.Div(Find.ById("moreExtensions")); }
        }
        #endregion

        #region SelectLists
        [Obsolete("Element no longer exists.")]
        public SelectList ExtensionPackageSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("Extensions_cboPackageTypes"))); }
        }
        [Obsolete("Element no longer exists.")]
        public SelectList FilterByTypeSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("Extensions_cboPackageTypes"))); }
        }

        public SelectList ModuleCatagorySelect
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(i => i.EndsWith("desktopModuleForm_category_category_ComboBox")));
                }
                else
                {
                    return IEInstance.SelectList(Find.ById(i => i.EndsWith("desktopModuleForm_category_category_ComboBox")));
                }
            }
        }
        #endregion

        #region Tables
        [Obsolete("Element no longer exists.")]
        public Table ExtensionTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("Extensions_grdExtensions"))); }
        }
        public Table CoreLanguagePacksTable
        {
            get { 
                return ExtensionTypeDivs.Filter(Find.ByText(s => s.Contains("Core Language Packs")))[0].Table(
                        Find.ById(s => s.Contains("Extensions_installedExtensionsControl_extensionTypeRepeater_extensionsGrid"))); }
        }
        public Table AuthenticationSystemsTable
        {
            get
            {
                return ExtensionTypeDivs.Filter(Find.ByText(s => s.Contains("Authentication Systems")))[0].Table(
                        Find.ById(s => s.Contains("Extensions_installedExtensionsControl_extensionTypeRepeater_extensionsGrid")));
            }
        }
        public Table ContainersTable
        {
            get
            {
                return ExtensionTypeDivs.Filter(Find.ByText(s => s.Contains("Containers")))[0].Table(
                        Find.ById(s => s.Contains("Extensions_installedExtensionsControl_extensionTypeRepeater_extensionsGrid")));
            }
        }
        public Table ExtensionLanguagePacksTable
        {
            get
            {
                return ExtensionTypeDivs.Filter(Find.ByText(s => s.Contains("Extension Language Packs")))[0].Table(
                        Find.ById(s => s.Contains("Extensions_installedExtensionsControl_extensionTypeRepeater_extensionsGrid")));
            }
        }
        public Table ModulesTable
        {
            get
            {
                return ExtensionTypeDivs.Filter(Find.ByText(s => s.Contains("Modules")))[0].Table(
                        Find.ById(s => s.Contains("Extensions_installedExtensionsControl_extensionTypeRepeater_extensionsGrid")));
            }
        }
        #endregion

        #region Spans
        public Span ModuleNameSpan
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Span(Find.ById(s => s.EndsWith("EditExtension_ModuleEditor_desktopModuleForm_moduleName_moduleName_Label")));
                }
                return IEInstance.Span(Find.ById(s => s.EndsWith("EditExtension_ModuleEditor_desktopModuleForm_moduleName_moduleName_Label")));
            }
        }
        #endregion

        #region Buttons
        /// <summary>
        /// The confirm (Yes) button in a DNN popup.
        /// </summary>
        public Button ConfirmButton
        {
            get { if(PopUpFrame != null)
            {
                return PopUpFrame.Div(Find.ByClass(s => s.StartsWith("ui-dialog ui-widget"))).Button(
                        Find.ByText(s => s.Contains("Yes")));
            }
                return
                    IEInstance.Div(Find.ByClass(s => s.StartsWith("ui-dialog ui-widget"))).Button(
                        Find.ByText(s => s.Contains("Yes")));
            }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Installs the extension at packagePath.
        /// Selects the package and completes the install extension wizard.
        /// </summary>
        /// <param name="packagePath">The path for the package that will be isntalled.</param>
        public void InstallExtension(string packagePath)
        {
            if (!ModuleTitleSpan.Text.Contains("Install Extension"))
            {
                InstallExtensionsWizardLink.Click();
            }
            System.Threading.Thread.Sleep(1500);
            UploadExtensionsPackage(packagePath);
            WizardStartLink.ClickNoWait();
            IEInstance.WaitForComplete();
            WizardNextLink.ClickNoWait();
            IEInstance.WaitForComplete();
            WizardNextLink.ClickNoWait();
            IEInstance.WaitForComplete();
            AcceptLicenseCheckBox.ClickNoWait();
            WizardNextLink.ClickNoWait();
            IEInstance.WaitForComplete();
            WizardFinishLink.Click();
        }

        /// <summary>
        /// Finds the edit button for the extension in the extension type table.
        /// </summary>
        /// <param name="extensionName">The name of the extension.</param>
        /// <param name="extensionTypeTable">The extension type table containing the extension.</param>
        /// <returns>The edit image/button for the extension.</returns>
        public Image GetExtensionEditButton(string extensionName, Table extensionTypeTable)
        {
            //Returns the Delete Button for the user specified 
            Image result = null;
            foreach (TableRow row in extensionTypeTable.TableRows)
            {
                if (row.TableCells[3].InnerHtml.Contains(extensionName))
                {
                    result = row.Image(Find.ByTitle("Edit this Extension"));
                    break;
                }
                continue;
            }
            return result;
        }

        public Image GetExtensionIcon(string extensionName, Table extensionTypeTable)
        {
            //Returns the Delete Button for the user specified 
            Image result = null;
            foreach (TableRow row in extensionTypeTable.TableRows)
            {
                if (row.TableCells[3].InnerHtml.Contains(extensionName))
                {
                    result = row.TableCells[2].Image(Find.First());
                    break;
                }
                continue;
            }
            return result;
        }

       
        /// <summary>
        /// Finds the uninstall link for the extension in the extension type table.
        /// </summary>
        /// <param name="extensionName">The name of the extension.</param>
        /// <param name="extensionTypeTable">The extension type table containing the extension.</param>
        /// <returns>The uninstall link for the extension.</returns>
        public Link GetExtensionUninstallButton(string extensionName, Table extensionTypeTable)
        {
            //Returns the Delete Button for the extension specified 
            Link result = null;
            foreach (TableRow row in extensionTypeTable.TableRows)
            {
                if (row.TableCells[3].InnerHtml.Contains(extensionName))
                {
                    result = row.Link(Find.ByTitle("UnInstall this Extension"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Sets the file path for the Select Package Dialog.
        /// </summary>
        /// <param name="packagePath">The package path.</param>
        public void UploadExtensionsPackage(string packagePath)
        {
            SelectPackageFileUpload.Set(packagePath);
        }

        #endregion
    }
}
