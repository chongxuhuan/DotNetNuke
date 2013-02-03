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
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using DotNetNuke.Common;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Tests.Steps;
using DotNetNuke.Tests.UI.WatiN.Common;
using DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects;

using NUnit.Framework;

using TechTalk.SpecFlow;

using WatiN.Core;

namespace DotNetNuke.Website.Specs.Modules.Html
{
	[Binding]
    public class TelerikEditorSteps : WatiNTest
    {
		public HTMLModule HtmlModule
		{
			get
			{
			    return GetPage<HTMLModule>();
			}
		}

		[When(@"I insert a document which file name contains space")]
		public void WhenIInsertADocumentWhichFileNameContainsSpace()
		{
            HtmlModule.TelerikEditor.TelerikPopUpFrame.Divs.Filter(Find.ByClass("rfeFileExtension doc"))
                .First(Find.ByText(t => t.Contains("Do Change.doc"))).Click();
            HtmlModule.TelerikEditor.TelerikPopUpFrame.Button(Find.ByTitle("Insert")).Click();
        }

        [When(@"Insert link with the file")]
        public void WhenInsertLinkWithTheFile()
        {
            HtmlModule.TelerikEditor.GetDialog("LinkManager").CheckBox(Find.ById("TrackLink")).Checked = true;
            HtmlModule.TelerikEditor.GetDialog("LinkManager").Button(Find.ById("lmInsertButton")).Click();
        }

		[Then(@"I should see the hyper link insert in rad text editor")]
		public void ThenIShouldSeeTheHyperLinkInsertInRadTextEditor()
		{
            var value = ExecuteEditorCommand("$find('dnn_ctr364_EditHTML_txtContent_txtContent').get_html();");
            Assert.IsTrue(Regex.IsMatch(value, "<a.+?>Hello</a>"));
		}

        [When("I click (.*) toolbar button")]
        public void IClickToolbarButton(string buttonName)
        {
            ExecuteEditorCommand("$find('dnn_ctr364_EditHTML_txtContent_txtContent').fire('" + buttonName + "', $find('dnn_ctr364_EditHTML_txtContent_txtContent'));");
        }

        [Then("(.*) Dialog must open")]
        public void DialogMustOpen(string dialogName)
        {
            if (HtmlModule.TelerikEditor.PopUpFrame != null)
            {
                WatiNAssert.AssertIsTrue(HtmlModule.TelerikEditor.PopUpFrame.Frame(Find.BySrc(s => s.Contains(dialogName))) != null, "DialogNotOpen.png");
            }
            else
            {
                WatiNAssert.AssertIsTrue(IEInstance.Frame(Find.BySrc(s => s.Contains(dialogName))) != null, "DialogNotOpen.png");
            }
        }

        private string ExecuteEditorCommand(string command)
        {
            if(HomePage.PopUpFrame != null)
            {
                return HomePage.PopUpFrame.Eval(command);
            }
            
            return IEInstance.Eval(command);
        }

	}
}
