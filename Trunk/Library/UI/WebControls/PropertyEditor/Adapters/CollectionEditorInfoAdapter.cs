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
using System.Reflection;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Users;

#endregion

namespace DotNetNuke.UI.WebControls
{
    /// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.UI.WebControls
    /// Class:      CollectionEditorInfoFactory
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The CollectionEditorInfoAdapter control provides an Adapter for Collection Onjects
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	05/08/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
    public class CollectionEditorInfoAdapter : IEditorInfoAdapter
    {
        private readonly object DataSource;
        private readonly Hashtable FieldNames;
        private readonly string Name;
        private string FieldName;

        public CollectionEditorInfoAdapter(object dataSource, string name, string fieldName, Hashtable fieldNames)
        {
            DataSource = dataSource;
            FieldName = fieldName;
            FieldNames = fieldNames;
            Name = name;
        }

        #region IEditorInfoAdapter Members

        public EditorInfo CreateEditControl()
        {
            return GetEditorInfo();
        }

        public bool UpdateValue(PropertyEditorEventArgs e)
        {
            string NameDataField = Convert.ToString(FieldNames["Name"]);
            string ValueDataField = Convert.ToString(FieldNames["Value"]);
            PropertyInfo objProperty;
            string PropertyName = "";
            bool changed = e.Changed;
            string name = e.Name;
            object oldValue = e.OldValue;
            object newValue = e.Value;
            object stringValue = e.StringValue;
            bool _IsDirty = Null.NullBoolean;
            objProperty = DataSource.GetType().GetProperty(NameDataField);
            if (objProperty != null)
            {
                PropertyName = Convert.ToString(objProperty.GetValue(DataSource, null));
                PropertyName = PropertyName.Replace(" ", "_");
                if (PropertyName == name)
                {
                    objProperty = DataSource.GetType().GetProperty(ValueDataField);
                    if ((!(ReferenceEquals(newValue, oldValue))) || changed)
                    {
                        if (objProperty.PropertyType.FullName == "System.String")
                        {
                            objProperty.SetValue(DataSource, stringValue, null);
                        }
                        else
                        {
                            objProperty.SetValue(DataSource, newValue, null);
                        }
                        _IsDirty = true;
                    }
                }
            }
            return _IsDirty;
        }

        public bool UpdateVisibility(PropertyEditorEventArgs e)
        {
            string NameDataField = Convert.ToString(FieldNames["Name"]);
            string VisibilityDataField = Convert.ToString(FieldNames["Visibility"]);
            PropertyInfo objProperty;
            string PropertyName = "";
            string name = e.Name;
            object newValue = e.Value;
            bool _IsDirty = Null.NullBoolean;
            objProperty = DataSource.GetType().GetProperty(NameDataField);
            if (objProperty != null)
            {
                PropertyName = Convert.ToString(objProperty.GetValue(DataSource, null));
                PropertyName = PropertyName.Replace(" ", "_");
                if (PropertyName == name)
                {
                    objProperty = DataSource.GetType().GetProperty(VisibilityDataField);
                    objProperty.SetValue(DataSource, newValue, null);
                    _IsDirty = true;
                }
            }
            return _IsDirty;
        }

        #endregion

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetEditorInfo builds an EditorInfo object for a propoerty
        /// </summary>
        /// <history>
        /// 	[cnurse]	05/05/2006	Created
        /// </history>
        /// -----------------------------------------------------------------------------
        private EditorInfo GetEditorInfo()
        {
            string CategoryDataField = Convert.ToString(FieldNames["Category"]);
            string EditorDataField = Convert.ToString(FieldNames["Editor"]);
            string NameDataField = Convert.ToString(FieldNames["Name"]);
            string RequiredDataField = Convert.ToString(FieldNames["Required"]);
            string TypeDataField = Convert.ToString(FieldNames["Type"]);
            string ValidationExpressionDataField = Convert.ToString(FieldNames["ValidationExpression"]);
            string ValueDataField = Convert.ToString(FieldNames["Value"]);
            string VisibilityDataField = Convert.ToString(FieldNames["Visibility"]);
            string MaxLengthDataField = Convert.ToString(FieldNames["Length"]);
            var editInfo = new EditorInfo();
            PropertyInfo objProperty;
            editInfo.Name = string.Empty;
            if (!String.IsNullOrEmpty(NameDataField))
            {
                objProperty = DataSource.GetType().GetProperty(NameDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Name = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }
            editInfo.Category = string.Empty;
            if (!String.IsNullOrEmpty(CategoryDataField))
            {
                objProperty = DataSource.GetType().GetProperty(CategoryDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Category = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }
            editInfo.Value = string.Empty;
            if (!String.IsNullOrEmpty(ValueDataField))
            {
                objProperty = DataSource.GetType().GetProperty(ValueDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Value = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }
            editInfo.Type = "System.String";
            if (!String.IsNullOrEmpty(TypeDataField))
            {
                objProperty = DataSource.GetType().GetProperty(TypeDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Type = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }
            editInfo.Editor = "DotNetNuke.UI.WebControls.TextEditControl, DotNetNuke";
            if (!String.IsNullOrEmpty(EditorDataField))
            {
                objProperty = DataSource.GetType().GetProperty(EditorDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Editor = EditorInfo.GetEditor(Convert.ToInt32(objProperty.GetValue(DataSource, null)));
                }
            }
            editInfo.LabelMode = LabelMode.Left;
            editInfo.Required = false;
            if (!String.IsNullOrEmpty(RequiredDataField))
            {
                objProperty = DataSource.GetType().GetProperty(RequiredDataField);
                if (!((objProperty == null) || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Required = Convert.ToBoolean(objProperty.GetValue(DataSource, null));
                }
            }
            editInfo.ResourceKey = editInfo.Name;
            editInfo.ResourceKey = string.Format("{0}_{1}", Name, editInfo.Name);
            editInfo.ControlStyle = new Style();
            editInfo.Visibility = UserVisibilityMode.AllUsers;
            if (!String.IsNullOrEmpty(VisibilityDataField))
            {
                objProperty = DataSource.GetType().GetProperty(VisibilityDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.Visibility = (UserVisibilityMode) objProperty.GetValue(DataSource, null);
                }
            }
            editInfo.ValidationExpression = string.Empty;
            if (!String.IsNullOrEmpty(ValidationExpressionDataField))
            {
                objProperty = DataSource.GetType().GetProperty(ValidationExpressionDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    editInfo.ValidationExpression = Convert.ToString(objProperty.GetValue(DataSource, null));
                }
            }
            if (!String.IsNullOrEmpty(MaxLengthDataField))
            {
                objProperty = DataSource.GetType().GetProperty(MaxLengthDataField);
                if (!(objProperty == null || (objProperty.GetValue(DataSource, null) == null)))
                {
                    int length = Convert.ToInt32(objProperty.GetValue(DataSource, null));
                    var attributes = new object[1];
                    attributes[0] = new MaxLengthAttribute(length);
                    editInfo.Attributes = attributes;
                }
            }
            editInfo.Name = editInfo.Name.Replace(" ", "_");
            return editInfo;
        }
    }
}
