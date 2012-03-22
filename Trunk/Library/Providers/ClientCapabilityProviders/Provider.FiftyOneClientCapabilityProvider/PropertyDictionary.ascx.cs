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
    using System.Linq;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using FiftyOne.Foundation.Mobile.Detection;
    using FiftyOne.Foundation.UI;

    /// <summary>
    /// Used to contain the 51Degrees.mobi property dictionary control.
    /// </summary>
    partial class PropertyDictionary : Components.ModuleBase
    {
        protected override void OnInit(System.EventArgs e)
        {
            base.OnInit(e);
            
            HardwareList.DataSource = DataProvider.HardwareProperties;
            SoftwareList.DataSource = DataProvider.SoftwareProperties;
            BrowserList.DataSource = DataProvider.BrowserProperties;
            ContentList.DataSource = DataProvider.ContentProperties;

            HardwareList.ItemDataBound += ListItemDataBound;

            HardwareList.DataBind();
            SoftwareList.DataBind();
            BrowserList.DataBind();
            ContentList.DataBind();
        }

        private void ListItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.DataItem == null || !(e.Item.DataItem is Property))
                return;

            var property = (Property) e.Item.DataItem;
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
    }
}

