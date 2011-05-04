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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.Localization;

#endregion

namespace DotNetNuke.UI.WebControls
{
    public enum EditorDisplayMode
    {
        Div,
        Table
    }

    public enum HelpDisplayMode
    {
        Never,
        EditOnly,
        Always
    }

    [ToolboxData("<{0}:FieldEditorControl runat=server></{0}:FieldEditorControl>")]
    public class FieldEditorControl : WebControl, INamingContainer
    {
        private readonly List<IValidator> Validators = new List<IValidator>();
        private IEditorInfoAdapter _EditorInfoAdapter;
        private bool _IsValid = true;
        private StandardEditorInfoAdapter _StdAdapter;
        private bool _Validated;

        public FieldEditorControl()
        {
            ValidationExpression = Null.NullString;
            ShowRequired = true;
            LabelMode = LabelMode.None;
            EditorTypeName = Null.NullString;
            EditorDisplayMode = EditorDisplayMode.Div;
            HelpDisplayMode = HelpDisplayMode.Always;
            VisibilityStyle = new Style();
            LabelStyle = new Style();
            HelpStyle = new Style();
            EditControlStyle = new Style();
            ErrorStyle = new Style();
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        [Browsable(false)]
        public object DataSource { get; set; }

        [Browsable(true), Category("Data"), DefaultValue(""), Description("Enter the name of the field that is data bound to the Control.")]
        public string DataField { get; set; }

        public EditorDisplayMode EditorDisplayMode { get; set; }

        public PropertyEditorMode EditMode { get; set; }

        public EditControl Editor { get; private set; }

        public IEditorInfoAdapter EditorInfoAdapter
        {
            get
            {
                if (_EditorInfoAdapter == null)
                {
                    if (_StdAdapter == null)
                    {
                        _StdAdapter = new StandardEditorInfoAdapter(DataSource, DataField);
                    }
                    return _StdAdapter;
                }
                else
                {
                    return _EditorInfoAdapter;
                }
            }
            set
            {
                _EditorInfoAdapter = value;
            }
        }

        public string EditorTypeName { get; set; }

        public bool EnableClientValidation { get; set; }

        public HelpDisplayMode HelpDisplayMode { get; set; }

        public bool IsDirty { get; private set; }

        public bool IsValid
        {
            get
            {
                if (!_Validated)
                {
                    Validate();
                }
                return _IsValid;
            }
        }

        public LabelMode LabelMode { get; set; }

        public string LocalResourceFile { get; set; }

        public bool Required { get; set; }

        public string RequiredUrl { get; set; }

        public bool ShowRequired { get; set; }

        public bool ShowVisibility { get; set; }

        public string ValidationExpression { get; set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), TypeConverter(typeof (ExpandableObjectConverter)), Description("Set the Style for the Edit Control.")]
        public Style EditControlStyle { get; private set; }

