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
namespace DotNetNuke.Website.Specs.MemberServices
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.8.1.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("Subscribing to a Member Service")]
    public partial class SubscribingToAMemberServiceFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
#line 1 "SubscribingToAMemberService.feature"
#line hidden
        
        [NUnit.Framework.TestFixtureSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Subscribing to a Member Service", "In order to subscribe to the website\'s services\r\nAs a customer\r\nI want to be able" +
                    " to subscribe to services", ProgrammingLanguage.CSharp, ((string[])(null)));
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
        [NUnit.Framework.DescriptionAttribute("Subscribing to a Member Service With A Fee")]
        [NUnit.Framework.CategoryAttribute("MustHaveAUserWithFullProfile")]
        [NUnit.Framework.CategoryAttribute("MustHaveSandboxedPaymentSettings")]
        [NUnit.Framework.CategoryAttribute("MustHaveServiceWithFee")]
        public virtual void SubscribingToAMemberServiceWithAFee()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Subscribing to a Member Service With A Fee", new string[] {
                        "MustHaveAUserWithFullProfile",
                        "MustHaveSandboxedPaymentSettings",
                        "MustHaveServiceWithFee"});
#line 9
this.ScenarioSetup(scenarioInfo);
#line 10
 testRunner.Given("I am on the site home page");
#line 11
 testRunner.And("I have logged in as the user MichaelWoods password1234");
#line 12
 testRunner.And("I have clicked on my name");
#line 13
 testRunner.When("I click Edit Profile");
#line 14
 testRunner.And("I click Manage Services");
#line 15
 testRunner.And("I Subscribe to a service with a fee");
#line 16
 testRunner.Then("I should be on the PayPal site with all the fields filled out");
#line hidden
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
