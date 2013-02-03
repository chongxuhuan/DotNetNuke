using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;
using DotNetNuke.Tests.UI.WatiN.Utilities;
using NUnit.Framework;
using TechTalk.SpecFlow;
using WatiN.Core;

namespace DotNetNuke.Tests.Steps
{
    public partial class BaseSteps
    {
        public HTMLModule HtmlModule
        {
            get { return GetPage<HTMLModule>(); }
        }

        /// <summary>
        /// Makes the host user a member of portal 0. 
        /// </summary>
        [BeforeScenario("MustHaveKnownFileInFileManager")]
        public void MustHaveKnownFileInFileManager()
        {
            File.Copy(Path.Combine(Environment.CurrentDirectory, "Resources\\Do Change.doc"), Path.Combine(PhysicalPath, "Portals\\0\\Do Change.doc"), true);
            FolderManager.Instance.Synchronize(0, "", true, true);
        }



        /// <summary>
        /// Creates an HTML module with the the title and visibility specified.
        /// Then adds the content specified to the module.
        /// </summary>
        /// <param name="moduleName">The module title.</param>
        /// <param name="visibility">The visibility for the module.
        /// One of the two visibility options must be used Same As Page or Page Editors Only.
        /// Options are case sensitive and must be entered exactly as listed previously.</param>
        /// <param name="moduleContent">The content that will be added to the module.</param>
        [When(@"I create the module (.*) with (.*) visibility and the content (.*)")]
        public void WhenICreateTheModuleWithSameAsPageVisibility(string moduleName, string visibility, string moduleContent)
        {
            var ribbonBar = GetPage<RibbonBar>();
            Thread.Sleep(1500);
            ribbonBar.AddHtmlModuleToPageSelectVisibility(moduleName, visibility, "HTML Pro", moduleContent, 0);
            Thread.Sleep(3500);
        }

        /// <summary>
        /// Updates the content specified to the HTML Pro.
        /// </summary>
        /// <param name="moduleContent">The content that will be added to the module.</param>
        [Given(@"I update the HTML module with the content (.*)")]
        [When(@"I update the HTML module with the content (.*)")]
        public void WhenIUpdateTheModuleWithTheContent(string moduleContent)
        {
            var ribbonBar = GetPage<RibbonBar>();
            Thread.Sleep(1500);
            ribbonBar.UpdateHTMLModuleContent(moduleContent, 0);
            Thread.Sleep(3500);
        }

        /// <summary>
        /// Creates an HTML module with the the title and visibility specified.
        /// </summary>
        /// <param name="moduleName">The module title.</param>
        /// <param name="visibility">The visibility for the module.
        /// One of the two visibility options must be used Same As Page or Page Editors Only.
        /// Options are case sensitive and must be entered exactly as listed previously.</param>
        [When(@"I create the module (.*) with (.*) visibility")]
        public void WhenICreateTheModuleWithSameAsPageVisibility(string moduleName, string visibility)
        {
            var ribbonBar = GetPage<RibbonBar>();
            Thread.Sleep(1500);
            ribbonBar.AddHTMLModuleToPage(moduleName, visibility, "HTML Pro");
            Thread.Sleep(3500);
        }

        /// <summary>
        /// Edits the first HTML module on the page.
        /// </summary>
        [Given(@"I edit one of the html module content")]
        [When(@"I edit one of the html module content")]
        public void WhenIEditOneOfTheHtmlModuleContent()
        {
            Thread.Sleep(1000);
            IEInstance.Link(Find.ByText(t => t.Contains("Edit Content"))).Click();
        }

        /// <summary>
        /// Adds the text to the HTML module.
        /// </summary>
        /// <param name="moduleContent">The content that will be added to the module.</param>
        [Given(@"I enter the text (.*) in rad text editor")]
        [When(@"I enter the text (.*) in rad text editor")]
        public void WhenIEditRadTextEditor(string moduleContent)
        {
            HtmlModule.RichTextBoxRadioButton.Click();

            ExecuteEditorCommand(string.Format("$find('{0}').set_html('{1}');", HtmlModule.EditHTMLDiv.Id, moduleContent));
        }

        /// <summary>
        /// Adds the text to the HTML module and selects it all.
        /// </summary>
        /// <param name="moduleContent">The content that will be added to the module.</param>
        [When(@"I enter (.*) and click hyper link manager button in rad text editor")]
        public void WhenIClickHyperLinkManagerButtonInRadTextEditor(string moduleContent)
        {
            ExecuteEditorCommand(string.Format("$find('{0}').set_html('{1}');", HtmlModule.EditHTMLDiv.Id, moduleContent));
            ExecuteEditorCommand(string.Format("$find('{0}').fire('SelectAll');", HtmlModule.EditHTMLDiv.Id));
            HtmlModule.TelerikEditor.HyperLinkManagerLink.Click();
        }

        /// <summary>
        /// Clicks the Document Manager button in the Telerik Rich Text Editor.
        /// </summary>
        [When(@"I click the Telerik Editor HyperLink button")]
        public void WhenIClickTelerikEditorHyperLinkButton()
        {
            HtmlModule.TelerikEditor.TelerikPopUpFrame.Link(Find.ById("DocumentManagerCaller")).Click();
        }

        /// <summary>
        /// Selects the document to be linked to and then inserts the link to the document selected.
        /// </summary>
        [When(@"I select the folder (.*)")]
        public void WhenISelectTheFolder(string folderName)
        {
            HtmlModule.TelerikEditor.TelerikPopUpFrame.Divs.Filter(Find.ByClass("rtTemplate")).First(Find.ByText(t => t.Contains(folderName))).Click();
        }

        /// <summary>
        /// Selects the document to be linked to and then inserts the link to the document selected.
        /// </summary>
        /// <param name="documentName">The document in the Document Manager to link to.</param>
        [When(@"I link to the document (.*)")]
        public void WhenIInsertADocument(string documentName)
        {
            HtmlModule.TelerikEditor.TelerikPopUpFrame.Divs.Filter(Find.ByClass("rfeFileExtension doc")).First(Find.ByText(t => t.Contains(documentName))).Click();
            HtmlModule.TelerikEditor.TelerikPopUpFrame.Button(Find.ByTitle("Insert")).Click();
            HtmlModule.TelerikEditor.GetDialog("LinkManager").Button(Find.ById("lmInsertButton")).Click();
        }

        [Given(@"I click Save on the Html Module")]
        [When(@"I click Save on the Html Module")]
        public void WhenISaveHtml()
        {
            HtmlModule.SaveContentLink.Click();
            Thread.Sleep(2000);
        }

        private string ExecuteEditorCommand(string command)
        {
            if (HomePage.PopUpFrame != null)
            {
                return HomePage.PopUpFrame.Eval(command);
            }

            return IEInstance.Eval(command);
        }

    }
}
