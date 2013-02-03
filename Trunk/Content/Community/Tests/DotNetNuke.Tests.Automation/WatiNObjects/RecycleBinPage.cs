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
    /// The recycle bin page object.
    /// </summary>
    public class RecycleBinPage : WatiNBase
    {
        #region Constructors

        public RecycleBinPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public RecycleBinPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }
        
        #endregion

        #region Public Properties

        #region SelectLists
        public SelectList PagesRecycleBin
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("RecycleBin_tabsListBox"))); }
        }
        public SelectList ModulesRecycleBin
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("RecycleBin_modulesListBox"))); }
        }
        #endregion

        #region Links
        public Link EmptyBinLink
        {
            get { return IEInstance.Link(Find.ByTitle("Empty Recycle Bin")); }
        }
        public Link DeleteModuleLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("RecycleBin_cmdDeleteModule"))); }
        }
        public Link RestorePageLink
        {
            get { return IEInstance.Link(Find.ByTitle("Restore Selected Page")); }
        }
        public Link DeletePageLink
        {
            get { return IEInstance.Link(Find.ByTitle("Delete Selected Page")); }
        }
        public Link RestoreModuleLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("RecycleBin_cmdRestoreModule"))); }
        }
        #endregion
        
        #endregion

        #region Public Methods



        #endregion
    }
}
