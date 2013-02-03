using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using DotNetNuke.Entities.Controllers;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using DotNetNuke.Tests.UI.WatiN.Utilities;
using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {

        public FileManagerPage FileManagerPage
        {
            get
            {
                return GetPage<FileManagerPage>();
            }
        }

        [When(@"I click the folder (.*)")]
        public void WhenIClicktheFolder(string folderName)
        {
            if (!FileManagerPage.PortalRootSpan.ClassName.Contains("FileManagerTreeNodeSelected"))
            {
                FileManagerPage.PortalRootSpan.Click();
                FileManagerPage.PortalRootSpan.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            }
            IEInstance.Span(Find.ByTitle(folderName)).Click();
            IEInstance.Span(Find.ByTitle(folderName)).WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }


        [When(@"I add a (.*) folder called (.*)")]
        public void WhenIAddFolder(string folderType, string folderName)
        {
            //check folder is exists
            if (!IEInstance.Span(Find.ByTitle(folderName)).Exists)
            {
                var expandImg = IEInstance.Image(Find.ById(i => i.Contains("DNNTreeexpcoldnn")));
                expandImg.Click();
                expandImg.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            }

            if (IEInstance.Span(Find.ByTitle(folderName)).Exists)
            {
                return;
            }

            //add folder
            if (!FileManagerPage.PortalRootSpan.ClassName.Contains("FileManagerTreeNodeSelected"))
            {
                FileManagerPage.PortalRootSpan.Click();
                FileManagerPage.PortalRootSpan.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            }

            FileManagerPage.StorageLocationSelectList.Select(folderType);
            FileManagerPage.FolderNameTextField.Value = folderName;
            FileManagerPage.AddFolderButton.Click();
        }

        [When(@"I (.*) the (.*) permission for the role (.*)")]
        public void WhenISetTheFolderPermissionForARole(string setting, string permission, string role)
        {
            FileManagerPage.SetPermissionForRole(setting, permission, role);
            Thread.Sleep(20000);
        }

        [When(@"I add the file (.*) to the folder (.*)")]
        public void WhenIAddTheFileToTheFolder(string fileName, string folderName)
        {
            SelectTreeNode(folderName);
            if (!FileManagerPage.FolderContentsTable.TableRow(Find.ByTextInColumn(t => t.Contains(fileName), 1)).Exists)
            {
                FileManagerPage.UploadFile(Path.Combine(Environment.CurrentDirectory, string.Format("Resources\\{0}", fileName)));
                IEInstance.WaitForComplete();
                FileManagerPage.ReturnLink.Click();
            }
        }

        [When(@"I click Update File Manager")]
        public void WhenIClickUpdateFileManager()
        {
            FileManagerPage.UpdateLink.Click();
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
