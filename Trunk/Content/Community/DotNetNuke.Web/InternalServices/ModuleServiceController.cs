#region Copyright

// DotNetNuke� - http://www.dotnetnuke.com
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

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Api.Internal;

namespace DotNetNuke.Web.InternalServices
{
    [DnnAuthorize]
    public class ModuleServiceController : DnnApiController
    {
    	private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof (ModuleServiceController));
        public class MoveModuleDTO
        {
            public int ModuleId { get; set; }
            public int ModuleOrder { get; set; }
            public string Pane { get; set; }
            public int TabId { get; set; }
        }

        [HttpGet]
        [DnnAuthorize(StaticRoles = "Registered Users" )]
        public HttpResponseMessage GetModuleShareable(int moduleId, int tabId, int portalId)
        {
            var requiresWarning = false;

            DesktopModuleInfo desktopModule;
            if (tabId < 0)
            {
                desktopModule = DesktopModuleController.GetDesktopModule(moduleId, portalId);
            }
            else
            {
                var moduleInfo = new ModuleController().GetModule(moduleId, tabId);

                desktopModule = moduleInfo.DesktopModule;

                requiresWarning = moduleInfo.PortalID != PortalSettings.PortalId && desktopModule.Shareable == ModuleSharing.Unknown;
            }

            if (desktopModule == null)
            {
                var message = string.Format("Cannot find module ID {0} (tab ID {1}, portal ID {2})", moduleId, tabId, portalId);
                Logger.Error(message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new {Shareable = desktopModule.Shareable.ToString(), RequiresWarning = requiresWarning});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DnnPageEditor]
        public HttpResponseMessage MoveModule(MoveModuleDTO postData)
        {
            var moduleController = new ModuleController();

            moduleController.UpdateModuleOrder(postData.TabId, postData.ModuleId, postData.ModuleOrder, postData.Pane);
            moduleController.UpdateTabModuleOrder(postData.TabId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}