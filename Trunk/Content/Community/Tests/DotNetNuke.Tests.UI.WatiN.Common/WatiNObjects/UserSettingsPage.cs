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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The user accounts settings page object.
    /// </summary>
    public class UserSettingsPage : WatiNBase
    {
        #region Constructors

        public UserSettingsPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public UserSettingsPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region RadioButton
        public RadioButton ValidProfileRequiredForLoginRadioButton
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.RadioButtons.Filter(Find.ByName(s => s.EndsWith("Security_RequireValidProfileAtLogin"))).Filter(Find.ByValue("True"))[0];
                }
                return IEInstance.RadioButtons.Filter(Find.ByName(s => s.EndsWith("Security_RequireValidProfileAtLogin"))).Filter(Find.ByValue("True"))[0];
            }
        }
        public RadioButton ValidProfileNotRequiredForLoginRadioButton
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.RadioButtons.Filter(Find.ByName(s => s.EndsWith("Security_RequireValidProfileAtLogin"))).Filter(Find.ByValue("False"))[0];
                }
                return IEInstance.RadioButtons.Filter(Find.ByName(s => s.EndsWith("Security_RequireValidProfileAtLogin"))).Filter(Find.ByValue("False"))[0];
            }
        }
        #endregion

        #region SelectList
        public SelectList RedirectAfterLoginSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$Redirect_AfterLogin")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$Redirect_AfterLogin")));
            }
        }
        public SelectList RedirectAfterRegistrationSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$Redirect_AfterRegistration")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$Redirect_AfterRegistration")));
            }
        }
        #endregion


        #endregion

        #region Public Methods





        #endregion
    }
}