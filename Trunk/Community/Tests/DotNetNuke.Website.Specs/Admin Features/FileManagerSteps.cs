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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;
using WatiN.Core.DialogHandlers;

namespace DotNetNuke.Website.Specs.Admin_Features
{
    [Binding]
    public class FileManagerSteps : WatiNTest
    {
        public FileManagerPage FileManagerPage
        {
            get
            {
                return GetPage<FileManagerPage>();
            }
        }

        [When(@"I copy a simple zip file to the secure folder called (.*)")]
        public void WhenICopyASimpleZipFileToTheSecureFolder(string folderName)
        {
            File.Copy(Path.Combine(Environment.CurrentDirectory, "Resources\\CustomerSkin.zip"),
                Path.Combine(PhysicalPath, string.Format("Portals\\0\\{0}\\CustomerSkin.zip", folderName)),
                true);
        }

        [When(@"I try to synchorize folder recursive")]
        public void WhenITryToSynchorizeFolderRecursive()
        {
            FileManagerPage.RecursiveSyncCheckbox.Checked = true;
            FileManagerPage.SyncFolderSpan.Click();
        }

        [When(@"I select the secure folder called (.*)")]
        public void WhenISelectTheSecureFolder(string folderName)
        {
            FileManagerPage.PortalRootSpan.PreviousSibling.PreviousSibling.Click();
            Thread.Sleep(3000);
            FileManagerPage.ContentPaneDiv.Span(Find.ByTitle(folderName)).Click();
            Thread.Sleep(3000);
        }

        [Then(@"I should see the file exist in file manager")]
        public void ThenIShouldSeeTheFileExistInFileManager()
        {
            Assert.True(FileExists("CustomerSkin.zip"));
        }

        [When(@"Moving the file from folder1 to folder2")]
        public void WhenMovingTheFileFromFolder1ToFolder2()
        {
            SelectTreeNode("folder1");
            var checkbox = FileManagerPage.FolderContentsTable.Span(Find.BySelector("[filename=\"Do Change.doc\"]"))
                            .CheckBox(Find.ById(i => i.Contains("dgFileList_chkFile")));
            checkbox.Checked = true;

            var dialog = new ConfirmDialogHandler();
            using (new UseDialogOnce(IEInstance.DialogWatcher, dialog))
            {
                FileManagerPage.MoveFileImage.ClickNoWait();
                dialog.WaitUntilExists();
                dialog.OKButton.Click();
            }

            SelectTreeNode("folder2");
            FileManagerPage.MoveFileOKButton.Click();
        }

        [Then(@"The file should moved to folder2 without error")]
        public void ThenTheFileShouldMovedToFolder2WithoutError()
        {
            SelectTreeNode("folder1");
            Assert.NotNull(FileManagerPage.FolderContentsTable.FindRow(t => t.Contains("Do Change.doc"), 1));

            SelectTreeNode("folder2");
            Assert.NotNull(FileManagerPage.FolderContentsTable.FindRow(t => t.Contains("Do Change.doc"), 1));
        }

        [Then(@"I should see the File Access Error message")]
        public void ThenIShouldSeeTheFileAccessErrorMessage()
        {
            WatiNAssert.AssertIsTrue(HomePage.ContentPaneDiv.InnerHtml.Contains("You do not have sufficient permissions to access this file"), "FileAccessErrorMessageDoesNotShow.jpg");
        }

        [When(@"I clik Manage Folder Types button from action menu")]
        public void WhenIClikManageFolderTypesButtonFromActionMenu()
        {
            FileManagerPage.ManageFolderTypesLink.Click();
        }

        [When(@"I Click Add New Type Button")]
        public void WhenIClickAddNewTypeButton()
        {
            FileManagerPage.AddNewTypeLink.Click();
            WaitAjaxRequestComplete();
        }

        [When(@"I input in folder type name field")]
        public void WhenIInputInFolderTypeNameField()
        {
            FileManagerPage.FolderProviderNameField.Value = "Test";
        }

        [When(@"I select Folder type as (.*)")]
        public void WhenISelectFolderTypeAs(string value)
        {
            FileManagerPage.FolderProviderSelectList.SelectByValue(value);
            WaitAjaxRequestComplete();
        }

        [Then(@"the UNC settings should be disabled")]
        public void ThenTheUNCSettingsShouldBeDisabled()
        {
            Assert.IsTrue(!FileManagerPage.UNCPathField.Enabled);
        }


        private bool FileExists(string fileName)
        {
            foreach (var row in FileManagerPage.FileManagerTable.TableRows.Filter(Find.ByClass("FileManager_Item")))
            {
                if (row.TableCells[1].InnerHtml.Contains(fileName))
                {
                    return true;
                }
            }

            return false;
        }

        private void SelectTreeNode(string nodeName)
        {
            if (!IEInstance.Span(Find.ByTitle(nodeName)).Exists)
            {
                var expandImg = IEInstance.Image(Find.ById(i => i.Contains("DNNTreeexpcoldnn")));
                expandImg.Click();
                expandImg.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            }

            var node = IEInstance.Span(Find.ByTitle(nodeName));
            node.Click();
            node.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }
    }
}
