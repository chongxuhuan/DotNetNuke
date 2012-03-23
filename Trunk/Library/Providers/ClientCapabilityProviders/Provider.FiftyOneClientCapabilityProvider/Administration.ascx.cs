/* *********************************************************************
 * The contents of this file are subject to the Mozilla Public License 
 * Version 1.1 (the "License"); you may not use this file except in 
 * compliance with the License. You may obtain a copy of the License at 
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS" 
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 * See the License for the specific language governing rights and 
 * limitations under the License.
 *
 * The Original Code is named 51Degrees.mobi DotNetNuke Module, first 
 * released under this licence on 26th January 2012.
 * 
 * The Initial Developer of the Original Code is owned by 
 * 51 Degrees Mobile Experts Limited. Portions created by 51 Degrees
 * Mobile Experts Limited are Copyright (C) 2012. All Rights Reserved.
 * 
 * Contributor(s):
 *     James Rosewell <james@51degrees.mobi>
 * 
 * ********************************************************************* */

using DotNetNuke.Framework;

namespace DotNetNuke.Providers.FiftyOneClientCapabilityProvider
{
    using System;
    using System.Linq;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using Components;
    using FiftyOne.Foundation.Mobile.Detection;
    using FiftyOne.Foundation.UI;

    /// <summary>
    /// Administration control is used as the main control off the hosts
    /// page to activate 51Degrees.mobi.
    /// </summary>
    partial class Administration : ModuleBase
    {
        /// <summary>
        ///  Records if premium data is in use when the control is first loaded.
        /// </summary>
        protected bool IsPremium = DataProvider.IsPremium;

        /// <summary>
        /// Executes the page initialization event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SearchButton.Click += SearchButtonClick;
            HardwareList.ItemDataBound += ListItemDataBound;
            
            NoResultsMessage.Visible = false;

            jQuery.RequestDnnPluginsRegistration();

            HardwareList.DataSource = DataProvider.HardwareProperties;
            SoftwareList.DataSource = DataProvider.SoftwareProperties;
            BrowserList.DataSource = DataProvider.BrowserProperties;
            ContentList.DataSource = DataProvider.ContentProperties;

            HardwareList.DataBind();
            SoftwareList.DataBind();
            BrowserList.DataBind();
            ContentList.DataBind();

            var refreshButtonText = LocalizeString("StatsRefreshButton.Text");
            var statsHtml = LocalizeString("StatsHtml.Text");
            FooterStats.RefreshButtonText = refreshButtonText;
            FooterStats.Html = statsHtml;
            LiteStats.RefreshButtonText = refreshButtonText;
            LiteStats.Html = statsHtml;

