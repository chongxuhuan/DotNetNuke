/* 
 * This file is released under the GNU General Public License. 
 * Refer to the COPYING file distributed with this package.
 *
 * Copyright (c) 2008 WURFL-Pro S.r.l.
 */
using System.Collections.Generic;
using System.Configuration;

using DotNetNuke.Services.Devices.Core.Resource;
using DotNetNuke.Services.Devices.Core.Resource.Config;
using DotNetNuke.Instrumentation;

namespace Provider.WURFLRequestProfileProvider.Core.Resource.Config
{
    public class WURFLConfig
    {

        private const string WURFL = "wurfl";


        private readonly WURFLResource _mainWURFLResource;


        private readonly List<WURFLResource> _patchWURFLResources;


        private readonly string wurflFile;

        private readonly List<string> wurflFilePatches;

        public WURFLConfig()
        {
            WURFLConfigSectionHandler config = (WURFLConfigSectionHandler) ConfigurationManager.GetSection(WURFL);
            
            wurflFile = config.FileLocationSection.path;
            if (wurflFile.StartsWith("~")) wurflFile = System.Web.HttpContext.Current.Server.MapPath(System.Web.VirtualPathUtility.ToAbsolute(wurflFile));
            DnnLog.Info("WURLF FILE" + wurflFile);
            _mainWURFLResource = CreateWURFResource(wurflFile);

            wurflFilePatches = new List<string>();
            _patchWURFLResources = new List<WURFLResource>();
            foreach (FileLocationConfigElement patchSection in config.PatchesLocation)
            {
                string path = patchSection.path;
                if (path.StartsWith("~")) path = System.Web.HttpContext.Current.Server.MapPath(System.Web.VirtualPathUtility.ToAbsolute(path));
                DnnLog.Info("WURLF PATCH FILE" + path);
                _patchWURFLResources.Add(CreateWURFResource(path));
                wurflFilePatches.Add(path);
            }
        }

        public WURFLResource MainWURFLResource
        {
            get { return _mainWURFLResource; }
        }

        public List<WURFLResource> PatchWURFLResources
        {
            get { return _patchWURFLResources; }
        }

        public string WURFLFile
        {
            get { return wurflFile; }
        }

        public List<string> WURFLFilePatches
        {
            get { return wurflFilePatches; }
        }

        private WURFLResource CreateWURFResource(string wurflFile)
        {
            if (WURFLResource.IsZipFile(wurflFile))
            {
                return new ZipWURFLResource(wurflFile);
            }

            if (WURFLResource.IsGZipFile(wurflFile))
            {
                return new GZipWURFLResource(wurflFile);
            }

            return new FlatWURFLResource(wurflFile);
        }
    }
}