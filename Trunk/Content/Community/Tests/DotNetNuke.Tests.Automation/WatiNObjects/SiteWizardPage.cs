/*
' DotNetNuke® - http://www.dotnetnuke.com
' Copyright (c) 2002-2012
' by DotNetNuke Corporation
'
' Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
' documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
' the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
' to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in all copies or substantial portions 
' of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
' DEALINGS IN THE SOFTWARE.
*/

using System.Collections.Specialized;
using System.Threading;
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The Admin Pages page object. 
    /// </summary>
    public class SiteWizardPage : WatiNBase
    {
        #region Constructors

        public SiteWizardPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public SiteWizardPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region TextFields

        public TextField PortalNameField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("txtPortalName"))); }
        }

        #endregion

        #region Links
        public Link NextButtonLink
        {
            get
            {
                var nextStartLink = IEInstance.Link(Find.ById(s => s.EndsWith("nextButtonStart")));
                if(nextStartLink.Exists)
                {
                    return nextStartLink;
                }

                return IEInstance.Link(Find.ById(s => s.EndsWith("nextButtonStep")));
            }
        }

        public Link PreviousButtonLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("StepPreviousButton"))); }
        }

        public Link FinishButtonLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("finishButtonStep"))); }
        }
        
        #endregion

        #endregion

        #region Public Methods
        
        public void SelectRadioButton(string value)
        {
            IEInstance.RadioButton(Find.ByValue(s => s.Contains(value))).Checked = true;
        }

        #endregion
    }
}
