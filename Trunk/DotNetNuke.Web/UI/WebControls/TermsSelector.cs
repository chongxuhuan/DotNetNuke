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
using System.Web.UI;

using DotNetNuke.Entities.Content;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Content.Taxonomy;

using Telerik.Web.UI;

#endregion

namespace DotNetNuke.Web.UI.WebControls
{
    public class TermsSelector : DnnComboBox
    {
        #region "Public Properties"

        public int PortalId
        {
            get
            {
                return Convert.ToInt32(ViewState["PortalId"]);
            }
            set
            {
                ViewState["PortalId"] = value;
            }
        }

        public List<Term> Terms
        {
            get
            {
                return ViewState["Terms"] as List<Term>;
            }
            set
            {
                ViewState["Terms"] = value;
            }
        }

        #endregion

        #region "Protected Methods"

        protected override void OnInit(EventArgs e)
        {
            ItemTemplate = new TreeViewTemplate();
            Items.Add(new RadComboBoxItem());
            base.OnInit(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Text = Terms.ToDelimittedString(", ");
            ToolTip = Terms.ToDelimittedString(", ");
        }

        #endregion

        #region "Private Template Class"

        public class TreeViewTemplate : ITemplate
        {
            private RadComboBoxItem _Container;
            private List<Term> _Terms;
            private TermsSelector _TermsSelector;

            private DnnTreeView _Tree;

            private List<Term> SelectedTerms
            {
                get
                {
                    return _TermsSelector.Terms;
                }
            }

            private List<Term> Terms
            {
                get
                {
                    if (_Terms == null)
                    {
                        ITermController termRep = Util.GetTermController();
                        IVocabularyController vocabRep = Util.GetVocabularyController();
                        _Terms = new List<Term>();
                        var vocabularies = from v in vocabRep.GetVocabularies() where v.ScopeType.ScopeType == "Application" || (v.ScopeType.ScopeType == "Portal" && v.ScopeId == PortalId) select v;

                        foreach (Vocabulary v in vocabularies)
                        {
                            //Add a dummy parent term if simple vocabulary
                            if (v.Type == VocabularyType.Simple)
                            {
                                Term dummyTerm = new Term(v.VocabularyId);
                                dummyTerm.ParentTermId = null;
                                dummyTerm.Name = v.Name;
                                dummyTerm.TermId = -v.VocabularyId;
                                _Terms.Add(dummyTerm);
                            }
                            foreach (Term t in termRep.GetTermsByVocabulary(v.VocabularyId))
                            {
                                if (v.Type == VocabularyType.Simple)
                                {
                                    t.ParentTermId = -v.VocabularyId;
                                }
                                _Terms.Add(t);
                            }
                        }
                    }
                    return _Terms;
                }
            }

            private int PortalId
            {
                get
                {
                    return _TermsSelector.PortalId;
                }
            }

            #region ITemplate Members

            public void InstantiateIn(Control container)
            {
                _Container = (RadComboBoxItem) container;
                _TermsSelector = (TermsSelector) container.Parent;

                _Tree = new DnnTreeView();
                _Tree.DataTextField = "Name";
                _Tree.DataValueField = "TermId";
                _Tree.DataFieldID = "TermId";
                _Tree.DataFieldParentID = "ParentTermId";
                _Tree.CheckBoxes = true;
                _Tree.ExpandAllNodes();

                _Tree.DataSource = Terms;

                _Tree.NodeDataBound += TreeNodeDataBound;
                _Tree.NodeCheck += TreeNodeChecked;
                _Tree.DataBound += TreeDataBound;

                _Container.Controls.Add(_Tree);
            }

            #endregion

            private void TreeDataBound(object sender, EventArgs e)
            {
                _Tree.ExpandAllNodes();
            }

            private void TreeNodeChecked(object sender, RadTreeNodeEventArgs e)
            {
                RadTreeNode node = e.Node;
                int termId = int.Parse(node.Value);

                if (node.Checked)
                {
                    //Add Term
                    foreach (Term term in Terms)
                    {
                        if (term.TermId == termId)
                        {
                            SelectedTerms.Add(term);
                            break;
                        }
                    }
                }
                else
                {
                    //Remove Term
                    foreach (Term term in SelectedTerms)
                    {
                        if (term.TermId == termId)
                        {
                            SelectedTerms.Remove(term);
                            break;
                        }
                    }
                }

                //Rebind
                _Tree.DataBind();
            }


            private void TreeNodeDataBound(object sender, RadTreeNodeEventArgs e)
            {
                RadTreeNode node = e.Node;
                Term term = node.DataItem as Term;

                if (term.TermId < 0)
                {
                    node.Checkable = false;
                }
                foreach (Term tag in SelectedTerms)
                {
                    if (tag.TermId == term.TermId)
                    {
                        node.Checked = true;
                        break;
                    }
                }
            }
        }

        #endregion
    }
}