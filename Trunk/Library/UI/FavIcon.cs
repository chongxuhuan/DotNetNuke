using System;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Portals;

namespace DotNetNuke.UI.Internals
{
    /// <summary>
    /// Manages the FavIcon of a portal
    /// </summary>
    public class FavIcon
    {
        private const string SettingName = "FavIconPath";

        private readonly int _portalId;

        /// <summary>
        /// Initializes a FavIcon instance
        /// </summary>
        /// <param name="portalId">The id of the portal</param>
        public FavIcon(int portalId)
        {
            _portalId = portalId;
        }

        /// <summary>
        /// Get the relative file path to the favicon file
        /// </summary>
        /// <returns>Relative path to the favicon file, or empty string when there is no favicon set</returns>
        public string GetFilePath()
        {
            var portalSettings = new PortalSettings(_portalId);
        	var filePath = string.Empty;
        	var favIcon = PortalController.GetPortalSetting(SettingName, _portalId, "");
			if(!string.IsNullOrEmpty(favIcon))
			{
				filePath = Globals.ResolveUrl("~" + portalSettings.HomeDirectory + favIcon);
			}

        	return filePath;
        }

        /// <summary>
        /// Update the file to use for a favIcon
        /// </summary>
        /// <param name="fileId">The file id or Null.NullInteger for none</param>
        public void Update(int fileId)
        {
            PortalController.UpdatePortalSetting(_portalId, SettingName, fileId != Null.NullInteger ? string.Format("FileID={0}", fileId) : "", clearCache: true);
            DataCache.ClearCache(GetCacheKey(_portalId));
        }

        /// <summary>
        /// Get the HTML for a favicon link
        /// </summary>
        /// <param name="portalId">The portal id</param>
        /// <returns>The HTML for the favicon link for the portal, or an empty string if there is no favicon</returns>
        public static string GetHeaderLink(int portalId)
        {
            string headerLink;
            object fromCache = DataCache.GetCache(GetCacheKey(portalId));

            if (fromCache == null)
            {
                //only create an instance of FavIcon when there is a cache miss
                string favIconPath = new FavIcon(portalId).GetFilePath();
                if (!string.IsNullOrEmpty(favIconPath))
                {
                    headerLink = string.Format("<link rel='SHORTCUT ICON' href='{0}' type='image/x-icon' />", favIconPath);
                }
                else
                {
                    headerLink = "";
                }

                //cache link or empty string to ensure we don't always have a
                //cache miss when no favicon is in use
                UpdateCachedHeaderLink(portalId, headerLink);
            }
            else
            {
                headerLink = fromCache.ToString();
            }

            return headerLink;
        }

        private static void UpdateCachedHeaderLink(int portalId, string headerLink)
        {
            if (Host.PerformanceSetting != Globals.PerformanceSettings.NoCaching)
            {
                DataCache.SetCache(GetCacheKey(portalId), headerLink);
            }
        }

        private static string GetCacheKey(int portalId)
        {
            return "FAVICON" + portalId;
        }
    }
}