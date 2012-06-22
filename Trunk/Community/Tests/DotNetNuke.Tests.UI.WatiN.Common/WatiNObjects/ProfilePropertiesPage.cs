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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The profile properties page object.
    /// </summary>
    public class ProfilePropertiesPage : WatiNBase
    {
        #region Constructors

        public ProfilePropertiesPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public ProfilePropertiesPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Links

        public Link AddNewProfilePropertyLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s =>s.EndsWith("profileDefinitions_cmdAdd")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("profileDefinitions_cmdAdd")));
            }
        }

        public Link NextButton
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_nextButtonStart")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("EditProfileDefinition_Wizard_StartNavigationTemplateContainerID_nextButtonStart")));
            }
        }

        public Link ReturnToProfilePropertiesLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_finishButtonStep")));
                }
                return IEInstance.Link(Find.ById(s => s.EndsWith("EditProfileDefinition_Wizard_FinishNavigationTemplateContainerID_finishButtonStep")));
            }
        }

        #endregion

        #region TextFields

        public TextField PropertyNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$PropertyName")));
                }
                return IEInstance.TextField(Find.ByName(s => s.EndsWith("$PropertyName")));
            }
        }

        public TextField PropertyCategoryField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$PropertyCategory")));
                }
                return IEInstance.TextField(Find.ByName(s => s.EndsWith("$PropertyCategory")));
            }
        }

        public TextField LengthTextField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$Length")));
                }
                return IEInstance.TextField(Find.ByName(s => s.EndsWith("$Length")));
            }
        }

        #endregion

        #region SelectLists

        public SelectList DataTypeSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$DataType")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$DataType")));
            }
        }

        #endregion

        #region CheckBoxes

        /// <summary>
        /// The visible checkbox from the Add New Profile Property Definition Page
        /// This will not find the visible checkbox for any property on the Profile Properties page.
        /// </summary>
        public CheckBox VisibleCheckBox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ByName(s => s.EndsWith("$Visible")));
                }
                return IEInstance.CheckBox(Find.ByName(s => s.EndsWith("$Visible")));
            }
        }

        /// <summary>
        /// The rreadonly checkbox from the Add New Profile Property Definition Page
        /// This will not find the required checkbox for any property on the Profile Properties page.
        /// </summary>
        public CheckBox ReadOnlyCheckBox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ByName(s => s.EndsWith("$ReadOnly")));
                }
                return IEInstance.CheckBox(Find.ByName(s => s.EndsWith("$ReadOnly")));
            }
        }

        /// <summary>
        /// The required checkbox from the Add New Profile Property Definition Page
        /// This will not find the required checkbox for any property on the Profile Properties page.
        /// </summary>
        public CheckBox RequiredCheckBox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ByName(s => s.EndsWith("$Required")));
                }
                return IEInstance.CheckBox(Find.ByName(s => s.EndsWith("$Required")));
            }
        }

        #endregion

        #region Tables

        public Table PropertyTable
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Table(Find.ById(s => s.EndsWith("profileDefinitions_grdProfileProperties")));
                }
                return IEInstance.Table(Find.ById(s => s.EndsWith("profileDefinitions_grdProfileProperties")));
            }
        }

        /// <summary>
        /// The table containing the property details.
        /// </summary>
        public Table PropertyDefTable
        {
            get { return PageContentDiv.Table(Find.ById(s => s.EndsWith("EditProfileDefinition_Wizard"))); }
        }

        #endregion

        #endregion

        #region Public Methods

        public Div GetPropertyEditorRow(string propertyName)
        {
            if (PopUpFrame != null)
            {
                return PopUpFrame.Div(Find.ById(s => s.EndsWith("EditProfileDefinition_Wizard_Properties_" + propertyName)));
            }
            return IEInstance.Div(Find.ById(s => s.EndsWith("EditProfileDefinition_Wizard_Properties_" + propertyName)));
        }

        /// <summary>
        /// Finds the Row for the property in the profile properties table.
        /// </summary>
        /// <param name="propertyName">The name of the profile property as it appears in the profile properties table.</param>
        /// <returns>The table row containing information about the profile property.</returns>
        public TableRow GetPropertyRow(string propertyName)
        {
            //Returns the property row with the specified property name
            TableRow propertyRow = null;
            foreach (TableRow row in PropertyTable.TableRows)
            {
                if (row.TableCells[4].InnerHtml.Contains(propertyName))
                {
                    propertyRow = row;
                    break;
                }
                continue;
            }
            return propertyRow;
        }

        /// <summary>
        /// Finds the edit button for the profile property.
        /// </summary>
        /// <param name="propertyName">The name of the profile property as it appears in the profile properties table.</param>
        /// <returns>The edit image/button for the profile property.</returns>
        public Image GetPropertyEditButton(string propertyName)
        {
            //Returns the property row with the specified property name
            Image result = null;
            foreach (TableRow row in PropertyTable.TableRows)
            {
                if (row.TableCells[4].InnerHtml.Contains(propertyName))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the property name cell for the profile property.
        /// </summary>
        /// <param name="propertyRow">The row from the profile properties table for the property.</param>
        /// <returns>The table cell containing the profile property name.</returns>
        public TableCell GetPropertyNameCell(TableRow propertyRow)
        {
            return propertyRow.TableCells[4];
        }

        /// <summary>
        /// Finds the property category cell for the profile property.
        /// </summary>
        /// <param name="propertyRow">The row from the profile properties table for the property.</param>
        /// <returns>The table cell containing the profile property category.</returns>
        public TableCell GetPropertyCategoryCell(TableRow propertyRow)
        {
            return propertyRow.TableCells[5];
        }

        /// <summary>
        /// Finds the property data type cell for the profile property.
        /// </summary>
        /// <param name="propertyRow">The row from the profile properties table for the property.</param>
        /// <returns>The table cell containing the profile property data type.</returns>
        public TableCell GetPropertyDataTypeCell(TableRow propertyRow)
        {
            return propertyRow.TableCells[6];
        }

        /// <summary>
        /// Finds the property visibility cell for the profile property.
        /// </summary>
        /// <param name="propertyRow">The row from the profile properties table for the property.</param>
        /// <returns>The table cell containing the visibility of the profile property.</returns>
        public TableCell GetPropertyVisibilityCell(TableRow propertyRow)
        {
            if (IEInstance.Url.Contains("Superuser%20Accounts"))
            {
                return propertyRow.TableCells[11];
            }
            return propertyRow.TableCells[12];
        }

        #endregion
    }
}