        [Browsable(true), Category("Appearance"), Description("Set the Width for the Edit Control.")]
        public Unit EditControlWidth { get; set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), TypeConverter(typeof (ExpandableObjectConverter)), Description("Set the Style for the Error Text.")]
        public Style ErrorStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), TypeConverter(typeof (ExpandableObjectConverter)), Description("Set the Style for the Help Text.")]
        public Style HelpStyle { get; private set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), TypeConverter(typeof (ExpandableObjectConverter)), Description("Set the Style for the Label Text")]
        public Style LabelStyle { get; private set; }

        [Browsable(true), Category("Appearance"), Description("Set the Width for the Label Control.")]
        public Unit LabelWidth { get; set; }

        [Browsable(true), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), TypeConverter(typeof (ExpandableObjectConverter)), Description("Set the Style for the Visibility Control")]
        public Style VisibilityStyle { get; private set; }

        public event PropertyChangedEventHandler ItemAdded;
        public event PropertyChangedEventHandler ItemChanged;
        public event EditorCreatedEventHandler ItemCreated;
        public event PropertyChangedEventHandler ItemDeleted;

        private void BuildDiv(EditorInfo editInfo)
        {
            HtmlGenericControl divLabel = null;
            if (editInfo.LabelMode != LabelMode.None)
            {
                divLabel = new HtmlGenericControl("div");
                divLabel.Controls.Add(BuildLabel(editInfo));
            }
            var divEdit = new HtmlGenericControl("div");
            divEdit.Attributes.Add("class", "dnnFormInput");
            EditControl propEditor = BuildEditor(editInfo);
            divEdit.Controls.Add(propEditor);

            VisibilityControl visibility = BuildVisibility(editInfo);
            if (visibility != null)
            {
                visibility.CssClass = "dnnFormVisibility";
            }

            var strValue = editInfo.Value as string; 
            if (ShowRequired && editInfo.Required && (editInfo.EditMode == PropertyEditorMode.Edit || (editInfo.Required && string.IsNullOrEmpty(strValue))))
            {
                propEditor.CssClass = "dnnFormRequired";
            }

            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Top)
            {
                Controls.Add(divLabel);
                Controls.Add(divEdit);
                if (visibility != null)
                {
                    Controls.Add(visibility);
                }
            }
            else
            {
                Controls.Add(divEdit);
                if (visibility != null)
                {
                    Controls.Add(visibility);
                }
                if ((divLabel != null))
                {
                    Controls.Add(divLabel);
                }
            }
            BuildValidators(editInfo, propEditor.ID);
            if (Validators.Count > 0)
            {
                foreach (BaseValidator validator in Validators)
                {
                    validator.Width = Width;
                    Controls.Add(validator);
                }
            }
        }

        private EditControl BuildEditor(EditorInfo editInfo)
        {
            EditControl propEditor = EditControlFactory.CreateEditControl(editInfo);
            propEditor.ControlStyle.CopyFrom(EditControlStyle);
            propEditor.LocalResourceFile = LocalResourceFile;
            if (editInfo.ControlStyle != null)
            {
                propEditor.ControlStyle.CopyFrom(editInfo.ControlStyle);
            }
            propEditor.ItemAdded += CollectionItemAdded;
            propEditor.ItemDeleted += CollectionItemDeleted;
            propEditor.ValueChanged += ValueChanged;
            if (propEditor is DNNListEditControl)
            {
                var listEditor = (DNNListEditControl) propEditor;
                listEditor.ItemChanged += ListItemChanged;
            }
            Editor = propEditor;
            return propEditor;
        }

        private PropertyLabelControl BuildLabel(EditorInfo editInfo)
        {
            var propLabel = new PropertyLabelControl();
            propLabel.ID = editInfo.Name + "_Label";
            propLabel.HelpStyle.CopyFrom(HelpStyle);
            propLabel.LabelStyle.CopyFrom(LabelStyle);
            var strValue = editInfo.Value as string;
            switch (HelpDisplayMode)
            {
                case HelpDisplayMode.Always:
                    propLabel.ShowHelp = true;
                    break;
                case HelpDisplayMode.EditOnly:
                    if (editInfo.EditMode == PropertyEditorMode.Edit || (editInfo.Required && string.IsNullOrEmpty(strValue)))
                    {
                        propLabel.ShowHelp = true;
                    }
                    else
                    {
                        propLabel.ShowHelp = false;
                    }
                    break;
                case HelpDisplayMode.Never:
                    propLabel.ShowHelp = false;
                    break;
            }
            propLabel.Caption = editInfo.Name;
            propLabel.HelpText = editInfo.Name;
            propLabel.ResourceKey = editInfo.ResourceKey;
            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Right)
            {
                propLabel.Width = LabelWidth;
            }
            return propLabel;
        }

        private Image BuildRequiredIcon(EditorInfo editInfo)
        {
            Image img = null;
            var strValue = editInfo.Value as string;
            if (ShowRequired && editInfo.Required && (editInfo.EditMode == PropertyEditorMode.Edit || (editInfo.Required && string.IsNullOrEmpty(strValue))))
            {
                img = new Image();
                if (String.IsNullOrEmpty(RequiredUrl) || RequiredUrl == Null.NullString)
                {
                    img.ImageUrl = "~/images/required.gif";
                }
                else
                {
                    img.ImageUrl = RequiredUrl;
                }
                img.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Required");
            }
            return img;
        }

        private void BuildTable(EditorInfo editInfo)
        {
            var tbl = new Table();
            var labelCell = new TableCell();
            var editorCell = new TableCell();
            labelCell.VerticalAlign = VerticalAlign.Top;
            labelCell.Controls.Add(BuildLabel(editInfo));
            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Right)
            {
                labelCell.Width = LabelWidth;
            }
            editorCell.VerticalAlign = VerticalAlign.Top;
            EditControl propEditor = BuildEditor(editInfo);
            Image requiredIcon = BuildRequiredIcon(editInfo);
            editorCell.Controls.Add(propEditor);
            if (requiredIcon != null)
            {
                editorCell.Controls.Add(requiredIcon);
            }
            if (editInfo.LabelMode == LabelMode.Left || editInfo.LabelMode == LabelMode.Right)
            {
                editorCell.Width = EditControlWidth;
            }
            VisibilityControl visibility = BuildVisibility(editInfo);
            if (visibility != null)
            {
                editorCell.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
                editorCell.Controls.Add(visibility);
            }
            var editorRow = new TableRow();
            var labelRow = new TableRow();
            if (editInfo.LabelMode == LabelMode.Bottom || editInfo.LabelMode == LabelMode.Top || editInfo.LabelMode == LabelMode.None)
            {
                editorCell.ColumnSpan = 2;
                editorRow.Cells.Add(editorCell);
                if (editInfo.LabelMode == LabelMode.Bottom || editInfo.LabelMode == LabelMode.Top)
                {
                    labelCell.ColumnSpan = 2;
                    labelRow.Cells.Add(labelCell);
                }
                if (editInfo.LabelMode == LabelMode.Top)
                {
                    tbl.Rows.Add(labelRow);
                }
                tbl.Rows.Add(editorRow);
                if (editInfo.LabelMode == LabelMode.Bottom)
                {
                    tbl.Rows.Add(labelRow);
                }
            }
            else if (editInfo.LabelMode == LabelMode.Left)
            {
                editorRow.Cells.Add(labelCell);
                editorRow.Cells.Add(editorCell);
                tbl.Rows.Add(editorRow);
            }
            else if (editInfo.LabelMode == LabelMode.Right)
            {
                editorRow.Cells.Add(editorCell);
                editorRow.Cells.Add(labelCell);
                tbl.Rows.Add(editorRow);
            }
            BuildValidators(editInfo, propEditor.ID);
            var validatorsRow = new TableRow();
            var validatorsCell = new TableCell();
            validatorsCell.ColumnSpan = 2;
            foreach (BaseValidator validator in Validators)
            {
                validatorsCell.Controls.Add(validator);
            }
            validatorsRow.Cells.Add(validatorsCell);
            tbl.Rows.Add(validatorsRow);
            Controls.Add(tbl);
        }

        private void BuildValidators(EditorInfo editInfo, string targetId)
        {
            Validators.Clear();
            if (editInfo.Required)
            {
                var reqValidator = new RequiredFieldValidator();
                reqValidator.ID = editInfo.Name + "_Req";
                reqValidator.ControlToValidate = targetId;
                reqValidator.Display = ValidatorDisplay.Dynamic;
                reqValidator.ControlStyle.CopyFrom(ErrorStyle);
                reqValidator.EnableClientScript = EnableClientValidation;
                reqValidator.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Required");
                reqValidator.ErrorMessage = editInfo.Name + " is Required";
                Validators.Add(reqValidator);
            }
            if (!String.IsNullOrEmpty(editInfo.ValidationExpression))
            {
                var regExValidator = new RegularExpressionValidator();
                regExValidator.ID = editInfo.Name + "_RegEx";
                regExValidator.ControlToValidate = targetId;
                regExValidator.ValidationExpression = editInfo.ValidationExpression;
                regExValidator.Display = ValidatorDisplay.Dynamic;
                regExValidator.ControlStyle.CopyFrom(ErrorStyle);
                regExValidator.EnableClientScript = EnableClientValidation;
                regExValidator.Attributes.Add("resourcekey", editInfo.ResourceKey + ".Validation");
                regExValidator.ErrorMessage = editInfo.Name + " is Invalid";
                Validators.Add(regExValidator);
            }
        }

        private VisibilityControl BuildVisibility(EditorInfo editInfo)
        {
            VisibilityControl visControl = null;
            if (ShowVisibility)
            {
                visControl = new VisibilityControl();
                visControl.ID = ID + "_vis";
                visControl.Caption = Localization.GetString("Visibility");
                visControl.Name = editInfo.Name;
                visControl.Value = editInfo.Visibility;
                visControl.ControlStyle.CopyFrom(VisibilityStyle);
                visControl.VisibilityChanged += VisibilityChanged;
            }
            return visControl;
        }

        private string GetOppositeSide(LabelMode labelMode)
        {
            switch (labelMode)
            {
                case LabelMode.Bottom:
                    return "top";
                case LabelMode.Left:
                    return "right";
                case LabelMode.Right:
                    return "left";
                case LabelMode.Top:
                    return "bottom";
                default:
                    return string.Empty;
            }
        }

        protected virtual void CreateEditor()
        {
            EditorInfo editInfo = EditorInfoAdapter.CreateEditControl();

            if (editInfo != null)
            {
                if (editInfo.EditMode == PropertyEditorMode.Edit)
                {
                    editInfo.EditMode = EditMode;
                }
                if (!string.IsNullOrEmpty(EditorTypeName))
                {
                    editInfo.Editor = EditorTypeName;
                }
                if (LabelMode != LabelMode.Left)
                {
                    editInfo.LabelMode = LabelMode;
                }
                if (Required)
                {
                    editInfo.Required = Required;
                }
                if (!string.IsNullOrEmpty(ValidationExpression))
                {
                    editInfo.ValidationExpression = ValidationExpression;
                }
                OnItemCreated(new PropertyEditorItemEventArgs(editInfo));
                Visible = editInfo.Visible;
                if (EditorDisplayMode == EditorDisplayMode.Div)
                {
                    BuildDiv(editInfo);
                }
                else
                {
                    BuildTable(editInfo);
                }
            }
        }

        protected virtual void CollectionItemAdded(object sender, PropertyEditorEventArgs e)
        {
            OnItemAdded(e);
        }

        protected virtual void CollectionItemDeleted(object sender, PropertyEditorEventArgs e)
        {
            OnItemDeleted(e);
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
            base.OnPreRender(e);

            if (String.IsNullOrEmpty(CssClass))
            {
                CssClass = "dnnFormItem";
            }
        }

        protected virtual void ValueChanged(object sender, PropertyEditorEventArgs e)
        {
            IsDirty = EditorInfoAdapter.UpdateValue(e);
        }

        protected virtual void VisibilityChanged(object sender, PropertyEditorEventArgs e)
        {
            IsDirty = EditorInfoAdapter.UpdateVisibility(e);
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

        public virtual void Validate()
        {
            _IsValid = Editor.IsValid;
            if (_IsValid)
            {
                IEnumerator valEnumerator = Validators.GetEnumerator();
                while (valEnumerator.MoveNext())
                {
                    var validator = (IValidator) valEnumerator.Current;
                    validator.Validate();
                    if (!validator.IsValid)
                    {
                        _IsValid = false;
                        break;
                    }
                }
                _Validated = true;
            }
        }

        protected virtual void ListItemChanged(object sender, PropertyEditorEventArgs e)
        {
            if (ItemChanged != null)
            {
                ItemChanged(this, e);
            }
        }
    }
}
