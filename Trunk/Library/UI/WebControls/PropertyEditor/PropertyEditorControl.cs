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
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.UI.Utilities;
using DotNetNuke.UI.WebControls.Design;

#endregion

namespace DotNetNuke.UI.WebControls
{
    [ToolboxData("<{0}:PropertyEditorControl runat=server></{0}:PropertyEditorControl>"), Designer(typeof (PropertyEditorControlDesigner)), PersistChildren(true)]
    public class PropertyEditorControl : WebControl, INamingContainer
    {
        private bool _itemChanged;
        private Hashtable _sections;
        private IEnumerable _underlyingDataSource;

        public PropertyEditorControl()
        {
            VisibilityStyle = new Style();
            ItemStyle = new Style();
            LabelStyle = new Style();
            HelpStyle = new Style();
            GroupHeaderStyle = new Style();
            ErrorStyle = new Style();
            EditControlStyle = new Style();
            Fields = new ArrayList();
            ShowRequired = true;
            LabelMode = LabelMode.Left;
            HelpDisplayMode = HelpDisplayMode.Always;
            Groups = Null.NullString;
            AutoGenerate = true;
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        protected virtual IEnumerable UnderlyingDataSource
        {
            get { return _underlyingDataSource ?? (_underlyingDataSource = GetProperties()); }
        }

        [Category("Behavior")]
        public bool AutoGenerate { get; set; }

        [Browsable(false), Category("Data")]
        public object DataSource { get; set; }

        [Category("Appearance")]
        public PropertyEditorMode EditMode { get; set; }

        public EditorDisplayMode DisplayMode { get; set; }

        [Category("Behavior")]
        public bool EnableClientValidation { get; set; }

        [Category("Appearance")]
        public GroupByMode GroupByMode { get; set; }

        [Category("Appearance")]
        public string Groups { get; set; }

        public HelpDisplayMode HelpDisplayMode { get; set; }

        [Browsable(false)]
        public bool IsDirty
        {
            get
            {
                return Fields.Cast<FieldEditorControl>().Any(editor => editor.Visible && editor.IsDirty);
            }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                return Fields.Cast<FieldEditorControl>().All(editor => !editor.Visible || editor.IsValid);
            }
        }

        public LabelMode LabelMode { get; set; }

        public string LocalResourceFile { get; set; }

        public string RequiredUrl { get; set; }

        public bool ShowRequired { get; set; }

        [Category("Appearance")]
        public bool ShowVisibility { get; set; }

        [Category("Appearance")]
        public PropertySortType SortMode { get; set; }

        [Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Fields { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)), PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Edit Control.")]
        public Style EditControlStyle { get; private set; }

