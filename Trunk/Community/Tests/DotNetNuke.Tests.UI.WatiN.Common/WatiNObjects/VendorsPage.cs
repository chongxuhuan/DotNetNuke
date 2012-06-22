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
    /// The admin/host vendors page object. 
    /// </summary>
    public class VendorsPage : WatiNBase
    {
        #region Constructors

        public VendorsPage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public VendorsPage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Links
        public Link AddNewVendorLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Add New Vendor"))); }
        }
        public Link CancelLink
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Link(Find.ByTitle("Cancel"));
                }
                return IEInstance.Link(Find.ByTitle("Cancel"));
            }
        }
        public Link DeleteLink
        {
            get { return PageContentDiv.Link(Find.ByTitle("Delete")); }
        }
        public Link AddNewBannerLink
        {
            get { return IEInstance.Link(Find.ById(s => s.EndsWith("EditVendors_/Banners.ascx_cmdAdd"))); }
        }
        public Link UnauthorizedDisplayLink
        {
            get { return IEInstance.Link(Find.ByText("Unauthorized")); }
        }
        /// <summary>
        /// The upload new file link for the vendor logo.
        /// </summary>
        public Link UploadLogoLink
        {
            get { return IEInstance.Link(Find.ByTitle("Upload New File")); }
        }
        /// <summary>
        /// The upload selected file link for the vendor logo.
        /// </summary>
        public Link UploadSelectedFileLink
        {
            get { return IEInstance.Link(Find.ByTitle("Upload Selected File")); }
        }
        public Link DeleteUnauthorizedVendorsLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Delete Unauthorized Vendors"))); }
        }
        public Link BannerSectionLink
        {
            get { return IEInstance.Link(Find.ByText(s => s.Contains("Banner Advertising"))); }
        }
        public Image VendorSearchButton
        {
            get { return IEInstance.Image(Find.ById(s => s.EndsWith("Vendors_btnSearch"))); }
        }
        #endregion

        #region Tables
        public Table VendorTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("Vendors_grdVendors"))); }
        }
        public Table BannerTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("EditVendors_/Banners.ascx_grdBanners"))); }
        }
        /// <summary>
        /// The table on the edit banner page containing a preview of the banner.
        /// </summary>
        public Table ViewBannerTable
        {
            get { return IEInstance.Table(Find.ById(s => s.EndsWith("EditBanner_lstBanners"))); }
        }
        #endregion

        #region TextFields
        public TextField CompanyNameField
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_txtVendorName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_txtVendorName")));
            }
        }
        public TextField FirstNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_txtFirstName")));
                } 
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_txtFirstName")));
            }
        }
        public TextField LastNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_txtLastName")));
                } 
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_txtLastName")));
            }
        }
        public TextField EmailField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_txtEmail")));
                } 
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_txtEmail")));
            }
        }
        public TextField StreetField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtStreet")));
                } 
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtStreet")));
            }
        }
        public TextField CityField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtCity")));
                } 
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtCity")));
            }
        }
        public TextField PostalCodeField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtPostal")));
                } 
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtPostal")));
            }
        }
        public TextField TelephoneField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtTelephone")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtTelephone")));
            }
        }
        public TextField CellphoneField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtCell")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtCell")));
            }
        }
        public TextField FaxField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtFax")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_txtFax")));
            }
        }
        public TextField BannerDescriptionField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditBanner_txtDescription")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditBanner_txtDescription")));
            }
        }
        public TextField BannerNameField
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.TextField(Find.ById(s => s.EndsWith("EditBanner_txtBannerName")));
                }
                return IEInstance.TextField(Find.ById(s => s.EndsWith("EditBanner_txtBannerName")));
            }
        }
        public TextField VendorSearchField
        {
            get
            {
               return IEInstance.TextField(Find.ById(s => s.EndsWith("Vendors_txtSearch")));
            }
        }
        #endregion

        #region SelectList
        public SelectList CountrySelectList
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_cboCountry")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_cboCountry")));
            }
        }
        public SelectList BannerTypeSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("EditBanner_cboBannerType")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("EditBanner_cboBannerType")));
            }
        }
        public SelectList BannerImageFileSelectList
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.SelectList(Find.ById(s => s.EndsWith("EditBanner_ctlImage_cboFiles")));
                }
                return IEInstance.SelectList(Find.ById(s => s.EndsWith("EditBanner_ctlImage_cboFiles")));
            }
        }
        public SelectList SearchTypeSelectList
        {
            get { return IEInstance.SelectList(Find.ById(s => s.EndsWith("Vendors_ddlSearchType"))); }
        }
        #endregion

        #region CheckBox
        public CheckBox RegionRequiredCheckbox
        {
            get
            {
                if(PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_chkRegion")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("EditVendors_addresssVendor_chkRegion")));
            }
        }
        public CheckBox AuthorizeVendorCheckBox
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.CheckBox(Find.ById(s => s.EndsWith("EditVendors_chkAuthorized")));
                }
                return IEInstance.CheckBox(Find.ById(s => s.EndsWith("EditVendors_chkAuthorized")));
            }
        }
        #endregion

        #region FileUploads
        public FileUpload LogoFileUpload
        {
            get { return IEInstance.FileUpload(Find.ById(s => s.EndsWith("EditVendors_ctlLogo_txtFile"))); }
        }
        #endregion

        #region Spans
        public Span VendorMessageSpan
        {
            get
            {
                if (PopUpFrame != null)
                {
                    return PopUpFrame.Div(Find.ById(s => s.EndsWith("ModuleContent"))).Span(Find.ById(s => s.EndsWith("lblMessage")));
                }
                return IEInstance.Div(Find.ById(s => s.EndsWith("ModuleContent"))).Span(Find.ById(s => s.EndsWith("lblMessage")));
            }
        }
        public Span ConfirmationSpan
        {
            get { return IEInstance.Span(Find.ByText("Yes")); }
        }
        #endregion

        #endregion

        #region Public Methods
        /// <summary>
        /// Finds the edit button for the vendor.
        /// </summary>
        /// <param name="vendorName">The vendor name as it appears in the vendor table.</param>
        /// <returns>The edit image/button for the vendor.</returns>
        public Image GetVendorEditButton(string vendorName)
        {
            //Returns the Edit Button for the vendor  
            Image result = null;
            foreach (TableRow row in VendorTable.TableRows)
            {
                if (row.TableCells[1].InnerHtml.Contains(vendorName))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the table row for the vendor.
        /// </summary>
        /// <param name="vendorName">The vendor name as it appears in the vendor table.</param>
        /// <returns>The table row for the vendor.</returns>
        public TableRow GetVendorRow(string vendorName)
        {
            //Returns the table Row for the vendor
            TableRow result = null;
            foreach (TableRow row in VendorTable.TableRows)
            {
                if (row.TableCells[1].InnerHtml.Contains(vendorName))
                {
                    result = row;
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the table row for the banner.
        /// </summary>
        /// <param name="bannerName">The banner name as it appears in the banner table.</param>
        /// <returns>The table row for the banner.</returns>
        public TableRow GetBannerRow(string bannerName)
        {
            //Returns the table row for the banner
            TableRow result = null;
            foreach (TableRow row in BannerTable.TableRows)
            {
                if (row.TableCells[1].InnerHtml.Contains(bannerName))
                {
                    result = row;
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Finds the edit button for the banner.
        /// </summary>
        /// <param name="bannerName">The banner name as it appears in the banner table.</param>
        /// <returns>The edit image/button for the banner.</returns>
        public Image GetBannerEditButton(string bannerName)
        {
            //Returns the Edit Button for the user specified 
            Image result = null;
            foreach (TableRow row in BannerTable.TableRows)
            {
                if (row.TableCells[1].InnerHtml.Contains(bannerName))
                {
                    result = row.Image(Find.ByTitle("Edit"));
                    break;
                }
                continue;
            }
            return result;
        }

        /// <summary>
        /// Adds a new vendor.
        /// Clicks the add new vendor link
        /// Fills in the new vendor field
        /// Clicks the update link
        /// The region field will be set to not required.
        /// </summary>
        /// <param name="vendorName">The company name.</param>
        /// <param name="vendorFName">The vendors first name.</param>
        /// <param name="vendorLname">The vendors last name.</param>
        /// <param name="vendorEmail">The vendors email.</param>
        /// <param name="vendorStreet">The vendors street address.</param>
        /// <param name="vendorCity">The vendors city.</param>
        /// <param name="vendorCountry">The vendors country.</param>
        /// <param name="vendorPostalCode">The vendors postal code.</param>
        /// <param name="vendorPhoneNumber">The vendors phone number. This number will be used for their Telephone, cell phone and fax numbers.</param>
        public void AddNewVendor(string vendorName, string vendorFName, string vendorLname, string vendorEmail, string vendorStreet, string vendorCity, string vendorCountry, string vendorPostalCode, string vendorPhoneNumber)
        {
            AddNewVendorLink.Click();
            CompanyNameField.Value = vendorName;
            FirstNameField.Value = vendorFName;
            LastNameField.Value = vendorLname;
            EmailField.Value = vendorEmail;
            StreetField.Value = vendorStreet;
            CityField.Value = vendorCity;
            CountrySelectList.Select(vendorCountry);
            System.Threading.Thread.Sleep(1500);
            RegionRequiredCheckbox.Checked = false;
            System.Threading.Thread.Sleep(1000);
            PostalCodeField.Value = vendorPostalCode;
            TelephoneField.Value = vendorPhoneNumber;
            CellphoneField.Value = vendorPhoneNumber;
            FaxField.Value = vendorPhoneNumber;
            UpdateLink.ClickNoWait();
        }

       #endregion
    }
}