            if (!IsPremium)
            {
                // lite
                Activate.ActivateButtonText = LocalizeString("ActivateButtonText.Text");
                Activate.ActivatedMessageHtml = LocalizeString("ActivatedMessageHtml.Text");
                Activate.ActivateInstructionsHtml = LocalizeString("ActivateInstructionsHtml.Text");
                Activate.ActivationDataInvalidHtml = LocalizeString("ActivationDataInvalidHtml.Text");
                Activate.ActivationFailureCouldNotUpdateConfigHtml = LocalizeString("ActivationFailureCouldNotUpdateConfigHtml.Text");
                Activate.ActivationFailureCouldNotWriteDataFileHtml = LocalizeString("ActivationFailureCouldNotWriteDataFileHtml.Text");
                Activate.ActivationFailureCouldNotWriteLicenceFileHtml = LocalizeString("ActivationFailureCouldNotWriteLicenceFileHtml.Text");
                Activate.ActivationFailureGenericHtml = LocalizeString("ActivationFailureGenericHtml.Text");
                Activate.ActivationFailureHttpHtml = LocalizeString("ActivationFailureHttpHtml.Text");
                Activate.ActivationFailureInvalidHtml = LocalizeString("ActivationFailureInvalidHtml.Text");
                Activate.ActivationStreamFailureHtml = LocalizeString("ActivationStreamFailureHtml.Text");
                Activate.ActivationSuccessHtml = LocalizeString("ActivationSuccessHtml.Text");
                Activate.ValidationFileErrorText = LocalizeString("ValidationFileErrorText.Text");
                Activate.ValidationRequiredErrorText = LocalizeString("ValidationRequiredErrorText.Text");
                Activate.ValidationRegExErrorText = LocalizeString("ValidationRegExErrorText.Text");
                Activate.RefreshButtonText = LocalizeString("RefreshButtonText.Text");
                Activate.UploadButtonText = LocalizeString("UploadButtonText.Text");
                Activate.UploadInstructionsHtml = LocalizeString("UploadInstructionsHtml.Text");
            }
            else
            {
                // premium
                DeviceExplorer.BackButtonDeviceText = LocalizeString("BackButtonDeviceText.Text");
                DeviceExplorer.BackButtonDevicesText = LocalizeString("BackButtonDevicesText.Text");
                DeviceExplorer.DeviceExplorerDeviceHtml = LocalizeString("DeviceExplorerDeviceInstructionsHtml.Text");
                DeviceExplorer.DeviceExplorerModelsHtml = LocalizeString("DeviceExplorerModelsInstructionsHtml.Text");
                DeviceExplorer.DeviceExplorerVendorsHtml = LocalizeString("DeviceExplorerVendorsHtml.Text");
            }

            if (IsPostBack)
                return;

            var vendor = Request.QueryString["Vendor"];
            var model = Request.QueryString["Model"];
            var deviceId = Request.QueryString["DeviceID"];
            var searchQuery = Request.QueryString["Query"];

            var hasVendor = !string.IsNullOrEmpty(vendor);
            var hasModel = !string.IsNullOrEmpty(model);
            var hasDeviceId = !string.IsNullOrEmpty(deviceId);
            var hasSearchQuery = !string.IsNullOrEmpty(searchQuery);

            if (hasVendor)
                DeviceExplorer.Vendor = vendor;

            if (hasModel)
                DeviceExplorer.Model = model;
            
            if (hasDeviceId)
                DeviceExplorer.DeviceID = deviceId;
            
            if (hasSearchQuery)
                SearchTextBox.Text = Server.UrlDecode(searchQuery);

            NoResultsMessage.Visible = hasSearchQuery && !hasDeviceId && !hasModel && !hasVendor;
        }

        private static void ListItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem == null || !(e.Item.DataItem is Property))
                return;

            var property = (Property)e.Item.DataItem;
            var premiumLabel = e.Item.FindControl("Premium") as HtmlGenericControl;
            if (premiumLabel != null)
            {
                premiumLabel.Visible = property.IsPremium;
            }

            var values = e.Item.FindControl("Values") as HtmlGenericControl;
            if (values == null)
                return;

            values.Visible = property.ShowValues;

            if (property.ShowValues)
            {
                values.InnerText = string.Join(", ", property.Values.Select(item => item.Name).ToArray());
            }
        }

        private void SearchButtonClick(object sender, EventArgs e)
        {

            string additionalParams = string.Empty;
            if (DataProvider.IsPremium)
            {
                var deviceList = DataProvider.FindDevices(this.SearchTextBox.Text);
                if (deviceList != null && deviceList.Count > 0)
                {
                    additionalParams = "DeviceID=" + deviceList.First().DeviceID;
                }
            }
            else
            {
                var device = DataProvider.Provider.GetDeviceInfo(this.SearchTextBox.Text);
                if (device != null)
                {
                    additionalParams = "DeviceID=" + device.DeviceId;
                }
            }
            
            var url = EditUrl(TabId, string.Empty, false, "Query=" + Server.UrlEncode(SearchTextBox.Text), additionalParams);
            Response.Redirect(url);
        }
    }
}

