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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Services.Localization;

using Telerik.Web.UI;

#endregion

namespace DotNetNuke.Web.UI.WebControls
{
    [ParseChildren(true)]
    public class DnnFormEditor : WebControl, INamingContainer
    {
        private object _dataSource;
        private int _itemCount;

        public DnnFormEditor()
        {
            FieldStyle = new Style();
            FooterStyle = new Style();
            GroupStyle = new Style();
            HeaderStyle = new Style();
            ItemStyleLong = new Style();
            ItemStyleShort = new Style();
            LabelStyle = new Style();
            SectionStyle = new Style();
            TabStyle = new Style();
            ToolTipStyle = new Style();

            Items = new List<DnnFormItemBase>();
            Sections = new List<DnnFormSection>();
            Tabs = new List<DnnFormTab>();

            FormMode = DnnFormMode.Long;
            SectionExpandMode = PanelBarExpandMode.MultipleExpandedItems;
        }

        protected string LocalResourceFile
        {
            get
            {
                return Utilities.GetLocalResourceFile(this);
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        public object DataSource
        {
            get
            {
                return _dataSource;
            }
            set
            {
                if (_dataSource != value)
                {
                    _dataSource = value;
                    DataBindItems(false);
                }
            }
        }

        public DnnFormMode FormMode { get; set; }

        public Unit FixedSectionHeight { get; set; }

        public bool IsValid
        {
            get
            {
                bool isValid = true;
                foreach (var item in GetAllItems())
                {
                    if(!item.IsValid)
                    {
                        isValid = false;
                        break;
                    }
                }
                return isValid;
            }
        }

        [Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DnnFormItemBase> Items { get; private set; }

        public PanelBarExpandMode SectionExpandMode { get; set; }

        [Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DnnFormSection> Sections { get; private set; }

        [Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<DnnFormTab> Tabs { get; private set; }

        #region Styles

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Fields.")]
        public Style FieldStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Footer.")]
        public Style FooterStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Groups.")]
        public Style GroupStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Header.")]
        public Style HeaderStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Items in the long form.")]
        public Style ItemStyleLong { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Items in the short form.")]
        public Style ItemStyleShort { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Labels.")]
        public Style LabelStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Sections.")]
        public Style SectionStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Tabs.")]
        public Style TabStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)),
         PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the ToolTips.")]
        public Style ToolTipStyle { get; private set; }

        #endregion

        #region Templates

        [Browsable(false), DefaultValue(null), Description("The Footer Template."), TemplateInstance(TemplateInstance.Single), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (DnnFormEmptyTemplate))]
        public ITemplate FooterTemplate { get; set; }

        [Browsable(false), DefaultValue(null), Description("The Header Template."), TemplateInstance(TemplateInstance.Single), PersistenceMode(PersistenceMode.InnerProperty),
         TemplateContainer(typeof (DnnFormEmptyTemplate))]
        public ITemplate HeaderTemplate { get; set; }

        #endregion

        private List<DnnFormItemBase> GetAllItems()
        {
            var items = new List<DnnFormItemBase>();

            //iterate over pages
            foreach (DnnFormTab page in Tabs)
            {
                foreach (DnnFormSection section in page.Sections)
                {
                    items.AddRange(section.Items);
                }
                items.AddRange(page.Items);
            }

            //iterate over section
            foreach (DnnFormSection section in Sections)
            {
                items.AddRange(section.Items);
            }

            //Add base items
            items.AddRange(Items);

            return items;
        }

        #region Private Methods

        private void SetUpFooter()
        {
            if (FooterTemplate != null)
            {
                var template = new DnnFormEmptyTemplate();
                FooterTemplate.InstantiateIn(template);
                template.ApplyStyle(FooterStyle);
                template.CssClass = "dnnformFooter";
                Controls.Add(template);
            }
        }

        private void SetUpHeader()
        {
            if (HeaderTemplate != null)
            {
                var template = new DnnFormEmptyTemplate();
                HeaderTemplate.InstantiateIn(template);
                template.ApplyStyle(HeaderStyle);
                template.CssClass = "dnnformHeader";
                Controls.Add(template);
            }
        }

        private static Panel SetUpGroup(string group, WebControl parentControl, string localResourceFile)
        {
            var groupPanel = new Panel();
            var resourceKey = group;
            groupPanel.GroupingText = Localization.GetString(resourceKey, localResourceFile);
            groupPanel.CssClass = "dnnformGroup";

            //Add Group Header
            var helpText = Localization.GetString(resourceKey + ".Help", localResourceFile);
            if (!String.IsNullOrEmpty(helpText))
            {
                groupPanel.Controls.Add(new LiteralControl(Localization.GetString(resourceKey + ".Help", localResourceFile)));
            }

            parentControl.Controls.Add(groupPanel);

            return groupPanel;
        }

        internal static void SetUpItems(IEnumerable<DnnFormItemBase> items, WebControl parentControl, string localResourceFile)
        {
            string previousGroup = "";
            Panel groupPanel = null;
            foreach (DnnFormItemBase item in items)
            {
                if (String.IsNullOrEmpty(item.Group))
                {
                    //Add directly to form
                    parentControl.Controls.Add(item);
                }
                else
                {
                    if (previousGroup != item.Group)
                    {
                        //new Group
                        groupPanel = SetUpGroup(item.Group, parentControl, localResourceFile);
                    }
                    if (groupPanel != null)
                    {
                        groupPanel.Controls.Add(item);
                    }
                    previousGroup = item.Group;
                }
            }
        }

        private void SetUpSections(List<DnnFormSection> sections, WebControl parentControl)
        {
            if (sections.Count > 0)
            {
                var panelBar = new DnnPanelBar
                                   {
                                       ExpandMode = SectionExpandMode, 
                                       ID = parentControl.ID + "_Sections", 
                                       CssClass = "dnnformSections"
                                   };
                if (SectionExpandMode == PanelBarExpandMode.FullExpandedItem)
                {
                    panelBar.Height = FixedSectionHeight;
 
                }
                parentControl.Controls.Add(panelBar);

                foreach (DnnFormSection section in sections)
                {
                    var resourceKey = section.ResourceKey;
                    if (String.IsNullOrEmpty(resourceKey))
                    {
                        resourceKey = section.ID;
                    }
                    var panel = new DnnPanelItem {Text = Localization.GetString(resourceKey, LocalResourceFile), Expanded = section.Expanded};
                    panel.ControlStyle.MergeWith(SectionStyle);
                    var template = new DnnFormSectionTemplate();
                    template.Items.AddRange(section.Items);
                    template.LocalResourceFile = LocalResourceFile;
                    panel.ContentTemplate = template;
                    panelBar.Items.Add(panel);
                }
            }
        }

        private void SetUpTabs()
        {
            if (Tabs.Count > 0)
            {
                var multiPage = new DnnMultiPage {ID = "Pages", CssClass = "dnnformPageContainer"};
                if (SectionExpandMode == PanelBarExpandMode.FullExpandedItem)
                {
                    multiPage.Height = FixedSectionHeight;
                }
                var tabStrip = new DnnTabStrip {ID = "Tabs", MultiPageID = multiPage.ID, CausesValidation = false, CssClass = "dnnformTabStrip"};
                tabStrip.ControlStyle.MergeWith(TabStyle);

                Controls.Add(tabStrip);
                Controls.Add(multiPage);

                multiPage.PageViews.Clear();
                tabStrip.Tabs.Clear();

                foreach (DnnFormTab formTab in Tabs)
                {
                    var resourceKey = formTab.ResourceKey;
                    if (String.IsNullOrEmpty(resourceKey))
                    {
                        resourceKey = formTab.ID;
                    }
                    var tab = new RadTab { Text = Localization.GetString(resourceKey, LocalResourceFile), CssClass = "dnnformTab"};
                    tabStrip.Tabs.Add(tab);

                    var pageView = new RadPageView { ID = formTab.ID, CssClass = "dnnformPage" };
                    if (SectionExpandMode == PanelBarExpandMode.FullExpandedItem)
                    {
                        pageView.Height = FixedSectionHeight;
                    }
                    multiPage.PageViews.Add(pageView);

                    if (formTab.Sections.Count > 0)
                    {
                        SetUpSections(formTab.Sections, pageView);
                    }
                    else
                    {
                        pageView.CssClass += " dnnformNoSections";
                    }

                    SetUpItems(formTab.Items, pageView, LocalResourceFile);
                }

                tabStrip.SelectedIndex = 0;
                multiPage.SelectedIndex = 0;
            }
        }

        #endregion

        #region Control Hierarchy and Data Binding

        protected override void CreateChildControls()
        {
            // CreateChildControls re-creates the children (the items)
            // using the saved view state.
            // First clear any existing child controls.
            Controls.Clear();

            // Create the items only if there is view state
            // corresponding to the children.
            if (_itemCount > 0)
            {
                CreateControlHierarchy(false);
            }
        }

        private void DataBindItems(bool useDataSource)
        {
            var items = GetAllItems();

            foreach (DnnFormItemBase item in items)
            {
                item.LocalResourceFile = LocalResourceFile;
                if (item.FormMode == DnnFormMode.Inherit)
                {
                    item.FormMode = FormMode;
                }

                if (DataSource != null)
                {
                    item.DataSource = DataSource;
                    if (useDataSource)
                    {
                        item.DataBind();
                    }
                }
            }
            _itemCount = GetAllItems().Count;
        }

        protected virtual void CreateControlHierarchy(bool useDataSource)
        {
            CssClass = "dnnForm";
            if (FixedSectionHeight.IsEmpty && SectionExpandMode == PanelBarExpandMode.FullExpandedItem)
            {
                FixedSectionHeight = new Unit(450);
            }
            if (SectionExpandMode != PanelBarExpandMode.FullExpandedItem)
            {
                FixedSectionHeight = Unit.Empty;
            }

            SetUpHeader();

            SetUpTabs();

            SetUpSections(Sections, this);

            SetUpItems(Items, this, LocalResourceFile);

            SetUpFooter();

            DataBindItems(useDataSource);
        }

        public override void DataBind()
        {
            base.OnDataBinding(EventArgs.Empty);
            Controls.Clear();
            ClearChildViewState();
            TrackViewState();
            CreateControlHierarchy(true);
            ChildControlsCreated = true;
        }

        #endregion

        #region Protected Methods

        protected override void LoadControlState(object state)
        {
            if (state != null)
            {
                _itemCount = (int) state;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
        }

        protected override object SaveControlState()
        {
            return _itemCount > 0 ? (object) _itemCount : null;
        }

        #endregion
    }
}