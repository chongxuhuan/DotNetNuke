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
 * The Original Code is named ClientCapabilityProvider, first 
 * released under this licence on 25th January 2012.
 * 
 * The Initial Developer of the Original Code is owned by 
 * 51 Degrees Mobile Experts Limited. Portions created by 51 Degrees
 * Mobile Experts Limited are Copyright (C) 2012. All Rights Reserved.
 * 
 * Contributor(s):
 *     James Rosewell <james@51degrees.mobi>
 * 
 * ********************************************************************* */

using System;
using System.Collections.Generic;
using FiftyOne.Foundation.Mobile.Detection;
using System.Web;

namespace FiftyOne.Services.ClientCapability
{
    /// <summary>
    /// 51Degrees.mobi Implementation of IClientCapability
    /// </summary>
    public class FiftyOneClientCapability : DotNetNuke.Services.ClientCapability.ClientCapability
    {
        #region Constructor

        /// <summary>
        /// Constructs a new instance of ClientCapability.
        /// See http://51degrees.mobi/Products/DeviceData/PropertyDictionary.aspx
        /// for a full list of available properties.
        /// All the properties used are non-lists and therefore the first
        /// item contained in the values list contains the only available value.
        /// </summary>
        public FiftyOneClientCapability(BaseDeviceInfo device)
        {
            Initialise(device.GetAllProperties());
        }

        /// <summary>
        /// Constructs a new instance of ClientCapability.
        /// See http://51degrees.mobi/Products/DeviceData/PropertyDictionary.aspx
        /// for a full list of available properties.
        /// All the properties used are non-lists and therefore the first
        /// item contained in the values list contains the only available value.
        /// </summary>
        public FiftyOneClientCapability(HttpBrowserCapabilities browserCaps)
        {
            if (browserCaps != null)
            {
                Initialise(
                    browserCaps.Capabilities[FiftyOne.Foundation.Mobile.Detection.Constants.FiftyOneDegreesProperties] 
                        as SortedList<string, List<string>>);
            }
        }

        /// <summary>
        /// Constructs a new instance of ClientCapability.
        /// See http://51degrees.mobi/Products/DeviceData/PropertyDictionary.aspx
        /// for a full list of available properties.
        /// All the properties used are non-lists and therefore the first
        /// item contained in the values list contains the only available value.
        /// </summary>
        public FiftyOneClientCapability(SortedList<string, List<string>> properties)
        {
            Initialise(properties);
        }

        private void Initialise(SortedList<string, List<string>> properties)
        {
            if (properties != null)
            {
                // Set Lite properties
                ID = GetStringValue(properties, "Id");
                IsMobile = GetBoolValue(properties, "IsMobile");
                ScreenResolutionWidthInPixels = GetIntValue(properties, "ScreenPixelsWidth");
                ScreenResolutionHeightInPixels = GetIntValue(properties, "ScreenPixelsHeight");

                // Set Premium properties
                IsTablet = GetBoolValue(properties, "IsTablet");
                IsTouchScreen = GetBoolValue(properties, "HasTouchScreen");
                BrowserName = GetStringValue(properties, "BrowserName");
                Capabilities = GetCapabilities(properties);

                // The following properties are not provided by 51Degrees.mobi and
                // are therefore set to default values.
                SupportsFlash = false;
                HtmlPreferedDTD = null;

                //set IsMobile to false when IsTablet is true.
                if (IsTablet)
                    IsMobile = false;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns a dictionary of capability names and values as strings based on the object
        /// keys and values held in the browser capabilities provided. The value string may
        /// contains pipe (|) seperated lists of values.
        /// </summary>
        /// <param name="properties">A collection of device related capabilities.</param>
        /// <returns>Device related capabilities with property names and values converted to strings.</returns>
        private static IDictionary<string, string> GetCapabilities(SortedList<string, List<string>> properties)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var key in properties.Keys)
            {
                dictionary.Add(
                    key,
                    String.Join(FiftyOne.Foundation.Mobile.Detection.Constants.ValueSeperator, properties[key].ToArray()));
            }
            return dictionary;
        }

        /// <summary>
        /// Returns the property of the HttpBrowserCapabilities collection 
        /// as an boolean.
        /// </summary>
        /// <param name="properties">A collection of device related capabilities.</param>
        /// <param name="property">The name of the property to return as a boolean.</param>
        /// <returns>The boolean value of the property, or false if the property is not found or it's value is not an boolean.</returns>
        private static bool GetBoolValue(SortedList<string, List<string>> properties, string property)
        {
            bool value;
            if (properties.ContainsKey(property) &&
                bool.TryParse(properties[property][0], out value))
                return value;
            return false;
        }

        /// <summary>
        /// Returns the property of the HttpBrowserCapabilities collection 
        /// as an integer.
        /// </summary>
        /// <param name="properties">A collection of device related capabilities.</param>
        /// <param name="property">The name of the property to return as a integer.</param>
        /// <returns>The integer value of the property, or 0 if the property is not found or it's value is not an integer.</returns>
        private static int GetIntValue(SortedList<string, List<string>> properties, string property)
        {
            int value;
            if (properties.ContainsKey(property) &&
                int.TryParse(properties[property][0], out value))
                return value;
            return 0;
        }

        /// <summary>
        /// Returns the property of the HttpBrowserCapabilities collection 
        /// as a string.
        /// </summary>
        /// <param name="properties">A collection of device related properties.</param>
        /// <param name="property">The name of the property to return as a string.</param>
        /// <returns>The string value of the property, or null if the property is not found.</returns>
        private static string GetStringValue(SortedList<string, List<string>> properties, string property)
        {
            if (properties.ContainsKey(property))
                return properties[property][0];
            return null;
        }

        #endregion
    }
}
