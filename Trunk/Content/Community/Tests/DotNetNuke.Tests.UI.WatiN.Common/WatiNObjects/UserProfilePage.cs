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
    /// The user profile page object
    /// </summary>
    public class UserProfilePage : WatiNBase
    {
        #region Constructors

        public UserProfilePage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public UserProfilePage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Links

        public Link EditProfileLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Edit Profile"))); }
        }

        public Link ManagePasswordTabLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Manage Password"))); }
        }

        public Link ManageProfileTabLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByText("Manage Profile"));
                }
                return IEInstance.Link(Find.ByText("Manage Profile"));
            }
        }

        public Link ProfileUpdateLink
        {
            get
            {
                return ContentPaneDiv.Link(Find.ById(s => s.EndsWith("ManageUsers_Profile_cmdUpdate")));
            }
        }

        /// <summary>
        /// The upload file link for the Photo profile proptery.
        /// </summary>
        public Link UploadFileLink
        {
            get { return ContentPaneDiv.Link(Find.ByText(s => s.Contains("Upload File"))); }
        }

        /// <summary>
        /// The upload file link for a newly added image profile property with the name "ImageProperty"
        /// </summary>
        public Link ImagePropertyUploadFileLink
        {
            get { return ContentPaneDiv.Span(Find.ById(s => s.EndsWith("ImagePropertyFileControl"))).Link(Find.ByText(s => s.Contains("Upload File"))); }
        }

        /// <summary>
        /// The save file link for a newly added image profile property with the name "ImageProperty"
        /// </summary>
        public Link ImagePropertySaveFileLink
        {
            get { return ContentPaneDiv.Span(Find.ById(s => s.EndsWith("ImagePropertyFileControl"))).Link(Find.ByText(s => s.Contains("Save File"))); }
        }

        public Link ChangePasswordLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Change Password"));
                }
                return IEInstance.Link(Find.ByTitle("Change Password"));
            }
        }

        /// <summary>
        /// The save file link for the Photo profile property.
        /// </summary>
        public Link SaveFileLink
        {
            get { return PageContentDiv.Span(Find.ById(s => s.EndsWith("PhotoFileControl"))).Link(Find.ByText(s => s.Contains("Save File"))); }
        }

        public Link ManageServicesTabLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Manage Services"))); }
        }

        /// <summary>
        /// The link to switch the telerik editors content display mode to HTML.
        /// </summary>
        public Link HTMLEditorModeLink
        {
            get { return ProfilePropertiesDiv.Div(Find.ByClass("reEditorModes")).Link(Find.ByTitle("HTML")); }
        }

        /// <summary>
        /// The load default template link on the edit module page of the User Profile module.
        /// </summary>
        public Link LoadDefaultProfileTemplateLink
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ById(s => s.EndsWith("ModuleSettings_Settings_cmdLoadDefault")));
                }
                return PageContentDiv.Link(Find.ById(s => s.EndsWith("ModuleSettings_Settings_cmdLoadDefault")));
            }
        }

        #endregion

        #region TextFields 

        public TextField UserStreetField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$Street")));
                }
                return IEInstance.TextField(Find.ByName(s => s.EndsWith("$Street")));
            }
        }

        /// <summary>
        /// The text field for a newly added text profile property with the name "TextProperty"
        /// </summary>
        public TextField TextPropertyField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$TextProperty")));
                }
                return IEInstance.TextField(Find.ByName(s => s.EndsWith("$TextProperty")));
            }
        }

        /// <summary>
        /// The date text field for a newly added date time profile property with the name "DateTimeProperty"
        /// </summary>
        public TextField DateTimeDateField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$DateTimePropertydate")));
                }
                return IEInstance.TextField(Find.ByName(s => s.EndsWith("$DateTimePropertydate")));
            }
        }

        /// <summary>
        /// The region text field for a newly added region profile property with the name "RegionProperty"
        /// </summary>
        public TextField RegionPropertyField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$RegionProperty")));
                }
                return IEInstance.TextField(Find.ByName(s => s.EndsWith("$RegionProperty")));
            }
        }

        public TextField CurrentPasswordField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("_Password_txtOldPassword")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("_Password_txtOldPassword")));
            }
        }

        public TextField NewPasswordField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("_Password_txtNewPassword")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("_Password_txtNewPassword")));
            }
        }

        public TextField ConfirmNewPasswordField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("_Password_txtNewConfirm")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("_Password_txtNewConfirm")));
            }
        }

        public TextField PrefixField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Prefix"))); }
        }

        public TextField MiddleNameField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$MiddleName"))); }
        }

        public TextField SuffixField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Suffix"))); }
        }

        public TextField UnitField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Unit"))); }
        }

        public TextField StreetField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Street"))); }
        }

        public TextField CityField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$City"))); }
        }

        public TextField RegionField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Region"))); }
        }

        public TextField PostalCodeField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$PostalCode"))); }
        }

        public TextField TelephoneField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Telephone"))); }
        }

        public TextField CellPhoneField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Cell"))); }
        }

        public TextField FaxField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Fax"))); }
        }

        public TextField WebsiteField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$Website"))); }
        }

        public TextField IMField
        {
            get { return ProfilePropertiesDiv.TextField(Find.ByName(s => s.EndsWith("$IM"))); }
        }

        /// <summary>
        /// The profile propety template text field on the edit module page of the User Profile module.
        /// </summary>
        public TextField ProfileTemplateTextField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("ModuleSettings_Settings_txtTemplate")));
                }
                return PageContentDiv.TextField(Find.ById(s => s.EndsWith("ModuleSettings_Settings_txtTemplate")));
            }
        }

        /// <summary>
        /// The HTML frame field in the biography text editor for a host user.
        /// </summary>
        public TextField htmlFieldHost
        {
            get { return HTMLHostFrame.TextField(Find.Any); }
        }

        /// <summary>
        /// The HTML frame field in the biography text editor for an admin user
        /// </summary>
        public TextField htmlFieldAdmin
        {
            get { return HTMLAdminFrame.TextField(Find.Any); }
        }

        /// <summary>
        /// The HTML frame field in the biography text editor for a registered user.
        /// </summary>
        public TextField htmlField
        {
            get { return HTMLFrame.TextField(Find.Any); }
        }

        /// <summary>
        /// The hidden text field in the design section of the biography text editor.
        /// </summary>
        public TextField biographyField
        {
            get { return ContentPaneDiv.TextField(Find.ById(s => s.EndsWith("BiographyeditContentHiddenTextarea"))); }
        }

        #endregion

        #region Divs

        /// <summary>
        /// The div containing the users street on their profile page.
        /// </summary>
        public Div ProfileStreetDiv
        {
            get { return ProfileAddressDiv.Divs[1]; }
        }

        /// <summary>
        /// The outer div containing the users profile properties.
        /// </summary>
        public Div ProfilePropertiesDiv
        {
            get { return IEInstance.Div(Find.ById(s => s.EndsWith("ManageUsers_Profile_ProfileProperties"))); }
        }

        /// <summary>
        /// The outer div containing the users address on their profile page.
        /// </summary>
        public Div ProfileAddressDiv
        {
            get { return PageContentDiv.Div(Find.ByClass("ProfileAddress")); }
        }

        /// <summary>
        /// The div containing the users contact information on their profile page.
        /// </summary>
        public Div ProfileContactInfoDiv
        {
            get { return PageContentDiv.Div(Find.ByClass("ProfileContact")); }
        }

        /// <summary>
        /// The div containing the users biography on their profile page.
        /// </summary>
        public Div ProfileBioDiv
        {
            get { return PageContentDiv.Div(Find.ByClass("ProfileBio")); }
        }

        #endregion

        #region SelectLists

        /// <summary>
        /// The hour selectlist for a newly added date time profile property with the name "DateTimeProperty"
        /// </summary>
        public SelectList HourSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$DateTimePropertyhours")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$DateTimePropertyhours")));
            }
        }

        /// <summary>
        /// The minute selectlist for a newly added date time profile property with the name "DateTimeProperty"
        /// </summary>
        public SelectList MinuteSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$DateTimePropertyminutes")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$DateTimePropertyminutes")));
            }
        }

        /// <summary>
        /// The am/pm selectlist for a newly added date time profile property with the name "DateTimeProperty"
        /// </summary>
        public SelectList AmPmSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$DateTimePropertyampm")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$DateTimePropertyampm")));
            }
        }

        /// <summary>
        /// The country selectlist for a newly added country profile property with the name "ContryProperty"
        /// </summary>
        public SelectList CountryPropertySelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$CountryProperty")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$CountryProperty")));
            }
        }

        /// <summary>
        /// The folder selectlist for a newly added image profile property with the name "ImageProperty"
        /// </summary>
        public SelectList ImageFolderSelectList
        {
            get { return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$ImagePropertyFileControl$Folder"))); }
        }

        /// <summary>
        /// The file selectlist for a newly added image profile property with the name "ImageProperty"
        /// </summary>
        public SelectList ImageFileSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("ImagePropertyFileControl_File")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("ImagePropertyFileControl_File")));
            }
        }
        
        public SelectList PreferredLocaleSelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ByName(s => s.EndsWith("$PreferredLocale")));
                }
                return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$PreferredLocale")));
            }
        }

        public SelectList TimeZoneSelectList
        {
            get { return IEInstance.SelectList(Find.ByName(s => s.EndsWith("$TimeZone"))); }
        }

        public SelectList CountrySelectList
        {
            get { return ProfilePropertiesDiv.SelectList(Find.ByName(s => s.EndsWith("$Country"))); }
        }

        public SelectList PhotoFolderSelectList
        {
            get { return ProfilePropertiesDiv.SelectList(Find.ById(s => s.EndsWith("_PhotoFileControl_Folder"))); }
        }

        public SelectList PhotoFileSelectList
        {
            get { return ProfilePropertiesDiv.SelectList(Find.ById(s => s.EndsWith("_PhotoFileControl_File"))); }
        }

        #endregion

        #region File Uploads
        /// <summary>
        /// The file upload for a newly create image profile property named "ImageProperty".
        /// </summary>
        public FileUpload ImagePropertyFileUpload
        {
            get { return ContentPaneDiv.FileUpload(Find.ByName(s => s.Contains("$ImagePropertyFileControl$"))); }
        }

       /// <summary>
       /// The file upload for the photo property.
       /// </summary>
        public FileUpload FileUpload
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.FileUpload(Find.ByName(s => s.Contains("$PhotoFileControl$")));
                }
                return IEInstance.FileUpload(Find.ByName(s => s.Contains("$PhotoFileControl$")));
            }
        }

        #endregion

        #region Collections
        //public ImageCollection RequiredImages
        //{
        //    get { return IEInstance.Images.Filter(Find.BySrc(s => s.EndsWith("required.gif"))); }
        //}
        /// <summary>
        /// The spans for all of the profile properties that are required. 
        /// </summary>
        public SpanCollection RequiredPropertiesSpans
        {
            get { return IEInstance.Spans.Filter(Find.ById(s => s.EndsWith("_Required"))); }
        }
        /// <summary>
        /// All of the profile property elements (text fields, selectlists etc) that are required.
        /// </summary>
        public ElementCollection RequiredElements
        {
            get { return IEInstance.Elements.Filter(Find.ByClass("dnnFormRequired")); }
        }
        #endregion

        #region Tables
        public Table UserRolesTable
        {
            get { return IEInstance.Table(Find.ById("dnn_ctr_ManageUsers_SecurityRoles_grdUserRoles")); }
        }
        public Table ServicesTable
        {
            get { return IEInstance.Table(Find.ById("dnn_ctr_ManageUsers_MemberServices_grdServices")); }
        }
        #endregion

        #region Frames
        /// <summary>
        /// The HTML frame in the biography text editor for superusers.
        /// </summary>
        public Frame HTMLHostFrame
        {
            get { return IEInstance.Frames[3]; }
        }
        /// <summary>
        /// The HTML frame in the biography text editor for admin users.
        /// </summary>
        public Frame HTMLAdminFrame
        {
            get { return IEInstance.Frames[3]; }
        }
        /// <summary>
        /// The HTML frame in the biography text editor for registered users.
        /// </summary>
        public Frame HTMLFrame
        {
            get { return IEInstance.Frames[1]; }
        }
        #endregion

        #region Images
        public Image ProfileImage
        {
            get { return PageContentDiv.Image(Find.ByAlt("Profile Avatar")); }
        }
        #endregion

        #endregion

        #region Public Methods

        public TextField GetProfilePropertyTextField(string propertyName)
        {
            if (PopUpFrame != null)
            {
                return PopUpFrame.TextField(Find.ByName(s => s.EndsWith("$ManageUsers$Profile$ProfileProperties$" + propertyName + "$" + propertyName)));
            }
            return IEInstance.TextField(Find.ByName(s => s.EndsWith("$ManageUsers$Profile$ProfileProperties$" + propertyName + "$" + propertyName)));
        }

        public Div GetProfilePropertyDiv(string propertyName)
        {
            if (PopUpFrame != null)
            {
                return PopUpFrame.Div(Find.ById(s => s.EndsWith("ManageUsers_Profile_ProfileProperties_" + propertyName)));
            }
            return IEInstance.Div(Find.ById(s => s.EndsWith("ManageUsers_Profile_ProfileProperties_" + propertyName)));
        }

        /// <summary>
        /// Fills in all the user profile properties for a superuser.
        /// The users country will be set to Belgium, since it will not affect the region field. Other countries will turn the region field into a select list.
        /// </summary>
        /// <param name="photoPath">The path for a photo.</param>
        /// <param name="prefix">The users prefix.</param>
        /// <param name="middle">The users middle name.</param>
        /// <param name="suffix">The users suffix.</param>
        /// <param name="unit">The users unit</param>
        /// <param name="street">The users street address.</param>
        /// <param name="city">The users city.</param>
        /// <param name="region">The users region.</param>
        /// <param name="postalCode">The users postal code.</param>
        /// <param name="telephone">The users telephone number. This will be used for their Telephone, Cell and Fax properties.</param>
        /// <param name="website">The users website.</param>
        /// <param name="IM">The users IM.</param>
        /// <param name="biography">The users biography.</param>
        public  void FillInProfileFieldsHost(string photoPath, string prefix, string middle, string suffix, string unit, string street, string city, string region, string postalCode, string telephone, string website, string IM, string biography)
        {
            PrefixField.Value = prefix;
            MiddleNameField.Value = middle;
            SuffixField.Value = suffix;
            UnitField.Value = unit;
            StreetField.Value = street;
            CityField.Value = city;
            CountrySelectList.SelectByValue("Belgium");
            System.Threading.Thread.Sleep(2500);
            RegionField.Value = region;
            PostalCodeField.Value = postalCode;
            TelephoneField.Value = telephone;
            CellPhoneField.Value = telephone;
            FaxField.Value = telephone;
            WebsiteField.Value = website;
            IMField.Value = IM;
            HTMLEditorModeLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
            //Gallio.Framework.TestLog.WriteLine("There are " + IEInstance.Frames.Count + " Frames");
            htmlFieldHost.Value = biography;
            System.Threading.Thread.Sleep(1500);
            ProfileUpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
            UploadFileLink.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
            FileUpload.Set(photoPath);
            System.Threading.Thread.Sleep(2000);
            SaveFileLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);
            ProfileUpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(2000);
        }

        /// <summary>
        /// Fills in all the user profile properties for an administrator.
        /// The users country will be set to Belgium, since it will not affect the region field. Other countries will turn the region field into a select list.
        /// </summary>
        /// <param name="photoPath">The path for a photo.</param>
        /// <param name="prefix">The users prefix.</param>
        /// <param name="middle">The users middle name.</param>
        /// <param name="suffix">The users suffix.</param>
        /// <param name="unit">The users unit</param>
        /// <param name="street">The users street address.</param>
        /// <param name="city">The users city.</param>
        /// <param name="region">The users region.</param>
        /// <param name="postalCode">The users postal code.</param>
        /// <param name="telephone">The users telephone number. This will be used for their Telephone, Cell and Fax properties.</param>
        /// <param name="website">The users website.</param>
        /// <param name="IM">The users IM.</param>
        /// <param name="biography">The users biography.</param>
        public void FillInProfileFieldsAdmin(string photoPath, string prefix, string middle, string suffix, string unit, string street, string city, string region, string postalCode, string telephone, string website, string IM, string biography)
        {
            PrefixField.Value = prefix;
            MiddleNameField.Value = middle;
            SuffixField.Value = suffix;
            UnitField.Value = unit;
            StreetField.Value = street;
            CityField.Value = city;
            CountrySelectList.SelectByValue("Belgium");
            System.Threading.Thread.Sleep(2500);
            RegionField.Value = region;
            PostalCodeField.Value = postalCode;
            TelephoneField.Value = telephone;
            CellPhoneField.Value = telephone;
            FaxField.Value = telephone;
            WebsiteField.Value = website;
            IMField.Value = IM;
            HTMLEditorModeLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
            //Gallio.Framework.TestLog.WriteLine("There are " + IEInstance.Frames.Count + " Frames");
            htmlFieldAdmin.Value = biography;
            System.Threading.Thread.Sleep(1500);
            ProfileUpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
            UploadFileLink.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
            FileUpload.Set(photoPath);
            System.Threading.Thread.Sleep(2000);
            SaveFileLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);
            ProfileUpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(2000);
        }

        /// <summary>
        /// Fills in all the user profile properties for a registered users.
        /// The users country will be set to Belgium, since it will not affect the region field. Other countries will turn the region field into a select list.
        /// </summary>
        /// <param name="photoPath">The path for a photo.</param>
        /// <param name="prefix">The users prefix.</param>
        /// <param name="middle">The users middle name.</param>
        /// <param name="suffix">The users suffix.</param>
        /// <param name="unit">The users unit</param>
        /// <param name="street">The users street address.</param>
        /// <param name="city">The users city.</param>
        /// <param name="region">The users region.</param>
        /// <param name="postalCode">The users postal code.</param>
        /// <param name="telephone">The users telephone number. This will be used for their Telephone, Cell and Fax properties.</param>
        /// <param name="website">The users website.</param>
        /// <param name="IM">The users IM.</param>
        /// <param name="biography">The users biography.</param>
        public void FillInProfileFields(string photoPath, string prefix, string middle, string suffix, string unit, string street, string city, string region, string postalCode, string telephone, string website, string IM, string biography)
        {
            PrefixField.Value = prefix;
            MiddleNameField.Value = middle;
            SuffixField.Value = suffix;
            UnitField.Value = unit;
            StreetField.Value = street;
            CityField.Value = city;
            CountrySelectList.SelectByValue("Belgium");
            System.Threading.Thread.Sleep(2500);
            RegionField.Value = region;
            PostalCodeField.Value = postalCode;
            TelephoneField.Value = telephone;
            CellPhoneField.Value = telephone;
            FaxField.Value = telephone;
            WebsiteField.Value = website;
            IMField.Value = IM;
            HTMLEditorModeLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
            //Gallio.Framework.TestLog.WriteLine("There are " + IEInstance.Frames.Count + " Frames");
            htmlField.Value = biography;
            System.Threading.Thread.Sleep(1500);
            ProfileUpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(1500);
            UploadFileLink.ClickNoWait();
            System.Threading.Thread.Sleep(1000);
            FileUpload.Set(photoPath);
            System.Threading.Thread.Sleep(2000);
            SaveFileLink.ClickNoWait();
            System.Threading.Thread.Sleep(3000);
            ProfileUpdateLink.ClickNoWait();
            System.Threading.Thread.Sleep(2000);
        }

        #endregion
    }
}