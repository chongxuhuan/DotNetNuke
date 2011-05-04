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
using System.ComponentModel;
using System.Xml.Serialization;

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.Entities.Profile
{
    [XmlRoot("profiledefinition", IsNullable = false)]
    [Serializable]
    public class ProfilePropertyDefinition : BaseEntityInfo
    {
        private int _DataType = Null.NullInteger;
        private string _DefaultValue;
        private UserVisibilityMode _DefaultVisibility = UserVisibilityMode.AdminOnly;
        private bool _IsDirty;
        private int _Length;
        private int _ModuleDefId = Null.NullInteger;
        private int _PortalId;
        private string _PropertyCategory;
        private int _PropertyDefinitionId = Null.NullInteger;
        private string _PropertyName;
        private string _PropertyValue;
        private bool _Required;
        private string _ValidationExpression;
        private int _ViewOrder;
        private UserVisibilityMode _Visibility = UserVisibilityMode.AdminOnly;
        private bool _Visible;

        public ProfilePropertyDefinition()
        {
            PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
            PortalId = _Settings.PortalId;
        }

        public ProfilePropertyDefinition(int portalId)
        {
            PortalId = portalId;
        }

        [Editor("DotNetNuke.UI.WebControls.DNNListEditControl, DotNetNuke", typeof (EditControl)), List("DataType", "", ListBoundField.Id, ListBoundField.Value), IsReadOnly(true), Required(true),
         SortOrder(1)]
        [XmlIgnore]
        public int DataType
        {
            get
            {
                return _DataType;
            }
            set
            {
                if (_DataType != value)
                {
                    _IsDirty = true;
                }
                _DataType = value;
            }
        }

        [SortOrder(4)]
        [XmlIgnore]
        public string DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                if (_DefaultValue != value)
                {
                    _IsDirty = true;
                }
                _DefaultValue = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }
        }

        [SortOrder(3)]
        [XmlElement("length")]
        public int Length
        {
            get
            {
                return _Length;
            }
            set
            {
                if (_Length != value)
                {
                    _IsDirty = true;
                }
                _Length = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public int ModuleDefId
        {
            get
            {
                return _ModuleDefId;
            }
            set
            {
                _ModuleDefId = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public int PortalId
        {
            get
            {
                return _PortalId;
            }
            set
            {
                _PortalId = value;
            }
        }

        [Required(true), SortOrder(2)]
        [XmlElement("propertycategory")]
        public string PropertyCategory
        {
            get
            {
                return _PropertyCategory;
            }
            set
            {
                if (_PropertyCategory != value)
                {
                    _IsDirty = true;
                }
                _PropertyCategory = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public int PropertyDefinitionId
        {
            get
            {
                return _PropertyDefinitionId;
            }
            set
            {
                _PropertyDefinitionId = value;
            }
        }

        [Required(true), IsReadOnly(true), SortOrder(0), RegularExpressionValidator("^[a-zA-Z0-9._%\\-+']+$")]
        [XmlElement("propertyname")]
        public string PropertyName
        {
            get
            {
                return _PropertyName;
            }
            set
            {
                if (_PropertyName != value)
                {
                    _IsDirty = true;
                }
                _PropertyName = value;
            }
        }

        [Browsable(false)]
        [XmlIgnore]
        public string PropertyValue
        {
            get
            {
                return _PropertyValue;
            }
            set
            {
                if (_PropertyValue != value)
                {
                    _IsDirty = true;
                }
                _PropertyValue = value;
            }
        }

        [SortOrder(6)]
        [XmlIgnore]
        public bool Required
        {
            get
            {
                return _Required;
            }
            set
            {
                if (_Required != value)
                {
                    _IsDirty = true;
                }
                _Required = value;
            }
        }

        [SortOrder(5)]
        [XmlIgnore]
        public string ValidationExpression
        {
            get
            {
                return _ValidationExpression;
            }
            set
            {
                if (_ValidationExpression != value)
                {
                    _IsDirty = true;
                }
                _ValidationExpression = value;
            }
        }

        [IsReadOnly(true), SortOrder(8)]
        [XmlIgnore]
        public int ViewOrder
        {
            get
            {
                return _ViewOrder;
            }
            set
            {
                if (_ViewOrder != value)
                {
                    _IsDirty = true;
                }
                _ViewOrder = value;
            }
        }

        [SortOrder(7)]
        [XmlIgnore]
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (_Visible != value)
                {
                    _IsDirty = true;
                }
                _Visible = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        ///   Gets and sets the Default Visibility of the Profile Property
        /// </summary>
        /// <history>
        ///   [sbwalker]	06/28/2010	created
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(9)]
        [XmlIgnore]
        public UserVisibilityMode DefaultVisibility
        {
            get
            {
                return _DefaultVisibility;
            }
            set
            {
                if (_DefaultVisibility != value)
                {
                    _IsDirty = true;
                }
                _DefaultVisibility = value;
            }
        }


        [Browsable(false)]
        [XmlIgnore]
        public UserVisibilityMode Visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                if (_Visibility != value)
                {
                    _IsDirty = true;
                }
                _Visibility = value;
            }
        }

        public void ClearIsDirty()
        {
            _IsDirty = false;
        }

        public ProfilePropertyDefinition Clone()
        {
            var objClone = new ProfilePropertyDefinition(PortalId);
            objClone.DataType = DataType;
            objClone.DefaultValue = DefaultValue;
            objClone.Length = Length;
            objClone.ModuleDefId = ModuleDefId;
            objClone.PropertyCategory = PropertyCategory;
            objClone.PropertyDefinitionId = PropertyDefinitionId;
            objClone.PropertyName = PropertyName;
            objClone.PropertyValue = PropertyValue;
            objClone.Required = Required;
            objClone.ValidationExpression = ValidationExpression;
            objClone.ViewOrder = ViewOrder;
            objClone.DefaultVisibility = DefaultVisibility;
            objClone.Visibility = Visibility;
            objClone.Visible = Visible;
            objClone.ClearIsDirty();
            return objClone;
        }
    }
}
