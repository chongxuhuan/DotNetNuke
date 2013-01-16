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
    /// The Admin Skins page object.
    /// </summary>
    public class AdminSkinPage : WatiNBase
    {
        #region Constructors

        public AdminSkinPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public AdminSkinPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Links
        public Link DeleteSkinPackageLink
        {
            get { return IEInstance.Link(Find.ByTitle("Delete Skin Package")); }
        }
        /// <summary>
        /// The first Apply link on the skins page.
        /// This could be an apply link for a skin or container, depending on the state of the page.
        /// </summary>
        public Link FirstApplyLink
        {
            get { return IEInstance.Links.Filter(Find.ByText("Apply"))[0]; }
        }
        /// <summary>
        /// The first Delete link in the containers section.
        /// The container that will be deleted depends on the state of the page.
        /// This object assumes that the current set of containers can be deleted.
        /// </summary>
        public Link FirstDeleteContainerLink
        {
            get { return ContainersTable.Links.Filter(Find.ByText("Delete"))[0]; }
        }
        /// <summary>
        /// The first Delete link in the skins section.
        /// The skin that will be deleted depends on the state of the page.
        /// This object assumes that the current set of skins can be deleted.
        /// </summary>
        public Link FirstDeleteSkinLink
        {
            get { return SkinsTable.Links.Filter(Find.ByText("Delete"))[0]; }
        }
        /// <summary>
        /// The first Preview link on the skins page.
        /// This could be a preview link for a skin or a container, depending on the state of the page.
        /// </summary>
        public Link FirstPreviewLink
        {
            get { return IEInstance.Links.Filter(Find.ByText("Preview"))[0]; }
        }       
        #endregion

        #region SelectLists
        public SelectList ContainerSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("editskins_cboContainers"))); }
        }

        public SelectList SkinSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("editskins_cboSkins"))); }
        }
        #endregion

        #region CheckBoxes
        public CheckBox AdminCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("editskins_chkAdmin"))); }
        }
        public CheckBox HostSkinsCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("editskins_chkHost"))); }
        }

        public CheckBox PortalCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("editskins_chkPortal"))); }
        }

        public CheckBox SiteSkinsCheckBox
        {
            get { return IEInstance.CheckBox(Find.ById(s => s.EndsWith("editskins_chkSite"))); }
        }
        #endregion
        
        #region Tables
        /// <summary>
        /// The containers section.
        /// </summary>
        public Table ContainersTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("editskins_tblContainers"))); }
        }
        /// <summary>
        /// The skins section
        /// </summary>
        public Table SkinsTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("editskins_tblSkins"))); }
        }
        #endregion

        #region Spans
        /// <summary>
        /// The span containing the name of the first container.
        /// </summary>
        public Span FirstContainerNameSpan
        {
            get { return ContainersTable.OwnTableRows[0].OwnTableCells[0].Span(Find.Any); }
        }
        /// <summary>
        /// The span containing the name of the first skin.
        /// </summary>
        public Span FirstSkinNameSpan
        {
            get { return SkinsTable.OwnTableRows[0].OwnTableCells[0].Span(Find.Any);}
        }
        #endregion

        #endregion

        #region Public Methods



        #endregion
    }
}