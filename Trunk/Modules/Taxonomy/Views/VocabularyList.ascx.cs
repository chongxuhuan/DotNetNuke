#region Copyright

// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

#region Usings

using System;

using DotNetNuke.Modules.Taxonomy.Presenters;
using DotNetNuke.Modules.Taxonomy.Views.Models;
using DotNetNuke.Web.Mvp;
using DotNetNuke.Web.UI.WebControls;

using WebFormsMvp;


#endregion

namespace DotNetNuke.Modules.Taxonomy.Views
{
    [PresenterBinding(typeof (VocabularyListPresenter))]
    public partial class VocabularyList : ModuleView<VocabularyListModel>, IVocabularyListView
    {
        #region "Protected Methods"

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            addVocabularyButton.Click += addVocabularyButton_Click;
            vocabulariesGrid.PreRender += vocabulariesGrid_PreRender;
        }

        #endregion

        #region "IVocabularyListView Implementation"

        public event EventHandler AddVocabulary;

        public void ShowAddButton(bool showButton)
        {
            addVocabularyButton.Visible = showButton;
        }

        #endregion

        #region "Event Handlers"

        private void addVocabularyButton_Click(object sender, EventArgs e)
        {
            if (AddVocabulary != null)
            {
                AddVocabulary(this, e);
            }
        }

        private void vocabulariesGrid_PreRender(object sender, EventArgs e)
        {
            var hyperlinkColumn = vocabulariesGrid.Columns[0] as DnnGridHyperLinkColumn;
            if (hyperlinkColumn != null)
            {
                hyperlinkColumn.Visible = Model.IsEditable;
                hyperlinkColumn.DataNavigateUrlFormatString = Model.NavigateUrlFormatString;
            }
        }

        #endregion
    }
}