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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Internal;

namespace DotNetNuke.Web.Api
{
    public static class HttpRequestMessageExtensions
    {
        private const string ModuleIdKey = "ModuleId";
        private const string TabIdKey = "TabId";

        public static int FindTabId(this HttpRequestMessage requestMessage)
        {
            return FindInt(requestMessage, TabIdKey);
        }

        public static int FindModuleId(this HttpRequestMessage requestMessage)
        {
            return FindInt(requestMessage, ModuleIdKey);
        }

        public static ModuleInfo FindModuleInfo(this HttpRequestMessage requestMessage)
        {
            int tabId = requestMessage.FindTabId();
            int moduleId = requestMessage.FindModuleId();

            if (moduleId != Null.NullInteger && tabId != Null.NullInteger)
            {
                return TestableModuleController.Instance.GetModule(moduleId, tabId);
            }

            return null;
        }

        private static int FindInt(HttpRequestMessage requestMessage, string key)
        {
            IEnumerable<string> values;
            string value = null;
            if (requestMessage.Headers.TryGetValues(key, out values))
            {
                value = values.FirstOrDefault();
            }

            if (String.IsNullOrEmpty(value) && requestMessage.RequestUri != null)
            {
                var queryString = HttpUtility.ParseQueryString(requestMessage.RequestUri.Query);
                value = queryString[key];
            }

            int id;
            if (Int32.TryParse(value, out id))
            {
                return id;
            }

            return Null.NullInteger;
        }
    }
}