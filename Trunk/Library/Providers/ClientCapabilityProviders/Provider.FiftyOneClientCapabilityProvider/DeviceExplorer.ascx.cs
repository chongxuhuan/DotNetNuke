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
 * The Original Code is named 51Degrees.mobi Dot Net Nuke Module, first 
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

namespace DotNetNuke.Providers.FiftyOneClientCapabilityProvider
{
    using System;
    using FiftyOne;

    /// <summary>
    /// Class to wrap access to the 51Degrees.mobi device explorer control.
    /// </summary>
    partial class DeviceExplorer : Components.ModuleBase
    {
        /// <summary>
        /// Displays all the vendors available in the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set the retailer details.
                Message.RetailerName = RetailerConstants.RETAILER_NAME;
                Message.RetailerUrl = new Uri(RetailerConstants.RETAILER_URL);

                // Set the parameters from the query string if DNN
                // has been used.
                Explorer.Vendor = Request.QueryString["Vendor"];
                Explorer.Model = Request.QueryString["Model"];
                Explorer.DeviceID = Request.QueryString["DeviceID"];
            }
        }
    }
}