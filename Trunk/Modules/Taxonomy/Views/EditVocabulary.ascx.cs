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
using System.Collections.Generic;
using System.Linq;

using DotNetNuke.Entities.Content.Taxonomy;
using DotNetNuke.Modules.Taxonomy.Presenters;
using DotNetNuke.Modules.Taxonomy.Views.Models;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Mvp;
using DotNetNuke.Web.UI.WebControls;

using WebFormsMvp;


#endregion

namespace DotNetNuke.Modules.Taxonomy.Views
{
    [PresenterBinding(typeof (EditVocabularyPresenter))]
    public partial class EditVocabulary : ModuleView<EditVocabularyModel>, IEditVocabularyView
    {
        #region "Protected Methods"

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            addTermButton.Click += addTermButton_Click;
            cancelEdit.Click += cancelEdit_Click;
            cancelTermButton.Click += cancelTermButton_Click;
            deleteTermButton.Click += deleteTermButton_Click;
            deleteVocabulary.Click += deleteVocabulary_Click;
            saveTermButton.Click += saveTermButton_Click;
            saveVocabulary.Click += saveVocabulary_Click;
            termsList.SelectedTermChanged += termsList_SelectedTermChanged;
        }

        #endregion

        #region "IEditVocabularyView Implementation"

        public event EventHandler AddTerm;
        public event EventHandler Cancel;
        public event EventHandler CancelTerm;
        public event EventHandler Delete;
        public event EventHandler DeleteTerm;
        public event EventHandler Save;
        public event EventHandler SaveTerm;
        public event EventHandler<TermsEventArgs> SelectTerm;


        public void BindTerm(Term term, IEnumerable<Term> terms, bool isHeirarchical, bool loadFromControl, bool editEnabled)
        {
            editTermControl.BindTerm(term, terms, isHeirarchical, loadFromControl, editEnabled);
        }

        public void BindTerms(IEnumerable<Term> terms, bool isHeirarchical, bool dataBind)
        {
            termsList.BindTerms(terms.ToList(), isHeirarchical, dataBind);
        }

        public void BindVocabulary(Vocabulary vocabulary, bool editEnabled, bool deleteEnabled, bool showScope)
        {
            editVocabularyControl.BindVocabulary(vocabulary, editEnabled, showScope);
            saveVocabulary.Enabled = editEnabled;
            deleteVocabulary.Visible = deleteEnabled;
            addTermButton.Enabled = editEnabled;
            saveTermButton.Enabled = editEnabled;
            deleteTermButton.Enabled = editEnabled;
        }

        public void ClearSelectedTerm()
        {
            termsList.ClearSelectedTerm();
            termEditor.Visible = false;
        }

        public void SetTermEditorMode(bool isAddMode, int termId)
        {
            if (isAddMode)
            {
                termLabel.Text = Localization.GetString("NewTerm", LocalResourceFile);
                saveTermButton.Text = "CreateTerm";
            }
            else
            {
                termLabel.Text = Localization.GetString("CurrentTerm", LocalResourceFile);
                saveTermButton.Text = "SaveTerm";
            }

            deleteTermPlaceHolder.Visible = !isAddMode;
        }

        public void ShowTermEditor(bool showEditor)
        {
            termEditor.Visible = showEditor;
        }

        #endregion

        #region "Event Handlers"

        private void addTermButton_Click(object sender, EventArgs e)
        {
            if (AddTerm != null)
            {
                AddTerm(this, e);
            }
        }

        private void cancelEdit_Click(object sender, EventArgs e)
        {
            if (Cancel != null)
            {
                Cancel(this, e);
            }
        }

        private void cancelTermButton_Click(object sender, EventArgs e)
        {
            if (CancelTerm != null)
            {
                CancelTerm(this, e);
            }
        }

        private void deleteTermButton_Click(object sender, EventArgs e)
        {
            if (DeleteTerm != null)
            {
                DeleteTerm(this, e);
            }
        }

        private void deleteVocabulary_Click(object sender, EventArgs e)
        {
            if (Delete != null)
            {
                Delete(this, e);
            }
        }

        private void saveTermButton_Click(object sender, EventArgs e)
        {
            if (SaveTerm != null)
            {
                SaveTerm(this, e);
            }
        }

        private void saveVocabulary_Click(object sender, EventArgs e)
        {
            if (Save != null)
            {
                Save(this, e);
            }
        }

        private void termsList_SelectedTermChanged(object sender, TermsEventArgs e)
        {
            if (SelectTerm != null)
            {
                SelectTerm(this, e);
            }
        }

        #endregion
    }
}