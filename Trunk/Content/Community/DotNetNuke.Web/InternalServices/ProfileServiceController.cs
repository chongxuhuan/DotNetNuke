#region Copyright

// 
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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Instrumentation;
using DotNetNuke.Web.Api;

#endregion

namespace DotNetNuke.Web.InternalServices
{
    [DnnAuthorize]
    public class ProfileServiceController : DnnApiController
    {
    	private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof (ProfileServiceController));
        private static void AddProperty(ICollection<SearchResult> results, string field, string searchTerm)
        {
            if (field.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant().Trim()))
            {
                results.Add(new SearchResult { id = field, name = field });
            }
        }

        [HttpGet]
        public HttpResponseMessage Search(string q)
        {
            try
            {
                var portalId = PortalController.GetEffectivePortalId(PortalSettings.PortalId);

                var controller = new ListController();

                var textType = controller.GetListEntryInfo("DataType", "Text");
                var regionType = controller.GetListEntryInfo("DataType", "Region");
                var countryType = controller.GetListEntryInfo("DataType", "Country");

                IList<SearchResult> results = new List<SearchResult>();
                foreach (var definition in ProfileController.GetPropertyDefinitionsByPortal(portalId)
                                            .Cast<ProfilePropertyDefinition>()
                                            .Where(definition => definition.DataType == textType.EntryID
                                                    || definition.DataType == regionType.EntryID
                                                    || definition.DataType == countryType.EntryID))
                {
                    AddProperty(results, definition.PropertyName, q);
                }

                AddProperty(results, "Email", q);
                AddProperty(results, "DisplayName", q);
                AddProperty(results, "Username", q);
                AddProperty(results, "Password", q);
                AddProperty(results, "PasswordConfirm", q);
                AddProperty(results, "PasswordQuestion", q);
                AddProperty(results, "PasswordAnswer", q);

                return Request.CreateResponse(HttpStatusCode.OK, results.OrderBy(sr => sr.id));
            }
            catch (Exception exc)
            {
                Logger.Error(exc);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, exc);
            }
        }

        private class SearchResult
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable NotAccessedField.Local
            public string id;
            public string name;
            // ReSharper restore NotAccessedField.Local
            // ReSharper restore InconsistentNaming
        }
    }
}