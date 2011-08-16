using System;
using System.Web;

using WURFL;
using WURFL.Config;

namespace DotNetNuke.Provider.WURFLClientCapabilityProvider.Helpers
{
    /// <summary>
    /// Loads WURFL data from the specified location
    /// </summary>
    public class WurflLoader
    {
        #region Constants
        /// <summary>
        /// The key used to cache WURFL data
        /// </summary>
        public const String WurflManagerCacheKey = "__WurflManager";

        /// <summary>
        /// WURFL data paths
        /// </summary>
        public const String DefaultWurflDataFilePath = "~/App_Data/wurfl-latest.zip";
        public const String DefaultWurflPatchFilePath = "~/App_Data/web_browsers_patch.xml";
        #endregion

        /// <summary>
        /// Static constructor used to set default path names
        /// </summary>
        static WurflLoader()
        {
            _wurflDataPath = DefaultWurflDataFilePath;
            _wurflPatchFilePath = DefaultWurflPatchFilePath;
        }

        // Locals
        private static String _wurflDataPath, _wurflPatchFilePath;

        /// <summary>
        /// Used to set different data paths
        /// </summary>
        /// <param name="wurflDataPath">WURFL data path</param>
        /// <param name="wurflPatchFilePath">Patch file data path</param>
        public void SetDataPath(String wurflDataPath, String wurflPatchFilePath)
        {
            if (!String.IsNullOrEmpty(wurflDataPath))
                _wurflDataPath = wurflDataPath;
            if (!String.IsNullOrEmpty(wurflPatchFilePath))
                _wurflPatchFilePath = wurflPatchFilePath;
        }

        /// <summary>
        /// Starter of the WURFL framework
        /// </summary>
        public static void Start()
        {
            Start(HttpContext.Current);
        }
        public static void Start(HttpContext context)
        {
            // Initialize the WURFL manager 
            InitializeWurflManager(context, _wurflDataPath, _wurflPatchFilePath);
        }
        
        /// <summary>
        /// Returns the current WURFL manager for device queries
        /// </summary>
        public static IWURFLManager GetManager()
        {
            return GetManager(HttpContext.Current);
        }
        public static IWURFLManager GetManager(HttpContext context)
        {
            var wurflManager = context.Cache[WurflManagerCacheKey] as IWURFLManager;
            if (wurflManager == null)
                return InitializeWurflManager(context, _wurflDataPath, _wurflPatchFilePath);

            return wurflManager;
        }

        /// <summary>
        /// Initializes the WURFL manager loading any required data and returns the instance.
        /// </summary>
        /// <param name="context">HTTP context</param>
        /// <param name="wurflDataPath">Relative WURFL data path</param>
        /// <param name="wurflPatchFilePath">Relative WURFL patch file path</param>
        /// <returns>IWURFLManager</returns>
        private static IWURFLManager InitializeWurflManager(HttpContext context, String wurflDataPath, String wurflPatchFilePath)
        {
            // Get the absolute path of required data files
            var wurflDataFile = context.Server.MapPath(wurflDataPath);
            var wurflPatchFile = context.Server.MapPath(wurflPatchFilePath);

            // Initializes the WURFL infrastructure
            var configurer = new InMemoryConfigurer()
                .MainFile(wurflDataFile)
                .PatchFile(wurflPatchFile);
            var manager = WURFLManagerBuilder.Build(configurer);

            // Cache manager (and managed data) for later use
            context.Cache[WurflManagerCacheKey] = manager;
            return manager;
        }
    }
}