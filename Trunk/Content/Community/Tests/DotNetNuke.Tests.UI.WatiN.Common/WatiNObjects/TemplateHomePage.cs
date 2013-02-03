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
using WatiN.Core;
using DotNetNuke.Tests.UI.WatiN.Common;

namespace DotNetNuke.Tests.UI.WatiN.Common.WatiNObjects
{
    /// <summary>
    /// The default home page template object.
    /// </summary>
    public class TemplateHomePage : WatiNBase
    {
        #region Constructors

        public TemplateHomePage(WatiNBase watinBase) : base(watinBase.IEInstance, watinBase.SiteUrl, watinBase.DBName) { }

        public TemplateHomePage(IE ieInstance, string siteUrl, string dbName) : base(ieInstance, siteUrl, dbName) { }

        #endregion

        #region Public Properties

        #region Images
        public Image MsAspNetImage
        {
            get { return FooterDiv.Image(Find.ByAlt("ASP.net")); }
        }
        public Image CBeyondImage
        {
            get { return FooterDiv.Image(Find.ByAlt(s => s.Contains("CBeyond Cloud Services"))); }
        }
        public Image MaximumASPImage
        {
            get { return FooterDiv.Image(Find.ByAlt("MaximumASP - Smart Hosting, Smart Choices")); }
        }
        public Image TelerikSponsorImage
        {
            get { return FooterDiv.Image(Find.ByAlt("Telerik - Deliver more than expected")); }
        }
        public Image ExactTargetImage
        {
            get { return FooterDiv.Image(Find.ByAlt("Exact Target")); }
        }
        public Image WindowsLiveImage
        {
            get { return FooterDiv.Image(Find.ByAlt("Windows Live - Click here for developer resources")); }
        }
        public Image RedGateImage
        {
            get { return FooterDiv.Image(Find.ByAlt("Redgate")); }
        }
        public Image AppliedInnovationsImage
        {
            get { return FooterDiv.Image(Find.ByAlt("Applied Innovations - Hosting with freedom to grow")); }
        }
        public Image ArrowConsultingDesignImage
        {
            get { return FooterDiv.Image(Find.ByAlt("Arrow Consulting & Design")); }
        }
        #endregion

        #endregion

        #region Public Methods




        #endregion
    }
}

