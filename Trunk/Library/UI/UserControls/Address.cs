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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.WebControls;

#endregion

namespace DotNetNuke.UI.UserControls
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Address UserControl is used to manage User Addresses
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
    ///                       and localisation
    /// </history>
    /// -----------------------------------------------------------------------------
    public abstract class Address : UserControlBase
    {
		#region "Private Members"
		
        private string MyFileName = "Address.ascx";
        private string _Cell;
        private string _City;
        private string _ControlColumnWidth = "";
        private string _Country;
        private string _CountryData = "Text";
        private string _Fax;
        private string _LabelColumnWidth = "";
        private int _ModuleId;
        private string _Postal;
        private string _Region;
        private string _RegionData = "Text";
        private bool _ShowCell = true;
        private bool _ShowCity = true;
        private bool _ShowCountry = true;
        private bool _ShowFax = true;
        private bool _ShowPostal = true;
        private bool _ShowRegion = true;
        private bool _ShowStreet = true;
        private bool _ShowTelephone = true;
        private bool _ShowUnit = true;
        private string _Street;
        private string _Telephone;
        private string _Unit;
        protected CountryListBox cboCountry;
        protected DropDownList cboRegion;
        protected CheckBox chkCell;
        protected CheckBox chkCity;
        protected CheckBox chkCountry;
        protected CheckBox chkFax;
        protected CheckBox chkPostal;
        protected CheckBox chkRegion;
        protected CheckBox chkStreet;
        protected CheckBox chkTelephone;
        protected Label lblCellRequired;
        protected Label lblCityRequired;
        protected Label lblCountryRequired;
        protected Label lblFaxRequired;
        protected Label lblPostalRequired;
        protected Label lblRegion;
        protected Label lblRegionRequired;
        protected Label lblStreetRequired;
        protected Label lblTelephoneRequired;
        protected LabelControl plCell;
        protected LabelControl plCity;
        protected LabelControl plCountry;
        protected LabelControl plFax;
        protected LabelControl plPostal;
        protected LabelControl plRegion;
        protected LabelControl plStreet;
        protected LabelControl plTelephone;
        protected LabelControl plUnit;
        protected HtmlTableRow rowCell;
        protected HtmlTableRow rowCity;
        protected HtmlTableRow rowCountry;
        protected HtmlTableRow rowFax;
        protected HtmlTableRow rowPostal;
        protected HtmlTableRow rowRegion;
        protected HtmlTableRow rowStreet;
        protected HtmlTableRow rowTelephone;
        protected HtmlTableRow rowUnit;
        protected TextBox txtCell;
        protected TextBox txtCity;
        protected TextBox txtFax;
        protected TextBox txtPostal;
        protected TextBox txtRegion;
        protected TextBox txtStreet;
        protected TextBox txtTelephone;
        protected TextBox txtUnit;
        protected RequiredFieldValidator valCell;
        protected RequiredFieldValidator valCity;
        protected RequiredFieldValidator valCountry;
        protected RequiredFieldValidator valFax;
        protected RequiredFieldValidator valPostal;
        protected RequiredFieldValidator valRegion1;
        protected RequiredFieldValidator valRegion2;
        protected RequiredFieldValidator valStreet;
        protected RequiredFieldValidator valTelephone;
		
		#endregion

		#region "Constructors"

        public Address()
        {
            StartTabIndex = 1;
        }
		
		#endregion

		#region "Properties"

        public int ModuleId
        {
            get
            {
                return Convert.ToInt32(ViewState["ModuleId"]);
            }
            set
            {
                _ModuleId = value;
            }
        }

        public string LabelColumnWidth
        {
            get
            {
                return Convert.ToString(ViewState["LabelColumnWidth"]);
            }
            set
            {
                _LabelColumnWidth = value;
            }
        }

        public string ControlColumnWidth
        {
            get
            {
                return Convert.ToString(ViewState["ControlColumnWidth"]);
            }
            set
            {
                _ControlColumnWidth = value;
            }
        }

        public int StartTabIndex { private get; set; }

        public string Street
        {
            get
            {
                return txtStreet.Text;
            }
            set
            {
                _Street = value;
            }
        }

        public string Unit
        {
            get
            {
                return txtUnit.Text;
            }
            set
            {
                _Unit = value;
            }
        }

        public string City
        {
            get
            {
                return txtCity.Text;
            }
            set
            {
                _City = value;
            }
        }

        public string Country
        {
            get
            {
                string retValue = "";
                if (cboCountry.SelectedItem != null)
                {
                    switch (_CountryData.ToLower())
                    {
                        case "text":
                            if (cboCountry.SelectedIndex == 0) //Return blank if 'Not_Specified' selected 
                            {
                                retValue = "";
                            }
                            else
                            {
                                retValue = cboCountry.SelectedItem.Text;
                            }
                            break;
                        case "value":
                            retValue = cboCountry.SelectedItem.Value;
                            break;
                    }
                }
                return retValue;
            }
            set
            {
                _Country = value;
            }
        }

        public string Region
        {
            get
            {
                string retValue = "";
                if (cboRegion.Visible)
                {
                    if (cboRegion.SelectedItem != null)
                    {
                        switch (_RegionData.ToLower())
                        {
                            case "text":
                                if (cboRegion.SelectedIndex > 0)
                                {
                                    retValue = cboRegion.SelectedItem.Text;
                                }
                                break;
                            case "value":
                                retValue = cboRegion.SelectedItem.Value;
                                break;
                        }
                    }
                }
                else
                {
                    retValue = txtRegion.Text;
                }
                return retValue;
            }
            set
            {
                _Region = value;
            }
        }

        public string Postal
        {
            get
            {
                return txtPostal.Text;
            }
            set
            {
                _Postal = value;
            }
        }

        public string Telephone
        {
            get
            {
                return txtTelephone.Text;
            }
            set
            {
                _Telephone = value;
            }
        }

        public string Cell
        {
            get
            {
                return txtCell.Text;
            }
            set
            {
                _Cell = value;
            }
        }

        public string Fax
        {
            get
            {
                return txtFax.Text;
            }
            set
            {
                _Fax = value;
            }
        }

        public bool ShowStreet
        {
            set
            {
                _ShowStreet = value;
            }
        }

        public bool ShowUnit
        {
            set
            {
                _ShowUnit = value;
            }
        }

        public bool ShowCity
        {
            set
            {
                _ShowCity = value;
            }
        }

        public bool ShowCountry
        {
            set
            {
                _ShowCountry = value;
            }
        }

        public bool ShowRegion
        {
            set
            {
                _ShowRegion = value;
            }
        }

        public bool ShowPostal
        {
            set
            {
                _ShowPostal = value;
            }
        }

        public bool ShowTelephone
        {
            set
            {
                _ShowTelephone = value;
            }
        }

        public bool ShowCell
        {
            set
            {
                _ShowCell = value;
            }
        }

        public bool ShowFax
        {
            set
            {
                _ShowFax = value;
            }
        }

        public string CountryData
        {
            set
            {
                _CountryData = value;
            }
        }

        public string RegionData
        {
            set
            {
                _RegionData = value;
            }
        }

        public string LocalResourceFile
        {
            get
            {
                return Localization.GetResourceFile(this, MyFileName);
            }
        }
		
		#endregion

		#region "Private Methods"

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Localize correctly sets up the control for US/Canada/Other Countries
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        /// -----------------------------------------------------------------------------
        private void Localize()
        {
            string countryCode = cboCountry.SelectedItem.Value;
            var ctlEntry = new ListController();
            //listKey in format "Country.US:Region"
            string listKey = "Country." + countryCode;
            ListEntryInfoCollection entryCollection = ctlEntry.GetListEntryInfoCollection("Region", listKey);

            if (entryCollection.Count != 0)
            {
                cboRegion.Visible = true;
                txtRegion.Visible = false;
                {
                    cboRegion.Items.Clear();
                    cboRegion.DataSource = entryCollection;
                    cboRegion.DataBind();
                    cboRegion.Items.Insert(0, new ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""));
                }
                if (countryCode.ToLower() == "us")
                {
                    valRegion1.Enabled = true;
                    valRegion2.Enabled = false;
                    valRegion1.ErrorMessage = Localization.GetString("StateRequired", Localization.GetResourceFile(this, MyFileName));
                    plRegion.Text = Localization.GetString("plState", Localization.GetResourceFile(this, MyFileName));
                    plRegion.HelpText = Localization.GetString("plState.Help", Localization.GetResourceFile(this, MyFileName));
                    plPostal.Text = Localization.GetString("plZip", Localization.GetResourceFile(this, MyFileName));
                    plPostal.HelpText = Localization.GetString("plZip.Help", Localization.GetResourceFile(this, MyFileName));
                }
                else
                {
                    valRegion1.ErrorMessage = Localization.GetString("ProvinceRequired", Localization.GetResourceFile(this, MyFileName));
                    plRegion.Text = Localization.GetString("plProvince", Localization.GetResourceFile(this, MyFileName));
                    plRegion.HelpText = Localization.GetString("plProvince.Help", Localization.GetResourceFile(this, MyFileName));
                    plPostal.Text = Localization.GetString("plPostal", Localization.GetResourceFile(this, MyFileName));
                    plPostal.HelpText = Localization.GetString("plPostal.Help", Localization.GetResourceFile(this, MyFileName));
                }
                valRegion1.Enabled = true;
                valRegion2.Enabled = false;
            }
            else
            {
                cboRegion.ClearSelection();
                cboRegion.Visible = false;
                txtRegion.Visible = true;
                valRegion1.Enabled = false;
                valRegion2.Enabled = true;
                valRegion2.ErrorMessage = Localization.GetString("RegionRequired", Localization.GetResourceFile(this, MyFileName));
                plRegion.Text = Localization.GetString("plRegion", Localization.GetResourceFile(this, MyFileName));
                plRegion.HelpText = Localization.GetString("plRegion.Help", Localization.GetResourceFile(this, MyFileName));
                plPostal.Text = Localization.GetString("plPostal", Localization.GetResourceFile(this, MyFileName));
                plPostal.HelpText = Localization.GetString("plPostal.Help", Localization.GetResourceFile(this, MyFileName));
            }
            if (String.IsNullOrEmpty(lblRegionRequired.Text))
            {
                valRegion1.Enabled = false;
                valRegion2.Enabled = false;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ShowRequiredFields sets up displaying which fields are required
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        /// -----------------------------------------------------------------------------
        private void ShowRequiredFields()
        {
            Dictionary<string, string> settings = PortalController.GetPortalSettingsDictionary(PortalSettings.PortalId);

            lblStreetRequired.Text = PortalController.GetPortalSettingAsBoolean("addressstreet", PortalSettings.PortalId, true) ? "*" : "";
            lblCityRequired.Text = PortalController.GetPortalSettingAsBoolean("addresscity", PortalSettings.PortalId, true) ? "*" : "";
            lblCountryRequired.Text = PortalController.GetPortalSettingAsBoolean("addresscountry", PortalSettings.PortalId, true) ? "*" : "";
            lblRegionRequired.Text = PortalController.GetPortalSettingAsBoolean("addressregion", PortalSettings.PortalId, true) ? "*" : "";
            lblPostalRequired.Text = PortalController.GetPortalSettingAsBoolean("addresspostal", PortalSettings.PortalId, true) ? "*" : "";
            lblTelephoneRequired.Text = PortalController.GetPortalSettingAsBoolean("addresstelephone", PortalSettings.PortalId, true) ? "*" : "";
            lblCellRequired.Text = PortalController.GetPortalSettingAsBoolean("addresscell", PortalSettings.PortalId, true) ? "*" : "";
            lblFaxRequired.Text = PortalController.GetPortalSettingAsBoolean("addressfax", PortalSettings.PortalId, true) ? "*" : "";

            if (TabPermissionController.CanAdminPage())
            {
                if (lblCountryRequired.Text == "*")
                {
                    chkCountry.Checked = true;
                    valCountry.Enabled = true;
                }
                if (lblRegionRequired.Text == "*")
                {
                    chkRegion.Checked = true;
                    if (cboRegion.Visible)
                    {
                        valRegion1.Enabled = true;
                        valRegion2.Enabled = false;
                    }
                    else
                    {
                        valRegion1.Enabled = false;
                        valRegion2.Enabled = true;
                    }
                }
                if (lblCityRequired.Text == "*")
                {
                    chkCity.Checked = true;
                    valCity.Enabled = true;
                }
                if (lblStreetRequired.Text == "*")
                {
                    chkStreet.Checked = true;
                    valStreet.Enabled = true;
                }
                if (lblPostalRequired.Text == "*")
                {
                    chkPostal.Checked = true;
                    valPostal.Enabled = true;
                }
                if (lblTelephoneRequired.Text == "*")
                {
                    chkTelephone.Checked = true;
                    valTelephone.Enabled = true;
                }
                if (lblCellRequired.Text == "*")
                {
                    chkCell.Checked = true;
                    valCell.Enabled = true;
                }
                if (lblFaxRequired.Text == "*")
                {
                    chkFax.Checked = true;
                    valFax.Enabled = true;
                }
            }
            if (String.IsNullOrEmpty(lblCountryRequired.Text))
            {
                valCountry.Enabled = false;
            }
            if (String.IsNullOrEmpty(lblRegionRequired.Text))
            {
                valRegion1.Enabled = false;
                valRegion2.Enabled = false;
            }
            if (String.IsNullOrEmpty(lblCityRequired.Text))
            {
                valCity.Enabled = false;
            }
            if (String.IsNullOrEmpty(lblStreetRequired.Text))
            {
                valStreet.Enabled = false;
            }
            if (String.IsNullOrEmpty(lblPostalRequired.Text))
            {
                valPostal.Enabled = false;
            }
            if (String.IsNullOrEmpty(lblTelephoneRequired.Text))
            {
                valTelephone.Enabled = false;
            }
            if (String.IsNullOrEmpty(lblCellRequired.Text))
            {
                valCell.Enabled = false;
            }
            if (String.IsNullOrEmpty(lblFaxRequired.Text))
            {
                valFax.Enabled = false;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateRequiredFields updates which fields are required
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        /// -----------------------------------------------------------------------------
        private void UpdateRequiredFields()
        {
            if (chkCountry.Checked == false)
            {
                chkRegion.Checked = false;
            }
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addressstreet", chkStreet.Checked ? "" : "N");
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresscity", chkCity.Checked ? "" : "N");
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresscountry", chkCountry.Checked ? "" : "N");
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addressregion", chkRegion.Checked ? "" : "N");
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresspostal", chkPostal.Checked ? "" : "N");
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresstelephone", chkTelephone.Checked ? "" : "N");
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addresscell", chkCell.Checked ? "" : "N");
            PortalController.UpdatePortalSetting(PortalSettings.PortalId, "addressfax", chkFax.Checked ? "" : "N");
            ShowRequiredFields();
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// Page_Load runs when the control is loaded
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <history>
        /// 	[cnurse]	10/08/2004	Updated to reflect design changes for Help, 508 support
        ///                       and localisation
        /// </history>
        /// -----------------------------------------------------------------------------
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            cboCountry.SelectedIndexChanged += cboCountry_SelectedIndexChanged;
            chkCell.CheckedChanged += chkCell_CheckedChanged;
            chkCity.CheckedChanged += chkCity_CheckedChanged;
            chkCountry.CheckedChanged += chkCountry_CheckedChanged;
            chkFax.CheckedChanged += chkFax_CheckedChanged;
            chkPostal.CheckedChanged += chkPostal_CheckedChanged;
            chkRegion.CheckedChanged += chkRegion_CheckedChanged;
            chkStreet.CheckedChanged += chkStreet_CheckedChanged;
            chkTelephone.CheckedChanged += chkTelephone_CheckedChanged;

            try
            {
                valStreet.ErrorMessage = Localization.GetString("StreetRequired", Localization.GetResourceFile(this, MyFileName));
                valCity.ErrorMessage = Localization.GetString("CityRequired", Localization.GetResourceFile(this, MyFileName));
                valCountry.ErrorMessage = Localization.GetString("CountryRequired", Localization.GetResourceFile(this, MyFileName));
                valPostal.ErrorMessage = Localization.GetString("PostalRequired", Localization.GetResourceFile(this, MyFileName));
                valTelephone.ErrorMessage = Localization.GetString("TelephoneRequired", Localization.GetResourceFile(this, MyFileName));
                valCell.ErrorMessage = Localization.GetString("CellRequired", Localization.GetResourceFile(this, MyFileName));
                valFax.ErrorMessage = Localization.GetString("FaxRequired", Localization.GetResourceFile(this, MyFileName));
                if (!Page.IsPostBack)
                {
                    if (!String.IsNullOrEmpty(_LabelColumnWidth))
                    {
                        plCountry.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plRegion.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plCity.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plStreet.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plUnit.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plPostal.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plTelephone.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plCell.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                        plFax.Width = System.Web.UI.WebControls.Unit.Parse(_LabelColumnWidth);
                    }
                    if (!String.IsNullOrEmpty(_ControlColumnWidth))
                    {
                        cboCountry.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        cboRegion.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtRegion.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtCity.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtStreet.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtUnit.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtPostal.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtTelephone.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtCell.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                        txtFax.Width = System.Web.UI.WebControls.Unit.Parse(_ControlColumnWidth);
                    }
                    txtStreet.TabIndex = Convert.ToInt16(StartTabIndex);
                    txtUnit.TabIndex = Convert.ToInt16(StartTabIndex + 1);
                    txtCity.TabIndex = Convert.ToInt16(StartTabIndex + 2);
                    cboCountry.TabIndex = Convert.ToInt16(StartTabIndex + 3);
                    cboRegion.TabIndex = Convert.ToInt16(StartTabIndex + 4);
                    txtRegion.TabIndex = Convert.ToInt16(StartTabIndex + 5);
                    txtPostal.TabIndex = Convert.ToInt16(StartTabIndex + 6);
                    txtTelephone.TabIndex = Convert.ToInt16(StartTabIndex + 7);
                    txtCell.TabIndex = Convert.ToInt16(StartTabIndex + 8);
                    txtFax.TabIndex = Convert.ToInt16(StartTabIndex + 9);

                    //<tam:note modified to test Lists
                    //Dim objRegionalController As New RegionalController
                    //cboCountry.DataSource = objRegionalController.GetCountries
                    //<this test using method 2: get empty collection then get each entry list on demand & store into cache

                    var ctlEntry = new ListController();
                    ListEntryInfoCollection entryCollection = ctlEntry.GetListEntryInfoCollection("Country");

                    cboCountry.DataSource = entryCollection;
                    cboCountry.DataBind();
                    cboCountry.Items.Insert(0, new ListItem("<" + Localization.GetString("Not_Specified", Localization.SharedResourceFile) + ">", ""));

                    switch (_CountryData.ToLower())
                    {
                        case "text":
                            if (String.IsNullOrEmpty(_Country))
                            {
                                cboCountry.SelectedIndex = 0;
                            }
                            else
                            {
                                if (cboCountry.Items.FindByText(_Country) != null)
                                {
                                    cboCountry.ClearSelection();
                                    cboCountry.Items.FindByText(_Country).Selected = true;
                                }
                            }
                            break;
                        case "value":
                            if (cboCountry.Items.FindByValue(_Country) != null)
                            {
                                cboCountry.ClearSelection();
                                cboCountry.Items.FindByValue(_Country).Selected = true;
                            }
                            break;
                    }
                    Localize();

                    if (cboRegion.Visible)
                    {
                        switch (_RegionData.ToLower())
                        {
                            case "text":
                                if (String.IsNullOrEmpty(_Region))
                                {
                                    cboRegion.SelectedIndex = 0;
                                }
                                else
                                {
                                    if (cboRegion.Items.FindByText(_Region) != null)
                                    {
                                        cboRegion.Items.FindByText(_Region).Selected = true;
                                    }
                                }
                                break;
                            case "value":
                                if (cboRegion.Items.FindByValue(_Region) != null)
                                {
                                    cboRegion.Items.FindByValue(_Region).Selected = true;
                                }
                                break;
                        }
                    }
                    else
                    {
                        txtRegion.Text = _Region;
                    }
                    txtStreet.Text = _Street;
                    txtUnit.Text = _Unit;
                    txtCity.Text = _City;
                    txtPostal.Text = _Postal;
                    txtTelephone.Text = _Telephone;
                    txtCell.Text = _Cell;
                    txtFax.Text = _Fax;

                    rowStreet.Visible = _ShowStreet;
                    rowUnit.Visible = _ShowUnit;
                    rowCity.Visible = _ShowCity;
                    rowCountry.Visible = _ShowCountry;
                    rowRegion.Visible = _ShowRegion;
                    rowPostal.Visible = _ShowPostal;
                    rowTelephone.Visible = _ShowTelephone;
                    rowCell.Visible = _ShowCell;
                    rowFax.Visible = _ShowFax;

                    if (TabPermissionController.CanAdminPage())
                    {
                        chkStreet.Visible = true;
                        chkCity.Visible = true;
                        chkCountry.Visible = true;
                        chkRegion.Visible = true;
                        chkPostal.Visible = true;
                        chkTelephone.Visible = true;
                        chkCell.Visible = true;
                        chkFax.Visible = true;
                    }
                    ViewState["ModuleId"] = Convert.ToString(_ModuleId);
                    ViewState["LabelColumnWidth"] = _LabelColumnWidth;
                    ViewState["ControlColumnWidth"] = _ControlColumnWidth;

                    ShowRequiredFields();
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cboCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Localize();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkCity_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkCountry_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkPostal_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkRegion_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkStreet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkTelephone_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkCell_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void chkFax_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateRequiredFields();
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
		
		#endregion
    }
}
