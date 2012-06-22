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
namespace DotNetNuke.Tests.BuildVerification
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Newsletter")]
    public partial class NewsletterFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "Newsletter.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Newsletter", "In order to inform my users about news and special offers\r\nAs a super user\r\nI wan" +
                    "t to be able to send newsletters to users of my site", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Send Newsletter To Registered Users")]
        [NUnit.Framework.CategoryAttribute("MustHaveEmailSetUpForSiteDumpToFolder")]
        [NUnit.Framework.CategoryAttribute("MustHaveUser1Added")]
        [NUnit.Framework.CategoryAttribute("MustHaveEmptyEmailFolder")]
        [NUnit.Framework.CategoryAttribute("SiteMustRunInFullTrust")]
        public virtual void SendNewsletterToRegisteredUsers()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Send Newsletter To Registered Users", new string[] {
                        "MustHaveEmailSetUpForSiteDumpToFolder",
                        "MustHaveUser1Added",
                        "MustHaveEmptyEmailFolder",
                        "SiteMustRunInFullTrust"});
#line 10
this.ScenarioSetup(scenarioInfo);
#line 11
 testRunner.Given("I am on the site home page");
#line 12
 testRunner.And("I have logged in as the host");
#line 13
 testRunner.And("I am on the admin page Newsletters");
#line hidden
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "Label",
                        "Value"});
            table1.AddRow(new string[] {
                        "FromAddress",
                        "test@dnncorp.com"});
            table1.AddRow(new string[] {
                        "ReplyTo",
                        "reply@dnncorp.com"});
            table1.AddRow(new string[] {
                        "SubjectField",
                        "New Newsletter Subject"});
            table1.AddRow(new string[] {
                        "ContentField",
                        "This is the Newsletter"});
#line 14
 testRunner.When("I fill in the newsletter form with the following information", ((string)(null)), table1);
#line 20
 testRunner.And("I check the role Registered Users on the newsletters page");
#line 21
 testRunner.And("I click Send");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "Label",
                        "Value"});
            table2.AddRow(new string[] {
                        "From",
                        "test@dnncorp.com"});
            table2.AddRow(new string[] {
                        "ReplyTo",
                        "reply@dnncorp.com"});
            table2.AddRow(new string[] {
                        "Subject",
                        "New Newsletter Subject"});
            table2.AddRow(new string[] {
                        "Content",
                        "This is the Newsletter"});
#line 22
 testRunner.Then("The following newsletter should be sent to testuser1@dnn.com", ((string)(null)), table2);
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "Label",
                        "Value"});
            table3.AddRow(new string[] {
                        "From",
                        "test@dnncorp.com"});
            table3.AddRow(new string[] {
                        "ReplyTo",
                        "reply@dnncorp.com"});
            table3.AddRow(new string[] {
                        "Subject",
                        "New Newsletter Subject"});
            table3.AddRow(new string[] {
                        "Content",
                        "This is the Newsletter"});
#line 28
 testRunner.And("The following newsletter should be sent to admin@change.me", ((string)(null)), table3);
#line hidden
            TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                        "Label",
                        "Value"});
            table4.AddRow(new string[] {
                        "From",
                        "test@dnncorp.com"});
            table4.AddRow(new string[] {
                        "Recipients",
                        "2"});
            table4.AddRow(new string[] {
                        "Messages",
                        "2"});
            table4.AddRow(new string[] {
                        "Subject",
                        "New Newsletter Subject"});
#line 34
 testRunner.And("The following bulk email report should be sent to test@dnncorp.com", ((string)(null)), table4);
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
