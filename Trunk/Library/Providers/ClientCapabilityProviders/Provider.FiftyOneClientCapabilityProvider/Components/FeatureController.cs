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

namespace DotNetNuke.Providers.FiftyOneClientCapabilityProvider.Components
{
    using System.Collections.Generic;
    using Common;
    using Common.Utilities;
    using Entities.Modules;
    using Entities.Modules.Definitions;
    using Entities.Tabs;
    using Services.Installer;
    using Services.Installer.Packages;
    using Services.Localization;
    using Services.Upgrade;
    using Constants = FiftyOne.Constants;

    /// -----------------------------------------------------------------------------
    ///<summary>
    /// The FeatureController class for the modules.
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <history>
    /// </history>
    /// -----------------------------------------------------------------------------

#if SEARCHABLE
    public class FeatureController : ModuleController, ISearchable, IUpgradeable
#else
    public class FeatureController : ModuleController, IUpgradeable
#endif
    {
        #region Constants

        private const string ResourceFileRelativePath = "~/DesktopModules/Admin/FiftyOneClientCapabilityProvider/App_LocalResources/FeatureController.cs.resx";

        #endregion

        #region Interfaces

#if SEARCHABLE

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// GetSearchItems implements the ISearchable Interface
        /// </summary>
        /// <param name="ModInfo">The ModuleInfo for the module to be Indexed</param>
        /// -----------------------------------------------------------------------------
        public DotNetNuke.Services.Search.SearchItemInfoCollection GetSearchItems(DotNetNuke.Entities.Modules.ModuleInfo ModInfo)
        {
            var items = new SearchItemInfoCollection();

            // Add the devices to the search index.
            foreach (var item in Factory.ActiveProvider.Devices)
            {
                string guid = String.Format("DeviceID={0}",
                    item.DeviceId);

                var device = new Device(item);

                items.Add(new SearchItemInfo(
                    device.HardwareCaption,
                    String.Format("{0} running {1} with browser {2}",
                        device.HardwareCaption,
                        device.SoftwareCaption,
                        device.BrowserCaption),
                    ModInfo.CreatedByUserID,
                    Factory.ActiveProvider.PublishedDate,
                    ModInfo.ModuleID,
                    device.DeviceID,
                    device.Content,
                    guid));
            }

            // Add the properties to the search.
            foreach (var property in Factory.ActiveProvider.Properties)
            {
                items.Add(new SearchItemInfo(
                    property.Name,
                    property.Description,
                    ModInfo.CreatedByUserID,
                    Factory.ActiveProvider.PublishedDate,
                    ModInfo.ModuleID,
                    property.Name,
                    property.Description));
                foreach (var value in property.Values)
                {
                    string name = String.Format("{0} - {1}", property.Name, value.Name);
                    items.Add(new SearchItemInfo(
                        name,
                        value.Description,
                        ModInfo.CreatedByUserID,
                        Factory.ActiveProvider.PublishedDate,
                        ModInfo.ModuleID,
                        name,
                        value.Description));
                }
            }

            return items;
        }

#endif

        /// <summary>
        /// Handles upgrading the module and adding the module to the hosts menu.
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public string UpgradeModule(string version)
        {
            switch (version)
            {
                case "2.1.1.5":
                    PackageInfo package = PackageController.GetPackageByName(Constants.PackageName);
                    IDictionary<int, TabInfo> moduleTabs = new TabController().GetTabsByPackageID(-1, package.PackageID, false);

                    if (moduleTabs.Count > 0)
                        return string.Empty;

                    AddClientResourceAdminHostPage();

                    RemoveWurflProvider();
                    break;
            }

            return Localization.GetString("SuccessMessage", ResourceFileRelativePath);
        }

        private void RemoveWurflProvider()
        {
            var package = PackageController.GetPackageByName("DotNetNuke.WURFLClientCapabilityProvider");
            if(package != null)
            {
                var installer = new Installer(package, Globals.ApplicationMapPath);
                installer.UnInstall(true);
            }
        }

        private static void AddClientResourceAdminHostPage()
        {
            DesktopModuleInfo desktopModule = DesktopModuleController.GetDesktopModuleByModuleName(Constants.ModuleName, Null.NullInteger);
            ModuleDefinitionInfo moduleDefinition = desktopModule.ModuleDefinitions[Constants.ModuleDefinitionName];

            // Remove the page if it already exists to ensure the page can be added.
            // Handles cases where the page has been removed by the user.
            try
            {
                Upgrade.RemoveHostPage(Localization.GetString("PageName", ResourceFileRelativePath));
            }
            catch
            {
                // Do nothing.
            }

            TabInfo hostPage = Upgrade.AddHostPage(Localization.GetString("PageName", ResourceFileRelativePath),
                                                   Localization.GetString("PageDescription", ResourceFileRelativePath),
                                                   Constants.ConfigIconFileThumbNail,
                                                   Constants.ConfigIconFileLarge, true);

            Upgrade.AddModuleToPage(hostPage, moduleDefinition.ModuleDefID, Localization.GetString("ModuleTitle", ResourceFileRelativePath), Constants.ConfigIconFileLarge, true);
        }

        #endregion
    }
}

