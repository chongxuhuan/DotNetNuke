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
    /// The search admin page object
    /// </summary>
    public class SearchAdminPage : WatiNBase
    {
        #region Constructors

        public SearchAdminPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public SearchAdminPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region TextFields
        public TextField MinimumWordLengthField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SearchAdmin_txtMinWordLength"))); }
        }
        public TextField MaximumWordLengthField
        {
            get { return IEInstance.TextField(Find.ById(s => s.EndsWith("SearchAdmin_txtMaxWordLength"))); }
        }
        #endregion

        #region CheckBoxes
        public CheckBox IncludeCommonWordsCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SearchAdmin_chkIncludeCommon"))); }
        }
        public CheckBox IncludeNumbersCheckbox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("SearchAdmin_chkIncludeNumeric")));}
        }
        #endregion

        #region Links
        public Link ReIndexContentLink
        {
            get { return IEInstance.Link(Find.ByTitle("Re-Index Content")); }
        }
        #endregion

        #region Spans
        public Span SearchResultsMessageSpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("SearchResults_lblMessage"))); }
        }
        public Span SearchResultsSummarySpan
        {
            get { return IEInstance.Span(Find.ById(s => s.EndsWith("lblSummary"))); }
        }
        #endregion

        #endregion

        #region Public Methods




        #endregion
    }
}