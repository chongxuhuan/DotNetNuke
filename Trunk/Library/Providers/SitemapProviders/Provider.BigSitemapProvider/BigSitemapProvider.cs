#region Copyright

// 
// DotNetNuke� - http://www.dotnetnuke.com
// Copyright (c) 2002-2011
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

using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Sitemap;

#endregion

namespace DotNetNuke.Sitemap
{
    public class BigSitemapProvider : SitemapProvider
    {
        public override List<SitemapUrl> GetUrls(int portalId, PortalSettings ps, string version)
        {
            var urls = new List<SitemapUrl>();
            for (int i = 0; i <= 50; i++)
            {
                urls.Add(GetPageUrl(i));
            }

            return urls;
        }


        private SitemapUrl GetPageUrl(int index)
        {
            var pageUrl = new SitemapUrl();
            pageUrl.Url = string.Format("http://mysite/page_{0}.aspx", index);
            pageUrl.Priority = 0.5F;
            pageUrl.LastModified = DateTime.Now;
            pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily;

            return pageUrl;
        }
    }
}