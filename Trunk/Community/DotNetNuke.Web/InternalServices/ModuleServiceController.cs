#region Copyright

// DotNetNuke® - http://www.dotnetnuke.com
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

namespace DotNetNuke.Web.InternalServices
{
    [DnnAuthorize]
    public class ModuleServiceController : DnnApiController
    {
        public class MoveModuleDTO
        {
            public int ModuleId { get; set; }
            public int ModuleOrder { get; set; }
            public string Pane { get; set; }
            public int TabId { get; set; }
        }

        public class SharableDTO
        {
            public int ModuleId { get; set; }
            public int TabId { get; set; }
            public int PortalId { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage GetModuleShareable(SharableDTO postData)
        {
            var requiresWarning = false;

            DesktopModuleInfo desktopModule;
            if (postData.TabId < 0)
            {
                desktopModule = DesktopModuleController.GetDesktopModule(postData.ModuleId, postData.PortalId);
            }
            else
            {
                var moduleInfo = new ModuleController().GetModule(postData.ModuleId, postData.TabId);

                desktopModule = moduleInfo.DesktopModule;

                requiresWarning = moduleInfo.PortalID != PortalSettings.PortalId && desktopModule.Shareable == ModuleSharing.Unknown;
            }

            if (desktopModule == null)
            {
                var message = string.Format("Cannot find module ID {0} (tab ID {1}, portal ID {2})", postData.ModuleId, postData.TabId, postData.PortalId);
                DnnLog.Error(message);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new {Shareable = desktopModule.Shareable.ToString(), RequiresWarning = requiresWarning});
        }

        [HttpPost]
        public HttpResponseMessage MoveModule(MoveModuleDTO postData)
        {
            var moduleController = new ModuleController();

            moduleController.UpdateModuleOrder(postData.TabId, postData.ModuleId, postData.ModuleOrder, postData.Pane);
            moduleController.UpdateTabModuleOrder(postData.TabId);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
