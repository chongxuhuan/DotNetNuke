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
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using DotNetNuke.Services.Localization;
using DotNetNuke.UI;

using Telerik.Web.UI;

#endregion

namespace DotNetNuke.Web.UI.WebControls
{
    public abstract class DnnFormItemBase : WebControl, INamingContainer
    {
        private object _value;
        
        protected DnnFormItemBase()
        {
            FormMode = DnnFormMode.Inherit;
            IsValid = true;

            Validators = new List<IValidator>();
        }

        #region Protected Properties

        protected PropertyDescriptor Property
        {
            get
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(DataSource);
                return props[DataField];
            }
        }

        protected override HtmlTextWriterTag TagKey
        {
            get
            {
                return HtmlTextWriterTag.Div;
            }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion

        #region Public Properties

        public string DataField { get; set; }

        internal object DataSource { get; set; }

        public DnnFormMode FormMode { get; set; }

        public bool IsValid { get; private set; }

        public string OnClientClicked { get; set; }

        public string LocalResourceFile { get; set; }

        public bool Required { get; set; }

        public string ResourceKey { get; set; }

        [Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public List<IValidator> Validators { get; private set; }

        public string ValidationExpression { get; set; }

        #endregion

        #region Control Hierarchy and Data Binding

        private void AddValidators(string controlId)
        {
            var value = Value as String;
            Validators.Clear();

            //Add Validators
            if (Required)
            {
                var requiredValidator = new RequiredFieldValidator
                                            {
                                                ID = ID + "_Required", 
                                                ErrorMessage = ResourceKey + ".Required"
                                            };
                if (String.IsNullOrEmpty(value))
                {
                    requiredValidator.IsValid = false;
                    IsValid = requiredValidator.IsValid;
                }
                Validators.Add(requiredValidator);
            }

            if (!String.IsNullOrEmpty(ValidationExpression))
            {
                var regexValidator = new RegularExpressionValidator
                                         {
                                             ID = ID + "_RegEx", 
                                             ErrorMessage = ResourceKey + ".RegExError", 
                                             ValidationExpression = ValidationExpression
                                         };
                if (!String.IsNullOrEmpty(value))
                {
                    regexValidator.IsValid = Regex.IsMatch(value, ValidationExpression);
                    IsValid = regexValidator.IsValid;
                }
                Validators.Add(regexValidator);
            }

            if (Validators.Count > 0)
            {
                foreach (BaseValidator validator in Validators)
                {
                    validator.ControlToValidate = controlId;
                    validator.Display = ValidatorDisplay.Dynamic;
                    validator.ErrorMessage = LocalizeString(validator.ErrorMessage);
                    validator.CssClass = "dnnFormMessage dnnFormError";                   
                    Controls.Add(validator);
                }
            }
        }

        public void CheckIsValid()
        {
            IsValid = true;
            foreach (BaseValidator validator in Validators)
            {
                validator.Validate();
                if (!validator.IsValid)
                {
                    IsValid = false;
                    break;
                }
            }
        }

        protected virtual void CreateControlHierarchy()
        {
            //Load Item Style
            CssClass = "dnnFormItem";
            CssClass += (FormMode == DnnFormMode.Long) ? "" : " dnnFormShort";

            if (String.IsNullOrEmpty(ResourceKey))
            {
                ResourceKey = DataField;
            }

            //Add Label
            var label = new DnnFormLabel 
                                {
                                    LocalResourceFile = LocalResourceFile, 
                                    ResourceKey = ResourceKey + ".Text", 
                                    ToolTipKey = ResourceKey + ".Help"
                                };

            Controls.Add(label);

            WebControl inputControl = CreateControlInternal(this);
            label.AssociatedControlID = inputControl.ID;
            if(Required)
            {
                inputControl.CssClass += " dnnFormRequired";
            }

            Controls.Add(inputControl);

            AddValidators(inputControl.ID);
        }

        protected virtual WebControl CreateControlInternal(Control control)
        {
            return null;
        }

        protected override void CreateChildControls()
        {
            // CreateChildControls re-creates the children (the items)
            // using the saved view state.
            // First clear any existing child controls.
            Controls.Clear();

            CreateControlHierarchy();
        }

        protected void DataBindInternal(string dataField, ref object value)
        {
            var dictionary = DataSource as IDictionary<string, string>;
            if (dictionary != null)
            {
                if (!String.IsNullOrEmpty(dataField) && dictionary.ContainsKey(dataField))
                {
                    value = dictionary[dataField];
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(dataField))
                {
                    if (Property != null && Property.GetValue(DataSource) != null)
                    {
                        // ReSharper disable PossibleNullReferenceException
                        value = Property.GetValue(DataSource);
                        // ReSharper restore PossibleNullReferenceException
                    }
                }
            }
        }

        protected virtual void DataBindInternal()
        {
            DataBindInternal(DataField, ref _value);
        }

        public void DataBindItem(bool useDataSource)
        {
            if (useDataSource)
            {
                base.OnDataBinding(EventArgs.Empty);
                Controls.Clear();
                ClearChildViewState();
                TrackViewState();

                DataBindInternal();

                CreateControlHierarchy();
                ChildControlsCreated = true;
            }
            else
            {
                if (!String.IsNullOrEmpty(DataField))
                {
                    UpdateDataSourceInternal(null, _value, DataField);
                }
            }
        }

        private void UpdateDataSourceInternal(object oldValue, object newValue, string dataField)
        {
            if (DataSource != null)
            {
                var dictionary = DataSource as IDictionary<string, string>;
                if (dictionary != null)
                {
                    if (dictionary.ContainsKey(dataField) && !ReferenceEquals(newValue, oldValue))
                    {
                        dictionary[dataField] = newValue as string;
                    }
                }
                else
                {
                    if (Property != null)
                    {
                        if (!ReferenceEquals(newValue, oldValue))
                        {
                            Property.SetValue(DataSource, Convert.ChangeType(newValue, Property.PropertyType));
                        }
                    }
                }
            }           
        }

        protected void UpdateDataSource(object oldValue, object newValue, string dataField)
        {
            CheckIsValid();

            _value = newValue;

            UpdateDataSourceInternal(oldValue, newValue, dataField);
        }

        #endregion

        #region Protected Methods

        protected override void LoadControlState(object state)
        {
            _value = state;
        }

        protected string LocalizeString(string key)
        {
            return Localization.GetString(key, LocalResourceFile);
        }

        protected override void OnInit(EventArgs e)
        {
            Page.RegisterRequiresControlState(this);
            base.OnInit(e);
        }

        protected override object SaveControlState()
        {
            return _value;
        }

        #endregion
    }
}