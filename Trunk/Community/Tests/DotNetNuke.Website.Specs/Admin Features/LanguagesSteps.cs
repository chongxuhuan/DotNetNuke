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
using WatiN.Core.Native.InternetExplorer;

using mshtml;

namespace DotNetNuke.Website.Specs.Steps
{
	[Binding]
    public class LanguagesSteps : WatiNTest
    {
        #region Properties

        public LanguagePage LanguagesPage
		{
			get
			{
                return GetPage<LanguagePage>();
			}
		}

	    public HostSettingsPage HostSettingsPage
	    {
	        get
	        {
                return GetPage<HostSettingsPage>();
	        }
	    }

        #endregion

        #region Scenario "Language selector should show the correct value of current language"

        [When(@"I enable content localization setting")]
        public void WhenIEnableContentLocalizationSetting()
        {
            if (!HostSettingsPage.AllowContentLocalizationCheckbox.Checked)
            {
                HostSettingsPage.AllowContentLocalizationCheckbox.Checked = true;
                HostSettingsPage.UpdateLink.Click();
                HostSettingsPage.UpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            }
        }

        [When(@"I add new language (.*)")]
        public void WhenIAddNewLanguage(string name)
        {
            if(LanguagesPage.LanguageTable.Image(Find.BySrc(s => s.Contains(name))).Exists)
            {
                return;
            }

            LanguagesPage.AddNewLanguage(name, "en-US");
            LanguagesPage.UpdateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            Thread.Sleep(1000);
            IEInstance.WaitForComplete();
        }

        [When(@"I enable content localization on the portal")]
        public void WhenIEnableContentLocalizationOnThePortal()
        {
            if(!LanguagesPage.EnableLocalizedContentLink.Exists)
            {
                return;
            }

            LanguagesPage.EnableLocalizedContentLink.Click();
            LanguagesPage.EnableLocalizedContentConfirmation.Click();
            LanguagesPage.EnableLocalizedContentConfirmation.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
            Thread.Sleep(1000);
            IEInstance.WaitForComplete();
        }

        [When(@"I modify skin (.*) to set showMenu of Language control")]
        public void WhenIModifySkinToSetShowMenuOfLanguageControl(string skinName)
        {
            var skinPath = Path.Combine(PhysicalPath, "Portals\\_default\\Skins\\DarkKnight\\" + skinName);
            var skinContent = File.ReadAllText(skinPath);
            skinContent = skinContent.Replace("showMenu=\"False\"", "showMenu=\"True\"");
            File.WriteAllText(skinPath, skinContent);
        }

        [When(@"I navigate to home page")]
        public void WhenINavigateToHomePage()
        {
            IEInstance.GoTo(string.Format("http://{0}/Home.aspx", SiteUrl));
        }

        [When(@"I click language icon of (.*)")]
        public void WhenIClickLanguageIcon(string languageName)
        {
            IEInstance.Div(Find.ByClass("language-object")).Image(Find.ByAlt(languageName)).Click();
        }

        [Then(@"The language drop down should also select language (.*)")]
        public void ThenTheLanguageDropDownShouldAlsoSelectLanguage(string languageName)
        {
            var selLang = IEInstance.Div(Find.ByClass("language-object"))
                .SelectList(Find.ById(s => s.EndsWith("selectCulture"))).SelectedOption.Value;

            Assert.AreEqual(languageName, selLang);
        }

        #endregion

        #region Scenario "RadEditorProvider language pack should create successful"

        [When(@"I try to create (.*) language pack of (.*)")]
        public void WhenITryToCreateLanguagePack(string type, string name)
        {
            //delete the language pack file if exist
            var langDir = Path.Combine(PhysicalPath, "Install\\Language");
            var packs = Directory.GetFiles(langDir, string.Format("*{0}*.zip", name));
            if(packs.Length > 0)
            {
                Array.ForEach<string>(packs, File.Delete);
            }

            LanguagesPage.CreateLanguagePackLink.Click();
            LanguagesPage.SelectLanguagePackType(type);
            LanguagesPage.CreateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));

            LanguagesPage.SelectModuleCheckbox(name);
            LanguagesPage.CreateLink.Click();
            LanguagesPage.CreateLink.WaitUntil(p => !Convert.ToBoolean(IEInstance.Eval("Sys.WebForms.PageRequestManager.getInstance().get_isInAsyncPostBack();")));
        }

        [Then(@"(.*) language pack should create successful")]
        public void ThenLanguagePackShouldCreateSuccessful(string name)
        {
            Thread.Sleep(2000);
            var langDir = Path.Combine(PhysicalPath, "Install\\Language");
            var packs = Directory.GetFiles(langDir, string.Format("*{0}*.zip", name));
            Assert.AreEqual(1, packs.Length);
        }

        #endregion
    }
}