        [Browsable(true), Category("Appearance"), Description("Set the Width for the Edit Control.")]
        public Unit EditControlWidth { get; set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)), PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Error Text.")]
        public Style ErrorStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)), PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Group Header Control.")]
        public Style GroupHeaderStyle { get; private set; }

        [Browsable(true), Category("Appearance"), Description("Set whether to include a rule <hr> in the Group Header.")]
        public bool GroupHeaderIncludeRule { get; set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)), PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Help Text.")]
        public Style HelpStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter)), PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Label Text")]
        public Style ItemStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)), PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Label Text")]
        public Style LabelStyle { get; private set; }

        [Browsable(true), Category("Appearance"), Description("Set the Width for the Label Control.")]
        public Unit LabelWidth { get; set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof (ExpandableObjectConverter)), PersistenceMode(PersistenceMode.InnerProperty), Description("Set the Style for the Visibility Control")]
        public Style VisibilityStyle { get; private set; }

        public event PropertyChangedEventHandler ItemAdded;
        public event EditorCreatedEventHandler ItemCreated;
        public event PropertyChangedEventHandler ItemDeleted;

        private IEnumerable<PropertyInfo> GetProperties()
        {
            if (DataSource != null)
            {
                const BindingFlags bindings = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;
                var properties = DataSource.GetType().GetProperties(bindings);
                switch (SortMode)
                {
                    case PropertySortType.Alphabetical:
                        Array.Sort(properties, new PropertyNameComparer());
                        break;
                    case PropertySortType.Category:
                        Array.Sort(properties, new PropertyCategoryComparer());
                        break;
                    case PropertySortType.SortOrderAttribute:
                        Array.Sort(properties, new PropertySortOrderComparer());
                        break;
                }
                return properties;
            }
            return null;
        }

        private void AddEditorRow(FieldEditorControl editor, WebControl container)
        {
            editor.ControlStyle.CopyFrom(ItemStyle);
            editor.LabelStyle.CopyFrom(LabelStyle);
            editor.HelpStyle.CopyFrom(HelpStyle);
            editor.ErrorStyle.CopyFrom(ErrorStyle);
            editor.VisibilityStyle.CopyFrom(VisibilityStyle);
            editor.EditControlStyle.CopyFrom(EditControlStyle);
            if (editor.EditControlWidth == Unit.Empty)
            {
                editor.EditControlWidth = EditControlWidth;
            }
            editor.LocalResourceFile = LocalResourceFile;
            editor.RequiredUrl = RequiredUrl;
            editor.ShowRequired = ShowRequired;
            editor.ShowVisibility = ShowVisibility;
            editor.Width = Width;
            editor.ItemAdded += CollectionItemAdded;
            editor.ItemChanged += ListItemChanged;
            editor.ItemCreated += EditorItemCreated;
            editor.ItemDeleted += CollectionItemDeleted;

            editor.DataBind();
            container.Controls.Add(editor);
        }

        protected void AddEditorRow(Table table, string name, IEditorInfoAdapter adapter)
        {
            var row = new TableRow();
            table.Rows.Add(row);

            var cell = new TableCell();
            row.Cells.Add(cell);

            var editor = new FieldEditorControl
                             {
                                 DataSource = DataSource,
                                 EditorInfoAdapter = adapter,
                                 DataField = name,
                                 EditorDisplayMode = DisplayMode,
                                 EnableClientValidation = EnableClientValidation,
                                 EditMode = EditMode,
                                 HelpDisplayMode = HelpDisplayMode,
                                 LabelMode = LabelMode,
                                 LabelWidth = LabelWidth
                             };
            AddEditorRow(editor, cell);

            Fields.Add(editor);
        }

        protected void AddEditorRow(WebControl container, string name, IEditorInfoAdapter adapter)
        {
            var editor = new FieldEditorControl
            {
                DataSource = DataSource,
                EditorInfoAdapter = adapter,
                DataField = name,
                EditorDisplayMode = DisplayMode,
                EnableClientValidation = EnableClientValidation,
                EditMode = EditMode,
                HelpDisplayMode = HelpDisplayMode,
                LabelMode = LabelMode,
                LabelWidth = LabelWidth
            };
            AddEditorRow(editor, container);

            Fields.Add(editor);
        }

        protected virtual void AddEditorRow(Table table, object obj)
        {
            var objProperty = (PropertyInfo) obj;
            AddEditorRow(table, objProperty.Name, new StandardEditorInfoAdapter(DataSource, objProperty.Name));
        }

        protected virtual void AddEditorRow(object obj)
        {
            var objProperty = (PropertyInfo)obj;
            AddEditorRow(this, objProperty.Name, new StandardEditorInfoAdapter(DataSource, objProperty.Name));
        }

        protected virtual void AddFields()
        {
            foreach (FieldEditorControl editor in Fields)
            {
                editor.DataSource = DataSource;
                editor.EditorInfoAdapter = new StandardEditorInfoAdapter(DataSource, editor.DataField);
                editor.EditorDisplayMode = DisplayMode;
                editor.EnableClientValidation = EnableClientValidation;
                if (editor.EditMode != PropertyEditorMode.View)
                {
                    editor.EditMode = EditMode;
                }
                editor.HelpDisplayMode = HelpDisplayMode;
                if (editor.LabelMode == LabelMode.None)
                {
                    editor.LabelMode = LabelMode;
                }
                if (editor.LabelWidth == Unit.Empty)
                {
                    editor.LabelWidth = LabelWidth;
                }

                AddEditorRow(editor, this);
            }
        }

        protected virtual void AddFields(Table tbl)
        {
            foreach (FieldEditorControl editor in Fields)
            {
                var row = new TableRow();
                tbl.Rows.Add(row);
                var cell = new TableCell();
                row.Cells.Add(cell);

                editor.DataSource = DataSource;
                editor.EditorInfoAdapter = new StandardEditorInfoAdapter(DataSource, editor.DataField);
                editor.EditorDisplayMode = DisplayMode;
                editor.EnableClientValidation = EnableClientValidation;
                if (editor.EditMode != PropertyEditorMode.View)
                {
                    editor.EditMode = EditMode;
                }
                editor.HelpDisplayMode = HelpDisplayMode;
                if (editor.LabelMode == LabelMode.None)
                {
                    editor.LabelMode = LabelMode;
                }
                if (editor.LabelWidth == Unit.Empty)
                {
                    editor.LabelWidth = LabelWidth;
                }

                AddEditorRow(editor, cell);
            }
        }

        protected virtual void AddHeader(Table tbl, string header)
        {
            var panel = new Panel();
            var icon = new Image {ID = "ico" + header, EnableViewState = false};
            var spacer = new Literal {Text = " ", EnableViewState = false};
            var label = new Label {ID = "lbl" + header};
            label.Attributes["resourcekey"] = ID + "_" + header + ".Header";
            label.Text = header;
            label.EnableViewState = false;
            label.ControlStyle.CopyFrom(GroupHeaderStyle);
            panel.Controls.Add(icon);
            panel.Controls.Add(spacer);
            panel.Controls.Add(label);
            if (GroupHeaderIncludeRule)
            {
                panel.Controls.Add(new LiteralControl("<hr noshade=\"noshade\" size=\"1\"/>"));
            }
            Controls.Add(panel);
            if (_sections == null)
            {
                _sections = new Hashtable();
            }
            _sections[icon] = tbl;
        }

        protected virtual void CreateEditor()
        {
            Table table = null;
            string[] arrGroups = null;
            Controls.Clear();
            if (!String.IsNullOrEmpty(Groups))
            {
                arrGroups = Groups.Split(',');
            }
            else if (GroupByMode != GroupByMode.None)
            {
                arrGroups = GetGroups(UnderlyingDataSource);
            }

            if (!AutoGenerate)
            {
                if (DisplayMode == EditorDisplayMode.Div)
                {
                    AddFields();
                }
                else
                {
                    table = new Table { ID = "tbl" };
                    AddFields(table);
                    Controls.Add(table);
                }
            }
            else
            {
                Fields.Clear();
                if (arrGroups != null && arrGroups.Length > 0)
                {
                    foreach (string strGroup in arrGroups)
                    {
                        if (GroupByMode == GroupByMode.Section)
                        {
                            table = new Table { ID = "tbl" + strGroup };
                            foreach (object obj in UnderlyingDataSource)
                            {
                                if (GetCategory(obj) == strGroup.Trim())
                                {
                                    if (GetRowVisibility(obj))
                                    {
                                        if (table.Rows.Count == 0)
                                        {
                                            AddHeader(table, strGroup);
                                        }

                                        AddEditorRow(table, obj);
                                    }
                                }
                            }
                            if (table.Rows.Count > 0)
                            {
                                Controls.Add(table);
                            }
                        }
                    }
                }
                else
                {
                    if (DisplayMode == EditorDisplayMode.Div)
                    {
                        foreach (object obj in UnderlyingDataSource)
                        {
                            if (GetRowVisibility(obj))
                            {
                                AddEditorRow(obj);
                            }
                        }
                    }
                    else
                    {
                        table = new Table { ID = "tbl" };
                        foreach (object obj in UnderlyingDataSource)
                        {
                            if (GetRowVisibility(obj))
                            {
                                AddEditorRow(table, obj);
                            }
                        }
                        Controls.Add(table);
                    }
                }
            }
        }

        protected virtual string GetCategory(object obj)
        {
            var objProperty = (PropertyInfo) obj;
            var categoryString = Null.NullString;
            var categoryAttributes = objProperty.GetCustomAttributes(typeof (CategoryAttribute), true);
            if (categoryAttributes.Length > 0)
            {
                var category = (CategoryAttribute) categoryAttributes[0];
                categoryString = category.Category;
            }
            return categoryString;
        }

        protected virtual string[] GetGroups(IEnumerable arrObjects)
        {
            var arrGroups = new ArrayList();

            foreach (PropertyInfo objProperty in arrObjects)
            {
                var categoryAttributes = objProperty.GetCustomAttributes(typeof (CategoryAttribute), true);
                if (categoryAttributes.Length > 0)
                {
                    var category = (CategoryAttribute) categoryAttributes[0];
                    if (!arrGroups.Contains(category.Category))
                    {
                        arrGroups.Add(category.Category);
                    }
                }
            }
            var strGroups = new string[arrGroups.Count];
            for (int i = 0; i <= arrGroups.Count - 1; i++)
            {
                strGroups[i] = Convert.ToString(arrGroups[i]);
            }
            return strGroups;
        }

        protected virtual bool GetRowVisibility(object obj)
        {
            var objProperty = (PropertyInfo) obj;
            bool isVisible = true;
            object[] browsableAttributes = objProperty.GetCustomAttributes(typeof (BrowsableAttribute), true);
            if (browsableAttributes.Length > 0)
            {
                var browsable = (BrowsableAttribute) browsableAttributes[0];
                if (!browsable.Browsable)
                {
                    isVisible = false;
                }
            }
            if (!isVisible && EditMode == PropertyEditorMode.Edit)
            {
                object[] requiredAttributes = objProperty.GetCustomAttributes(typeof (RequiredAttribute), true);
                if (requiredAttributes.Length > 0)
                {
                    var required = (RequiredAttribute) requiredAttributes[0];
                    if (required.Required)
                    {
                        isVisible = true;
                    }
                }
            }
            return isVisible;
        }

        protected virtual void OnItemAdded(PropertyEditorEventArgs e)
        {
            if (ItemAdded != null)
            {
                ItemAdded(this, e);
            }
        }

        protected virtual void OnItemCreated(PropertyEditorItemEventArgs e)
        {
            if (ItemCreated != null)
            {
                ItemCreated(this, e);
            }
        }

        protected virtual void OnItemDeleted(PropertyEditorEventArgs e)
        {
            if (ItemDeleted != null)
            {
                ItemDeleted(this, e);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_itemChanged)
            {
                DataBind();
            }
            if (String.IsNullOrEmpty(CssClass))
            {
                CssClass = "dnnForm";
            }
            if (GroupByMode == GroupByMode.Section && (_sections != null))
            {
                foreach (DictionaryEntry key in _sections)
                {
                    var tbl = (Table) key.Value;
                    var icon = (Image) key.Key;
                    DNNClientAPI.EnableMinMax(icon, tbl, false, Page.ResolveUrl("~/images/minus.gif"), Page.ResolveUrl("~/images/plus.gif"), DNNClientAPI.MinMaxPersistanceType.Page);
                }
            }
            base.OnPreRender(e);
        }

        public override void DataBind()
        {
            base.OnDataBinding(EventArgs.Empty);
            Controls.Clear();
            ClearChildViewState();
            TrackViewState();
            CreateEditor();
            ChildControlsCreated = true;
        }

        protected virtual void CollectionItemAdded(object sender, PropertyEditorEventArgs e)
        {
            OnItemAdded(e);
        }

        protected virtual void CollectionItemDeleted(object sender, PropertyEditorEventArgs e)
        {
            OnItemDeleted(e);
        }

        protected virtual void EditorItemCreated(object sender, PropertyEditorItemEventArgs e)
        {
            OnItemCreated(e);
        }

        protected virtual void ListItemChanged(object sender, PropertyEditorEventArgs e)
        {
            _itemChanged = true;
        }
    }
}
