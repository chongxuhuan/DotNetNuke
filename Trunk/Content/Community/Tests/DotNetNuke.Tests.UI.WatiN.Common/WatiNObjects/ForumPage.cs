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
    /// The Forum object.
    /// </summary>
    public class ForumPage : WatiNBase
    {
        #region Constructors

        public ForumPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ForumPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties 

        #region Links
        public Link GeneralDiscussionLink
        {
            get { return IEInstance.Link(Find.ByText("General")); }
        }
        public Link NewThreadLink
        {
            get { return IEInstance.Link(Find.ByText("New Thread")); }
        }
        public Link SubmitLink
        {
            get { return IEInstance.Link(Find.ByTitle("Submit")); }
        }
        #endregion

        #region TextFields
        public TextField ForumContentField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("Forum_PostEdit_teContent_txtDesktopHTML"))); }
        }
        public TextField ForumSubjectField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("Forum_PostEdit_txtSubject"))); }
        }
        #endregion 

        #region RadioButtons
        public RadioButton BasicTextBoxEditorRadioButton
        {
            get { return IEInstance.RadioButton(Find.ByValue("BASIC")); }
        }

        #endregion

        #region Divs
        public Div ForumHeaderDiv
        {
            get { return IEInstance.Div(Find.ByClass("Forum_HeaderText")); }
        }
        #endregion

        #region Spans
        public Span ForumPostAuthorSpan
        {
            get { return IEInstance.Spans.Filter(Find.ByClass("Forum_LastPostText"))[1]; }
        }
        public Span ForumPostTimeSpan
        {
            get { return IEInstance.Spans.Filter(Find.ByClass("Forum_LastPostText"))[0]; }
        }        
        #endregion

        #endregion

        #region Public Methods


        #endregion
    }
}
