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
	/// -----------------------------------------------------------------------------
    /// Project:    DotNetNuke
    /// Namespace:  DotNetNuke.Entities.Profile
    /// Class:      ProfilePropertyDefinition
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The ProfilePropertyDefinition class provides a Business Layer entity for 
    /// property Definitions
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    ///     [cnurse]	01/31/2006	created
    /// </history>
    /// -----------------------------------------------------------------------------
   [XmlRoot("profiledefinition", IsNullable = false)]
    [Serializable]
    public class ProfilePropertyDefinition : BaseEntityInfo
    {
        #region Private Members

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
        private bool _ReadOnly = false;
        private bool _Required;
        private string _ValidationExpression;
        private int _ViewOrder;
        private UserVisibilityMode _Visibility = UserVisibilityMode.AdminOnly;
        private bool _Visible;
        private bool _Deleted;

        #endregion

        #region Constructors

        public ProfilePropertyDefinition()
        {
            //Get the default PortalSettings
            PortalSettings _Settings = PortalController.GetCurrentPortalSettings();
            PortalId = _Settings.PortalId;
        }

        public ProfilePropertyDefinition(int portalId)
        {
            PortalId = portalId;
        }

        #endregion

        #region Public Properties

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Data Type of the Profile Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Default Value of the Profile Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets whether the Definition has been modified since it has been retrieved
        /// </summary>
        /// <history>
        ///     [cnurse]	02/21/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        [XmlIgnore]
        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Length of the Profile Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the ModuleDefId
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Deleted
        /// </summary>
        /// -----------------------------------------------------------------------------
        [Browsable(false)]
        [XmlIgnore]
        public bool Deleted
        {
            get
            {
                return _Deleted;
            }
            set
            {
                _Deleted = value;
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the PortalId
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Category of the Profile Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Id of the ProfilePropertyDefinition
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Name of the Profile Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the Value of the Profile Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the property is read only
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(7)]
        [XmlIgnore]
        public bool ReadOnly
        {
            get
            {
                return _ReadOnly;
            }
            set
            {
                if (_ReadOnly != value)
                {
                    _IsDirty = true;
                }
                _ReadOnly = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the property is required
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets a Validation Expression (RegEx) for the Profile Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets the View Order of the Property
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        [IsReadOnly(true), SortOrder(9)]
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the property is visible
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        [SortOrder(8)]
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
        [SortOrder(10)]
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

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Gets and sets whether the property is visible
        /// </summary>
        /// <history>
        ///     [cnurse]	01/31/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
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

        #endregion

        #region Public Methods

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Clears the IsDirty Flag
        /// </summary>
        /// <history>
        ///     [cnurse]	02/23/2006	created
        /// </history>
        /// -----------------------------------------------------------------------------
        public void ClearIsDirty()
        {
            _IsDirty = false;
        }

        /// <summary>
        /// Clone a ProfilePropertyDefinition
        /// </summary>
        /// <returns>A ProfilePropertyDefinition</returns>
        public ProfilePropertyDefinition Clone()
        {
            var clone = new ProfilePropertyDefinition(PortalId)
                            {
                                DataType = DataType,
                                DefaultValue = DefaultValue,
                                Length = Length,
                                ModuleDefId = ModuleDefId,
                                PropertyCategory = PropertyCategory,
                                PropertyDefinitionId = PropertyDefinitionId,
                                PropertyName = PropertyName,
                                PropertyValue = PropertyValue,
                                ReadOnly = ReadOnly,
                                Required = Required,
                                ValidationExpression = ValidationExpression,
                                ViewOrder = ViewOrder,
                                DefaultVisibility = DefaultVisibility,
                                Visibility = Visibility,
                                Visible = Visible,
                                Deleted = Deleted
                            };
            clone.ClearIsDirty();
            return clone;
        }

        #endregion
    }
}
