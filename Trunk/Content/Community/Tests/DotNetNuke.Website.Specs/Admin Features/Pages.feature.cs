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
#region Designer generated code
#pragma warning disable
namespace DotNetNuke.Website.Specs.AdminFeatures
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("pages management")]
    public partial class PagesManagementFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Pages.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "pages management", "In order to let user can manage pages easily\r\nAs an administrator\r\nI want to be u" +
                    "se page management correctly", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.TestFixtureTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public virtual void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Click help icon")]
        [NUnit.Framework.CategoryAttribute("MustBeDefaultAdminCredentials")]
        public virtual void ClickHelpIcon()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Click help icon", new string[] {
                        "MustBeDefaultAdminCredentials"});
#line 7
this.ScenarioSetup(scenarioInfo);
#line 8
 testRunner.Given("I am on the site home page");
#line 9
 testRunner.And("I have logged in as the admin");
#line 10
 testRunner.When("I navigate to the admin page Pages");
#line 11
 testRunner.And("I click on a page in the page treeview");
#line 12
 testRunner.And("I click help icon");
#line 13
 testRunner.Then("I should see help text");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Page create with template should apply permisson from template")]
        [NUnit.Framework.CategoryAttribute("MustBeDefaultAdminCredentials")]
        [NUnit.Framework.CategoryAttribute("MustNotHaveTheTestPages")]
        public virtual void PageCreateWithTemplateShouldApplyPermissonFromTemplate()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Page create with template should apply permisson from template", new string[] {
                        "MustBeDefaultAdminCredentials",
                        "MustNotHaveTheTestPages"});
#line 17
this.ScenarioSetup(scenarioInfo);
#line 18
 testRunner.Given("I am on the site home page");
#line 19
 testRunner.And("I have logged in as the host");
#line 20
 testRunner.And("I clean the cache");
#line 21
 testRunner.And("I have created the default page called Test A from the Ribbon Bar");
#line 22
 testRunner.And("The page Test A has View permission set to Grant for the role Registered Users");
#line 23
 testRunner.And("I export template of the page Test A");
#line 24
 testRunner.And("I have created page called Test B with template TestA from the Ribbon Bar");
#line 25
 testRunner.Then("The page Test B should have View permisson on role Registered Users");
#line hidden
            this.ScenarioCleanup();
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Should not allow to add page when page quota is exceed")]
        [NUnit.Framework.CategoryAttribute("MustBeDefaultAdminCredentials")]
        [NUnit.Framework.CategoryAttribute("ResetPageQuota")]
        public virtual void ShouldNotAllowToAddPageWhenPageQuotaIsExceed()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Should not allow to add page when page quota is exceed", new string[] {
                        "MustBeDefaultAdminCredentials",
                        "ResetPageQuota"});
#line 29
this.ScenarioSetup(scenarioInfo);
#line 30
 testRunner.Given("I am on the site home page");
#line 31
 testRunner.And("I have logged in as the host");
#line 32
 testRunner.When("I navigate to the admin page Site Settings");
#line 33
 testRunner.And("I update page quota to 5");
#line 34
 testRunner.And("I log off");
#line 35
 testRunner.And("I have logged in as the admin");
#line 36
 testRunner.Then("the add page button should disabled in Ribbon bar");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
